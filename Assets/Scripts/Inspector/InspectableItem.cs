using UnityEngine;

public class InspectableItem : MonoBehaviour, IInteracting
{
    [SerializeField] Outline outlineScript;
    private Inspector inspector;

    private void Start()
    {
        //outlineScript = transform.GetComponent<Outline>();
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
        inspector.InspectItem();
    }

    public void ReleaseInteract()
    {
        inspector.StopInspection();
    }

    public bool shouldObjectBeDestroyed()
    {
        throw new System.NotImplementedException();
    }
}
