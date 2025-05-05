using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PushBlock : MonoBehaviour , IInteracting
{
    [SerializeField] float pullSpeed = 1f;
    UIScript script;
    PlayerMovement playerMove;
    [SerializeField] Transform playerTransform;
    bool canMove;
    Bounds playerBounds;
    Bounds plateBounds;
    Collider playerCollider;
    [SerializeField] Outline outlineScript;
    [SerializeField] float buffer;
    float bufferSides = 2f;
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public Interact interact;
    public LayerMask groundMask;

    public float distToGround = 1f;

    [SerializeField] private bool isGrounded;
    private void Awake()
    {   
        script = FindAnyObjectByType<UIScript>();
        playerMove = FindAnyObjectByType<PlayerMovement>();
        interact = FindAnyObjectByType<Interact>();
    }

    private void Update()
    {
        GroundCheck();
        if (playerCollider != null)
        {
            playerBounds = playerCollider.bounds;
            plateBounds = GetComponent<Collider>().bounds;
            plateBounds.Expand(new Vector3(bufferSides, buffer, bufferSides));

            if (!plateBounds.Intersects(playerBounds))
            {
                playerTransform = null;
                playerCollider = null;
                canMove = false;
                Debug.Log("Bye");
            }
        }

        if (!isGrounded)
        {
            canMove = false;
            playerTransform = null;
            playerCollider = null;

        }

        if (canMove && playerTransform != null && playerMove != null)
        {
            Vector3 moveDir = playerMove.movement;
            moveDir.y = 0;
            
            if (moveDir.magnitude > 0)
            {
                transform.position += moveDir.normalized * pullSpeed * Time.deltaTime;
            }
        }
    }
    private void OnTriggerStay(Collider collision)
    {
        playerBounds = collision.bounds; // The collider bounds of the block
        plateBounds = GetComponent<Collider>().bounds; // The bounds of the plate
        plateBounds.Expand(new Vector3(bufferSides, buffer, bufferSides));
        if (plateBounds.Contains(playerBounds.min) && plateBounds.Contains(playerBounds.max))
        {
            playerCollider = collision;
            playerTransform = collision.transform;
        }

    }
    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }

    private void GroundCheck()
    {
        Bounds bounds = GetComponent<Collider>().bounds;

        Vector3 boxCenter = new Vector3(bounds.center.x, bounds.min.y - 0.05f, bounds.center.z);
        Vector3 boxSize = new Vector3(bounds.size.x * 1f, 0.20f, bounds.size.z * 1f);

        isGrounded = Physics.CheckBox(boxCenter, boxSize / 2, Quaternion.identity, groundMask);
    }

    private void OnDrawGizmosSelected()
    {
        if (GetComponent<Collider>() == null) return;

        Bounds bounds = GetComponent<Collider>().bounds;
        Vector3 boxCenter = new Vector3(bounds.center.x, bounds.min.y - 0.05f, bounds.center.z);
        Vector3 boxSize = new Vector3(bounds.size.x * 1f, 0.20f, bounds.size.z * 1f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
    public void PressInteract()
    {
        if (interact.Interacting && isGrounded)
        {
            canMove = true;
        }
    }

    public void ReleaseInteract() 
    {
        canMove = false;
    }

    public void InteractInRange()
    {
        script.EnableUIHold();
        if (!canMove)
        {
            outlineScript.enabled = true;
        }
    }

    public void InteractNotInRange()
    {
        script.DisebleUIHold();
        outlineScript.enabled = false;
    }

}
