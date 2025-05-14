using Unity.AppUI.UI;
using UnityEngine;

public class RotatingObject : MonoBehaviour, IInteracting
{
    [Header("Settings")]
    [SerializeField] float rotateSpeed;
    [SerializeField] float offsetToPlayer;
    [SerializeField] float correctPlayerPosSpeed;

    [Header("Reference")]
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform centerPoint;
    [SerializeField] Outline outlineScript;

    PlayerMovement playerMovement;
    Vector3 targetPos;
    bool interact;
    bool inRange;
    bool playerAttached;
    bool calculatedPlayerPos;

    void Awake()
    {
        playerMovement = playerTransform.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (!inRange) return;

        if (interact)
        {
            if (!playerAttached)
            {
                if (!calculatedPlayerPos)
                {
                    playerMovement.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
                    playerTransform.SetParent(centerPoint);
                    playerMovement.rotatingObject = this;

                    Vector3 toPlayer = playerTransform.position - transform.position;
                    Vector3 localToPlayer = transform.InverseTransformDirection(toPlayer);

                    if (localToPlayer.x < 0)
                    {
                        rotateSpeed = Mathf.Abs(rotateSpeed);
                        offsetToPlayer = -Mathf.Abs(offsetToPlayer);
                    }
                    else
                    {
                        rotateSpeed = -Mathf.Abs(rotateSpeed);
                        offsetToPlayer = Mathf.Abs(offsetToPlayer);
                    }

                    Vector3 offset = transform.right * Mathf.Sign(localToPlayer.x) * Mathf.Abs(offsetToPlayer);
                    targetPos = transform.position + offset;

                    calculatedPlayerPos = true;
                }
                else
                {
                    Debug.Log(Vector3.Distance(playerTransform.position, targetPos));
                    if (Vector3.Distance(playerTransform.position, targetPos) > 0.1)
                    {
                        playerTransform.position = Vector3.Lerp(playerTransform.position, targetPos, correctPlayerPosSpeed * Time.deltaTime);
                    }
                    else
                    {
                        playerAttached = true;
                    }
                }
            }
            else
            {
                float input = playerMovement.movementInput.y;
                float angle = -input * rotateSpeed * Time.deltaTime;
                centerPoint.Rotate(Vector3.up, angle);
            }
        }
        else if (playerAttached)
        {
            StopPlayer();
        }
    }

    void StopPlayer()
    {
        playerMovement.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        playerTransform.SetParent(null);
        playerMovement.rotatingObject = null;
        playerAttached = false;
        calculatedPlayerPos = false;
        playerTransform.localScale = Vector3.one;
    }

    public void PressInteract()
    {
        interact = true;
    }

    public void ReleaseInteract()
    {
        interact = false;
        StopPlayer();
    }

    public void InteractInRange()
    {
        inRange = true;
        outlineScript.enabled = true;
    }

    public void InteractNotInRange()
    {
        inRange = false;
        StopPlayer();
        outlineScript.enabled = false;
    }
}
