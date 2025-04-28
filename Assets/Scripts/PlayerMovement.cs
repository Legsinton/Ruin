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

    private Vector3 playerMoveDir; // Add at top of class

    Rigidbody rb;

    readonly float distToGround = 1f;

    private bool isGrounded;

    public LayerMask groundMask; // assign this in Inspector

    Vector2 movementInput;

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

    private void GroundCheck()
    {
        Vector3 origin = transform.position; // or you can lower this a bit if needed
        origin.y -= 0.5f; // move origin slightly downward if your player is tall
        isGrounded = Physics.Raycast(origin, Vector3.down, distToGround, groundMask);
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

        IsPulling();

        StepClimb();

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
        Debug.Log($"isPulling: {isPulling}, targetBlock: {targetBlock}");

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
            currentSpeed = 10f;
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
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 movement = movementInput.x * cameraRight + movementInput.y * cameraForward;

        playerMoveDir = movement.normalized;

        if (movement.magnitude > 0)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, currentSpeed, acceleration * Time.deltaTime);
        }
        else if (movement.magnitude == 0 && isPulling)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 0, groundDrag * Time.deltaTime);

        }
        else
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 4, groundDrag * Time.deltaTime);
        }

        rb.linearVelocity = movement.normalized * currentVelocity;
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
