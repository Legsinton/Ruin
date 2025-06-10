using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Offset")]
    [SerializeField] Vector3 cameraPlayerOffset;
    [SerializeField] Vector3 cameraFocusOffset;

    [Header("Camera Sensitivity")]
    [SerializeField] float smoothTime;
    [SerializeField] public float mouseSensitivity;
    [SerializeField] public float controllerSensitivity;

    [Header("Camera Rotation and Wall settings")]
    [SerializeField] Vector2 rotationClamp;
    [SerializeField] float targetCameraDistance;
    [SerializeField] float wallDistance;

    [Header("Reference")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform playerRotation;
    [SerializeField] PlayerInput playerInput;

    [HideInInspector] public bool lockCameraToPlayer;
    [HideInInspector] public bool stopFollowingPlayer;
    [HideInInspector] public Vector3 cameraStartPosition;
    float targetXRotation;
    int layerMask;
    float sensitivity;

    Vector3 focusTarget;
    Vector3 previousPlayerPos;
    Vector2 mouseDelta;
    Vector2 currentRotation;

    void Start()
    {
        previousPlayerPos = transform.position;
        layerMask = LayerMask.GetMask("Wall", "whatIsGround", "Interactable");
        cameraStartPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void OnLook(InputValue value)
    {
        mouseDelta = value.Get<Vector2>();

        bool isGamepad = playerInput.currentControlScheme == "Gamepad";

        sensitivity = isGamepad
            ? controllerSensitivity
            : mouseSensitivity;
    }

    void LateUpdate()
    {
        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    void UpdateCameraPosition()
    {
        if (lockCameraToPlayer)
        {
            currentRotation.x = Mathf.LerpAngle(currentRotation.x, targetXRotation, smoothTime * Time.deltaTime);
            currentRotation.y = Mathf.Lerp(currentRotation.y, 45f, smoothTime * Time.deltaTime);
        }
        else
        {
            currentRotation += mouseDelta * sensitivity;

            currentRotation.y = Mathf.Clamp(currentRotation.y, rotationClamp.x, rotationClamp.y);
        }

        // Lerped position offset
        Vector3 smoothedPlayerPos = Vector3.Slerp(previousPlayerPos, transform.position, smoothTime * Time.deltaTime);
        Vector3 playerOffset = smoothedPlayerPos - transform.position;
        previousPlayerPos = smoothedPlayerPos;

        // Instant rotation offset
        Quaternion rotation = Quaternion.Euler(-currentRotation.y, currentRotation.x, 0);
        Vector3 rotatedOffset = rotation * (cameraPlayerOffset.normalized * targetCameraDistance);

        Vector3 newCameraOffset = playerOffset + rotatedOffset;

        // Final camera position and offset
        Vector3 newCameraPos = transform.position + newCameraOffset;

        // Check wall collision
        Vector3 direction = transform.position - newCameraPos;
        float distance = Vector3.Distance(transform.position, newCameraPos);
        if (Physics.Raycast(transform.position, -direction.normalized, out RaycastHit hit1, distance + wallDistance, layerMask))
        {
            newCameraPos = new Vector3(hit1.point.x, newCameraPos.y, hit1.point.z) + direction.normalized * wallDistance;
        }

        if (!stopFollowingPlayer)
        {
            // Update camera position
            cameraTransform.position = newCameraPos;
        }

        // Update focus point position
        Vector3 newFocusPos = transform.position - newCameraOffset + cameraFocusOffset;
        focusTarget = newFocusPos;
    }

    void UpdateCameraRotation()
    {
        Vector3 directionToFocus = focusTarget - cameraTransform.position;

        Quaternion targetRotation = Quaternion.LookRotation(directionToFocus.normalized);

        cameraTransform.rotation = targetRotation;
    }

    public void EnableLockCamera(float rotation)
    {
        targetXRotation = rotation;
        lockCameraToPlayer = true;
    }

    public void DisableLockCamera()
    {
        lockCameraToPlayer = false;
    }

    public void StopFollowing()
    {
        stopFollowingPlayer = true;
    }

    public void StartFollowing()
    {
        stopFollowingPlayer = false;
    }

    public void SnapToPosition()
    {
        // Lerped position offset
        Vector3 smoothedPlayerPos = Vector3.Slerp(previousPlayerPos, transform.position, smoothTime * Time.deltaTime);
        Vector3 playerOffset = smoothedPlayerPos - transform.position;
        previousPlayerPos = smoothedPlayerPos;

        // Instant rotation offset
        Quaternion rotation = Quaternion.Euler(-currentRotation.y, currentRotation.x, 0);
        Vector3 rotatedOffset = rotation * (cameraPlayerOffset.normalized * targetCameraDistance);

        Vector3 newCameraOffset = playerOffset + rotatedOffset;

        // Final camera position and offset
        Vector3 newCameraPos = transform.position + newCameraOffset;
        transform.position = newCameraPos;
    }
}