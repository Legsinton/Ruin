using UnityEngine;

public enum SlotTag {Arm,Leg,Eyes,Key }

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public GameObject itemPrefab;
    public SlotTag itemTag;
    public int itemId;
    public string itemName;
}
