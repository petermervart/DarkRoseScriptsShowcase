using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UnityEngine.Events;

public class AgentSpawner : MonoBehaviour
{
    //Spawn enemies on generated NavMesh

    public static AgentSpawner Instance;

    [SerializeField]
    private float _maxDistanceFromRandomStartPoint = 5.0f;  //Max distance from random point next to NavMesh (The point does not have to be on valid NavMesh, so closest point is chosen)

    //Handling of multiple enemy types with different navmeshes for the future
    [SerializeField]
    private NavMeshSurface[] _surfaces;

    [SerializeField]
    private SpawnerConfig[] _spawnerConfigs;

    [SerializeField]
    private Transform _player;

    [SerializeField]
    public UnityEvent OnSurvived = new UnityEvent();

    private int[] _currentSpawnedEnemiesAmount;

    private int[] _allSpawnedEnemiesAmount;

    private float _xNavMeshMaxDistance;
    private float _yNavMeshMaxDistance;
    private float _zNavMeshMaxDistance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _currentSpawnedEnemiesAmount = new int[_spawnerConfigs.Length];
        _allSpawnedEnemiesAmount = new int[_spawnerConfigs.Length];
        Vector3 _navMeshSize = NavMeshAreaBaker.Instance.GetTheNavMeshSize();

        _xNavMeshMaxDistance = _navMeshSize.x / 2.0f;  // Calculate the max for each axis before hand
        _yNavMeshMaxDistance = _navMeshSize.y / 2.0f;
        _zNavMeshMaxDistance = _navMeshSize.z / 2.0f;
    }

    public void OnEnemyDied(int enemyID)
    {
        _currentSpawnedEnemiesAmount[enemyID] -= 1;

        if (_allSpawnedEnemiesAmount[enemyID] < _spawnerConfigs[enemyID].MaxEnemiesAmountPerLevel)
        {
            SpawnNavMeshAgentOnBorder(enemyID);
        }
        else
        {
            OnSurvived.Invoke();  // if player kills all the enemies possible he wins
        }
    }

    public void StartSpawnEnemies()
    {
        for (int i = 0; i < _spawnerConfigs.Length; i++)
        {
            for (int index = 0; index < _spawnerConfigs[i].MaxEnemiesAmountAtOneTime; index++)
            {
                SpawnNavMeshAgentOnBorder(i);
            }
        }
    }

    public void SpawnNavMeshAgentOnBorder(int enemyID)
    {
        Vector3 randomPoint = GetRandomPointOnNavMeshBorder(enemyID);

        var newPrefab = Instantiate(_spawnerConfigs[enemyID].EnemyPrefab, randomPoint, Quaternion.identity);

        Enemy enemyComponent = newPrefab.GetComponent<Enemy>();

        if (enemyComponent != null)
            enemyComponent.SetPlayer(_player);

        _currentSpawnedEnemiesAmount[enemyID] += 1;
        _allSpawnedEnemiesAmount[enemyID] += 1;
    }

    public float GetRandomNumberInRanges(float minRange1, float maxRange1, float minRange2, float maxRange2)
    {
        if (Random.Range(0, 2) == 0)
        {
            return Random.Range(minRange1, maxRange1);
        }
        else
        {
            return Random.Range(minRange2, maxRange2);
        }
    }

    public int GetNavMeshMask()  // Build NavMeshMask to make sure enemy is not spawned on not allowed area
    {
        int mask = 0;
        mask += 1 << NavMesh.GetAreaFromName("Walkable");
        mask += 0 << NavMesh.GetAreaFromName("Not walkable");
        mask += 1 << NavMesh.GetAreaFromName("Jump");
        mask += 0 << NavMesh.GetAreaFromName("Destructable");

        return mask;
    }

    Vector3 GetRandomPointOnNavMeshBorder(int index)
    {
        Vector3 playerPosition = _player.transform.position;

        Vector3 randomOffset;

        if (Random.Range(0, 2) == 0) // Border on X, random on Z
        {
            randomOffset = new Vector3(
            GetRandomNumberInRanges(-_xNavMeshMaxDistance, -_spawnerConfigs[index].MinDistanceFromPlayer.x, _spawnerConfigs[index].MinDistanceFromPlayer.x, _xNavMeshMaxDistance),
            GetRandomNumberInRanges(-_yNavMeshMaxDistance, -_spawnerConfigs[index].MinDistanceFromPlayer.y, _spawnerConfigs[index].MinDistanceFromPlayer.y, _yNavMeshMaxDistance),
            Random.Range(-_zNavMeshMaxDistance, _zNavMeshMaxDistance)
        );
        }
        else // Border on Z, random on X
        {
            randomOffset = new Vector3(
            Random.Range(-_xNavMeshMaxDistance, _xNavMeshMaxDistance),
            GetRandomNumberInRanges(-_yNavMeshMaxDistance, -_spawnerConfigs[index].MinDistanceFromPlayer.y, _spawnerConfigs[index].MinDistanceFromPlayer.y, _yNavMeshMaxDistance),
            GetRandomNumberInRanges(-_zNavMeshMaxDistance, -_spawnerConfigs[index].MinDistanceFromPlayer.z, _spawnerConfigs[index].MinDistanceFromPlayer.z, _zNavMeshMaxDistance)
        );
        }

        Vector3 randomPoint = playerPosition + randomOffset;

        NavMeshHit hit;

        NavMeshSurface currentSurface = _surfaces[_spawnerConfigs[index].EnemyNavMeshIndex];

        NavMeshQueryFilter filter = new NavMeshQueryFilter { agentTypeID = currentSurface.agentTypeID, areaMask = GetNavMeshMask()};  // Build query with the agent ID and mask

        if (NavMesh.SamplePosition(randomPoint, out hit, _maxDistanceFromRandomStartPoint, filter))  // Get point on NavMesh
        {
            return hit.position;
        }

        return randomPoint;
    }
}
