using System;
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
    public float smallPressDepth;
    public float moveSpeed;
    bool added = false;
    public bool smallTrigger;
    public float buffer;

    private void Start()
    { 
        originalPosition = transform.position;
    }

    private void Update()
    {
        MovementFull();
        MovementSmall();
    }

    void MovementSmall()
    {
        if (smallTrigger && !triggerd)
        {
            targetPosition = originalPosition - Vector3.up * smallPressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        }
        else if (!smallTrigger && !triggerd)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    void MovementFull()
    {
        if (triggerd && !smallTrigger)
        {
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        }
        else if (!triggerd && !smallTrigger)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Bounds blockBounds = other.bounds; // The collider bounds of the block
        Bounds plateBounds = GetComponent<Collider>().bounds; // The bounds of the plate
        Vector2 plateMin = new Vector2(plateBounds.min.x, plateBounds.min.z);
        Vector2 plateMax = new Vector2(plateBounds.max.x, plateBounds.max.z);

        Vector2 blockMin = new Vector2(blockBounds.min.x, blockBounds.min.z);
        Vector2 blockMax = new Vector2(blockBounds.max.x, blockBounds.max.z);

        // Calculate intersection area
        float intersectMinX = Mathf.Max(plateMin.x, blockMin.x);
        float intersectMaxX = Mathf.Min(plateMax.x, blockMax.x);
        float intersectMinY = Mathf.Max(plateMin.y, blockMin.y);
        float intersectMaxY = Mathf.Min(plateMax.y, blockMax.y);

        float intersectWidth = Mathf.Max(0, intersectMaxX - intersectMinX);
        float intersectHeight = Mathf.Max(0, intersectMaxY - intersectMinY);
        float intersectArea = intersectWidth * intersectHeight;

        float blockArea = (blockMax.x - blockMin.x) * (blockMax.y - blockMin.y);

        float overlapRatio = intersectArea / blockArea;

        if (other.CompareTag("Pullable"))
        {
            if (overlapRatio >= 0.85f)
            {
                if (!added)
                {
                    smallTrigger = false;
                    SoundFXManager.Instance.PlaySoundFX(SoundType.Coin,transform.position);
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

        if (other.CompareTag("Player") && !triggerd)
        {
            smallTrigger = true;
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
                smallTrigger = false;
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
        if (other.CompareTag("Player") && !triggerd)
        {
            smallTrigger = false;
        }
    }
}

