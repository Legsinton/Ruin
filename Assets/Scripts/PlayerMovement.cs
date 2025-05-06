using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]

    public Vector3 movement;
    public float acceleration;
    public float groundDrag;
    [SerializeField] float rotateSpeed;

    [SerializeField] float currentSpeed = 8;

    Rigidbody rb;
    [HideInInspector] public PushBlock PushBlock;
    RotatingObject rotatingObject;

    Vector2 movementInput;
    Vector3 playerMoveDir;

    [HideInInspector] public float currentVelocity;
    float gravityForce;
    bool interact;

    [HideInInspector] public bool rightMoveDisabled;
    [HideInInspector] public bool leftMoveDisabled;
    [HideInInspector] public bool forwardMoveDisabled;
    [HideInInspector] public bool backMoveDisabled;

    [Header("GroundCheck")]

    public LayerMask groundMask;

    readonly float distToGround = 1.05f;

    [SerializeField] private bool isGrounded;

    [Header("Camera")]

    private Vector3 cachedCameraForward;
    private Vector3 cachedCameraRight;

    [SerializeField] Transform capsule;
    [SerializeField] Transform cameraTransform;

    public CinemachineCamera virtualCamera;
    CinemachineOrbitalFollow orbitalFollow;

    [Header("Stairs")]

    [SerializeField] GameObject stepUpLower;
    [SerializeField] GameObject stepUpHigher;

    [SerializeField] float stepSmooth;
    [SerializeField] float stepHeight;
    [SerializeField] float value;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Awake()
    {
        orbitalFollow = virtualCamera.GetComponent<CinemachineOrbitalFollow>();
        stepUpHigher.transform.position = new Vector3(stepUpHigher.transform.position.x, stepHeight, stepUpHigher.transform.position.z);
    }
    private void Update()
    {
        //SetCamera();
    }
    private void LateUpdate()
    {
        // For the camera to move the capsule so the interaction cast will move based on camera movement
        cachedCameraForward = cameraTransform.forward;
        cachedCameraForward.y = 0;
        cachedCameraForward.Normalize();

        cachedCameraRight = cameraTransform.right;
        cachedCameraRight.y = 0;
        cachedCameraRight.Normalize();
    }
    private void FixedUpdate()
    {
        MovePlayer();

        GroundCheck();

        if (!isGrounded)
        {
            gravityForce = 40;
            rb.linearVelocity += Vector3.down * gravityForce * Time.deltaTime;
        }
        else
        {
            gravityForce = 1;
        }
    }
    private void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }
    private void GroundCheck()
    {
        /*Vector3 origin = transform.position; // or you can lower this a bit if needed
        origin.y -= 0.5f;*/ // move origin slightly downward if your player is tall
        isGrounded = Physics.Raycast(transform.position, Vector3.down, distToGround, groundMask);
    }

    private void SetCamera()
    {
        if (PushBlock != null /*&& PushBlock.CanMove*/)
        {
            Quaternion targetRotation = virtualCamera.transform.rotation;
            //virtualCamera.LookAt = null;
            if (movementInput.y > 0.5f && movement.x > 0.5)
            {
                targetRotation = Quaternion.Euler(21, 91, 4);
                orbitalFollow.HorizontalAxis.Value = 82;
                orbitalFollow.VerticalAxis.Value = 17.5f;
            }
            else if (movementInput.y > 0.5f && movement.x < -0.5f)
            {
                targetRotation = Quaternion.Euler(17, -89, 0);

                orbitalFollow.HorizontalAxis.Value = -104;
                orbitalFollow.VerticalAxis.Value = 17.5f;
            }
            else if (movementInput.y > 0.5f && movement.z > 0.5f)
            {
                targetRotation = Quaternion.Euler(14, 1, 0);
                orbitalFollow.HorizontalAxis.Value = 8;
                orbitalFollow.VerticalAxis.Value = 17.5f;
            }
            else if (movementInput.y > 0.5f && movement.z < -0.5f)
            {
                targetRotation = Quaternion.Euler(20, 181, 0);
                orbitalFollow.HorizontalAxis.Value = 148;
                orbitalFollow.VerticalAxis.Value = 17.5f;
            }
            if (movementInput.y < -0.5f && movement.z > 0.5f)
            {
                targetRotation = Quaternion.Euler(20, 181, 0);
                orbitalFollow.HorizontalAxis.Value = 148;
                orbitalFollow.VerticalAxis.Value = 17.5f;
            }
            else if (movementInput.y < -0.5f && movement.z < -0.5f)
            {
                targetRotation = Quaternion.Euler(14, 1, 0);
                orbitalFollow.HorizontalAxis.Value = 8;
                orbitalFollow.VerticalAxis.Value = 17.5f;

            }
            else if (movementInput.y < -0.5f && movement.x < -0.5f)
            {
                targetRotation = Quaternion.Euler(21, 91, 4);
                orbitalFollow.HorizontalAxis.Value = 82;
                orbitalFollow.VerticalAxis.Value = 17.5f;

            }
            else if (movementInput.y < -0.5f && movement.x > 0.5f)
            {
                targetRotation = Quaternion.Euler(17, -89, 0);

                orbitalFollow.HorizontalAxis.Value = -104;
                orbitalFollow.VerticalAxis.Value = 17.5f;
            }

            virtualCamera.transform.rotation = Quaternion.Lerp(
                virtualCamera.transform.rotation,
                targetRotation,
                Time.deltaTime * 3);
        }
        else
        {
            virtualCamera.LookAt = capsule;
        }

        if (PushBlock != null)
        {
            //interact = PushBlock.CanMove;
        }
    }
    private void MovePlayer()
    {
        if (PushBlock != null)
        {
            if (forwardMoveDisabled && movementInput.y > 0)
            {
                movementInput.y = 0;
            }
            if (backMoveDisabled && movementInput.y < 0)
            {
                movementInput.y = 0;
            }
            if (rightMoveDisabled && movementInput.x > 0)
            {
                movementInput.x = 0;
            }
            if (leftMoveDisabled && movementInput.x < 0)
            {
                movementInput.x = 0;
            }
            movement = movementInput.x * capsule.transform.right + movementInput.y * capsule.transform.forward;
        }
        else
        {
            movement = movementInput.x * cachedCameraRight + movementInput.y * cachedCameraForward;
        }

        playerMoveDir = movement.normalized;

        if (PushBlock != null && movement.magnitude > 0)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 1.75f, acceleration * Time.deltaTime);
        }
        else if (rotatingObject != null && movement.magnitude > 0 && rotatingObject.CanRotate)
        {
            Debug.Log("Rotating");
            currentVelocity = Mathf.MoveTowards(currentVelocity, rotateSpeed, acceleration * Time.deltaTime);
        }
        else if (movement.magnitude > 0)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, currentSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 2, groundDrag * Time.deltaTime);
        }

        Vector3 vel = playerMoveDir * currentVelocity;
        vel.y = rb.linearVelocity.y; // preserve current fall speed
        rb.linearVelocity = vel;

        if (playerMoveDir != Vector3.zero && !interact && PushBlock == null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(playerMoveDir);
            capsule.transform.rotation = Quaternion.Slerp(capsule.transform.rotation, targetRotation, 5 * Time.deltaTime);
        }
        else if (PushBlock != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(PushBlock.transform.position - capsule.transform.position);
            capsule.transform.rotation = Quaternion.Slerp(capsule.transform.rotation, targetRotation, 5 * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.CompareTag("RotatingTag"))
        {
            rotatingObject = hit.gameObject.GetComponent<RotatingObject>();
        }
    }
}
