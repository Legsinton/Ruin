using UnityEngine;

public class KeyInteract : MonoBehaviour, IInteracting
{
    [SerializeField] Outline outlineScript;
    [SerializeField] Item item;
    [SerializeField] ChestScript chestScript;

    public void PressInteract()
    {
        if (!chestScript.DoorOpening)
        {
            SoundFXManager.Instance.PlaySoundFX(SoundType.KeyFound, transform.position);
            Inventory.Instance.AddItem(item.itemId);
            Destroy(gameObject);
        }
    }

    public void ReleaseInteract(){}

    public void InteractInRange()
    {
        if (!chestScript.DoorOpening)
        {
            outlineScript.enabled = true;
        }
    }

    public void InteractNotInRange()
    {
        outlineScript.enabled = false;
    }
}
