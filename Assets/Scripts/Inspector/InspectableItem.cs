using UnityEngine;

public class InspectableItem : MonoBehaviour, IInteracting
{
    [SerializeField] Outline outlineScript;
    Inspector inspector;
    public bool canInteract;
    public Item item;

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
        inspector.InspectItem(item);
        inspector.inspectableItem = this;
    }

    public void ReleaseInteract()
    {
        inspector.StopInspection();
    }

    public bool shouldObjectBeDestroyed()
    {
        if (item.equipable)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DestroyOnEquip()
    {
        Destroy(gameObject);
    }
}
