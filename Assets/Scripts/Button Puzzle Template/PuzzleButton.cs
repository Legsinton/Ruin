using UnityEngine;

public class PuzzleButton : MonoBehaviour, IInteracting
{
    [SerializeField] int buttonID;
    [SerializeField] PuzzleManager puzzleManager;
    [SerializeField] Outline outlineComponent;

    bool puzzleComplete;
    bool buttonPressed;
    Color startColor;

    void Start()
    {
        startColor = GetComponent<Renderer>().material.color;
    }

    public void InteractInRange()
    {
        if (!puzzleComplete && !buttonPressed)
        {
            outlineComponent.enabled = true;
        }
    }

    public void InteractNotInRange()
    {
        outlineComponent.enabled = false;
    }

    public void PressInteract()
    {
        if (!puzzleComplete && !buttonPressed)
        {
            Press();
        }
    }

    public void ReleaseInteract(){}

    public void ResetButton()
    {
        GetComponent<Renderer>().material.color = startColor;
        buttonPressed = false;
    }

    public void PuzzleComplete() 
    {
        puzzleComplete = true;
    }

    public void Press()
    {
        buttonPressed = true;
        GetComponent<Renderer>().material.color = Color.black;
        puzzleManager.RegisterButtonPress(buttonID);
    }
}