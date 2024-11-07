using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class StorableItem
{
    public InventoryItemConfig InventoryItem;

    public List<TextMeshProUGUI> TextAmountOfItem;

    [HideInInspector] 
    public int CurrentAmount = 0;

    public void AddAmount(int amount)
    {
        CurrentAmount += amount;
    }

    public void ReduceAmount(int amount)
    {
        CurrentAmount -= amount;
    }

    public void UpdateAmountText()
    {
        foreach(TextMeshProUGUI text in TextAmountOfItem)
        {
            text.SetText(CurrentAmount.ToString());
        }
    }

    public bool CheckIfAvailable(int amount)
    {
        if (amount <= CurrentAmount)
            return true;

        return false;
    }
}
