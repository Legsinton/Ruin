using Unity.Behavior;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent Agent;

    public GameObject PlayerTarget { get; set; }
    [SerializeField]
    private GameObject playerReference;
    [SerializeField]
    private LayerMask playerLayerMask;
    [SerializeField]
    private float detectionRange = 10.0f;
    public Vector3 LastKnownPlayerPosition { get; set; }

    private void Start()
    {
        LastKnownPlayerPosition = transform.position;
        //GetComponent<BlackboardVariable>
    }

    private void Update()
    {
        RaycastHit hit;
        Vector3 direction = playerReference.transform.position - transform.position;
        Physics.Raycast(transform.position, direction, out hit, detectionRange, playerLayerMask);
        if (hit.collider != null && hit.collider.gameObject == playerReference)
        {
            PlayerTarget = hit.collider.gameObject;
            LastKnownPlayerPosition = PlayerTarget.transform.position;
            Debug.Log("Chase player!");
        }
        else
        {
            PlayerTarget = null;
            Debug.Log("Stop chasing player");
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (Application.isPlaying)
        {
            if (PlayerTarget != null)
                Gizmos.color = Color.green;
            if (playerReference != null)
            {
                Vector3 direction = playerReference.transform.position - transform.position;
                Gizmos.DrawLine(transform.position, playerReference.transform.position);
            }
        }
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
