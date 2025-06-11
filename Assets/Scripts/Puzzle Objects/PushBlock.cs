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
    [SerializeField] CameraFollow cameraFollow;
    [SerializeField] GameObject buttonPromptSelect;
    [SerializeField] GameObject buttonPromptMove;

    GameObject player;
    PlayerMovement playerMovement;
    Transform playerRotation;
    Transform currentPlayerPosTarget;

    bool checkDistanceToPlayer;
    bool playerInRange;
    bool moveBlock;
    bool isAttached;
    bool isfalling;

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
                    cameraFollow.EnableLockCamera(currentPlayerPosTarget.eulerAngles.y);

                    isAttached = true;
                    buttonPromptSelect.SetActive(false);
                    buttonPromptMove.SetActive(true);
                    playerMovement.ResetPlayerVelocity();
                }
                if (!movedPlayerToTargetPos)
                {
                    Vector3 flatPlayerPos = new Vector3(player.transform.position.x, 0, player.transform.position.z);
                    Vector3 flatTargetPos = new Vector3(currentPlayerPosTarget.position.x, 0, currentPlayerPosTarget.position.z);

                    if (Vector3.Distance(flatPlayerPos, flatTargetPos) > 0.05f)
                    {
                        float moveSpeed = 7.5f;
                        Vector3 newPos = Vector3.MoveTowards(player.transform.position, currentPlayerPosTarget.position, moveSpeed * Time.deltaTime);
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
                DetachPlayer();
            }
        }

        if (!IsGroundedBelow() && !isfalling)
        {
            isfalling = true;
            moveBlock = false;
        }
        else
        {
            if (isfalling)
            {
                if (rb.linearVelocity.magnitude < 0.01f && rb.angularVelocity.magnitude < 0.01f)
                {
                    UnFreeze();
                }
            }
        }
    }

    void DetachPlayer()
    {
        cameraFollow.DisableLockCamera();
        playerMovement.PushBlock = null;
        movedPlayerToTargetPos = false;
        isAttached = false;
        buttonPromptSelect.SetActive(true);
        buttonPromptMove.SetActive(false);
    }

    void CheckIfPlayerInRange()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < minDistanceToPlayer)
        {
            playerInRange = true;
            outlineScript.enabled = true;
            if (!moveBlock)
            {
                buttonPromptSelect.SetActive(true);
            }
        }
        else
        {
            playerInRange = false;
            outlineScript.enabled = false;
            buttonPromptSelect.SetActive(false);
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
        DetachPlayer();
    }

    public void InteractInRange()
    {
        checkDistanceToPlayer = true;
    }

    public void InteractNotInRange()
    {
        checkDistanceToPlayer = false;
        outlineScript.enabled = false;
        buttonPromptSelect.SetActive(false);
    }

    public bool shouldObjectBeDestroyed()
    {
        return false;
    }
}
