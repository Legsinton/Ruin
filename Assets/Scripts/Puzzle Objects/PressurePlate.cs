using System.Collections;
using TMPro;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{

    public bool triggerd = false;
    public GateScript[] gate;
    public MovingPlatform[] platforms;
    Vector3 targetPosition;
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
        if (other.CompareTag("Rotating"))
        {
            Debug.Log("Rotating object on pressure plate!");

            if (!added)
            {
                triggerd = true;
                added = true;
                for (int i = 0; i < platforms.Length; i++)
                {
                    platforms[i].Switches++;
                }
                for (int i = 0; i < gate.Length; i++)
                {
                    gate[i].Switches++;
                }

            }
        }

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
                    for (int i = 0; i < gate.Length; i++)
                    {
                        gate[i].Switches++;
                    }
                    for (int i = 0; i < platforms.Length; i++)
                    {
                        platforms[i].Switches++;
                    }
                }
            }
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pullable"))
        {
            if (added)
            {
                triggerd = false;
                added = false;
                for (int i = 0; i < gate.Length; i++)
                {
                    gate[i].Switches--;
                }
                for (int i = 0; i < platforms.Length; i++)
                {
                    platforms[i].Switches--;
                }
            }

        }
        if (other.CompareTag("Rotating"))
        {
            if (added)
            {
                triggerd = false;
                added = false;

                for (int i = 0; i < gate.Length; i++)
                {
                    gate[i].Switches--;
                }
                for (int i = 0; i < platforms.Length; i++)
                {
                    platforms[i].Switches--;
                }
            }
        }
    }
}

