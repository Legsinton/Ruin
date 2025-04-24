using UnityEngine;

public class DoorEasyScript : MonoBehaviour
{

    public PlayerControls playerControls;
    
    Vector3 targetPosition;
    Vector3 currentPoisition;
    Vector3 originalPosition;
    public float pressDepth;
    public float moveSpeed;
    bool did = false;

    private void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (playerControls.IsDoor) 
        {
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime); 

        }

        OpenDoor();
    }

    void OpenDoor()
    {
        if (!playerControls.IsDoor)
        {
            targetPosition = originalPosition - Vector3.down * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
}
