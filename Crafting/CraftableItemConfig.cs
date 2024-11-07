using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Craftable Item Config", fileName = "CraftableItemConfig")]

public class CraftableItemConfig : ScriptableObject
{
    [Header("Crafting")]
    public List<CraftingMaterial> MatsToCraftItem = new List<CraftingMaterial>();
    public InventoryItemConfig InventoryItemType;
    public float CraftingTime;
}
