using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

public class CharacterMotor : MonoBehaviour
{
    [SerializeField] 
    protected CharacterMotorConfig _motorConfig;

    [SerializeField] 
    protected Transform _playerCamera;

    [SerializeField] 
    UnityEvent<bool> OnRunChanged = new UnityEvent<bool>();

    [SerializeField]
    UnityEvent<bool> OnIsMovingChanged = new UnityEvent<bool>();

    [SerializeField] 
    UnityEvent<Vector3> OnHitGround = new UnityEvent<Vector3>();

    [SerializeField] 
    UnityEvent<Vector3, float, ESurfaceType> OnFootStep = new UnityEvent<Vector3, float, ESurfaceType>();

    [SerializeField] 
    UnityEvent<Vector2> OnCameraChange = new UnityEvent<Vector2>();

    protected Rigidbody _playerRB;
    protected CapsuleCollider _playerCollider;
    protected float _currentCameraPitch = 0f;
    protected float _currentCameraYaw = 0f;
    protected float _timePassedSinceLastFootStep = 0f;
    protected float _timeInAir = 0f;
    protected float _headbobProgress = 0f;
    protected float _accumulatedRotation = 0f;
    protected float _currentRigidbodyYRotation = 0f;
    protected Vector3 _lastHeadBobOffset = Vector3.zero;

    public bool IsRunning { get; protected set; } = false;
    public bool IsMoving { get; protected set; } = false;
    public bool IsGrounded { get; protected set; } = true;
    public bool IsMovementLocked { get; protected set; } = false;
    public bool IsLookingLocked { get; protected set; } = false;
    public Transform CurrentParent { get; protected set; } = null;

    protected ESurfaceType _currentGroundMaterial = ESurfaceType.Ground;

    public float CurrentMaxSpeed
    {
        get
        {
            if (!IsGrounded && IsRunning)
                return _motorConfig.CanAirControl ? _motorConfig.AirSpeedRunning : 0;
            else if (!IsGrounded && !IsRunning)
                return _motorConfig.CanAirControl ? _motorConfig.AirSpeedWalking : 0;
            else
                return IsRunning ? _motorConfig.RunSpeed : _motorConfig.WalkSpeed;
        }
    }

    protected Vector2 Input_Move;
    protected void OnMove(InputValue value)
    {
        Input_Move = value.Get<Vector2>();
    }

    protected Vector2 Input_Look;
    protected void OnLook(InputValue value)
    {
        Input_Look = value.Get<Vector2>();
    }

    protected bool Input_Run;
    protected void OnRun(InputValue value)
    {
        Input_Run = value.isPressed;
    }

    private void Awake()
    {
        _playerRB = GetComponent<Rigidbody>();
        _playerCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        SetCursorLock(true);
        _playerCollider.radius = _motorConfig.CharacterRadius;
        _playerCollider.height = _motorConfig.CharacterHeight;
        _playerCollider.center = Vector3.up * (_motorConfig.CharacterHeight * 0.5f);
    }

    protected void FixedUpdate()
    {
        bool wasGrounded = IsGrounded;
        bool wasRunning = IsRunning;
        bool wasMoving = IsMoving;

        RaycastHit groundCheckResult = UpdateIsGrounded();

        UpdateRunning();

        if (wasRunning != IsRunning)
            OnRunChanged.Invoke(IsRunning);

        if (wasGrounded != IsGrounded && IsGrounded)
        {
            _timePassedSinceLastFootStep = 0f;
        }

        UpdateMovement(groundCheckResult);

        if (wasMoving != IsMoving)
            OnIsMovingChanged.Invoke(IsMoving);

        UpdateCharacterRotation();
    }

    protected void LateUpdate()
    {
        UpdateCamera();
        OnCameraChange.Invoke(Input_Look);

        if(!IsLookingLocked)
            _accumulatedRotation += Input_Look.x * Time.deltaTime;
    }

