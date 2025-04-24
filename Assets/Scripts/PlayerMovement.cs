using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

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

    Rigidbody rb;

    public bool isInteracting = false;

    bool isPulling = false;

    bool isPainting;

    bool isDoor;
    public bool IsPainting { get { return isPainting; } set { isPainting = value; } }

    [SerializeField] int interacted;

    public int haveInteracted { get { return interacted; } set { interacted = value; } }

    GameObject targetBlock;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementZ = movementVector.y;
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
        if (isInteracting && targetBlock != null && isPulling)
        {
            // Direction player is moving in
            Vector3 moveDir = (transform.right * movementZ + transform.forward * movementX).normalized;
            rb.mass = 5;
            //rb.linearDamping = 5;
            currentSpeed = 4.2f;
            // If the player is moving, pull the block
            if (moveDir.magnitude > 0f)
            {
                targetBlock.transform.position += moveDir * pullSpeed * Time.deltaTime;
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

        if (isInteracting && haveInteracted == 0 && isDoor)
        {
            haveInteracted += 1;
        }
    }

    private void MovePlayer()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementZ);

        if (movement.magnitude > 0)
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, currentSpeed, acceleration * Time.deltaTime);
        }

        else
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 4, groundDrag * Time.deltaTime);
        }

        //rb.transform.Translate(movement.normalized * currentVelocity);
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

        if (hit.gameObject.CompareTag("DoorEasy"))
        {
            isDoor = true;
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

        if (collision.gameObject.CompareTag("DoorEasy"))
        {
            isDoor = false;
        }
    }
}
