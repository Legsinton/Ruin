using UnityEngine;

public enum SlotTag {Arm,Leg,Eyes,Key }

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public Texture2D sprite;
    public SlotTag itemTag;
    public int itemId;
    public string itemName;
}
