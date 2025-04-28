using UnityEngine;

public class PaintingScript : MonoBehaviour, IInteracting
{
    [SerializeField] Outline outlineScript;

    public void PressInteract()
    {
        Debug.Log("Give Me Money");
    }

    public void ReleaseInteract() { }

    public void InteractInRange() 
    {
        outlineScript.enabled = true;
    }

    public void InteractNotInRange()
    {
        outlineScript.enabled = false;
    }
}
