using UnityEngine;

public class DoorEasyScript : MonoBehaviour, IInteracting
{
    
    Vector3 targetPosition;
    Vector3 currentPoisition;
    Vector3 originalPosition;
    public float pressDepth;
    public float moveSpeed;
    bool did = false;
    public bool doorClosed = false;

    private void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (doorClosed) 
        {
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime); 

        }

        OpenDoor();
    }

    void OpenDoor()
    {
        if (!doorClosed)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition , moveSpeed * Time.deltaTime);
        }
    }

    public void PressInteract()
    {
        doorClosed = true;
    }

    public void ReleaseInteract() { }

    public void InteractInRange() { }

    public void InteractNotInRange() { }
}
