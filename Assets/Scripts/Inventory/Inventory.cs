using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private ItemDatabase itemDatabase;
    List<Item> inventoryItems = new List<Item>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(int itemId)
    {
        Item itemToAdd = itemDatabase.GetItemById(itemId);
        if (itemToAdd != null)
        {
            inventoryItems.Add(itemToAdd);
            Debug.Log($"Added: {itemToAdd.itemName}");
        }
        else
        {
            Debug.LogWarning($"Item with ID {itemId} not found.");
        }
    }

    public void RemoveItem(int itemId)
    {
        Item itemToRemove = inventoryItems.Find(item => item.itemId == itemId);
        if (itemToRemove != null)
        {
            inventoryItems.Remove(itemToRemove);
            Debug.Log($"Removed: {itemToRemove.itemName}");
        }
        else
        {
            Debug.LogWarning($"Item with ID {itemId} not in inventory.");
        }
    }
}
