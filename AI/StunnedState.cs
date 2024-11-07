using System.Collections;
using UnityEngine;


public class StunnedState : EnemyState
{
    private float _timeAlreadyStunned = 0f;

    protected override void OnEnter()
    {
        _timeAlreadyStunned = 0f;
        _enemyController.GetAnimator().SetBool("stunned", true);
        _enemyController.HandleCollider(false);
    }

    protected override void OnUpdate()
    {
        _timeAlreadyStunned += Time.deltaTime;
    }

    protected bool ShouldChangeToFollow()
    {
        if (!_enemyController.CheckDistanceToTarget(_enemyController.GetPlayer().position, _enemyConfig.MinDistanceToStartAttack) && _timeAlreadyStunned > _enemyController.GetCurrentStunTime())
            return true;

        return false;
    }

    protected bool ShouldChangeToAttack()
    {
        if (_enemyController.CheckDistanceToTarget(_enemyController.GetPlayer().position, _enemyConfig.MinDistanceToStartAttack) && _timeAlreadyStunned > _enemyController.GetCurrentStunTime())
            return true;

        return false;
    }

    protected override EnemyState OnProcessState()
    {
        if (ShouldChangeToAttack())
            return _enemyController.GetAttackState();

        if (ShouldChangeToFollow())
            return _enemyController.GetFollowState();

        return this;
    }

    protected override void OnExit()
    {
        _enemyController.GetAnimator().SetBool("stunned", false);
        _enemyController.HandleCollider(true);
    }

}
