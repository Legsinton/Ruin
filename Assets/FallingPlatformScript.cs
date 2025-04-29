using UnityEngine;

public class FallingPlatformScript : MonoBehaviour
{
    Vector3 targetPosition;
    Vector3 currentPoisition;
    Vector3 originalPosition;
    public bool triggerd = false;
    public float pressDepth;
    public float moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
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
        Bounds plateBounds = GetComponent<Collider>().bounds; // The bounds of the plate        if (other.CompareTag("Pullable"))
        Vector2 plateMin = new Vector2(plateBounds.min.x, plateBounds.min.z);
        Vector2 plateMax = new Vector2(plateBounds.max.x, plateBounds.max.z);

        Vector2 blockMin = new Vector2(playerBounds.min.x, playerBounds.min.z);
        Vector2 blockMax = new Vector2(playerBounds.max.x, playerBounds.max.z);

        bool blockIsFullyOnPlate =
            plateMin.x <= blockMin.x && plateMax.x >= blockMax.x &&
            plateMin.y <= blockMin.y && plateMax.y >= blockMax.y;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (blockIsFullyOnPlate)
            {
                Debug.Log("Hello");
                triggerd = true;
            }

        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            triggerd = false;
        }
    }

}
