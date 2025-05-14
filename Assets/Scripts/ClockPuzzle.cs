using System.Collections;
using TMPro;
using UnityEngine;

public class ClockPuzzle : MonoBehaviour
{

    public bool triggerd = false;
    Vector3 targetPosition;
    Vector3 originalPosition;
    Vector3 previousPosition;
    public float pressDepth;
    public float moveSpeed;
    bool added = false;
    public PlayerMovement playerMovement;
    float movementThreshold = 0.001f;
    bool played;

    private void Start()
    {
        originalPosition = transform.position;
        previousPosition = transform.position;
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
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
        // Check movement
        Vector3 movement = transform.position - previousPosition;

        // Play sound only when moving downward
        if (movement.magnitude > movementThreshold) // Small threshold to avoid false positives
        {
            if (!played)
            {
                played = true;
                SoundFXManager.Instance.StartLoopFor(gameObject, SoundType.RollingOther, transform.position);
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

    void EnablePlayer()
    {
        playerMovement.enabled = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("RotatingTag"))
        {
            if (!added)
            {
                triggerd = true;
                added = true;
                playerMovement.enabled = false;
                playerMovement.movement = new Vector3(0, 0, 0);
                Invoke(nameof(EnablePlayer), 2f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("RotatingTag"))
        {
            if (added)
            {
                triggerd = false;
                added = false;
            }
        }
    }
}





