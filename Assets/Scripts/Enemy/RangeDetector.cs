using System.Collections;
using UnityEngine;

public class RangeDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float radius;
    [Range(0, 360)]
    public float angle;
    public LayerMask targetMask;
    [SerializeField] private LayerMask obstructionMask;

    public GameObject DetectedTarget { get; private set; }
    public Vector3? LastKnownPosition { get; private set; }

    public string position;
    private bool previouslyDetected = false;

    void Start()
    {
        StartCoroutine(FOVRoutine());
    }

    void Update()
    {
        position = LastKnownPosition.ToString();

    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            UpdateDetector();
        }
    }

    public GameObject UpdateDetector()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetMask);

        GameObject newDetectedTarget = null;

        foreach (var collider in colliders)
        {
            Transform target = collider.transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    newDetectedTarget = collider.gameObject;
                    break; // Found a valid target
                }
            }
        }

        // Handle transition from seeing → not seeing
        if (previouslyDetected && newDetectedTarget == null && DetectedTarget != null)
        {
            LastKnownPosition = DetectedTarget.transform.position;
            Debug.Log("Agnes, last known position: " + LastKnownPosition);
        }

        DetectedTarget = newDetectedTarget;
        previouslyDetected = DetectedTarget != null;

        return DetectedTarget;
    }

    public void ClearLastKnownPosition()
    {
        LastKnownPosition = null;
    }
}


















// using System.Collections;
// using UnityEngine;

// public class RangeDetector : MonoBehaviour
// {

//     [Header("Detection Settings")]
//     [SerializeField] private float radius;
//     [Range(0, 360)]
//     public float angle;
//     public LayerMask targetMask;
//     [SerializeField] private LayerMask obstructionMask;


//     public GameObject DetectedTarget
//     {
//         get;
//         set;
//     }

//     private void Start()
//     {
//         StartCoroutine(FOVRoutine());
//     }

//     private IEnumerator FOVRoutine()
//     {
//         WaitForSeconds wait = new WaitForSeconds(0.2f);

//         while (true)
//         {
//             yield return wait;
//             UpdateDetector();
//         }
//     }
//     public GameObject UpdateDetector()
//     {
//         Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetMask);

//         DetectedTarget = null;

//         foreach (var collider in colliders)
//         {
//             Transform target = collider.transform;
//             Vector3 directionToTarget = (target.position - transform.position).normalized;
//             float distanceToTarget = Vector3.Distance(transform.position, target.position);

//             if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
//             {
//                 if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
//                 {
//                     DetectedTarget = collider.gameObject;
//                     break; // stop at first valid target
//                 }
//             }
//         }

//         return DetectedTarget;
//     }

//     // Move Agent(enemy) toward players last known position.
//     // When the enemy is there, make it search around in the radius looking for the player for x amount of time.
// }