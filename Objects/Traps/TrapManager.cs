using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour
{
    [SerializeField] 
    private List<TrapItem> _traps = new List<TrapItem>();

    private readonly Dictionary<int, TrapItem> _trapsDictionary = new Dictionary<int, TrapItem>();

    private TrapItem _currentTrap = null;

    private void Awake()
    {
        foreach (TrapItem trapItem in _traps)
        {
            _trapsDictionary.Add(trapItem.TrapObjectConfig.InventoryItem.ItemID, trapItem);
        }
    }

    public bool GetIsOccupied()
    {
        return _currentTrap != null;
    }

    // Managing traps, only one trap per door
    public bool OnTrapPlaced(TrapConfig newTrap)
    {
        Debug.Log("Activated");

        if (_currentTrap != null)
            return false;

        if (_trapsDictionary.TryGetValue(newTrap.InventoryItem.ItemID, out TrapItem trapItem))
        {
            _currentTrap = trapItem;
            _currentTrap.TrapObject.SetActive(true);
            return true;
        }
        else
        {
            Debug.LogError("Trap not in the dictionary");
            return false;
        }
    }

    public void OnTrapUsed()
    {
        _currentTrap.TrapObject.SetActive(false);
        _currentTrap = null;
    }
}
