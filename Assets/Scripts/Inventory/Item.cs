using UnityEngine;
using TMPro;

public enum SlotTag { Arm, Leg, Eyes, Key }

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    // public Texture2D sprite;
    public GameObject prefab;
    public SlotTag itemTag;
    public int itemId;
    public string itemName;
    public bool equipable;
    public string itemInfo;
}
