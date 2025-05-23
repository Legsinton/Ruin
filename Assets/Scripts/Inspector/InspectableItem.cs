using System.Collections.Generic;
using UnityEngine;

public class InspectableItem : MonoBehaviour, IInteracting
{
    [SerializeField] Outline outlineScript;
    Inspector inspector;
    //[SerializeField] Item item;
    public bool canInteract;
    public Item item;

    public bool shouldBeDestroyed;

    private void Start()
    {
        inspector = FindFirstObjectByType<Inspector>();
    }

    public void InteractInRange()
    {
        outlineScript.enabled = true;
    }

    public void InteractNotInRange()
    {
        outlineScript.enabled = false;
    }

    public void PressInteract()
    {
        if (canInteract)
        {
            Debug.Log("Agnes: Can interact");
            InputHandler.Instance.OnEnable();
        }
        inspector.InspectItem(item);

        // Logic should be handled from a public sound type
        // SoundFXManager.Instance.PlaySoundFX(SoundType.KeyFound, transform.position);
        // Inventory.Instance.AddItem(item.itemId);
        // Destroy(gameObject);
    }

    public void ReleaseInteract()
    {
        inspector.StopInspection();
    }

    public bool shouldObjectBeDestroyed()
    {
        if (shouldBeDestroyed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
