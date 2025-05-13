using UnityEngine;

public class RotatingObject : MonoBehaviour, IInteracting
{
    [Header("Settings")]
    [SerializeField] float rotateSpeed;

    [Header("Reference")]
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform centerPoint;
    [SerializeField] Outline outlineScript;

    PlayerMovement playerMovement;
    bool move;
    bool inRange;

    void Awake()
    {
        playerMovement = playerTransform.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (inRange)
        {
            if (move)
            {
                playerMovement.rotatingObject = this;

                float input = playerMovement.movementInput.y;

                float angle = -input * rotateSpeed * Time.deltaTime;

                centerPoint.RotateAround(centerPoint.position, Vector3.up, angle);
                playerTransform.RotateAround(centerPoint.position, Vector3.up, angle);
            }
            else
            {
                playerMovement.rotatingObject = null;
            }
        }
    }

    public void PressInteract()
    {
        move = true;
    }

    public void ReleaseInteract()
    {
        move = false;
    }

    public void InteractInRange()
    {
        inRange = true;
        outlineScript.enabled = true;
    }

    public void InteractNotInRange()
    {
        inRange = false;
        playerMovement.rotatingObject = null;
        outlineScript.enabled = false;
    }
}
