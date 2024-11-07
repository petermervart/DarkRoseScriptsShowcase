using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CraftableItemHolder : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    // UI crafting item
    [SerializeField] 
    private CraftableItemConfig _craftableItemConfig;

    [SerializeField] 
    private Image _craftableItemForeground;

    [SerializeField]
    UnityEvent<CraftableItemConfig> OnCraftedItem = new UnityEvent<CraftableItemConfig>();

    [SerializeField]
    UnityEvent<bool> OnCraftingChanged = new UnityEvent<bool>();

    private float _currentFillAmount = 0f;

    private bool _isMouseOver = false;

    private bool _isPlayingSound = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Mouse Down");
        _isMouseOver = true;
        _currentFillAmount = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Mouse Up");
        _isMouseOver = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isMouseOver = false;
        Debug.Log("Mouse Exit");
    }

    public void ResetCraftingItem()
    {
        _currentFillAmount = 0f;
        _craftableItemForeground.fillAmount = 0f;
        OnCraftingChanged.Invoke(false);
        _isPlayingSound = false;
    }

    public void OnItemCrafted()
    {
        ResetCraftingItem();
        OnCraftedItem.Invoke(_craftableItemConfig);
    }

    public bool CheckIfCanBeCrafted()
    {
        return InventoryManager.Instance.CheckMaterialsAvailability(_craftableItemConfig.MatsToCraftItem);
    }

    public void CraftingHoldUpdate()
    {
        if (_isMouseOver && Mouse.current.leftButton.isPressed && CheckIfCanBeCrafted())
        {
            if (!_isPlayingSound)
            {
                _isPlayingSound = true;
                OnCraftingChanged.Invoke(true);
            }

            _currentFillAmount += Time.deltaTime / _craftableItemConfig.CraftingTime;

            _currentFillAmount = Mathf.Clamp01(_currentFillAmount);

            _craftableItemForeground.fillAmount = _currentFillAmount;

            if (_currentFillAmount >= 1f)
            {
                OnItemCrafted();
            }
        }
        else if(_currentFillAmount > 0)
        {
            ResetCraftingItem();
        }
    }

    private void Update()
    {
        CraftingHoldUpdate();
    }

    private void OnMouseOver()
    {
        _isMouseOver = true;
        Debug.Log("Mouse Over");
    }
}
