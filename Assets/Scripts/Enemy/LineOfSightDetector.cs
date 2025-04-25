using UnityEngine;

public class LineOfSightDetector : MonoBehaviour
{
    [SerializeField]
    private LayerMask playerLayerMask;
    [SerializeField]
    private float detectionRange = 10.0f;
    [SerializeField]
    private float detectionHeight = 3f;

    [SerializeField] private bool showDebugVisuals = true;

    public GameObject PerformDetection(GameObject potentialTarget)
    {
        RaycastHit hit;
        Vector3 direction = potentialTarget.transform.position - transform.position;
        Physics.Raycast(transform.position + Vector3.up * detectionHeight,
            direction, out hit, detectionRange, playerLayerMask);

        if (hit.collider != null && hit.collider.gameObject == potentialTarget)
        {
            if (showDebugVisuals && this.enabled)
            {
                Debug.DrawLine(transform.position + Vector3.up * detectionHeight,
                    potentialTarget.transform.position, Color.green);
            }
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (showDebugVisuals)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + Vector3.up * detectionHeight, 0.3f);
        }
    }
}
