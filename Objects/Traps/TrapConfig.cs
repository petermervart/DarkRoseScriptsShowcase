using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Trap Config", fileName = "TrapConfig")]
public class TrapConfig : ScriptableObject
{
    public InventoryItemConfig InventoryItem;

    [Header("Block")]
    public bool CanBlock = false;
    public float MaxHealth;

    [Header("Damage")]
    public bool CanDamage = false;
    public int MinDamage = 10;
    public int MaxDamage = 30;

    [Header("Speed")]
    public bool CanSlowEnemies = false;

    [Range(0f, 1f)] 
    public float SpeedDebuff = 0.5f;

    [Header("Stun")]
    public bool CanStun = false;
    public float StunDurationInSeconds = 10;

}