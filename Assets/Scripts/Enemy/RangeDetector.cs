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
        Collider[] colliders = Physics.OverlapSphere(transform.position, angle, targetMask);

        if (colliders.Length > 0)
        {
            DetectedTarget = colliders[0].gameObject;
            Transform target = colliders[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    DetectedTarget = colliders[0].gameObject;
                else
                    DetectedTarget = null;
            }
            else
            {
                DetectedTarget = null;
            }
        }
        return DetectedTarget;
    }
}