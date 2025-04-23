using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTest : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector3 cameraOffset;
    [SerializeField] Vector3 cameraFocusOffset;
    [SerializeField] float smoothTime;
    [SerializeField] float rotationSpeed;
    [SerializeField] float cameraDistance;
    [SerializeField] float wallDistance;

    [Header("Reference")]
    [SerializeField] Transform cameraTransform;

    Vector3 velocity = Vector3.zero;
    Vector2 mouseDelta;
    Vector2 currentRotation;

    bool disableRotateRight = false;
    bool disableRotateLeft = false;
    
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    void UpdateCameraPosition()
    {
        Vector3 lastCameraPos = cameraTransform.position;
        float lastXRotation = currentRotation.x;

        mouseDelta = Mouse.current.delta.ReadValue();

        currentRotation += mouseDelta * rotationSpeed;

        currentRotation.y = Mathf.Clamp(currentRotation.y, -20, 60);

        if (disableRotateRight)
        {
            if (currentRotation.x < lastXRotation)
            {
                currentRotation.x = lastXRotation;
            }
            else
            {
                disableRotateRight = false;
            }
        }

        if (disableRotateLeft)
        {
            if (currentRotation.x > lastXRotation)
            {
                currentRotation.x = lastXRotation;
            }
            else
            {
                disableRotateLeft = false;
            }
        }

        Quaternion rotation = Quaternion.Euler(-currentRotation.y, currentRotation.x, 0);

        Vector3 newCameraOffset = rotation * cameraOffset * cameraDistance;

        Vector3 newCameraPos = transform.position + newCameraOffset;

        Vector3 direction = newCameraPos - lastCameraPos;
        float distance = direction.magnitude;

        if (Physics.Raycast(lastCameraPos, direction.normalized, out RaycastHit hit2, distance + wallDistance, LayerMask.GetMask("Wall")))
        {
            newCameraPos = hit2.point + hit2.normal * wallDistance;

            Vector3 hitDirection = hit2.point - lastCameraPos;
            float dotProduct = Vector3.Dot(hitDirection, cameraTransform.right);

            if (dotProduct > 0)
            {
                //Debug.Log("The raycast collided to the RIGHT of the camera.");
                disableRotateRight = true;
            }
            else
            {
                //Debug.Log("The raycast collided to the LEFT of the camera.");
                disableRotateLeft = true;
            }
        }

        // Move the camera
        //cameraTransform.position = newCameraPos;

        // Move the camera smooth
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, newCameraPos, ref velocity, smoothTime * Time.deltaTime);
    }

    void UpdateCameraRotation()
    {
        Vector3 directionToPlayer = transform.position - cameraTransform.TransformPoint(cameraFocusOffset);

        Quaternion angleToPlayer = Quaternion.LookRotation(directionToPlayer);

        Quaternion targetRotation = Quaternion.Euler(angleToPlayer.eulerAngles.x, angleToPlayer.eulerAngles.y, 0);

        // Rotate the camera
        cameraTransform.rotation = targetRotation;

        // Rotate the camera smooth
        //cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }
}