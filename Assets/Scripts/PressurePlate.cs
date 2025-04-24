using System.Collections;
using TMPro;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{

    Rigidbody rb;
    public bool triggerd = false;
    public DoorScript[] door;
    Vector3 targetPosition;
    Vector3 currentPoisition;
    Vector3 originalPosition;
    public float pressDepth;
    public float moveSpeed;
    bool added = false;
    
    private void Start()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (triggerd)
        {
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        }
        else if (!triggerd)
        {
            targetPosition = originalPosition - Vector3.down * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Bounds blockBounds = other.bounds; // The collider bounds of the block
        Bounds plateBounds = GetComponent<Collider>().bounds; // The bounds of the plate        if (other.CompareTag("Pullable"))
        Vector2 plateMin = new Vector2(plateBounds.min.x, plateBounds.min.z);
        Vector2 plateMax = new Vector2(plateBounds.max.x, plateBounds.max.z);

        Vector2 blockMin = new Vector2(blockBounds.min.x, blockBounds.min.z);
        Vector2 blockMax = new Vector2(blockBounds.max.x, blockBounds.max.z);

        bool blockIsFullyOnPlate =
            plateMin.x <= blockMin.x && plateMax.x >= blockMax.x &&
            plateMin.y <= blockMin.y && plateMax.y >= blockMax.y;
        if (other.CompareTag("Pullable"))
        {
            if (blockIsFullyOnPlate)
            {
                if (!added)
                {
                    triggerd = true;
                    added = true;
                    door[1].Switches++;
                    door[0].Switches++;

                }

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        rb = other.GetComponent<Rigidbody>();
        if (other.CompareTag("Pullable"))
        {
            triggerd = false;
        }
    }
}
