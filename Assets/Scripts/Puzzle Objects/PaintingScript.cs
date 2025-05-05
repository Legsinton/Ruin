using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaintingScript : MonoBehaviour, IInteracting
{
    [SerializeField] Outline outlineScript;
    [SerializeField] TextMeshProUGUI paintingText;
    [SerializeField] Image background;

    [SerializeField] UIScript script;

    private void Awake()
    {
        script = FindAnyObjectByType<UIScript>();
        paintingText.enabled = false;
        background.enabled = false;
    }

    public void PressInteract()
    {
        paintingText.enabled = !paintingText.enabled;
        background.enabled = !background.enabled;
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
        paintingText.enabled = false;
        background.enabled = false;
    }
}
