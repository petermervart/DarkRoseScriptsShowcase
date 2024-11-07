using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Consumable Config", fileName = "ConsumableConfig")]
public class ConsumableConfig : ScriptableObject
{
    public InventoryItemConfig InventoryItem;

    [Header("Heal")]
    public bool CanHeal = false;
    public float HealAmount;
}