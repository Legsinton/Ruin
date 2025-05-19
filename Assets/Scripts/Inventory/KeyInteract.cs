using UnityEngine;

public class KeyInteract : MonoBehaviour, IInteracting
{
    [SerializeField] Outline outlineScript;
    [SerializeField] Item item;
    [SerializeField] float spinSpeed;
    [HideInInspector] public bool canInteract;
    private void Update()
    {
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }

    public void PressInteract()
    {
        if (canInteract)
        {
            SoundFXManager.Instance.PlaySoundFX(SoundType.KeyFound, transform.position);
            Inventory.Instance.AddItem(item.itemId);
            Destroy(gameObject);
        }    
    }

    public void ReleaseInteract(){}

    public void InteractInRange()
    {
        if (canInteract)
        {
            outlineScript.enabled = true;
        }
    }

    public void InteractNotInRange()
    {
        outlineScript.enabled = false;
    }
}
