using UnityEngine;

public class RotatingObject : MonoBehaviour, IInteracting
{
    [Header("Settings")]
    [SerializeField] float rotateSpeed;
    [SerializeField] float offsetToPlayer;

    [Header("Reference")]
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform centerPoint;
    [SerializeField] Outline outlineScript;

    PlayerMovement playerMovement;
    bool move;
    bool inRange;
    bool playerAttached;

    void Awake()
    {
        playerMovement = playerTransform.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (!inRange) return;

        if (move)
        {
            if (!playerAttached)
            {
                playerMovement.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
                playerTransform.SetParent(centerPoint);
                playerMovement.rotatingObject = this;
                playerAttached = true;
            }

            float input = playerMovement.movementInput.y;
            float angle = -input * rotateSpeed * Time.deltaTime;
            centerPoint.Rotate(Vector3.up, angle);
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
        Debug.Log("TEEEST");
        
        inRange = false;
        StopPlayer();
        outlineScript.enabled = false;
    }
}
