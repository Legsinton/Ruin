using System;
using UnityEngine;

public class RotatingObject : MonoBehaviour, IInteracting
{
    [Header("Settings")]
    [SerializeField] float rotateSpeed;
    [SerializeField] float playerOffset;
    [SerializeField] float correctPlayerPosSpeed;
    [SerializeField] float interactRange;
    [SerializeField] Vector2 clampRotation;

    [Header("Reference")]
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform centerPoint;
    [SerializeField] public Transform interactPoint;
    [SerializeField] Outline outlineScript;
    PlayerMovement playerMovement;

    Vector3 targetPos;
    bool interact;
    bool inInteractRange;
    bool playerAttached;
    bool calculatedPlayerPos;

    [HideInInspector] public event Action<float> UpdateTriggerBlocks;

    void Awake()
    {
        playerMovement = playerTransform.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (!inInteractRange) return;

        if (!CheckIfPlayerInRange()) return;

        if (interact)
        {
            if (!playerAttached)
            {
                if (!calculatedPlayerPos)
                {
                    playerMovement.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
                    playerTransform.SetParent(centerPoint);
                    playerMovement.rotatingObject = this;

                    Vector3 toPlayer = playerTransform.position - interactPoint.position;
                    Vector3 localToPlayer = interactPoint.InverseTransformDirection(toPlayer);

                    if (localToPlayer.x < 0)
                    {
                        rotateSpeed = -Mathf.Abs(rotateSpeed);
                        playerOffset = -Mathf.Abs(playerOffset);
                    }
                    else
                    {
                        rotateSpeed = Mathf.Abs(rotateSpeed);
                        playerOffset = Mathf.Abs(playerOffset);
                    }

                    Vector3 offset = interactPoint.right * Mathf.Sign(localToPlayer.x) * Mathf.Abs(playerOffset);
                    targetPos = interactPoint.position + offset;

                    calculatedPlayerPos = true;
                }
                else
                {
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

                if (centerPoint.rotation.eulerAngles.y + angle > clampRotation.y)
                {
                    angle = clampRotation.y - centerPoint.rotation.eulerAngles.y;
                }
                else if (centerPoint.rotation.eulerAngles.y + angle < clampRotation.x)
                {
                    angle = clampRotation.x - centerPoint.rotation.eulerAngles.y;
                }

                centerPoint.Rotate(Vector3.up, angle);
                UpdateTriggerBlocks?.Invoke(centerPoint.rotation.eulerAngles.y);
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

    bool CheckIfPlayerInRange()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) < interactRange)
        {
            outlineScript.enabled = true;
            return true;
        }
        else
        {
            return false;
        }
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
        inInteractRange = true;
    }

    public void InteractNotInRange()
    {
        StopPlayer();
        inInteractRange = false;
        outlineScript.enabled = false;
    }
}
