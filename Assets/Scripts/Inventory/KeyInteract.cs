using UnityEngine;

public class KeyInteract : MonoBehaviour, IInteracting
{
    [SerializeField] Outline outlineScript;
    [SerializeField] Item item;

    public void PressInteract()
    {
        SoundFXManager.Instance.PlaySoundFX(SoundType.Coin, transform.position);
        Inventory.Instance.AddItem(item.itemId);
        Destroy(gameObject);
    }

    public void ReleaseInteract(){}

    public void InteractInRange()
    {
        outlineScript.enabled = true;
    }

    public void InteractNotInRange()
    {
        outlineScript.enabled = false;
    }
}
