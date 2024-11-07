using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DestructableObstruction : MonoBehaviour
{
    [SerializeField] 
    private TrapConfig _trapConfig;

    [SerializeField] 
    private TrapManager _trapManager;

    private float _currentHealth;

    [SerializeField]
    UnityEvent OnDamaged = new UnityEvent();

    [SerializeField]
    UnityEvent OnDestroyedEvent = new UnityEvent();

    public delegate void DestroyEvent();
    public DestroyEvent OnDestroy;

    private bool _isPlaced = false;

    private void OnEnable()
    {
        Debug.Log("Obstruction Placed");
        _isPlaced = true;
    }

    private void Update()
    {
        if (_isPlaced)
        {
            NavMeshAreaBaker.Instance.BuildNavMesh(true);
            _isPlaced = false;
        }
    }

    public void OnTakeDamage(float damage)
    {
        _currentHealth -= damage;
        if(_currentHealth < 0)
        {
            OnDestroyed();
            return;
        }
        OnDamaged.Invoke();
    }

    public void OnDestroyed()
    {
        _currentHealth = _trapConfig.MaxHealth;

        _trapManager.OnTrapUsed();

        OnDestroyedEvent.Invoke();

        // notify all the enemies focusing this trap to stop destroying it
        OnDestroy?.Invoke();

        gameObject.SetActive(false);

        NavMeshAreaBaker.Instance.BuildNavMesh(true);
    }

    private void Start()
    {
        _currentHealth = _trapConfig.MaxHealth;
    }

}