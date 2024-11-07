using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory Item Config", fileName = "inventoryItemConfig")]

public class InventoryItemConfig : ScriptableObject
{
    public int ItemID;
    public string Name;
    public int MaxAmount;
}
