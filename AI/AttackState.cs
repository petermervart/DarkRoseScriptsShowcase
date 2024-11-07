using System.Collections;
using UnityEngine;

public class AttackState : EnemyState
{
    protected override void OnEnter()
    {
        _enemyController.GetAnimator().SetBool("isattacking", true);
    }

    protected override EnemyState OnProcessState()
    {
        if (ShouldChangeToFollow())
            return _enemyController.GetFollowState();

        return this;
    }

    protected bool ShouldChangeToFollow()
    {
        if (!_enemyController.CheckDistanceToTarget(_enemyController.GetPlayer().position, _enemyConfig.MinDistanceToStopAttackState))
            return true;

        return false;
    }

    protected override void OnUpdate()
    {
        _enemyController.FaceTarget(_enemyController.GetPlayer().position);
    }

    public void OnStartAttack()
    {
        _enemyController.OnAttacked.Invoke();
    }

    public void OnAttack()
    {
        float radians = _enemyConfig.AttackMaxAngle * Mathf.Deg2Rad;

        Vector3 forwardDirection = _enemyController.transform.forward;

        // Check all hits for multiple objects in the future
        RaycastHit[] hits = Physics.SphereCastAll(_enemyController.transform.position, _enemyConfig.MinDistanceToHitAttack, forwardDirection, 0f, _enemyConfig.AttackLayerMask);

        foreach (RaycastHit hit in hits)
        {
            // Spherecast with angle check
            Vector3 directionToTarget = (hit.transform.position - _enemyController.transform.position).normalized;
            float dotProduct = Vector3.Dot(forwardDirection, directionToTarget);

            if (dotProduct >= Mathf.Cos(radians))
            {
                if (hit.collider.TryGetComponent<CharacterBehaviour>(out var script))
                {
                    Debug.Log("HIT PLAYER");
                    script.OnHit(Random.Range(_enemyConfig.MinDamage, _enemyConfig.MaxDamage));
                }
            }
        }
    }

    protected override void OnExit()
    {
        _enemyController.GetAnimator().SetBool("isattacking", false);
    }
}
