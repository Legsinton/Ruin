using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    
    public Vector3 movement;
    public float acceleration;
    public float groundDrag;

    [SerializeField] float currentSpeed = 8;
    
    Rigidbody rb;
    PushBlock PushBlock;

    Vector2 movementInput;
    Vector3 playerMoveDir; 

    float gravityForce;
    float currentVelocity;

    bool interact;

    [Header("GroundCheck")]

    public LayerMask groundMask; 

    readonly float distToGround = 1f;

    private bool isGrounded;

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
        if (PushBlock != null && PushBlock.CanMove)
        {
            virtualCamera.LookAt = null;
            if (movement.x > 0.5)
            {
                orbitalFollow.HorizontalAxis.Value = 82;
                orbitalFollow.VerticalAxis.Value = 17.5f;
            }
            else if ( movement.x < -0.5f)
            {
                orbitalFollow.HorizontalAxis.Value = -104;
                orbitalFollow.VerticalAxis.Value = 17.5f;
            }
            else if (movement.z > 0.5f)
            {
                orbitalFollow.HorizontalAxis.Value = 8;
                orbitalFollow.VerticalAxis.Value = 17.5f;
            }
            else if (movement.z < -0.5f)
            {
                orbitalFollow.HorizontalAxis.Value = 148;
                orbitalFollow.VerticalAxis.Value = 17.5f;
            }         
        }
        else
        {
            virtualCamera.LookAt = capsule;
        }

        if (PushBlock != null)
        {
            interact = PushBlock.CanMove;
        }
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

        StepClimb();

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
        Vector3 origin = transform.position; // or you can lower this a bit if needed
        origin.y -= 0.5f; // move origin slightly downward if your player is tall
        isGrounded = Physics.Raycast(origin, Vector3.down, distToGround, groundMask);
    }
    private void StepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepUpLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepUpHigher.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.1f))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        else
        {
            stepUpHigher.transform.position = new Vector3(stepUpHigher.transform.position.x, stepHeight, stepUpHigher.transform.position.z);
        }
    }
    private void MovePlayer()
    {
        if (PushBlock != null && PushBlock.CanMove)
        {
            movement = movementInput.y * cachedCameraForward;

            if (movement.x > 0.5 || movement.x < -0.5f)
            {
                movement.z = 0;

            }
            else if (movement.z > 0.5f || movement.z < -0.5f)
            {
                movement.x = 0; 
            }
        }
        else
        {
            movement = movementInput.x * cachedCameraRight + movementInput.y * cachedCameraForward;
        }

        playerMoveDir = movement.normalized;


        if (PushBlock != null && movement.magnitude > 0 && PushBlock.CanMove)
        {
            Debug.Log("PushBlack");
            currentVelocity = Mathf.MoveTowards(currentVelocity, 1, 1);

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

        if (playerMoveDir != Vector3.zero && PushBlock != null && interact)
        {
         
        }

        else if (playerMoveDir != Vector3.zero && !interact)
        {
            Quaternion targetRotation = Quaternion.LookRotation(playerMoveDir);
            capsule.transform.rotation = Quaternion.Slerp(capsule.transform.rotation, targetRotation, 5 * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.CompareTag("Pullable")) // Optional: filter by tag
        {
            PushBlock = hit.gameObject.GetComponent<PushBlock>();
        }
    }
    private void OnCollisionExit(Collision collision) 
    {
        if (collision.gameObject.CompareTag("Pullable")) // Optional: filter by tag
        {
            //PushBlock = null;
            
        }
    }
}
