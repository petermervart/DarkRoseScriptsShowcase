using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


public class CraftingManager : MonoBehaviour
{
    [SerializeField]
    UnityEvent<bool> OnPressedCrafting = new UnityEvent<bool>();

    private bool _isCraftingOpened = false;

    private bool _isCraftingLocked = false;

    public void OnLockCrafting(bool isLocked)
    {
        _isCraftingLocked = isLocked;
        if(_isCraftingOpened)
            HandleCrafting();
    }

    protected void HandleCrafting()
    {
        _isCraftingOpened = !_isCraftingOpened;
        OnPressedCrafting.Invoke(_isCraftingOpened);
    }

    protected void OnCraft(InputValue value)
    {
        if (!_isCraftingLocked)
        {
            HandleCrafting();
        }
    }
}
