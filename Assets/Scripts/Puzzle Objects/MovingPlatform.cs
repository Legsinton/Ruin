using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class MovingPlatform : MonoBehaviour
{
    [Header("Switch Settings")]
    [SerializeField] int switches;
    public int Switches { get { return switches; } set { switches = value; } }
    public int switchAmount;

    Vector3 targetPosition;
    Vector3 originalPosition;
    Vector3 previousPosition;
    bool played;
    bool playedCutScene;
    readonly float movementThreshold = 0.001f;
    bool objectDetected;
    private float stopHeight = 0f;

    // Gizmo
    public bool drawGizmo = true;
    public Color gizmoColor = Color.red;

    public LayerMask layerMask;
    Rigidbody rb;

    [Header("Settings To Move Platform")]

    [SerializeField] bool down;
    [SerializeField] bool sideZ;
    [SerializeField] bool sideX;
    public float pressDepth;
    public float moveSpeed;

    [Header("Settings For Cameras")]
    [SerializeField] float cutSceneLength;

    [Header("Camera References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera cutSceneCamera;

    private void Start()
    {
        originalPosition = transform.position;
        previousPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        MovementZ();
        MovementX();
    }

    private void FixedUpdate()
    {
        if (!down) return;

        RaycastHit hit;
        Vector3 halfExtents = transform.localScale * 0.5f;
        Vector3 direction = -transform.up;
        Quaternion orientation = transform.rotation;

        if (Physics.BoxCast(transform.position, halfExtents, direction, out hit, orientation, Mathf.Infinity, layerMask))
        {
            objectDetected = true;
            stopHeight = hit.point.y + halfExtents.y;
            stopHeight = Mathf.Max(stopHeight, 0f);
        }
        else
        {
            objectDetected = false;
            stopHeight = pressDepth;
        }

        MovementUp();

        Vector3 movement = transform.position - previousPosition;
        if (movement.magnitude > movementThreshold)
        {
            if (!played)
            {
                played = true;
                SoundFXManager.Instance.StartLoopFor(gameObject, SoundType.Chain, this.transform);
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

    void MovementUp()
    {
        if (!down) return;
        {
            if (Switches == switchAmount)
            {
                // Debug.Log("Move down");
                if (!played)
                {
                    played = true;

                    if (cutSceneCamera != null && !playedCutScene)
                    {
                        ActivateCamera();
                        Invoke(nameof(DisableActiveCamera), cutSceneLength);
                        playedCutScene = true;
                    }
                }


                Vector3 targetPosition;

                if (objectDetected)
                {
                    targetPosition = new Vector3(originalPosition.x, stopHeight, originalPosition.z);
                }
                else
                {
                    targetPosition = new Vector3(originalPosition.x, originalPosition.y - stopHeight, originalPosition.z);
                }

                Debug.Log("targetPos: " + targetPosition);

                if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    rb.MovePosition(Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
                }

                // if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                // {
                //     rb.MovePosition(Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
                // }
            }

            else if (Switches < switchAmount)
            {
                Debug.Log("Move up");
                if (played) played = false;

                Vector3 returnPosition = originalPosition;

                if (Vector3.Distance(transform.position, returnPosition) > 0.01f)
                {
                    rb.MovePosition(Vector3.MoveTowards(transform.position, returnPosition, moveSpeed * Time.fixedDeltaTime));
                }
            }
        }
    }

    void MovementZ()
    {
        if (sideZ)
        {
            if (Switches == switchAmount)
            {
                if (!played)
                {
                    //PlaySoundFX();
                    if (cutSceneCamera != null && !playedCutScene)
                    {
                        ActivateCamera();
                        Invoke(nameof(DisableActiveCamera), cutSceneLength);
                        playedCutScene = true;
                    }
                }
                targetPosition = originalPosition - Vector3.forward * pressDepth;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            else if (Switches == switchAmount)
            {
                if (played)
                {
                    //PlaySoundFX();
                    played = false;
                }
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            }
        }
    }

    void MovementX()
    {
        if (sideX)
        {
            if (Switches == switchAmount)
            {
                if (!played)
                {
                    //PlaySoundFX();
                    played = true;
                    if (cutSceneCamera != null && !playedCutScene)
                    {
                        ActivateCamera();
                        Invoke(nameof(DisableActiveCamera), cutSceneLength);
                        playedCutScene = true;
                    }
                }
                targetPosition = originalPosition - Vector3.right * pressDepth;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            else if (Switches == switchAmount)
            {
                if (played)
                {
                    //PlaySoundFX();
                    played = false;
                }
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            }
        }
    }

    void ActivateCamera()
    {
        playerCamera.enabled = false;
        cutSceneCamera.enabled = true;
    }

    void DisableActiveCamera()
    {
        playerCamera.enabled = true;
        cutSceneCamera.enabled = false;
    }

    void PlaySoundFX()
    {
        SoundFXManager.Instance.PlaySoundFX(SoundType.Chain, transform.position);
    }


    public void AddSwitch()
    {
        switches++;
    }

    void OnDrawGizmos()
    {
        if (!drawGizmo) return;

        Vector3 halfExtents = transform.localScale * 0.5f;
        Vector3 direction = -transform.up;
        Quaternion orientation = transform.rotation;

        // Perform the actual boxcast
        if (Physics.BoxCast(transform.position, halfExtents, direction, out RaycastHit hit, orientation, Mathf.Infinity, layerMask))
        {
            // Draw cast path
            Gizmos.color = gizmoColor;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, orientation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, transform.localScale); // Start box

            // Draw end box at hit point
            Vector3 castDistance = direction * hit.distance;
            Gizmos.matrix = Matrix4x4.TRS(transform.position + castDistance, orientation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, transform.localScale);

            // Draw line connecting the two boxes
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawLine(transform.position, transform.position + castDistance);
        }
    }
}