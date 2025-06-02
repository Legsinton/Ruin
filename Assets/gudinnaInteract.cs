using UnityEngine;

public class gudinnaInteract : MonoBehaviour, IInteracting
{
    [Header("References")]
    [SerializeField] Outline outline;
    [SerializeField] GameObject arm;
    [SerializeField] GameObject leg;
    [SerializeField] int armId;
    [SerializeField] int legId;

    public void PressInteract()
    {
        PlayerUI.Instance.DisplayText("Something is missing...", 3);
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
