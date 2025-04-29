using System.Collections;
using UnityEngine;

public class FallingPlatformScript : MonoBehaviour
{
    Vector3 targetPosition;
    Vector3 originalPosition;
    public bool triggerd = false;
    public float pressDepth;
    public float moveSpeed;
    Coroutine untriggerRoutine;
    [SerializeField] float buffer;
    [SerializeField] float bufferSides;
    float timeOutsideBounds; 


    void Start()
    {
        originalPosition = transform.position;
    }

    void FixedUpdate()
    {

        timeOutsideBounds += Time.fixedDeltaTime;

        if (timeOutsideBounds > 1)
        {
            triggerd = false;
        }
        if (triggerd)
        {
            Debug.Log("Triggered");

            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        }
        else if (!triggerd)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        Bounds playerBounds = collision.bounds; // The collider bounds of the block
        Bounds plateBounds = GetComponent<Collider>().bounds; // The bounds of the plate
        plateBounds.Expand(new Vector3(bufferSides, buffer, bufferSides));

        if (plateBounds.Contains(playerBounds.min) && plateBounds.Contains(playerBounds.max))
        {
            timeOutsideBounds = 0;
            Debug.Log("Hello");
            triggerd = true;
        }
    }
}
