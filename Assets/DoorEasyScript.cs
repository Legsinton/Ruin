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
        if (playerControls.haveInteracted == 1) 
        {
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime); 

        }       
    }
}
