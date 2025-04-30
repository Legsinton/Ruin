using UnityEngine;

public enum SlotTag {Arm,Leg,Eyes,Key }

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public Texture2D sprite;
    public SlotTag itemTag;

    [Header("If the item can be equipped")]

    public GameObject equipmenntItem;
}
