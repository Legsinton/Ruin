using UnityEngine;

public class PushBlock : MonoBehaviour, IInteracting
{
    [Header("Settings")]
    [SerializeField] float minDistanceToPlayer;
    [SerializeField] float rotationResetSpeed;
    [SerializeField] float minPlayerDistanceToEdge;

    [Header("Reference")]
    [SerializeField] Transform[] playerPositionTargets;
    [SerializeField] Rigidbody rb;
    [SerializeField] Outline outlineScript;

    GameObject player;
    PlayerMovement playerMovement;
    Transform playerRotation;
    Transform currentPlayerPosTarget;

    bool checkDistanceToPlayer;
    bool playerInRange;
    bool moveBlock;
    bool isAttached;
    bool isfalling;
    bool isResettingRotation;

    [HideInInspector] public bool movedPlayerToTargetPos;
    Vector3 offsetToPlayer;

    void Start()
    {
        player = GameObject.Find("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerRotation = player.transform.GetChild(0).transform;
    }

    void Update()
    {
        if (checkDistanceToPlayer)
        {
            CheckIfPlayerInRange();

            if (moveBlock && playerInRange)
            {
                CheckBlockCollision();

                if (!isAttached)
                {
                    playerMovement.PushBlock = this;

                    float closestPlayerPosTarget = float.PositiveInfinity;
                    for (int i = 0; i < playerPositionTargets.Length; i++)
                    {
                        if (Vector3.Distance(playerPositionTargets[i].position, player.transform.position) < closestPlayerPosTarget)
                        {
                            closestPlayerPosTarget = Vector3.Distance(playerPositionTargets[i].position, player.transform.position);
                            currentPlayerPosTarget = playerPositionTargets[i];
                        }
                    }

                    isAttached = true;
                    playerMovement.ResetPlayerVelocity();
                }
                if (!movedPlayerToTargetPos)
                {
                    if (Vector3.Distance(player.transform.position, currentPlayerPosTarget.position) > 0.05)
                    {
                        Vector3 newPos = Vector3.Lerp(player.transform.position, currentPlayerPosTarget.position, 10 * Time.deltaTime);
                        player.transform.position = new Vector3(newPos.x, player.transform.position.y, newPos.z);
                    }
                    else
                    {
                        offsetToPlayer = transform.position - player.transform.position;
                        movedPlayerToTargetPos = true;

                        float snappedY = Mathf.Round(playerRotation.eulerAngles.y / 90f) * 90f;

                        playerRotation.transform.rotation = Quaternion.Euler(playerRotation.rotation.x, snappedY, playerRotation.rotation.z);
                    }
                }
                else
                {
                    rb.transform.position = player.transform.position + offsetToPlayer;
                }
            }
            else if (isAttached)
            {
                playerMovement.PushBlock = null;
                movedPlayerToTargetPos = false;
                isAttached = false;
            }
        }

        if (isResettingRotation)
        {
            Quaternion currentRot = transform.rotation;

            Vector3 currentEuler = currentRot.eulerAngles;
            Quaternion targetRot = Quaternion.Euler(currentEuler.x, 0f, currentEuler.z);

            transform.rotation = Quaternion.RotateTowards(currentRot, targetRot, rotationResetSpeed * Time.deltaTime);
        }

        if (!IsGroundedBelow())
        {
            isfalling = true;
            moveBlock = false;
        }
        else
        {
            if (isfalling)
            {
                if (rb.linearVelocity.magnitude < 0.01f && rb.angularVelocity.magnitude < 0.02f)
                {
                    UnFreeze();
                }
            }
        }
    }

    void CheckIfPlayerInRange()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < minDistanceToPlayer)
        {
            playerInRange = true;
            outlineScript.enabled = true;  
        }
        else
        {
            playerInRange = false;
            outlineScript.enabled = false;
        }
    }

    void CheckBlockCollision()
    {
        float rayDistance = 0.5f;
        Vector3 origin = transform.position + new Vector3(0, 0.5f, 0);
        Quaternion orientation = playerRotation.transform.rotation;

        //Forward
        if (Physics.BoxCast(origin, new Vector3(1f, 0.5f, 0.5f), playerRotation.transform.forward, out RaycastHit hitForward, orientation, rayDistance))
        {
            playerMovement.forwardMoveDisabled = true;
        }
        else
        {
            playerMovement.forwardMoveDisabled = false;
        }
        
        // Backward box
        if (Physics.BoxCast(origin, new Vector3(1f, 0.5f, 0.5f), -playerRotation.transform.forward, out RaycastHit hitBack, orientation, rayDistance))
        {
            if (!hitBack.collider.CompareTag("Player"))
            {
                playerMovement.backMoveDisabled = true;
            }
        }
        else
        {
            playerMovement.backMoveDisabled = false;
        }

        // Backward player
        if (Physics.Raycast(player.transform.position - (playerRotation.transform.forward * minPlayerDistanceToEdge), Vector3.down, 1.5f))
        {
            playerMovement.backMoveDisabled = false;
        }
        else
        {
            playerMovement.backMoveDisabled = true;
        }
    }

    void UnFreeze()
    {
        rb.constraints = RigidbodyConstraints.None;
        transform.rotation = Quaternion.Euler(0, 0f, 0);
        rb.constraints = RigidbodyConstraints.FreezeRotationY;
        isfalling = false;
    }

    bool IsGroundedBelow()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        return Physics.Raycast(origin, Vector3.down, 1.7f);
    }

    public void PressInteract()
    {
        moveBlock = true;
    }

    public void ReleaseInteract() 
    {
        moveBlock = false;
    }

    public void InteractInRange()
    {
        checkDistanceToPlayer = true;
    }

    public void InteractNotInRange()
    {
        checkDistanceToPlayer = false;
        outlineScript.enabled = false;
    }

    public bool shouldObjectBeDestroyed()
    {
        return false;
    }
}
