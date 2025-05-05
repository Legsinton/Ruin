using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> allItems;

    public Item GetItemById(int id)
    {
        return allItems.Find(item => item.itemId == id);
    }
}
