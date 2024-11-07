using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowState : EnemyState
{
    private float _timer = 0f;

    private float _timeSinceLastCheck = 0f;

    private DestructableObstruction _currentDestructable = null;

    private bool _isDestroyingObstruction = false;

    private float _timeSinceLastAttack = 0f;

    private bool _canAttack = true;

    protected float _timePassedSinceLastFootStep = 0f;

    protected override void OnEnter()
    {
        _timer = 0f;
        _enemyController.ResumeAgent();
    }

    protected override void OnUpdate()
    {
        if (_isDestroyingObstruction)
            DamageDestructibleObstruction();
        else
        {
            HandleFollowing();
            HandleSteps();
        }
    }

    protected void HandleFollowing()
    {
        _timer += Time.deltaTime;

        if (_enemyController.GetNavMeshAgent().enabled && _timer >= _enemyConfig.PathUpdateRate) // update the destinatio based on the update rate
        {
            if (_enemyController.GetNavMeshAgent().isOnNavMesh)
            {
                _enemyController.GetNavMeshAgent().SetDestination(_enemyController.GetPlayer().position);
            }

            _timer = 0f;
        }

        CheckForDestructibleObstructions();
    }

    protected void HandleSteps()
    {
        _timePassedSinceLastFootStep += Time.deltaTime;

        float currentStepInterval = (_enemyConfig.NormalSpeed / _enemyController.GetNavMeshAgent().speed) * _enemyConfig.StepInterval;

        if(currentStepInterval > _enemyConfig.MaxStepInterval)
            currentStepInterval = _enemyConfig.MaxStepInterval;

        if (_timePassedSinceLastFootStep > currentStepInterval)
        {
            ESurfaceType groundMaterial = ESurfaceType.Ground;

            if(Physics.Raycast(_enemyController.transform.position, Vector3.down, out RaycastHit hitResult, _enemyConfig.GroundTypeDistanceCheck))
            {
                if (hitResult.collider.TryGetComponent<SurfaceTypeHelper>(out var surface))
                    groundMaterial = surface.GetSurfaceType();
            }

            _enemyController.OnStep.Invoke(groundMaterial);
            _timePassedSinceLastFootStep -= currentStepInterval;
        }
    }

    private void CheckForDestructibleObstructions()
    {
        _timeSinceLastCheck += Time.deltaTime;

        if (_timeSinceLastCheck > _enemyConfig.CheckForObstructionRate)
        {
            _timeSinceLastCheck = 0f;

            Vector3[] corners = new Vector3[2];
            int length = _enemyController.GetNavMeshAgent().path.GetCornersNonAlloc(corners);

            // Check if the barricade is in front of the enemy, if it is start destroying it

            if (length > 1 && Physics.Raycast(corners[0], (corners[1] - corners[0]).normalized, out RaycastHit hit, _enemyConfig.MinDistanceToStartDestroying, _enemyConfig.ObstructionsLayerMask))
            {
                if(hit.collider.TryGetComponent(out DestructableObstruction destructible))
                {
                    _isDestroyingObstruction = true;
                    _currentDestructable = destructible;
                    _currentDestructable.OnDestroy += OnDestructableDestroyed;
                    _enemyController.StopAgent();
                    _enemyController.GetAnimator().SetBool("isattacking", true);
                }
            }
        }
    }

    private void DamageDestructibleObstruction()
    {
        if (_canAttack)
        {
            if (_timeSinceLastAttack >= _enemyConfig.ObstructionAttackDelay)
            {
                _currentDestructable.OnTakeDamage(Random.Range(_enemyConfig.ObstructionMinDamage, _enemyConfig.ObstructionMaxDamage));
                Debug.Log("Damaged the destructable");
                _canAttack = false;
                _timeSinceLastAttack = 0f;
            }
            else
            {
                _timeSinceLastAttack += Time.deltaTime;
            }
        }
        else
        {
            if (_timeSinceLastAttack >= _enemyConfig.ObstructionTimeBetweenAttacks)
            {
                _canAttack = true;
                _timeSinceLastAttack = 0f;
            }
            else
            {
                _timeSinceLastAttack += Time.deltaTime;
            }
        }

        _enemyController.FaceTarget(_enemyController.GetPlayer().position);
    }

    private void OnDestructableDestroyed()
    {
        // if barricade was destroyed by anybody stop destroying
        if (!_enemyController.GetIsDead())
        {
            _isDestroyingObstruction = false;
            _currentDestructable = null;
            _enemyController.ResumeAgent();
            _enemyController.GetAnimator().SetBool("isattacking", false);
        }
    }


    protected bool ShouldChangeToAttack()
    {
        if (_enemyController.CheckDistanceToTarget(_enemyController.GetPlayer().position, _enemyConfig.MinDistanceToStartAttack))
            return true;

        return false;
    }

    protected override EnemyState OnProcessState()
    {
        if (ShouldChangeToAttack())
            return _enemyController.GetAttackState();

        return this;
    }

    protected override void OnExit()
    {
        _enemyController.StopAgent();
    }
}
