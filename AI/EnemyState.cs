using System.Collections;
using UnityEngine;

public abstract class EnemyState
{
    // Base class for enemy state

    protected Enemy _enemyController;
    protected EnemyConfig _enemyConfig;

    public void OnStateEnter(Enemy enemyController, EnemyConfig enemyConfig)
    {
        _enemyController = enemyController;
        _enemyConfig = enemyConfig;
        OnEnter();
    }

    protected virtual void OnEnter(){}

    public void OnStateUpdate()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate(){}

    public EnemyState OnStateProcessState()
    {
        return OnProcessState();
    }
    protected virtual EnemyState OnProcessState()
    {
        return this;
    }

    public void OnStateExit()
    {
        OnExit();
    }

    protected virtual void OnExit(){}
}
