using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]

    public Vector3 movement;
    public float acceleration;
    public float groundDrag;
    public bool DisableRotation { get; set; }

    [SerializeField] float currentSpeed = 8;

    Rigidbody rb;
    [HideInInspector] public PushBlock PushBlock;
    [HideInInspector] public RotatingObject rotatingObject;

    [HideInInspector] public Vector2 movementInput;
    Vector3 playerMoveDir;
    [SerializeField] private float stepRateAtFullSpeed = 0.4f;
    private float stepTimer = 0f;
    [HideInInspector] public float currentVelocity;
    float gravityForce;

    [HideInInspector] public bool forwardMoveDisabled;
    [HideInInspector] public bool backMoveDisabled;
    Gamepad gamepad;

    [Header("GroundCheck")]

    public LayerMask groundMask;

    readonly float distToGround = 1.2f;

    [SerializeField] private bool isGrounded;

    [Header("Check Player Falling")]
    bool isDead = false;
    public float deathHeight;

    [Header("Camera")]

    private Vector3 cachedCameraForward;
    private Vector3 cachedCameraRight;

    [SerializeField] Transform capsule;
    public Transform Capsule => capsule;
    [SerializeField] Transform cameraTransform;
    SceneManagement sceneManagement;

    private void Start()
    {
        gamepad = Gamepad.current;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        sceneManagement = FindFirstObjectByType<SceneManagement>();
    }

    private void Update()
    {
        RotatePlayer();
        PlayWalkingSound();
        CheckPlayerFalling();
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
        GroundCheck();
        MovePlayer();

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

    public void MovePlayer()
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

            movement = movementInput.y * capsule.transform.forward;

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
            if(gamepad != null)
            {
                gamepad.SetMotorSpeeds(0.005f, 0.0015f);
            }
            SoundFXManager.Instance.StartLoopFor(gameObject, SoundType.PushBlock, PushBlock.transform);
            currentVelocity = Mathf.MoveTowards(currentVelocity, 2, acceleration * Time.deltaTime);
        }
        else if (rotatingObject != null)
        {
            currentVelocity = 0;
            if (movementInput.y != 0)
            {
                if (gamepad != null)
                {
                    gamepad.SetMotorSpeeds(0.0005f, 0.0015f);
                }
                SoundFXManager.Instance.StartLoopFor(gameObject, SoundType.PushBlock, this.rotatingObject.transform);
            }
            else
            {
                if (gamepad != null)
                {
                    gamepad.SetMotorSpeeds(0f, 0f);
                }
                SoundFXManager.Instance.StopLoopFor(gameObject);
            }
        }
        else if (movement.magnitude > 0)
        {
            float targetSpeed = currentSpeed * inputMagnitude;
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(0f, 0f);
            }
            SoundFXManager.Instance.StopLoopFor(gameObject);
            currentVelocity = Mathf.MoveTowards(currentVelocity, 0, groundDrag * Time.deltaTime);
        }

        Vector3 vel = playerMoveDir * currentVelocity;
        vel.y = rb.linearVelocity.y; // preserve current fall speed
        rb.linearVelocity = vel;
    }

    void RotatePlayer()
    {
        if (PushBlock != null)
        {
            if (!PushBlock.movedPlayerToTargetPos)
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(PushBlock.transform.position.x, 0, PushBlock.transform.position.z) - new Vector3(capsule.transform.position.x, 0, capsule.transform.position.z));
                capsule.transform.rotation = Quaternion.Slerp(capsule.transform.rotation, targetRotation, 20 * Time.deltaTime);
            }
        }
        else if (rotatingObject != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(rotatingObject.interactPoint.position.x, 0, rotatingObject.interactPoint.position.z) - new Vector3(capsule.transform.position.x, 0, capsule.transform.position.z));
            capsule.transform.rotation = Quaternion.Slerp(capsule.transform.rotation, targetRotation, 5 * Time.deltaTime);
        }
        else if (playerMoveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(playerMoveDir);
            capsule.transform.rotation = Quaternion.Slerp(capsule.transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
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
                SoundFXManager.Instance.PlaySoundFX(SoundType.Walk);
                stepTimer = stepRate;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    /*private void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.CompareTag("RotatingTag"))
        {
            rotatingObject = hit.gameObject.GetComponent<RotatingObject>();
        }
    }*/

    // Player falls
    // private void OnTriggerEnter(Collider other)
    // {
    //     if ((abyssMask.value & (1 << other.transform.gameObject.layer)) > 0)
    //     {
    //         sceneManagement.OnDeath();
    //     }
    // }
    private void CheckPlayerFalling()
    {
        if (isDead == false && transform.position.y < deathHeight)
        {
            Debug.Log("Agnes: Player dies");
            isDead = true;
            sceneManagement.OnDeath();
        }
    }
}
