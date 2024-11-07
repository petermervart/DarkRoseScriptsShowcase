using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Spawner Config", fileName = "SpawnerConfig")]

// Config for enemy spawning
public class SpawnerConfig : ScriptableObject
{
    [Header("Enemies Settings")]
    public int MaxEnemiesAmountAtOneTime;
    public int MaxEnemiesAmountPerLevel;
    public GameObject EnemyPrefab;
    public EnemyConfig EnemyConfig;

    [Header("Spawning Settings")]
    public int EnemyNavMeshIndex;
    public Vector3 MinDistanceFromPlayer;
}