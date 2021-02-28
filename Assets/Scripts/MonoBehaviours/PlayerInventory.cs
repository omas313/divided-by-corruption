using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    Dictionary<PickupType, int> _inventory = new Dictionary<PickupType, int>();

    public bool HasPickupTypeAmount(PickupType pickupType, int amount)
    {
        if (!_inventory.ContainsKey(pickupType))
            return false;

        if (_inventory[pickupType] >= amount)
            return true;
        else
            return false;
    }

    public void Take(PickupType pickupType, int amount)
    {
        if (!_inventory.ContainsKey(pickupType))
        {
            Debug.Log("Error: Trying to take somethign that doesn't exist in PlayerInventory");
            return;
        }

        _inventory[pickupType] -= amount;
        EnvironmentEvents.InvokePlayerInventoryUpdated(_inventory);
    }

    void OnPlayerPickedUpObject(PickupType pickupType)
    {
        AddToInventory(pickupType);        
    }

    void AddToInventory(PickupType pickupType)
    {
        if (!_inventory.ContainsKey(pickupType))
            _inventory[pickupType] = 0;

        
        _inventory[pickupType]++;

        EnvironmentEvents.InvokePlayerInventoryUpdated(_inventory);
    }

    void Awake()
    {
        EnvironmentEvents.PlayerPickedUpObject += OnPlayerPickedUpObject;
    }

    void OnDestroy()
    {
        EnvironmentEvents.PlayerPickedUpObject -= OnPlayerPickedUpObject;
    }
}
