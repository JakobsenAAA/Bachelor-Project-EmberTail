using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private readonly Dictionary<string, int> items =
        new Dictionary<string, int>();

    public event Action InventoryChanged;

    public void AddItem(
        ItemData item,
        int amount = 1
    )
    {
        if (item == null)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        string itemId =
            item.ItemId;

        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning(
                "Tried to add an item with no Item ID."
            );

            return;
        }

        if (items.ContainsKey(itemId))
        {
            items[itemId] += amount;
        }
        else
        {
            items.Add(
                itemId,
                amount
            );
        }

        Debug.Log(
            "Added " +
            amount +
            " " +
            item.DisplayName +
            ". Total: " +
            items[itemId]
        );

        InventoryChanged?.Invoke();
    }

    public bool RemoveItem(
        ItemData item,
        int amount = 1
    )
    {
        if (item == null)
        {
            return false;
        }

        return RemoveItem(
            item.ItemId,
            amount
        );
    }

    public bool RemoveItem(
        string itemId,
        int amount = 1
    )
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return false;
        }

        if (amount <= 0)
        {
            return false;
        }

        if (!HasItem(itemId, amount))
        {
            return false;
        }

        items[itemId] -= amount;

        if (items[itemId] <= 0)
        {
            items.Remove(itemId);
        }

        InventoryChanged?.Invoke();

        return true;
    }

    public bool HasItem(
        ItemData item,
        int amount = 1
    )
    {
        if (item == null)
        {
            return false;
        }

        return HasItem(
            item.ItemId,
            amount
        );
    }

    public bool HasItem(
        string itemId,
        int amount = 1
    )
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return false;
        }

        if (amount <= 0)
        {
            return false;
        }

        return
            items.TryGetValue(
                itemId,
                out int currentAmount
            ) &&
            currentAmount >= amount;
    }

    public int GetItemAmount(
        ItemData item
    )
    {
        if (item == null)
        {
            return 0;
        }

        return GetItemAmount(
            item.ItemId
        );
    }

    public int GetItemAmount(
        string itemId
    )
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return 0;
        }

        if (
            items.TryGetValue(
                itemId,
                out int amount
            )
        )
        {
            return amount;
        }

        return 0;
    }

    public void ClearInventory()
    {
        items.Clear();

        InventoryChanged?.Invoke();
    }
}