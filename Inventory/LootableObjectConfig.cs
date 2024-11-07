using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Lootable Object Config", fileName = "LootableObjectConfig")]

public class LootableObjectConfig : ScriptableObject
{
    [System.Serializable]
    public class PossibleLootItem
    {
        public int WeightToDropPerSlot;
        public int MaxAmountToDrop;
        public LootableItemConfig InventoryItem;
    }

    [Header("Loot")]
    public List<PossibleLootItem> PossibleLootedItems = new List<PossibleLootItem>();
    public List<int> AmountOfSlotsWeights = new List<int>();
    public float LootTime;
}
