using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector3 cameraPlayerOffset;
    [SerializeField] Vector3 cameraFocusOffset;
    [SerializeField] float smoothTime;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float controllerSensitivity;
    [SerializeField] Vector2 rotationClamp;
    [SerializeField] float targetCameraDistance;
    [SerializeField] float wallDistance;

    [Header("Reference")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] PlayerInput playerInput;

    int layerMask;
    float sensitivity;

    Vector3 focusTarget;
    Vector3 previousPlayerPos;
    Vector2 mouseDelta;
    Vector2 currentRotation;

    void Start()
    {
        previousPlayerPos = transform.position;
        layerMask = LayerMask.GetMask("Wall", "whatIsGround");
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
        currentRotation += mouseDelta * sensitivity;

        currentRotation.y = Mathf.Clamp(currentRotation.y, rotationClamp.x, rotationClamp.y);

        // Lerped position offset
        Vector3 smoothedPlayerPos = Vector3.Slerp(previousPlayerPos, transform.position, smoothTime * Time.deltaTime);
        Vector3 playerOffset = smoothedPlayerPos - transform.position;
        previousPlayerPos = smoothedPlayerPos;

        // Instant rotation offset
        Quaternion rotation = Quaternion.Euler(-currentRotation.y, currentRotation.x, 0);
        Vector3 rotatedOffset = rotation * (cameraPlayerOffset.normalized * targetCameraDistance);

        // Final camera position and offset
        Vector3 newCameraOffset = playerOffset + rotatedOffset;
        Vector3 newCameraPos = transform.position + newCameraOffset;

        // Check wall collision
        Vector3 direction = transform.position - newCameraPos;
        float distance = Vector3.Distance(transform.position, newCameraPos);
        if (Physics.Raycast(transform.position, -direction.normalized, out RaycastHit hit1, distance + wallDistance, layerMask))
        {
            newCameraPos = new Vector3(hit1.point.x, newCameraPos.y, hit1.point.z) + direction.normalized * wallDistance;
        }

        // Update camera position
        cameraTransform.position = newCameraPos;

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
}