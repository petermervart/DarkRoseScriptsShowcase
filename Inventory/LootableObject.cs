using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LootableObject : MonoBehaviour
{
    [SerializeField]
    private LootableObjectConfig _lootableObjectConfig;

    [SerializeField]
    private Material _lootedMaterial;

    private bool _isLooted = false;

    [System.Serializable]
    public class LootedItem
    {
        public int Amount;
        public LootableItemConfig LootableItem;
    }

    private void ChangeMaterialsForImmediateChildren()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
        {
            ChangeMaterialsForRenderer(renderer);
        }
    }

    private void ChangeMaterialsForRenderer(Renderer renderer)
    {
        if (renderer != null)
        {
            renderer.material = _lootedMaterial;
        }
        else
        {
            Debug.LogError("Renderer component is null.");
        }
    }

    public float GetLootingTime()
    {
        return _lootableObjectConfig.LootTime;
    }

    public bool GetIsLooted()
    {
        return _isLooted;
    }

    private static int GetRandomNumber(List<int> weights)
    {
        int _totalWeight = 0;

        foreach (int weight in weights)
        {
            _totalWeight += weight;
        }

        return Random.Range(0, _totalWeight);
    }

    private static int SelectIndex(List<int> weights)
    {
        if (weights == null || weights.Count == 0)
        {
            Debug.Log("The weights list was empty");
            return -1;
        }

        int _randomValue = GetRandomNumber(weights);
        int _sum = 0;

        for (int i = 0; i < weights.Count; i++)
        {
            _sum += weights[i];

            if (_randomValue < _sum)
            {
                return i;
            }
        }

        // This should not happen, but just in case
        return weights.Count - 1;
    }

    // Generate loot with weighted random on how many slots and what item per slot. (both weighted)
    public List<LootedItem> GenerateLoot()
    {
        int _amountOfSlots = SelectIndex(_lootableObjectConfig.AmountOfSlotsWeights);

        Debug.Log("Amount looted: " + _amountOfSlots.ToString());

        List<LootedItem> _lootedItems = new List<LootedItem>();

        List<int> _listOfWeights = _lootableObjectConfig.PossibleLootedItems.Select(item => item.WeightToDropPerSlot).ToList();

        for (int i = 0; i < _amountOfSlots; i++)
        {
            int _indexSlotItem = SelectIndex(_listOfWeights);

            if(_indexSlotItem == -1)
            {
                break;
            }

            int _randomAmount = Random.Range(1, _lootableObjectConfig.PossibleLootedItems[_indexSlotItem].MaxAmountToDrop + 1);
            LootedItem _newLootedItem = new LootedItem { LootableItem = _lootableObjectConfig.PossibleLootedItems[_indexSlotItem].InventoryItem, Amount = _randomAmount };
            _lootedItems.Add(_newLootedItem);
        }

        foreach(LootedItem item in _lootedItems)
        {
            Debug.Log("Item: " + item.LootableItem.InventoryItem.Name.ToString() + " Amount: " + item.Amount.ToString());
        }

        _isLooted = true;

        ChangeMaterialsForImmediateChildren();

        return _lootedItems;
    }
}
