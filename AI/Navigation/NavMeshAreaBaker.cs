using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

public class NavMeshAreaBaker : MonoBehaviour
{
    public static NavMeshAreaBaker Instance;

    [SerializeField]
    private NavMeshSurface[] _surfaces;

    [SerializeField]
    private Transform _playerTransform;

    [SerializeField]
    private float _updateRate = 0.1f;

    [SerializeField]
    private float _movementThreshold = 3f;

    [SerializeField]
    private Vector3 _navMeshSize = new Vector3(20, 20, 20);

    [SerializeField]
    private bool _shouldCacheSources;

    public delegate void NavMeshUpdatedEvent(Bounds Bounds);
    public NavMeshUpdatedEvent OnNavMeshUpdate;

    private Vector3 _worldAnchor;
    private NavMeshData[] _navMeshDatas;

    private float _lastChecked;

    private Dictionary<int, List<NavMeshBuildSource>> _sourcesPerSurface = new Dictionary<int, List<NavMeshBuildSource>>();
    private Dictionary<int, List<NavMeshBuildMarkup>> _markupsPerSurface = new Dictionary<int, List<NavMeshBuildMarkup>>();
    private Dictionary<int, List<NavMeshModifier>> _modifiersPerSurface = new Dictionary<int, List<NavMeshModifier>>();

    private List<NavMeshModifierVolume> _volumes;
    private List<NavMeshBuildSource> _volumesSources;

    private Bounds _navMeshBounds;

    private async void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        _navMeshDatas = new NavMeshData[_surfaces.Length];

        _volumes = new List<NavMeshModifierVolume>();
        _volumesSources = new List<NavMeshBuildSource>();

        for (int i = 0; i < _surfaces.Length; i++)
        {
            _navMeshDatas[i] = new NavMeshData();
            NavMesh.AddNavMeshData(_navMeshDatas[i]);
            _sourcesPerSurface.Add(i, new List<NavMeshBuildSource>());
            _markupsPerSurface.Add(i, new List<NavMeshBuildMarkup>());
            _modifiersPerSurface.Add(i, new List<NavMeshModifier>());
        }

        await BuildNavMesh(false);
    }

    public Vector3 GetTheNavMeshSize()
    {
        return _navMeshSize;
    }

    public void Update()
    {
        if (_lastChecked > _updateRate)
        {
            if (_playerTransform != null && Vector3.Distance(_worldAnchor, _playerTransform.position) > _movementThreshold)
            {
                BuildNavMesh(true);
                _worldAnchor = _playerTransform.position;
            }

            _lastChecked = 0f;
        }

        _lastChecked += Time.deltaTime;
    }

    public async Task BuildNavMesh(bool Async)
    {
        _navMeshBounds = new Bounds(_playerTransform.position, _navMeshSize);

        _volumes = new List<NavMeshModifierVolume>();

        _volumesSources = new List<NavMeshBuildSource>();

        for (int index = 0; index < _surfaces.Length; index++)
        {
            if (_markupsPerSurface[index].Count == 0)
            {
                if (_surfaces[index].collectObjects == CollectObjects.Children && _modifiersPerSurface[index].Count == 0)
                {
                    _modifiersPerSurface[index] = new List<NavMeshModifier>(GetComponentsInChildren<NavMeshModifier>());
                }
                else if (_surfaces[index].collectObjects != CollectObjects.Children)
                {
                    _modifiersPerSurface[index] = NavMeshModifier.activeModifiers;
                }

                for (int i = 0; i < _modifiersPerSurface[index].Count; i++)
                {
                    if (((_surfaces[index].layerMask & (1 << _modifiersPerSurface[index][i].gameObject.layer)) != 0)
                        && _modifiersPerSurface[index][i].AffectsAgentType(_surfaces[index].agentTypeID))
                    {
                        _markupsPerSurface[index].Add(new NavMeshBuildMarkup()
                        {
                            root = _modifiersPerSurface[index][i].transform,
                            overrideArea = _modifiersPerSurface[index][i].overrideArea,
                            area = _modifiersPerSurface[index][i].area,
                            ignoreFromBuild = _modifiersPerSurface[index][i].ignoreFromBuild
                        });
                    }
                }
            }

            _volumes = NavMeshModifierVolume.activeModifiers;

            if (!_shouldCacheSources || _sourcesPerSurface[index].Count == 0)
            {
                if (_surfaces[index].collectObjects == CollectObjects.Children)
                {
                    NavMeshBuilder.CollectSources(transform, _surfaces[index].layerMask, _surfaces[index].useGeometry, _surfaces[index].defaultArea, _markupsPerSurface[index], _sourcesPerSurface[index]);
                }
                else
                {
                    NavMeshBuilder.CollectSources(_navMeshBounds, _surfaces[index].layerMask, _surfaces[index].useGeometry, _surfaces[index].defaultArea, _markupsPerSurface[index], _sourcesPerSurface[index]);
                }
            }

            for (int i = 0; i < _volumes.Count; i++) // add the volumes to the sources
            {
                if(((_surfaces[index].layerMask & (1 << _volumes[i].gameObject.layer)) != 0) && _volumes[i].AffectsAgentType(_surfaces[index].agentTypeID))
                {
                    var mcenter = _volumes[i].transform.TransformPoint(_volumes[i].center);
                    var scale = _volumes[i].transform.lossyScale;
                    var msize = new Vector3(_volumes[i].size.x * Mathf.Abs(scale.x), _volumes[i].size.y * Mathf.Abs(scale.y), _volumes[i].size.z * Mathf.Abs(scale.z)); // set the size of the volume

                    var src = new NavMeshBuildSource();
                    src.shape = NavMeshBuildSourceShape.ModifierBox;
                    src.transform = Matrix4x4.TRS(mcenter, _volumes[i].transform.rotation, Vector3.one);
                    src.size = msize;
                    src.area = _volumes[i].area;
                    _volumesSources.Add(src);
                }
            }

            _sourcesPerSurface[index].AddRange(_volumesSources);

            _sourcesPerSurface[index].RemoveAll(RemoveNavMeshAgentPredicate);  // remove other NavMeshAgents so they are not considered

            if (Async)  // Async bake or not
            {
                AsyncOperation navMeshUpdateOperation = NavMeshBuilder.UpdateNavMeshDataAsync(_navMeshDatas[index], _surfaces[index].GetBuildSettings(), _sourcesPerSurface[index], _navMeshBounds);
                navMeshUpdateOperation.completed += HandleNavMeshUpdate;
            }
            else
            {
                NavMeshBuilder.UpdateNavMeshData(_navMeshDatas[index], _surfaces[index].GetBuildSettings(), _sourcesPerSurface[index], _navMeshBounds);
                OnNavMeshUpdate?.Invoke(_navMeshBounds);
            }
        }

        await Task.Yield();
    }

    private bool RemoveNavMeshAgentPredicate(NavMeshBuildSource Source)
    {
        return Source.component != null && Source.component.gameObject.GetComponent<NavMeshAgent>() != null;
    }

    private void HandleNavMeshUpdate(AsyncOperation Operation)
    {
        OnNavMeshUpdate?.Invoke(new Bounds(_worldAnchor, _navMeshSize));
    }
}
