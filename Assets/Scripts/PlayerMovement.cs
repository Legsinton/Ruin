using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]

    public Vector3 movement;
    public float acceleration;
    public float groundDrag;
    [SerializeField] float rotateSpeed;
    public bool DisableRotation { get; set; }

    [SerializeField] float currentSpeed = 8;

    Rigidbody rb;
    [HideInInspector] public PushBlock PushBlock;
    public RotatingObject rotatingObject;

    Vector2 movementInput;
    Vector3 playerMoveDir;
    [SerializeField] private float stepRateAtFullSpeed = 0.4f;
    private float stepTimer = 0f;
    [HideInInspector] public float currentVelocity;
    float gravityForce;
    bool interact;

    [HideInInspector] public bool rightMoveDisabled;
    [HideInInspector] public bool leftMoveDisabled;
    [HideInInspector] public bool forwardMoveDisabled;
    [HideInInspector] public bool backMoveDisabled;

    [Header("GroundCheck")]

    public LayerMask groundMask;

    readonly float distToGround = 1.2f;

    [SerializeField] private bool isGrounded;

    [Header("Camera")]

    private Vector3 cachedCameraForward;
    private Vector3 cachedCameraRight;

    [SerializeField] Transform capsule;
    public Transform Capsule => capsule;
    [SerializeField] Transform cameraTransform;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        //SetCamera();
        PlayWalkingSound();
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
        isGrounded = Physics.Raycast(transform.position, Vector3.down, distToGround, groundMask);
    }

    public void ResetPlayerVelocity()
    {
        currentVelocity = 0;
    }

    private void MovePlayer()
    {
        Debug.Log(PushBlock);
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

            //movement = movementInput.x * capsule.transform.right + movementInput.y * capsule.transform.forward;
            movement = movementInput.y * capsule.transform.forward;

            //SoundFXManager.Instance.Start3DLoop(SoundType.Roll, transform.position);
            //SoundFXManager.Instance.StopLoop();

            if (!PushBlock.movedPlayerToTargetPos)
            {
                movement = Vector2.zero;
            }
        }
        else
        {
            movement = movementInput.x * cachedCameraRight + movementInput.y * cachedCameraForward;
        }

        float inputMagnitude = movement.magnitude;
        playerMoveDir = movement.normalized;


        if (PushBlock != null && movement.magnitude > 0)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 2, acceleration * Time.deltaTime);
        }
        else if (rotatingObject != null && movement.magnitude > 0 && rotatingObject.CanRotate)
        {
            Debug.Log("Rotating");
            currentVelocity = Mathf.MoveTowards(currentVelocity, rotateSpeed, acceleration * Time.deltaTime);
        }
        else if (movement.magnitude > 0)
        {
            float targetSpeed = currentSpeed * inputMagnitude;
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 0, groundDrag * Time.deltaTime);
        }

        Vector3 vel = playerMoveDir * currentVelocity;
        vel.y = rb.linearVelocity.y; // preserve current fall speed
        rb.linearVelocity = vel;

        if (playerMoveDir != Vector3.zero && !interact && PushBlock == null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(playerMoveDir);
            capsule.transform.rotation = Quaternion.Slerp(capsule.transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
        else if (PushBlock != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(PushBlock.transform.position - capsule.transform.position);
            capsule.transform.rotation = Quaternion.Slerp(capsule.transform.rotation, targetRotation, 5 * Time.deltaTime);
        }
       /* else if (rotatingObject != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rotatingObject.transform.position - capsule.transform.position);
            capsule.transform.rotation = Quaternion.Slerp(capsule.transform.rotation, targetRotation, 5 * Time.deltaTime);
        }*/
    }

    void PlayWalkingSound()
    {
        if (playerMoveDir.magnitude > 0.1f && isGrounded)
        {
            stepTimer -= Time.deltaTime;

            float normalizedSpeed = currentVelocity / currentSpeed;
            float stepRate = Mathf.Lerp(0.8f, stepRateAtFullSpeed, normalizedSpeed);

            if (stepTimer <= 0f)
            {
                SoundFXManager.Instance.PlaySoundFX(SoundType.Walk, transform.position);
                stepTimer = stepRate;
            }
        }
        else
        {
            stepTimer = 0f;
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
