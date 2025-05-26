using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class MovingPlatform : MonoBehaviour
{
    [Header("Switch Settings")]
    [SerializeField] int switches;
    public int Switches { get { return switches; } set { switches = value; } }
    public int switchAmount;
    public PlayerMovement playerMovement;

    Vector3 targetPosition;
    Vector3 originalPosition;
    Vector3 previousPosition;

    bool move;
    bool played;
    bool playedCutScene;
    bool objectDetected;
    readonly float movementThreshold = 0.001f;
    float stopHeight = 0f;
    float gizmoValue = 0.42f;
    Vector3 gizmoOffset = new Vector3(0, 0.1f, 0);

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
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        originalPosition = transform.position;
        previousPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void FixedUpdate()
    {
        if (move)
        {
            Vector3 castOrigin = transform.position + gizmoOffset;
            RaycastHit hit;
            Vector3 halfExtents = transform.localScale * gizmoValue;
            Vector3 direction = -transform.up;
            Quaternion orientation = transform.rotation;

            if (Physics.BoxCast(castOrigin, halfExtents, direction, out hit, orientation, Mathf.Infinity, layerMask))
            {
                objectDetected = true;
                stopHeight = hit.point.y + halfExtents.y;
                stopHeight = Mathf.Max(stopHeight, pressDepth);
                //Debug.Log($"Check: [BoxCast] Hit: {hit.collider.name}, Distance: {hit.distance}");
                //Debug.Log($"Check: Platform position: {transform.position}, Hit distance: {hit.distance}, Stop distance: {stopHeight}");
            }
            else
            {
                objectDetected = false;
                stopHeight = pressDepth;
                // Debug.Log("Agnes: stopHeight? " + stopHeight);
            }
        }
        if (move)
        {
            MovementUp();
            MovementZ();
            MovementX();
        }

        if (move)
        {
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
                    move = false;
                    played = false;
                    SoundFXManager.Instance.StopLoopFor(gameObject);
                }
            }

            previousPosition = transform.position;
        }
    }

    void MovementUp()
    {
        if (down)
        {
            if (Switches == switchAmount)
            {
                // Debug.Log("Move down");

                if (cutSceneCamera != null && !playedCutScene)
                {
                    ActivateCamera();
                    Invoke(nameof(DisableActiveCamera), cutSceneLength);
                    playedCutScene = true;
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

                if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    rb.MovePosition(Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime));
                }
            }

            else if (Switches < switchAmount)
            {
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
                if (cutSceneCamera != null && !playedCutScene)
                {
                    ActivateCamera();
                    Invoke(nameof(DisableActiveCamera), cutSceneLength);
                    playedCutScene = true;
                }

                targetPosition = originalPosition - Vector3.forward * pressDepth;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            else if (Switches != switchAmount)
            {

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

                if (cutSceneCamera != null && !playedCutScene)
                {
                    ActivateCamera();
                    Invoke(nameof(DisableActiveCamera), cutSceneLength);
                    playedCutScene = true;
                }

                targetPosition = originalPosition - Vector3.right * pressDepth;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            else if (Switches != switchAmount)
            {

                transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            }
        }
    }

    void ActivateCamera()
    {
        playerMovement.enabled = false;
        playerMovement.movement = new Vector3(0, 0, 0);
        playerCamera.enabled = false;
        cutSceneCamera.enabled = true;
    }

    void DisableActiveCamera()
    {
        playerMovement.enabled = true;
        playerCamera.enabled = true;
        cutSceneCamera.enabled = false;
    }

    public void AddSwitch()
    {
        switches++;
        move = true;
    }

    public void RemoveSwitch()
    {
        switches--;
        move = true;
    }

    void OnDrawGizmos()
    {
        if (!drawGizmo) return;

        Vector3 castOrigin = transform.position + gizmoOffset;
        Vector3 halfExtents = transform.localScale * gizmoValue;
        Vector3 direction = -transform.up;
        Quaternion orientation = transform.rotation;

        // Perform the actual boxcast

        if (Physics.BoxCast(castOrigin, halfExtents, direction, out RaycastHit hit, orientation, Mathf.Infinity, layerMask))
        {
            // Draw cast path
            Gizmos.color = gizmoColor;
            Gizmos.matrix = Matrix4x4.TRS(castOrigin, orientation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, transform.localScale); // Start box

            // Draw end box at hit point
            Vector3 castDistance = direction * hit.distance;
            Gizmos.matrix = Matrix4x4.TRS(castOrigin + castDistance, orientation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, transform.localScale);

            // Draw line connecting the two boxes
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawLine(castOrigin, castOrigin + castDistance);
        }
    }
}