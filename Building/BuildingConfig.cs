using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Building Config", fileName = "BuildingConfig")]

public class BuildingConfig : ScriptableObject
{
    [Header("Building Settings")]
    public float MinDistanceToBuild;
    public LayerMask BuildingLayer = ~0;

    [Header("UI Settings")]
    public string PlaceTrapHelperText;
    public string DoorAlreadyOccupiedText;
    public string MissingTrapHelperText;
    public float TimeToShowAlreadyOccupiedText;
    public float TimeToShowMissingTrapText;
}