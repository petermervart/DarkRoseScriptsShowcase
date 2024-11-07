using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(menuName = "Looting Config", fileName = "LootingConfig")]

public class LootingConfig : ScriptableObject
{
    [Header("Looting Settings")]
    public float MinDistanceToLoot;

    [Header("UI Settings")]
    public string CrosshairHelperLootingText;
    public string CrosshairHelperEmptyText;
    public float TimeToShowAlreadyLootedText;
    public GameObject LootedItemShowPrefab;
    public float TimeBetweenLootedItemShow;
}
