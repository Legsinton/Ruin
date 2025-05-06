using UnityEngine;

public class PushBlock : MonoBehaviour , IInteracting
{
    [Header("Settings")]
    [SerializeField] float minDistanceToPlayer;

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
    bool movedPlayerToTargetPos;
    Vector3 offsetToPlayer;

    void Awake()
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
                }
                if (!movedPlayerToTargetPos)
                {
                    if (Vector3.Distance(player.transform.position, currentPlayerPosTarget.position) > 0.025)
                    {
                        player.transform.position = Vector3.Lerp(player.transform.position, currentPlayerPosTarget.position, 10 * Time.deltaTime);
                    }
                    else
                    {
                        offsetToPlayer = transform.position - player.transform.position;
                        movedPlayerToTargetPos = true;
                    }
                }
                else
                {
                    rb.transform.position = player.transform.position + offsetToPlayer;
                }
            }
            else if (isAttached)
            {
                player.GetComponent<PlayerMovement>().PushBlock = null;
                movedPlayerToTargetPos = false;
                isAttached = false;
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
        Vector3 origin = transform.position;
        Quaternion orientation = playerRotation.transform.rotation;

        if (Physics.BoxCast(origin, new Vector3(1f, 1f, 0.5f), playerRotation.transform.forward, out RaycastHit hitForward, orientation, rayDistance))
        {
            playerMovement.forwardMoveDisabled = true;
        }
        else
        {
            playerMovement.forwardMoveDisabled = false;
        }
        
        // Backward
        if (Physics.BoxCast(origin, new Vector3(1f, 1f, 0.5f), -playerRotation.transform.forward, out RaycastHit hitBack, orientation, rayDistance))
        {
            playerMovement.backMoveDisabled = true;
        }
        else
        {
            playerMovement.backMoveDisabled = false;
        }

        // Left
        if (Physics.BoxCast(origin, new Vector3(0.5f, 1f, 1f), -playerRotation.transform.right, out RaycastHit hitLeft, orientation, rayDistance))
        {
            playerMovement.leftMoveDisabled = true;
        }
        else
        {
            playerMovement.leftMoveDisabled = false;
        }

        // Right
        if (Physics.BoxCast(origin, new Vector3(0.5f, 1f, 1f), playerRotation.transform.right, out RaycastHit hitRight, orientation, rayDistance))
        {
            playerMovement.rightMoveDisabled = true;
        }
        else
        {
            playerMovement.rightMoveDisabled = false;
        }

        // Down
        if (!Physics.Raycast(origin, -playerRotation.transform.up, out RaycastHit hitDown, 1.05f))
        {
            moveBlock = false;
            Debug.Log("TEEST");
        }
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
}
