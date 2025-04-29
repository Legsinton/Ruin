using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
//using static Unity.Cinemachine.InputAxisControllerBase<T>;

public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]

    public float currentVelocity;

    readonly private float movementX;
    readonly private float movementZ;

    private float currentSpeed = 10;
    
    public float acceleration;

    public float groundDrag;

    public float pullSpeed;

    float gravityForce;

    public Vector3 playerMoveDir; // Add at top of class

    Rigidbody rb;
    public Vector3 movement;

    readonly float distToGround = 1f;

    private bool isGrounded;

    public LayerMask groundMask; // assign this in Inspector

    Vector2 movementInput;

    //[SerializeField] Transform movementTransform;

    [Header("Camera")]

    private Vector3 cachedCameraForward;
    private Vector3 cachedCameraRight;

    [SerializeField] Transform capsule;

    [Header("Stairs")]

    [SerializeField] GameObject stepUpLower;
    [SerializeField] GameObject stepUpHigher;

    [SerializeField] float stepHeight;

    [SerializeField] float stepSmooth;
    

    [Header("Interaction")]

    [SerializeField] Transform cameraTransform;

    [SerializeField] bool isInteracting;

    bool isPulling = false;

    GameObject targetBlock;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Awake()
    {
        stepUpHigher.transform.position = new Vector3(stepUpHigher.transform.position.x, stepHeight, stepUpHigher.transform.position.z);
    }

    private void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    private void LateUpdate()
    {
        CacheCameraVectors();

    }

    private void GroundCheck()
    {
        Vector3 origin = transform.position; // or you can lower this a bit if needed
        origin.y -= 0.5f; // move origin slightly downward if your player is tall
        isGrounded = Physics.Raycast(origin, Vector3.down, distToGround, groundMask);
    }

    private void CacheCameraVectors()
    {
        cachedCameraForward = cameraTransform.forward;
        cachedCameraForward.y = 0;
        cachedCameraForward.Normalize();

        cachedCameraRight = cameraTransform.right;
        cachedCameraRight.y = 0;
        cachedCameraRight.Normalize();
    }

    private void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            isInteracting = true;
        }

        else
        {
            rb.linearDamping = 1;
            rb.mass = 1;
            currentSpeed = 10;
            isInteracting = false;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();

       // IsPulling();

        //StepClimb();

        GroundCheck();


        if (!isGrounded)
        {
            gravityForce = 1000;
            rb.linearVelocity += Vector3.down * gravityForce * Time.deltaTime;
        }
        else
        {
            gravityForce = 1;
        }
    }

    private void IsPulling()
    {
        if (isInteracting && targetBlock != null && isPulling)
        {
            // Direction player is moving in
            Vector3 moveDir = (transform.right * movementZ + transform.forward * movementX).normalized;
            rb.mass = 5;
            rb.linearDamping = 5;
            currentSpeed = 1f;
            // If the player is moving, pull the block
            if (playerMoveDir.magnitude > 0f)
            {
                targetBlock.transform.position += playerMoveDir * pullSpeed * Time.deltaTime;
            }
        }
        else
        {
            rb.mass = 1;
            rb.linearDamping = 1;
           
        }
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
        movement = movementInput.x * cachedCameraRight + movementInput.y * cachedCameraForward;

        playerMoveDir = movement.normalized;

        if (movement.magnitude > 0)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, currentSpeed, acceleration * Time.deltaTime);
        }
        else if (movement.magnitude > 0 && isPulling)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 0, 1);
            currentSpeed = 1;
        }
        else
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 4, groundDrag * Time.deltaTime);
        }

        rb.linearVelocity = movement.normalized * currentVelocity * movement.magnitude;

        if (!isInteracting && playerMoveDir != Vector3.zero)
        {
           
            Quaternion targetRotation = Quaternion.LookRotation(playerMoveDir);
            capsule.transform.rotation = Quaternion.Slerp(capsule.transform.rotation, targetRotation, 5 * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.CompareTag("Pullable")) // Optional: filter by tag
        {
            Debug.Log("It Collides");
            targetBlock = hit.gameObject;
            isPulling = true;
            currentSpeed = 1;
        }
    }


    private void OnCollisionExit(Collision collision) 
    {
        if (collision.gameObject.CompareTag("Pullable")) // Optional: filter by tag
        {
            isPulling = false;
            rb.linearDamping = 1;
            rb.mass = 1;
            
        }
    }
}
