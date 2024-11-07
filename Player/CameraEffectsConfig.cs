using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera Effects Config", fileName = "CameraEffectsConfig")]
public class CameraEffectsConfig : ScriptableObject
{
    [Header("FOV")]
    public float WalkingFOV = 60f;
    public float RunningFOV = 80f;
    public float FOVRate = 30f;
}
