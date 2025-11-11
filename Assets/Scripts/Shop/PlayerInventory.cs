using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Simple inventory system for consumable items
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [System.Serializable]
    public class InventoryItem
    {
        public string itemId;
        public string itemName;
        public int quantity;
        public Sprite icon;
    }

    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();

    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(string itemId, string itemName, Sprite icon, int quantity = 1)
    {
        InventoryItem existingItem = items.Find(item => item.itemId == itemId);

        if (existingItem != null)
        {
            existingItem.quantity += quantity;
        }
        else
        {
            items.Add(new InventoryItem
            {
                itemId = itemId,
                itemName = itemName,
                quantity = quantity,
                icon = icon
            });
        }

        OnInventoryChanged?.Invoke();
    }

    public bool HasItem(string itemId)
    {
        InventoryItem item = items.Find(i => i.itemId == itemId);
        return item != null && item.quantity > 0;
    }

    public bool UseItem(string itemId)
    {
        InventoryItem item = items.Find(i => i.itemId == itemId);

        if (item != null && item.quantity > 0)
        {
            item.quantity--;
            OnInventoryChanged?.Invoke();
            return true;
        }

        return false;
    }

    public List<InventoryItem> GetItems()
    {
        return new List<InventoryItem>(items);
    }

    public int GetItemCount(string itemId)
    {
        InventoryItem item = items.Find(i => i.itemId == itemId);
        return item?.quantity ?? 0;
    }
}
