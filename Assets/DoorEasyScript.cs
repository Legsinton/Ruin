using UnityEngine;
using UnityEngine.InputSystem;
using static Interact;

public class DoorEasyScript : MonoBehaviour, IInteracting
{
    
    Vector3 targetPosition;
    Vector3 currentPoisition;
    Vector3 originalPosition;
    public float pressDepth;
    public float moveSpeed;
    bool did = false;
    public bool interacting = false;

    private void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (!interacting) 
        {
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime); 

        }

        OpenDoor();
    }

    void OpenDoor()
    {
        if (interacting)
        {
            targetPosition = originalPosition - Vector3.down * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    public void Interact()
    {
        interacting = !interacting; // toggle open/close
    }
}
