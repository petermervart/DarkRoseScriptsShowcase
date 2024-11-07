using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] 
    private EnemyConfig _enemyConfig;

    [Header("Navigation")]
    [SerializeField] 
    private NavMeshAgent _agent;

    [SerializeField] 
    private Transform _player;

    [Header("Animation")]
    [SerializeField] 
    private Animator _linkedAnimator;

    public UnityEvent OnGotHurt = new UnityEvent();

    public UnityEvent OnAttacked = new UnityEvent();

    public UnityEvent OnDeathEvent = new UnityEvent();

    public UnityEvent OnGotHitImpact = new UnityEvent();

    public UnityEvent<ESurfaceType> OnStep = new UnityEvent<ESurfaceType>();

    protected FollowState _followState = new FollowState();
    protected AttackState _attackState = new AttackState();
    protected StunnedState _stunnedState = new StunnedState();

    protected EnemyState _currentState;
    protected CapsuleCollider _collider;

    protected SlowDownTrap _currentSlowDownTrap = null;

    protected float _currentHealth;
    protected float _currentStunTime;

    protected bool _isSpeedDebuffActive = false;
    protected bool _isDead = false;

    public FollowState GetFollowState()
    {
        return _followState;
    }

    public AttackState GetAttackState()
    {
        return _attackState;
    }

    public StunnedState GetStunnedState()
    {
        return _stunnedState;
    }

    public bool GetIsDead()
    {
        return _isDead;
    }

    public Animator GetAnimator()
    {
        return _linkedAnimator;
    }

    public Transform GetPlayer()
    {
        return _player;
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return _agent;
    }

    public void SetPlayer(Transform playerObject)
    {
        _player = playerObject;
    }

    public float GetCurrentStunTime()
    {
        return _currentStunTime;
    }

    public bool CheckDistanceToTarget(Vector3 target, float minDistance)
    {
        Vector3 vecToTarget = transform.position - target;
        vecToTarget.y = 0f;

        if (vecToTarget.magnitude <= minDistance)
        {
            return true;
        }
        return false;
    }

    public void HandleCollider(bool isOn)
    {
        if (_collider != null)
        {
            _collider.enabled = isOn;
        }
    }

    public void ResumeAgent()
    {
        if (_agent.isOnNavMesh)
            _agent.isStopped = false;

        _agent.enabled = true;
    }

    public void StopAgent()
    {
        if(_agent.enabled == true)
        {
            _agent.velocity = Vector3.zero;
            _agent.isStopped = true;
            _agent.ResetPath();
            _agent.enabled = false;
        }
    }

    public void OnEnemyStun(float stunTime)
    {
        _currentStunTime += stunTime;

        if (_currentState != _stunnedState)
            ChangeState(_stunnedState);
    }

    public void OnEnemyDamaged(float damage)
    {
        _currentHealth -= damage;

        Debug.Log("Enemy Damaged");

        OnGotHitImpact.Invoke();

        if(_currentHealth <= 0)
        {
            OnDeath();
            return;
        }

        OnGotHurt.Invoke();

        if(_currentState != _stunnedState)
        {
            GotDamagedStun();
        }
    }

    protected IEnumerator GotDamageStunDelay(EnemyState currentState)
    {
        yield return new WaitForSeconds(_enemyConfig.TimeStunnedAfterAttack);

        _linkedAnimator.SetBool("gotHit", false);

        _currentState = currentState;

        ResumeAgent();
    }

    public void GotDamagedStun()
    {
        _linkedAnimator.SetBool("gotHit", true);

        StartCoroutine(GotDamageStunDelay(_currentState));

        StopAgent();

        _currentState = null;
    }

    public void OnEnemyStartedAttacked()
    {
        if (_currentState == _attackState)
        {
            _attackState.OnStartAttack();
        }
    }

    public void OnEnemyAttacked()
    {
        if(_currentState == _attackState)
        {
            _attackState.OnAttack();
        }
    }

    public void OnActivateSpeedDebuff(float speedDebuffRatio, SlowDownTrap trapAffectingEnemy)
    {
        if (_currentSlowDownTrap == null) // 2 slow down traps can't affect enemy at the same time
        {
            _currentSlowDownTrap = trapAffectingEnemy;
            _agent.speed = speedDebuffRatio * _enemyConfig.NormalSpeed;
            _linkedAnimator.SetFloat("speedMultiplier", Mathf.Clamp(speedDebuffRatio, _enemyConfig.MinRunningAnimationSpeedRatio, 1.0f));
        }
    }

    public void OnEnemyResetSpeed()
    {
        _currentSlowDownTrap = null;
        _agent.speed = _enemyConfig.NormalSpeed;
        _linkedAnimator.SetFloat("speedMultiplier", 1.0f);
    }

    protected IEnumerator DeathDestroyDelay()
    {
        // wait for N seconds before destroying the enemy
        yield return new WaitForSeconds(_enemyConfig.TimeToDisappearAfterDeath);

        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void OnDeath()
    {
        _isDead = true;
        _linkedAnimator.SetTrigger("death");
        _agent.enabled = false;
        _currentState = null;
        AgentSpawner.Instance.OnEnemyDied(_enemyConfig.EnemyID);

        OnDeathEvent.Invoke();

        if(_currentSlowDownTrap != null)
        {
            _currentSlowDownTrap.OnEnemyDiedInTrap();
        }

        HandleCollider(false);

        StartCoroutine(DeathDestroyDelay());
        Debug.Log("Enemy Dead");
    }

    public void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _enemyConfig.RotationSpeed);
    }

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        ChangeState(_followState);
        _currentHealth = _enemyConfig.MaxHealth;
        _agent.speed = _enemyConfig.NormalSpeed;
        _linkedAnimator.SetFloat("speedMultiplier", 1.0f);
    }

    void Update()
    {
        // Update Current State and change current state.

        if (_currentState == null)
            return;
        
        EnemyState newState = _currentState.OnStateProcessState();

        if (_currentState != newState)
            ChangeState(newState);

        if (_currentState != null)
        {
            _currentState.OnStateUpdate();
        }
    }
    public void ChangeState(EnemyState newState)
    {
        if (_currentState != null)
        {
            _currentState.OnStateExit();
        }
        _currentState = newState;
        _currentState.OnStateEnter(this, _enemyConfig);
    }
}
