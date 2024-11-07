using System.Collections;
using UnityEngine;


public class DamageTrap : StunTrap
{
    protected override void TrapActivated()
    {
        _targetEnemy.OnEnemyDamaged(Random.Range(_trapConfig.MinDamage, _trapConfig.MaxDamage));

        _isAlreadyActivated = true;

        OnTrapActivated.Invoke();

        _removeCoroutine = StartCoroutine(TrapDelayRemove());
    }
}