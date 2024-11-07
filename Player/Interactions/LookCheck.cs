using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LookCheck : MonoBehaviour
{
    [SerializeField] private Transform _playerCameraTransform;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private LootingConfig _lootingConfig;
    [SerializeField] private BuildingConfig _buildingConfig;
    [SerializeField] private LayerMask _layer = ~0;

    private bool _isLookingAtLootableObject = false;
    private bool _isLookingAtTrapManagerObject = false;

    public void CheckLookingAtLootableObject(RaycastHit hitResult)
    {
        if (hitResult.collider.TryGetComponent<LootableObject>(out var lootableObject) && !lootableObject.GetIsLooted())
        {
            if (!_isLookingAtLootableObject)
                UpdateLookingAtLootableObject(true);
        }
        else if (hitResult.collider.transform.parent != null && hitResult.collider.transform.parent.TryGetComponent<LootableObject>(out var parentLootableObject) && !parentLootableObject.GetIsLooted())
        {
            if (!_isLookingAtLootableObject)
                UpdateLookingAtLootableObject(true);
        }
        else if (_isLookingAtLootableObject)
        {
            UpdateLookingAtLootableObject(false);
        }
    }

    public void CheckLookingAtTrapManagerObject(RaycastHit hitResult)
    {
        if (hitResult.collider.TryGetComponent<TrapManager>(out var trapManager) && !trapManager.GetIsOccupied())
        {
            if(!_isLookingAtTrapManagerObject)
                UpdateLookingAtTrapManagerObject(true);
        }
        else if (_isLookingAtTrapManagerObject)
        {
            UpdateLookingAtTrapManagerObject(false);
        }
    }

    public void UpdateLookingAtLootableObject(bool isLooking)
    {
        if (isLooking)
        {
            _uiManager.ShowCrosshairText(_lootingConfig.CrosshairHelperLootingText);
            _isLookingAtLootableObject = true;
        }
        else
        {
            _uiManager.ShowCrosshairText("");
            _isLookingAtLootableObject = false;
        }
    }

    public void UpdateLookingAtTrapManagerObject(bool isLooking)
    {
        if (isLooking)
        {
            _uiManager.ShowCrosshairText(_buildingConfig.PlaceTrapHelperText);
            _isLookingAtTrapManagerObject = true;
        }
        else
        {
            _uiManager.ShowCrosshairText("");
            _isLookingAtTrapManagerObject = false;
        }
    }

    public void CheckLookingTarget()
    {
        RaycastHit hitResult;
        Ray ray = new Ray(_playerCameraTransform.position, _playerCameraTransform.forward);

        if (Physics.Raycast(ray, out hitResult, _layer))
        {
            if (hitResult.distance <= _lootingConfig.MinDistanceToLoot)
            {
                CheckLookingAtLootableObject(hitResult);
            }
            else if (_isLookingAtLootableObject)
            {
                UpdateLookingAtLootableObject(false);
            }

            if(BuildingManager.Instance.GetIsBuildingOpen() && !_isLookingAtLootableObject && hitResult.distance <= _buildingConfig.MinDistanceToBuild)
            {
                CheckLookingAtTrapManagerObject(hitResult);
            }
            else if (_isLookingAtTrapManagerObject)
            {
                UpdateLookingAtTrapManagerObject(false);
            }
        }
    }

    void Update()
    {
        CheckLookingTarget();
    }
}