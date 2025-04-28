using UnityEngine;

public class PuzzleButton : MonoBehaviour, IInteracting
{
    [SerializeField] Outline outlineComponent;

    public void InteractInRange()
    {
        outlineComponent.enabled = true;
    }

    public void InteractNotInRange()
    {
        outlineComponent.enabled = false;
    }

    public void PressInteract()
    {
        Debug.Log(gameObject.name);
    }

    public void ReleaseInteract()
    {

    }
}