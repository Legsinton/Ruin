using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PushBlock : MonoBehaviour , IInteracting
{
    float pullSpeed = 0.87f;
    UIScript script;
    PlayerMovement playerMove;
    Transform playerTransform;
    bool canMove;
    Bounds playerBounds;
    Bounds plateBounds;
    Collider playerCollider;
    [SerializeField] Outline outlineScript;
    [SerializeField] float buffer;
    float bufferSides = 2f;
    public bool CanMove { get { return canMove; } }
    private void Awake()
    {   
        script = FindAnyObjectByType<UIScript>();
        playerMove = FindAnyObjectByType<PlayerMovement>();
    }

    private void Update()
    {

        if (playerCollider != null)
        {
            playerBounds = playerCollider.bounds;
            plateBounds = GetComponent<Collider>().bounds;
            plateBounds.Expand(new Vector3(bufferSides, buffer, bufferSides));

            if (!plateBounds.Intersects(playerBounds))
            {
                playerTransform = null;
                playerCollider = null;
                Debug.Log("Bye");
            }
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
            Debug.Log("Hello");
        }

    }
    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }


    public void PressInteract()
    {
        canMove = true;
        //playerMove.CurrentSpeed = 1;

    }

    public void ReleaseInteract() 
    {
        canMove = false;
        //playerMove.CurrentSpeed = 10;
    }

    public void InteractInRange()
    {
        script.EnableUI();
        outlineScript.enabled = true;
    }

    public void InteractNotInRange()
    {
        script.DisebleUI();
        outlineScript.enabled = false;
    }

}
