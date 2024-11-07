using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class SlowDownTrap : Trap
{
    [SerializeField]
    protected UnityEvent<bool> OnTrapActivated = new UnityEvent<bool>();

    protected float _currentlyAffectedAmount = 0;
    protected bool _isAlreadyActivated = false;
    protected bool _hasTarget = false;
    protected Coroutine _activateCoroutine = null;
    protected Coroutine _removeCoroutine = null;

    public void OnEnemyDiedInTrap()
    {
        _currentlyAffectedAmount -= 1;

        if (_currentlyAffectedAmount == 0)
            TrapDisabled();
    }

    protected void HandleTargetEntered(Collider target)
    {
        Enemy enemy = target.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.OnActivateSpeedDebuff(_trapConfig.SpeedDebuff, this);

            if(_currentlyAffectedAmount == 0)
                TrapActivated();

            _currentlyAffectedAmount += 1;
        }
    }

    protected void HandleTargetExited(Collider target)
    {
        // if 0 enemies are in the trap stop playing the sound

        Enemy enemy = target.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.OnEnemyResetSpeed();
            _currentlyAffectedAmount -= 1;

            if(_currentlyAffectedAmount == 0)
                TrapDisabled();
        }
    }

    protected override void TrapActivated()
    {
        OnTrapActivated.Invoke(true);
    }

    protected override void TrapDisabled()
    {
        OnTrapActivated.Invoke(false);
    }

    protected void OnTriggerEnter(Collider other)
    {
        HandleTargetEntered(other);
    }

    protected void OnTriggerExit(Collider other)
    {
        HandleTargetExited(other);
    }
}
