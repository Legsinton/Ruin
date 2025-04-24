using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
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


    public bool isInteracting = false;

    bool isPulling = false;

    bool isPainting;

    bool isDoor;

    bool door;

    public bool IsDoor { get { return isDoor; } set { isDoor = value; } }
    public bool IsPainting { get { return isPainting; } set { isPainting = value; } }

    [SerializeField] int interacted;

    public int haveInteracted { get { return interacted; } set { interacted = value; } }

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
        MovePlayer();

        IsPulling();    

        IsInteracting();
    }

    private void IsPulling()
    {
        if (isInteracting && targetBlock != null /*&& isPulling*/)
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
    }

    private void IsInteracting()
    {
        if (isInteracting && haveInteracted == 0 && isPainting)
        {
            haveInteracted += 1;
            IsPainting = true;
        }

        if (isInteracting && haveInteracted == 0 && door)
        {
            haveInteracted += 1;

            isDoor = true; 
        }
        else if (isInteracting && haveInteracted == 1 && door)
        {
            isDoor = false;
            haveInteracted = 0;

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

        if (hit.gameObject.CompareTag("Painting")) // Optional: filter by tag
        {
            Debug.Log("It Painting");
            targetBlock = hit.gameObject;
            isPainting = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DoorEasy"))
        {
            door = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("DoorEasy"))
        {
            door = false;
            IsDoor = false;
        }
    }

    private void OnCollisionExit(Collision collision) 
    {
        if (collision.gameObject.CompareTag("Painting")) // Optional: filter by tag
        {
            isPainting = false;
            haveInteracted = 0;
        }

        if (collision.gameObject.CompareTag("Pullable")) // Optional: filter by tag
        {
            isPulling = false;
            rb.linearDamping = 1;
            rb.mass = 1;
            currentSpeed = 10;
        }
    }
}
