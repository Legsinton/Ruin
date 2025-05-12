using System.Collections.Generic;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class RotatingObject : MonoBehaviour, IInteracting
{
    float pullSpeed = 30;
    UIScript script;
    [SerializeField] bool canMove;
    [SerializeField] PlayerMovement playerMove;
    Transform playerTransform;
    Transform playerRotation;
    bool canRotate;
    bool isStoppingMovement = false;
    [SerializeField] Camera mainCamera;
    [SerializeField] CinemachineCamera cameraCin;
    bool cameraWasDisabled = false;
    Coroutine stopMoveCoroutine;


    float Value;
    public bool CanRotate { get { return canRotate; } set { canRotate = value; } }
    [SerializeField] Outline outlineScript;
    [SerializeField] float buffer;
    [SerializeField] Transform centerPoint;
    public Interact interact;
    private void Awake()
    {
        mainCamera = Camera.main;
        script = FindAnyObjectByType<UIScript>();
        playerMove = FindAnyObjectByType<PlayerMovement>();
        interact = FindAnyObjectByType<Interact>();
        playerRotation = playerMove.transform.GetChild(0).transform;
    }

    private void FixedUpdate()
    {
        if (canRotate && playerTransform != null)
        {
            if (stopMoveCoroutine == null)
            {
                stopMoveCoroutine = StartCoroutine(StopMovement());

            }
            Vector3 directionToCenter = (centerPoint.position - playerTransform.position);
            directionToCenter.y = 0f; // flatten the direction so no vertical tilt

            if (directionToCenter != Vector3.zero)
            {
                playerTransform.forward = directionToCenter.normalized;
            }

            if (canMove)
            {
                if (!cameraWasDisabled)
                {
                    cameraCin.enabled = false;
                    cameraCin.Follow = null;
                    cameraCin.LookAt = null;
                    cameraWasDisabled = true;
                }
                Vector3 toPlayer = (playerTransform.position - centerPoint.position).normalized;
                Vector3 input = new Vector3(playerMove.movement.x, 0f, playerMove.movement.z).normalized;
                float direction = Vector3.Cross(toPlayer, input).y;
                Value = direction * pullSpeed * Time.deltaTime;

                transform.RotateAround(centerPoint.position, Vector3.up, Value);
                playerTransform.RotateAround(centerPoint.position, Vector3.up, Value);
                mainCamera.transform.RotateAround(centerPoint.position, Vector3.up, Value);

            }


        }
        else
        {
            isStoppingMovement = false; // Reset here so it can run again next time

            canMove = false;
            cameraWasDisabled = false;
            playerMove.GetComponent<PlayerMovement>().PushBlock = null;
            if (playerTransform != null)
            {
                cameraCin.enabled = true;
                cameraCin.Follow = playerTransform;
                cameraCin.LookAt = playerTransform;
            }
        }
    }
    private void OnCollisionStay(Collision other)
    {
        playerTransform = other.gameObject.transform;
    }

    private void OnCollisionExit(Collision collision)
    {
        playerTransform = null;
    }

    IEnumerator StopMovement()
    {
        isStoppingMovement = true;
        playerMove.ResetPlayerVelocity();
        yield return new WaitForSeconds(1);
        canMove = true;
        stopMoveCoroutine = null;

    }
    public void SetPlayer(Transform player)
    {
        playerTransform = player;

    }
    public void PressInteract()
    {
        canRotate = true;
    }
    public void ReleaseInteract()
    {
        canRotate = false;
        cameraCin.enabled = true;
        cameraCin.Follow = playerTransform;
        cameraCin.LookAt = playerTransform;
        canMove = false;
        if (stopMoveCoroutine != null)
        {
            StopCoroutine(stopMoveCoroutine);
            stopMoveCoroutine = null;
        }
        isStoppingMovement = false; 
    }

    public void InteractInRange()
    {
        script.EnableUIHold();
        if (!canRotate)
        {
            outlineScript.enabled = true;
        }
    }

    public void InteractNotInRange()
    {
        cameraCin.enabled = true;
        cameraCin.Follow = playerTransform;
        cameraCin.LookAt = playerTransform;
        canMove = false;
        script.DisebleUIHold();
        outlineScript.enabled = false;
    }
}
