using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class RotatingObject : MonoBehaviour, IInteracting
{
    float pullSpeed = 15;
    UIScript script;
    PlayerMovement playerMove;
    Transform playerTransform;
    bool canRotate;
    [SerializeField] Camera mainCamera;
    [SerializeField] CinemachineCamera cameraCin;

    [SerializeField] float Value;
    public bool CanRotate { get { return canRotate; } set { canRotate = value; } }
    [SerializeField] Outline outlineScript;
    [SerializeField] float buffer;
    [SerializeField] Transform centerPoint;
    public Interact interact;
    private void Awake()
    {
        script = FindAnyObjectByType<UIScript>();
        playerMove = FindAnyObjectByType<PlayerMovement>();
        interact = FindAnyObjectByType<Interact>();        
    }

    private void FixedUpdate()
    {
        if (canRotate && playerTransform != null)
        {
            cameraCin.enabled = false;
            cameraCin.Follow = null;
            cameraCin.LookAt = null;
            Vector3 toPlayer = (playerTransform.position - centerPoint.position).normalized;
            Vector3 input = new Vector3(playerMove.movement.x, 0f, playerMove.movement.z).normalized;
            float direction = Vector3.Cross(toPlayer, input).y;
            Value = direction * pullSpeed * Time.deltaTime;

            transform.RotateAround(centerPoint.position, Vector3.up, Value);
            playerTransform.RotateAround(centerPoint.position, Vector3.up, Value);
            mainCamera.transform.RotateAround(centerPoint.position, Vector3.up, Value);

        }
    }
    private void OnTriggerStay(Collider other)
    {
        playerTransform = other.transform;
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
        script.DisebleUIHold();
        outlineScript.enabled = false;
    }
}