    protected RaycastHit UpdateIsGrounded()
    {
        Vector3 startPos = _playerRB.position + 0.5f * _motorConfig.CharacterHeight * Vector3.up;
        float groundCheckDistance = (_motorConfig.CharacterHeight * 0.5f) + _motorConfig.GroundCheckBuffer;
  
        RaycastHit hitResult;

        if (Physics.SphereCast(startPos, _motorConfig.GroundCheckRadiusBuffer, Vector3.down, out hitResult, groundCheckDistance, _motorConfig.GroundedLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (!IsGrounded)
            {
                _timePassedSinceLastFootStep = 0f;

                if (_timeInAir > _motorConfig.MinTimeForDropSound)
                {
                    OnHitGround.Invoke(_playerRB.position);
                }

                IsGrounded = true;
                _timeInAir = 0f;
            }

            if (hitResult.collider.TryGetComponent<SurfaceTypeHelper>(out var surface))
                _currentGroundMaterial = surface.GetSurfaceType();
        }
        else
        {
            _timeInAir += Time.fixedDeltaTime;
            if (IsGrounded)
                IsGrounded = false;
        }

        return hitResult;
    }

    protected void UpdateIsMoving()
    {
        if (Input_Move.magnitude > float.Epsilon)
            IsMoving = true;
        else
            IsMoving = false;
    }

    protected void UpdateRunning()
    {
        if (Input_Move.magnitude < float.Epsilon)
        {
            IsRunning = false;
            return;
        }

        if (!_motorConfig.CanRun)
        {
            IsRunning = false;
            return;
        }

        if (_motorConfig.RunToggle)
        {
            if (Input_Run && !IsRunning)
            {
                IsRunning = true;
            }
        }
        else
            IsRunning = Input_Run;
    }

    protected void UpdateFootStepsAudio()
    {
        if (IsMoving)
        {
            _timePassedSinceLastFootStep += Time.deltaTime;

            float currentStepInterval = IsRunning ? _motorConfig.FootStepInterval_Running : _motorConfig.FootStepInterval_Walking;
            if (_timePassedSinceLastFootStep > currentStepInterval)
            {
                OnFootStep.Invoke(_playerRB.position, _playerRB.velocity.magnitude, _currentGroundMaterial);
                _timePassedSinceLastFootStep -= currentStepInterval;
            }
        }
    }

    protected void UpdateMovement(RaycastHit groundCheckResult)
    {
        if (IsMovementLocked)
        {
            Input_Move = Vector2.zero;
        }

        UpdateIsMoving();

        Vector3 movementVector = (transform.forward * Input_Move.y + transform.right * Input_Move.x) * CurrentMaxSpeed;

        RaycastHit obstacleHit;

        if (IsGrounded)
        {
            // Project the movement vector onto the slope of the ground
            movementVector = Vector3.ProjectOnPlane(movementVector, groundCheckResult.normal);

            if (Vector3.Angle(Vector3.up, groundCheckResult.normal) > _motorConfig.SlopeLimit && _motorConfig.CanSlide)
            {
                movementVector += new Vector3(groundCheckResult.normal.x, -groundCheckResult.normal.y, groundCheckResult.normal.z) * _motorConfig.SlideSpeed;
            }

            UpdateFootStepsAudio();
        }

        // small obstacle handling since no jumping
        if (Physics.Raycast(_playerRB.position, movementVector.normalized, out obstacleHit, _motorConfig.ObstacleCheckBuffer, _motorConfig.ObstacleLayerMask, QueryTriggerInteraction.Ignore))
        {
            // Use a separate raycast to determine the height of the obstacle
            RaycastHit obstacleTopHit;
            Vector3 obstacleTopRayStart = _playerRB.position + Vector3.up * _motorConfig.MaxObstacleHeight;

            if (!Physics.Raycast(obstacleTopRayStart, movementVector.normalized, out obstacleTopHit, _motorConfig.ObstacleCheckBuffer, _motorConfig.ObstacleLayerMask, QueryTriggerInteraction.Ignore))
            {
                movementVector.y += _motorConfig.ObstacleForce;
            }
        }
        else if (!IsGrounded)
        {
            movementVector.y = -_motorConfig.FallVelocity;
        }

        _playerRB.velocity = Vector3.MoveTowards(_playerRB.velocity, movementVector, _motorConfig.SpeedTransition);
    }

    protected void UpdateCharacterRotation()
    {
        float cameraYawDelta = _accumulatedRotation * _motorConfig.Camera_HorizontalSensitivity * (_motorConfig.Camera_InvertX ? -1f : 1f);

        _currentRigidbodyYRotation = _currentRigidbodyYRotation + cameraYawDelta;

        _playerRB.rotation = Quaternion.Euler(0f, _currentRigidbodyYRotation, 0f);

        _accumulatedRotation = 0f;
    }

    // camera headbob and rotation calculation
    protected void UpdateCamera()
    {
        if (IsLookingLocked)
            return;

        float cameraYawDelta = Input_Look.x * _motorConfig.Camera_HorizontalSensitivity * Time.deltaTime * (_motorConfig.Camera_InvertX ? -1f : 1f);
        float cameraPitchDelta = Input_Look.y * _motorConfig.Camera_VerticalSensitivity * Time.deltaTime * (_motorConfig.Camera_InvertY ? 1f : -1f);

        _playerCamera.transform.position = transform.position + _motorConfig.CameraOffset;

        if (_motorConfig.Headbob_enable && IsGrounded)
        {
            float currentSpeed = _playerRB.velocity.magnitude;

            Vector3 defaultCameraOffset = Vector3.zero;
            if (currentSpeed >= _motorConfig.Headbob_MinSpeedToBob)
            {
                float speedFactor = currentSpeed / (_motorConfig.CanRun ? _motorConfig.RunSpeed : _motorConfig.WalkSpeed);
                _headbobProgress += Time.deltaTime / _motorConfig.Headbob_PeriodSpeed.Evaluate(speedFactor);
                _headbobProgress %= 1f;

                float MaxVerticalTranslation = _motorConfig.Headbob_VerticalTranslationSpeed.Evaluate(speedFactor);
                float MaxHorizontalTranslation = _motorConfig.Headbob_HorizontalTranslationSpeed.Evaluate(speedFactor);

                float sinProgress = Mathf.Sin(_headbobProgress * Mathf.PI * 2f);

                defaultCameraOffset += MaxVerticalTranslation * sinProgress * Vector3.up;
                defaultCameraOffset += MaxHorizontalTranslation * sinProgress * Vector3.right;
            }
            else
            {
                _headbobProgress = 0f;
            }

            _lastHeadBobOffset = Vector3.MoveTowards(_lastHeadBobOffset, defaultCameraOffset, _motorConfig.Headbob_TranslationSpeedBlend * Time.deltaTime);

            _playerCamera.transform.position += _lastHeadBobOffset;
        }

        _currentCameraPitch = Mathf.Clamp(_currentCameraPitch + cameraPitchDelta, _motorConfig.Camera_MinPitch, _motorConfig.Camera_MaxPitch);
        _currentCameraYaw += cameraYawDelta;
        _playerCamera.transform.localRotation = Quaternion.Euler(_currentCameraPitch, _currentCameraYaw, 0f);
    }

    public void SetCursorLock(bool locked)
    {
        Cursor.visible = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void SetMovementLock(bool locked)
    {
        IsMovementLocked = locked;
    }

    public void SetLookLock(bool locked)
    {
        IsLookingLocked = locked;
    }
    public void OnStopLookingAndMoving(bool isLooting)
    {
        SetMovementLock(isLooting);
        SetLookLock(isLooting);
    }
}

