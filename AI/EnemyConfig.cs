using UnityEngine;


[CreateAssetMenu(menuName = "Enemy Config", fileName = "EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [Header("Enemy Management")]
    public int EnemyID;

    [Header("Combat")]
    public float MaxHealth;
    public float TimeStunnedAfterAttack;

    [Header("Navigation")]
    public float PathUpdateRate;

    [Header("Movement")]
    public float NormalSpeed = 5f;
    public float GroundTypeDistanceCheck = 7f;

    [Header("Audio")]
    public float StepInterval = 0.7f;
    public float MaxStepInterval = 1.2f;

    [Header("Death")]
    public float TimeToDisappearAfterDeath = 10f;

    [Header("Attacking")]
    public float MinDistanceToHitAttack = 2f;
    public float MinDistanceToStopAttackState = 3f;
    public float MinDistanceToStartAttack = 1.5f;
    public LayerMask AttackLayerMask = ~0;
    public float MinDamage = 20;
    public float MaxDamage = 40;
    public float AttackMaxAngle = 45f;
    public float RotationSpeed = 5f;

    [Header("Obstructions")]
    public float MinDistanceToStartDestroying = 1.5f;
    public float CheckForObstructionRate = 0.1f;
    public LayerMask ObstructionsLayerMask = ~0;
    public float ObstructionMinDamage = 10;
    public float ObstructionMaxDamage = 20;
    public float ObstructionAttackDelay = 0.5f;
    public float ObstructionTimeBetweenAttacks = 1f;

    [Header("Animation")]
    public float MinRunningAnimationSpeedRatio = 0.3f;
}