using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaintingScript : MonoBehaviour, IInteracting
{
    [SerializeField] Outline outlineScript;
    [SerializeField] UIScript script;

    private void Awake()
    {
        script = FindAnyObjectByType<UIScript>();
    }

    public void PressInteract()
    {
        

        Debug.Log("Give Me Money");
    }

    public void ReleaseInteract() { }

    public void InteractInRange() 
    {
        script.EnableUI();
        outlineScript.enabled = true;
    }

    public void InteractNotInRange()
    {
        script.DisebleUI();
        outlineScript.enabled = false;
    }
}
