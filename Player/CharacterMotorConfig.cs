using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Motor Config", fileName = "CharacterMotorConfig")]
public class CharacterMotorConfig : ScriptableObject
{
    [Header("Obstacles")]
    public float ObstacleCheckBuffer = 0.2f;
    public LayerMask ObstacleLayerMask = ~0;
    public float ObstacleForce = 0.6f;
    public float MaxObstacleHeight = 0.3f;

    [Header("Character")]
    public float CharacterHeight = 1.9f;
    public float CharacterRadius = 0.3f;

    [Header("Grounded Check")]
    public LayerMask GroundedLayerMask = ~0;
    public float GroundCheckBuffer = 0.1f;
    public float GroundCheckRadiusBuffer = 0.05f;

    [Header("Audio")]
    public float FootStepInterval_Walking = 0.5f;
    public float FootStepInterval_Running = 0.25f;
    public float MinTimeForDropSound = 0.5f;

    [Header("Camera")]
    public bool Camera_InvertY = false;
    public bool Camera_InvertX = false;
    public float Camera_HorizontalSensitivity = 5f;
    public float Camera_VerticalSensitivity = 5f;
    public float Camera_MinPitch = -75f;
    public float Camera_MaxPitch = 75f;
    public Vector3 CameraOffset;

    [Header("HeadBob")]
    public bool Headbob_enable = true;
    public float Headbob_MinSpeedToBob = 0.2f;
    public float Headbob_TranslationSpeedBlend = 1f;
    public AnimationCurve Headbob_VerticalTranslationSpeed;
    public AnimationCurve Headbob_HorizontalTranslationSpeed;
    public AnimationCurve Headbob_PeriodSpeed;

    [Header("Movement")]
    public float WalkSpeed = 7f;
    public float RunSpeed = 15f;
    public float SlideSpeed = 10f;
    public float FallVelocity = 5f;
    public bool CanSlide = true;
    public bool CanRun = true;
    public bool RunToggle = true;
    public float SlopeLimit = 60f;
    public float SpeedTransition = 1f;

    [Header("AirControl")]
    public bool CanAirControl = true;
    public float AirSpeedRunning = 10f;
    public float AirSpeedWalking = 5f;
}
