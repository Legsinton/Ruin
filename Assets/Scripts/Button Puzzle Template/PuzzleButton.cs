using TMPro;
using UnityEngine;

public class PuzzleButton : MonoBehaviour, IInteracting
{
    [SerializeField] int buttonID;
    [SerializeField] PuzzleManager puzzleManager;
    [SerializeField] Outline outlineComponent;
    public float pressDepth;
    public float moveSpeed;

    bool played;
    bool puzzleComplete;
    bool buttonPressed;
    //Color startColor;

    Vector3 targetPosition;
    Vector3 originalPosition;
    Vector3 previousPosition;
    readonly float movementThreshold = 0.001f;

    void Start()
    {
       // startColor = GetComponent<Renderer>().material.color;
        originalPosition = transform.position;
        previousPosition = transform.position;
    }

    private void Update()
    {
        MoveObject();
        PlaySoundFX();
    }

    void MoveObject()
    {
        Debug.Log("is it moving?");
        if (buttonPressed)
        {
            targetPosition = originalPosition - Vector3.left * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else if (!buttonPressed)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    void PlaySoundFX()
    {
        Vector3 movement = transform.position - previousPosition;
        if (movement.magnitude > movementThreshold)
        {
            if (!played)
            {
                played = true;
                SoundFXManager.Instance.StartLoopFor(gameObject, SoundType.PressurePlate, this.transform);
            }
        }
        else
        {
            if (played)
            {
                played = false;
                SoundFXManager.Instance.StopLoopFor(gameObject);
            }
        }
        previousPosition = transform.position;
    }

    public void InteractInRange()
    {
        if (!puzzleComplete && !buttonPressed)
        {
            outlineComponent.enabled = true;
        }
        else if (puzzleComplete)
        {
            outlineComponent.enabled = false;
        }
    }

    public void InteractNotInRange()
    {
        outlineComponent.enabled = false;
    }

    public void PressInteract()
    {
        if (!buttonPressed && !puzzleComplete)
        {
            buttonPressed = true;
            puzzleManager.RegisterButtonPress(buttonID);
        }
        else if (buttonPressed && !puzzleComplete)
        {
            buttonPressed = false;
            //GetComponent<Renderer>().material.color = startColor;
            puzzleManager.UnRegisterButtonPress(buttonID);
        }
    }
    public void ReleaseInteract(){}

    public void ResetButton()
    {
        //GetComponent<Renderer>().material.color = startColor;
        buttonPressed = false;
    }

    public void PuzzleComplete() 
    {
        puzzleComplete = true;
    }

    public bool shouldObjectBeDestroyed()
    {
        return false;
    }
}