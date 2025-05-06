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
    }

    void Update()
    {
        if (checkDistanceToPlayer)
        {
            CheckIfPlayerInRange();

            if (moveBlock && playerInRange)
            {
                if (!isAttached)
                {
                    player.GetComponent<PlayerMovement>().PushBlock = this;

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
                    if (Vector3.Distance(player.transform.position, currentPlayerPosTarget.position) > 0.05)
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
