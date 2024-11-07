using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StunTrap : Trap
{
    [SerializeField] 
    protected float _trapActivationDelay = 0.3f;

    [SerializeField] 
    protected float _timeToDisappearInSeconds = 0.5f;

    [SerializeField] 
    protected Animator _trapAnimator;

    [SerializeField]
    protected UnityEvent OnTrapActivated = new UnityEvent();

    protected Enemy _targetEnemy;
    protected bool _isAlreadyActivated = false;
    protected bool _hasTarget = false;
    protected Coroutine _activateCoroutine = null;
    protected Coroutine _removeCoroutine = null;

    // trap can affect only one enemy
    protected void HandleTargetEntered(Collider target)
    {
        if (!_hasTarget && !_isAlreadyActivated)
        {
            _targetEnemy = target.gameObject.GetComponent<Enemy>();
            if (_targetEnemy != null)
            {
                _trapAnimator.SetTrigger("activate");
                _hasTarget = true;
            }
        }
    }

    protected override void TrapActivated()
    {
        _targetEnemy.OnEnemyStun(_trapConfig.StunDurationInSeconds);

        OnTrapActivated.Invoke();

        _removeCoroutine = StartCoroutine(TrapDelayRemove());
    }

    protected IEnumerator TrapDelayActivate()
    {
        yield return new WaitForSeconds(_trapActivationDelay);

        TrapActivated();

        _activateCoroutine = null;
    }

    protected IEnumerator TrapDelayRemove()
    {
        yield return new WaitForSeconds(_timeToDisappearInSeconds);

        TrapRemoved();

        _removeCoroutine = null;
    }

    protected override void TrapRemoved()
    {
        _hasTarget = false;
        _targetEnemy = null;
        _isAlreadyActivated = false;
        _trapManager.OnTrapUsed();
    }

    protected void OnTriggerEnter(Collider other)
    {
        HandleTargetEntered(other);
    }

    protected void Update()
    {
        if (!_isAlreadyActivated && _hasTarget)
        {
            _isAlreadyActivated = true;
            _activateCoroutine = StartCoroutine(TrapDelayActivate());
        }
    }
}