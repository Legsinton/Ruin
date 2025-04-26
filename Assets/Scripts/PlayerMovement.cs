using UnityEngine;
using UnityEngine.InputSystem;
using static Interact;

public class PlayerMovement : MonoBehaviour, Interact.IInteracting
{

    [Header("Movement")]

    public float currentVelocity;

    private float movementX;
    private float movementZ;

    public float currentSpeed;
    
    public float acceleration;

    public float groundDrag;

    public float pullSpeed;

    private Vector3 playerMoveDir; // Add at top of class

    Rigidbody rb;

    [Header("Stairs")]

    [SerializeField] GameObject stepUpLower;
    [SerializeField] GameObject stepUpHigher;

    [SerializeField] float stepHeight;

    [SerializeField] float stepSmooth;


    bool isInteracting;

    bool isPulling = false;

    GameObject targetBlock;

    [SerializeField] Transform cameraTransform;
    Vector2 movementInput;

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

    private void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            isInteracting = true;
            Debug.Log("Interact Pressed");
        }

        else
        {
            rb.linearDamping = 1;
            rb.mass = 1;
            currentSpeed = 10;
            isInteracting = false;
            Debug.Log("Interact Released");
        }
    }

    private void FixedUpdate()
    {
        Debug.Log($"isInteracting: {isInteracting}");

        MovePlayer();

        IsPulling();

        StepClimb();
    }

    private void IsPulling()
    {
        Debug.Log($"isPulling: {isPulling}, targetBlock: {targetBlock}");

        if (isInteracting && targetBlock != null && isPulling)
        {
            Debug.Log("Helloooooo");
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
        }
    }


    private void OnCollisionExit(Collision collision) 
    {
        if (collision.gameObject.CompareTag("Pullable")) // Optional: filter by tag
        {
            isPulling = false;
            rb.linearDamping = 1;
            rb.mass = 1;
            currentSpeed = 10;
        }
    }

    public void OnInteractHold()
    {
        isInteracting = !isInteracting; // Toggle pulling based on button state
        

    }

    public void OnInteractTap()
    {
        // Nothing here yet
    }
}
