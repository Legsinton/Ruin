using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float groundDrag;
    [SerializeField] float stepRateAtFullSpeed;
    [SerializeField] bool isGrounded;

    [HideInInspector] public Vector3 movement;
    Vector3 playerMoveDir;
    float stepTimer;
    float gravityForce;
    public bool cutscene;

    [HideInInspector] public PushBlock PushBlock;
    [HideInInspector] public RotatingObject rotatingObject;

    [HideInInspector] public Vector2 movementInput;
    [HideInInspector] public float currentVelocity;

    [HideInInspector] public bool forwardMoveDisabled;
    [HideInInspector] public bool backMoveDisabled;
    Gamepad gamepad;

    [Header("Movement References")]
    [SerializeField] Transform capsule;
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animationController;

    [Header("GroundCheck Settings")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float distToGround;
    [SerializeField] float deathHeight;

    SceneManagement sceneManagement;
    bool isDead;
    [HideInInspector] public Vector3 playerStartPosition;

    [Header("Camera")]
    [SerializeField] Transform cameraTransform;
    CameraFollow cameraFollow;
    Vector3 cachedCameraForward;
    Vector3 cachedCameraRight;

    void Start()
    {
        gamepad = Gamepad.current;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        cameraFollow = GetComponent<CameraFollow>();
        sceneManagement = FindFirstObjectByType<SceneManagement>();
        rb.freezeRotation = true;
        playerStartPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    void Update()
    {
        animationController.SetFloat("Velocity", currentVelocity / maxSpeed);
        RotatePlayer();
        CheckPlayerFalling();
    }
    public void LateUpdate()
    {
        cachedCameraForward = cameraTransform.forward;
        cachedCameraForward.y = 0;
        cachedCameraForward.Normalize();

        cachedCameraRight = cameraTransform.right;
        cachedCameraRight.y = 0;
        cachedCameraRight.Normalize();
    }

    void FixedUpdate()
    {
        GroundCheck();
        MovePlayer();

        if (!isGrounded)
        {
            gravityForce = 20;
            rb.linearVelocity += Vector3.down * gravityForce * Time.deltaTime;
        }
        else
        {
            gravityForce = 1;
        }
    }
    void OnMove(InputValue movementValue)
    {
        if (!cutscene)
        {
            movementInput = movementValue.Get<Vector2>();
        }
    }
    void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, distToGround, groundMask);
    }

    public void ResetPlayerVelocity()
    {
        currentVelocity = 0;
    }

    void MovePlayer()
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

        if (PushBlock != null)
        {
            if (movement.magnitude > 0)
            {
                if (gamepad != null)
                {
                    gamepad.SetMotorSpeeds(0.005f, 0.0015f);
                }
                SoundFXManager.Instance.StartLoopFor(gameObject, SoundType.PushBlock, PushBlock.transform);
                currentVelocity = Mathf.MoveTowards(currentVelocity, 2, acceleration * Time.deltaTime);
            }
            else
            {
                if (gamepad != null)
                {
                    gamepad.SetMotorSpeeds(0f, 0f);
                }
                SoundFXManager.Instance.StopLoopFor(gameObject);
            }

            animationController.SetBool("pushblock", true);

            if (movementInput.y > 0)
            {
                animationController.SetBool("pushblockForward", true);
                animationController.SetBool("pushblockBackward", false);
            }
            else if (movementInput.y < 0)
            {
                animationController.SetBool("pushblockForward", false);
                animationController.SetBool("pushblockBackward", true);
            }
            else
            {
                animationController.SetBool("pushblockForward", false);
                animationController.SetBool("pushblockBackward", false);
            }
        }
        else if (rotatingObject != null)
        {
            animationController.SetBool("pushblocklow", true);
            currentVelocity = 0;
            if (movementInput.magnitude > 0)
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

            if (movementInput.y > 0)
            {
                animationController.SetBool("pushblocklowForward", true);
                animationController.SetBool("pushblocklowBackward", false);
            }
            else if (movementInput.y < 0)
            {
                animationController.SetBool("pushblocklowForward", false);
                animationController.SetBool("pushblocklowBackward", true);
            }
            else
            {
                animationController.SetBool("pushblocklowForward", false);
                animationController.SetBool("pushblocklowBackward", false);
            }
        }
        else if (movement.magnitude > 0) // If player walks
        {
            float targetSpeed = maxSpeed * inputMagnitude;
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetSpeed, acceleration * Time.deltaTime);
            animationController.SetBool("walk", true);

            animationController.SetBool("pushblock", false);
            animationController.SetBool("pushblockForward", false);
            animationController.SetBool("pushblockBackward", false);

            animationController.SetBool("pushblocklow", false);
            animationController.SetBool("pushblocklowForward", false);
            animationController.SetBool("pushblocklowBackward", false);
        }
        else // If player idle
        {
            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(0f, 0f);
            }
            animationController.SetBool("walk", false);

            animationController.SetBool("pushblock", false);
            animationController.SetBool("pushblockForward", false);
            animationController.SetBool("pushblockBackward", false);

            animationController.SetBool("pushblocklow", false);
            animationController.SetBool("pushblocklowForward", false);
            animationController.SetBool("pushblocklowBackward", false);

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
    void CheckPlayerFalling()
    {
        if (/*!isDead &&*/ transform.position.y < deathHeight)
        {
            //isDead = true;
            cameraFollow.StopFollowing();
            cameraFollow.EnableLockCamera(capsule.eulerAngles.y);
            sceneManagement.OnDeath();
        }
    }

    // Action map change
    void OnEnable()
    {
        // playerInput.SwitchCurrentActionMap("UI");
    }
}
