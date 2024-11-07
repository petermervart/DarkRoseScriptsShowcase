using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "lootable Item Config", fileName = "LootableItemConfig")]
public class LootableItemConfig : ScriptableObject
{
    public Sprite IconSprite;
    public InventoryItemConfig InventoryItem;
}
