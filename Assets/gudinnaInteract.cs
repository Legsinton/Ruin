using UnityEngine;

public class gudinnaInteract : MonoBehaviour, IInteracting
{
    [Header("References")]
    [SerializeField] Outline outline;
    [SerializeField] GameObject arm;
    [SerializeField] GameObject leg;
    [SerializeField] int armId;
    [SerializeField] int legId;

    bool missingText = true;

    public void PressInteract()
    {
        for (int i = 0; Inventory.Instance.inventoryItems.Count > i; i++)
        {
            if (Inventory.Instance.inventoryItems[i].itemId == armId && !arm.activeInHierarchy)
            {
                arm.SetActive(true);
                missingText = false;
            }
            if (Inventory.Instance.inventoryItems[i].itemId == legId && !leg.activeInHierarchy)
            {
                leg.SetActive(true);
                missingText = false;
            }
        }

        if (missingText)
        {
            PlayerUI.Instance.DisplayText("Something is missing...", 3);
        }
        else
        {
            missingText = true;
        }
    }

    void CheckIfComplete()
    {
        if (arm.activeInHierarchy && leg.activeInHierarchy)
        {
            Debug.Log("GAME COMPLETE!");
        }
    }

    public void ReleaseInteract() { }

    public void InteractInRange() 
    {
        outline.enabled = true;
    }

    public void InteractNotInRange() 
    { 
        outline.enabled = false;
    }

    public bool shouldObjectBeDestroyed()
    {
        return false;
    }
}
