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


    public GameObject DetectedTarget
    {
        get;
        set;
    }

    private void Start()
    {
        StartCoroutine(FOVRoutine());
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

        DetectedTarget = null;

        foreach (var collider in colliders)
        {
            Transform target = collider.transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    DetectedTarget = collider.gameObject;
                    break; // stop at first valid target
                }
            }
        }

        return DetectedTarget;
    }
}