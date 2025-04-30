using Unity.Behavior;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent Agent;

    public GameObject PlayerTarget { get; private set; }
    [SerializeField] private GameObject playerReference;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private float detectionRange = 10.0f;
    [SerializeField] private float moveSpeed = 3.5f; // Enemy move speed
    public Vector3 LastKnownPlayerPosition { get; private set; }

    private bool isChasingPlayer = false;
    private float memoryTimer = 0f;
    [SerializeField]
    private float memoryDuration = 5f; // Time the enemy remembers the player after losing them

    private void Start()
    {
        LastKnownPlayerPosition = transform.position;
    }

    private void Update()
    {
        if (playerReference == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerReference.transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Player inside detection range
            if (!isChasingPlayer)
            {
                isChasingPlayer = true;
                Debug.Log("Player entered detection range: Start chasing!");
            }

            PlayerTarget = playerReference;
            LastKnownPlayerPosition = playerReference.transform.position;
            memoryTimer = memoryDuration; // Reset memory timer
        }
        else
        {
            // Player outside detection range
            if (memoryTimer > 0)
            {
                memoryTimer -= Time.deltaTime;

                if (memoryTimer <= 0)
                {
                    // Fully forget the player
                    isChasingPlayer = false;
                    PlayerTarget = null;
                    Debug.Log("Memory faded: Stop chasing player!");
                }
                else
                {
                    // Still remembering player
                    Debug.Log($"Remembering player... ({memoryTimer:F1}s left)");
                }
            }
        }

        if (PlayerTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, PlayerTarget.transform.position, moveSpeed * Time.deltaTime);
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
                Gizmos.DrawLine(transform.position, playerReference.transform.position);
            }
        }

        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
