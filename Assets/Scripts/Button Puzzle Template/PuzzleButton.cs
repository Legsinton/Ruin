using UnityEngine;

public class PuzzleButton : MonoBehaviour, IInteracting
{
    [SerializeField] int buttonID;
    [SerializeField] PuzzleManager puzzleManager;
    [SerializeField] Outline outlineComponent;
    public float pressDepth;
    public float moveSpeed;

    bool move;
    bool calculateNextPosition;
    bool puzzleComplete;
    bool buttonPressed;

    Vector3 targetPosition;
    Vector3 originalPosition;
    readonly float movementThreshold = 0.001f;

    void Start()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (move)
        {
            MoveObject();
        }
    }

    void MoveObject()
    {
        if (!calculateNextPosition)
        {
            if (buttonPressed)
            {
                targetPosition = originalPosition - Vector3.left * pressDepth;
            }
            else
            {
                targetPosition = originalPosition;
            }
            SoundFXManager.Instance.StartLoopFor(gameObject, SoundType.PressurePlate, this.transform);
            calculateNextPosition = true;
        }

        if (Vector3.Distance(transform.position, targetPosition) < movementThreshold)
        {
            move = false;
            calculateNextPosition = false;
            SoundFXManager.Instance.StopLoopFor(gameObject);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
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
            move = true;
            puzzleManager.RegisterButtonPress(buttonID);
        }
        else if (buttonPressed && !puzzleComplete)
        {
            buttonPressed = false;
            move = true;
            puzzleManager.UnRegisterButtonPress(buttonID);
        }
    }
    public void ReleaseInteract() { }

    public void ResetButton()
    {
        buttonPressed = false;
        move = true;
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