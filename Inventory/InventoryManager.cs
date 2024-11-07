using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] 
    private Transform _playerCameraTransform;

    [SerializeField] 
    private LootingConfig _lootingConfig;

    [SerializeField] 
    private List<StorableItem> _storableItems = new List<StorableItem>();

    [SerializeField] 
    UnityEvent<bool> OnChangedLootingState = new UnityEvent<bool>();

    [SerializeField] 
    UnityEvent<float> OnUpdateTheLootingBar = new UnityEvent<float>();

    [SerializeField] 
    UnityEvent<string, float> OnAlreadyLooted = new UnityEvent<string, float>();

    [SerializeField] 
    UnityEvent<string, Sprite> OnLootedNewItem = new UnityEvent<string, Sprite>();

    private bool _isLooting = false;
    private readonly Dictionary<int, StorableItem> _itemDictionary = new Dictionary<int, StorableItem>();
    private LootableObject _currentLootableObject;

    protected void OnLoot(InputValue value)
    {
        if (value.isPressed && !_isLooting)
        {
            GetLootedItems();

            Debug.Log("Pressed Loot");
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);

        foreach (StorableItem storableItem in _storableItems) // build dictionary from the list, so faster checking
        {
            _itemDictionary.Add(storableItem.InventoryItem.ItemID, storableItem);
        }
    }

    public bool CheckMaterialsAvailability(List<CraftingMaterial> materials)
    {
        bool areItemsAvailable = true;
        foreach (CraftingMaterial item in materials)
        {
            if (_itemDictionary.TryGetValue(item.CraftingMaterialInventory.ItemID, out StorableItem storableItem))
            {
                areItemsAvailable = areItemsAvailable && storableItem.CheckIfAvailable(item.CraftingMaterialAmount);
            }
        }

        return areItemsAvailable;
    }

    public bool CheckItemAvailability(InventoryItemConfig item)
    {
        if (_itemDictionary.TryGetValue(item.ItemID, out StorableItem storableItem))
        {
            return storableItem.CheckIfAvailable(1);
        }

        return false;
    }

    public int AddItemToInventory(InventoryItemConfig item, int amount)
    {
        if (_itemDictionary.TryGetValue(item.ItemID, out StorableItem storableItem))
        {
            storableItem.AddAmount(amount);
            storableItem.UpdateAmountText();
            return storableItem.CurrentAmount;
        }
        else
        {
            Debug.LogWarning("Item with ID " + item.ItemID + " not found in the inventory.");
            return -1;
        }
    }

    public int RemoveItemFromInventory(InventoryItemConfig item, int amount)
    {
        if (_itemDictionary.TryGetValue(item.ItemID, out StorableItem storableItem))
        {
            storableItem.ReduceAmount(amount);
            storableItem.UpdateAmountText();
            return storableItem.CurrentAmount;
        }
        else
        {
            Debug.LogWarning("Item with ID " + item.ItemID + " not found in the inventory.");
            return -1;
        }
    }

    public void OnCraftedItem(CraftableItemConfig craftingItem)
    {
        foreach (CraftingMaterial item in craftingItem.MatsToCraftItem)
        {
            RemoveItemFromInventory(item.CraftingMaterialInventory, item.CraftingMaterialAmount);
        }

        AddItemToInventory(craftingItem.InventoryItemType, 1);
    }

    public void CheckLootedObject(LootableObject lootableObject)
    {
        if (!lootableObject.GetIsLooted())
        {
            _currentLootableObject = lootableObject;
            StartCoroutine(LootDelayCoroutine());
        }
        else
        {
            OnAlreadyLooted.Invoke(_lootingConfig.CrosshairHelperEmptyText, _lootingConfig.TimeToShowAlreadyLootedText);
        }
    }

    public void StartLooting()
    {
        OnChangedLootingState.Invoke(true);
        _isLooting = true;
    }

    public void StopLooting()
    {
        OnChangedLootingState.Invoke(false);
        _isLooting = false;
    }

    IEnumerator LootDelayCoroutine()
    {
        float elapsedTime = 0f;

        StartLooting();

        while (elapsedTime < _currentLootableObject.GetLootingTime())
        {
            float fillAmount = Mathf.Clamp01(elapsedTime / _currentLootableObject.GetLootingTime());
            OnUpdateTheLootingBar.Invoke(fillAmount);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StopLooting();

        StartCoroutine(InstantiateLootWithDelay(_currentLootableObject.GenerateLoot()));
    }

    IEnumerator InstantiateLootWithDelay(List<LootableObject.LootedItem> lootItems)
    {
        foreach (LootableObject.LootedItem item in lootItems)
        {
            int newAmount = AddItemToInventory(item.LootableItem.InventoryItem, item.Amount);

            if (newAmount == -1)
                continue;

            string lootText = "+ " + item.Amount.ToString() + " (" + newAmount.ToString() + ")";
            Sprite icon = item.LootableItem.IconSprite;

            OnLootedNewItem.Invoke(lootText, icon);

            yield return new WaitForSeconds(_lootingConfig.TimeBetweenLootedItemShow);
        }
    }

    public void GetLootedItems()
    {
        RaycastHit hitResult;
        Ray ray = new Ray(_playerCameraTransform.position, _playerCameraTransform.forward);

        if (Physics.Raycast(ray, out hitResult, _lootingConfig.MinDistanceToLoot))
        {
            if (hitResult.collider.TryGetComponent<LootableObject>(out var lootableObject))
            {
                CheckLootedObject(lootableObject);
            }
            else if (hitResult.collider.transform.parent != null) // check if the object's parent has the script on it
            {
                if (hitResult.collider.transform.parent.TryGetComponent<LootableObject>(out var parentLootableObject))
                {
                    CheckLootedObject(parentLootableObject);
                }
            }
        }
    }
}