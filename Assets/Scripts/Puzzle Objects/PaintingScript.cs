using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaintingScript : MonoBehaviour, IInteracting
{
    [SerializeField] Outline outlineScript;
    [SerializeField] TextMeshProUGUI paintingText;
    [SerializeField] Image background;

    //[SerializeField] PlayerUI script;

    private void Awake()
    {
        //script = FindAnyObjectByType<PlayerUI>();
        paintingText.enabled = false;
        background.enabled = false;
    }

    public void PressInteract()
    {
        paintingText.enabled = !paintingText.enabled;
        background.enabled = !background.enabled;

        Debug.Log("Give Me Money");
    }

    public void ReleaseInteract() { }

    public void InteractInRange() 
    {
        //script.EnableUI();
        outlineScript.enabled = true;
    }

    public void InteractNotInRange()
    {
        //script.DisebleUI();
        outlineScript.enabled = false;
        paintingText.enabled = false;
        background.enabled = false;
    }

    public bool shouldObjectBeDestroyed()
    {
        return false;
    }
}
