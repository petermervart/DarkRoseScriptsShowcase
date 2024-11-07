using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [SerializeField]
    private Transform _playerCameraTransform;

    [SerializeField]
    private List<TrapConfig> _traps = new List<TrapConfig>();

    [SerializeField]
    private PlayerInput _playerInput;

    [SerializeField] 
    private BuildingConfig _buildingConfig;

    [SerializeField] 
    UnityEvent<bool> OnBuildingMenuChanged = new UnityEvent<bool>();

    [SerializeField] 
    UnityEvent<int> OnCurrentTrapChanged = new UnityEvent<int>();

    [SerializeField] 
    UnityEvent<string, float> OnAlreadyOccupied = new UnityEvent<string, float>();

    [SerializeField] 
    UnityEvent<string, float> OnNotEnoughTraps = new UnityEvent<string, float>();

    [SerializeField] 
    UnityEvent PlacedTrap = new UnityEvent();

    private int _currentTrap = 0;

    private bool _isBuildingOpen = false;

    private bool _isBuildingLocked = false;

    protected void OnBuild(InputValue value)
    {
        PressedBuilding();
    }

    protected void OnBuildFirstSlot(InputValue value)
    {
        if (_isBuildingOpen)
            HandleCurrenBuildChanged(0);
    }

    protected void OnBuildSecondSlot(InputValue value)
    {
        if (_isBuildingOpen)
            HandleCurrenBuildChanged(1);
    }

    protected void OnBuildThirdSlot(InputValue value)
    {
        if (_isBuildingOpen)
            HandleCurrenBuildChanged(2);
    }

    protected void OnBuildFourthSlot(InputValue value)
    {
        if (_isBuildingOpen)
            HandleCurrenBuildChanged(3);
    }

    protected void OnLeftClick(InputValue value)
    {
        if (_isBuildingOpen)
        {
            PlaceTrap();
            Debug.Log("Left Click");
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    public bool GetIsBuildingOpen()
    {
        return _isBuildingOpen;
    }

    public void OnLockBuilding(bool isLocked)
    {
        _isBuildingLocked = isLocked;
        if (_isBuildingOpen)
            HandleBuilding();
    }

    public void HandleBuilding()
    {
        _isBuildingOpen = !_isBuildingOpen;
        OnBuildingMenuChanged.Invoke(_isBuildingOpen);

        if (_isBuildingOpen)
        {
            _currentTrap = 0;
            OnCurrentTrapChanged.Invoke(0);
        }
    }

    public void PressedBuilding()
    {
        if (!_isBuildingLocked)
        {
            HandleBuilding();
        }
    }

    public void HandleCurrenBuildChanged(int index)
    {
        if (_isBuildingOpen && _currentTrap != index)
        {
            _currentTrap = index;
            OnCurrentTrapChanged.Invoke(index);
        }
    }

    public void PlaceTrap()
    {
        RaycastHit hitResult;
        Ray ray = new Ray(_playerCameraTransform.position, _playerCameraTransform.forward);

        if (Physics.Raycast(ray, out hitResult, _buildingConfig.MinDistanceToBuild, _buildingConfig.BuildingLayer))
        {
            if (hitResult.collider.TryGetComponent<TrapManager>(out var trapManager))
            {
                if (!InventoryManager.Instance.CheckItemAvailability(_traps[_currentTrap].InventoryItem)) // Check if trap in inventory
                {
                    OnNotEnoughTraps.Invoke(_buildingConfig.MissingTrapHelperText, _buildingConfig.TimeToShowMissingTrapText);
                    Debug.Log("Trap not available");
                    return;
                }

                bool wasPlaced = trapManager.OnTrapPlaced(_traps[_currentTrap]);  // try to place the trap

                if (!wasPlaced) // if not placed it is already occupied
                {
                    OnAlreadyOccupied.Invoke(_buildingConfig.DoorAlreadyOccupiedText, _buildingConfig.TimeToShowAlreadyOccupiedText);
                }
                else
                {
                    InventoryManager.Instance.RemoveItemFromInventory(_traps[_currentTrap].InventoryItem, 1); // if placed remove it from the inventory
                    PlacedTrap.Invoke();
                }
            }
        }
    }
}
