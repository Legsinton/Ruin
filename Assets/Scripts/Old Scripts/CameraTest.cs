using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTest : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector3 cameraPlayerOffset;
    [SerializeField] Vector3 cameraFocusOffset;
    [SerializeField] float smoothTime;
    [SerializeField] float rotationSpeed;
    [SerializeField] Vector2 rotationClamp;
    [SerializeField] float targetCameraDistance;
    [SerializeField] float wallDistance;

    [Header("Reference")]
    [SerializeField] Transform cameraTransform;

    Vector3 focusTarget;
    Vector2 mouseDelta;
    Vector2 currentRotation;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnLook(InputValue lookValue)
    {
        mouseDelta = lookValue.Get<Vector2>();
    }

    void LateUpdate()
    {
        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    void UpdateCameraPosition()
    {
        currentRotation += mouseDelta * rotationSpeed;

        currentRotation.y = Mathf.Clamp(currentRotation.y, rotationClamp.x, rotationClamp.y);

        // Calculated the next camera position
        Quaternion rotation = Quaternion.Euler(-currentRotation.y, currentRotation.x, 0);

        Vector3 newCameraOffset = rotation * cameraPlayerOffset.normalized * targetCameraDistance;

        Vector3 newCameraPos = transform.position + newCameraOffset;

        // Check wall collision
        Vector3 direction = transform.position - newCameraPos;
        float distance = Vector3.Distance(transform.position, newCameraPos);
        if (Physics.Raycast(transform.position, -direction.normalized, out RaycastHit hit1, distance + wallDistance, LayerMask.GetMask("Wall")))
        {
            newCameraPos = new Vector3(hit1.point.x, newCameraPos.y, hit1.point.z) + direction.normalized * wallDistance;
        }

        // Update camera position
        //cameraTransform.position = Vector3.Slerp(cameraTransform.position, newCameraPos, smoothTime * Time.deltaTime);
        cameraTransform.position = newCameraPos;

        // Update focus point position
        Vector3 newFocusPos = transform.position - newCameraOffset + cameraFocusOffset;

        //focusTarget = Vector3.Slerp(focusTarget, newFocusPos, smoothTime * Time.deltaTime);
        focusTarget = newFocusPos;
    }

    void UpdateCameraRotation()
    {
        Vector3 directionToFocus = focusTarget - cameraTransform.position;

        Quaternion targetRotation = Quaternion.LookRotation(directionToFocus.normalized);

        cameraTransform.rotation = targetRotation;
    }
}