using Unity.VisualScripting;
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
    [SerializeField] float minCameraDistance;
    [SerializeField] float wallDistance;

    [Header("Reference")]
    [SerializeField] Transform cameraTransform;

    Vector3 velocity = Vector3.zero;
    Vector2 mouseDelta;
    Vector2 currentRotation;

    bool disableRotateRight = false;
    bool disableRotateLeft = false;
    bool disableRotateUp = false;
    bool disableRotateDown = false;

    float distanceToplayer;
    bool movingCamera;

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
        // Save last camera position and last mmouse x value
        Vector3 lastCameraPos = cameraTransform.position;
        Vector2 lastRotation = currentRotation;

        mouseDelta = mouseDelta;

        currentRotation += mouseDelta * rotationSpeed;

        currentRotation.y = Mathf.Clamp(currentRotation.y, -20, 60);

        // Check if left or right horizontal rotation is disabled
        if (disableRotateRight)
        {
            if (currentRotation.x < lastRotation.x)
            {
                currentRotation.x = lastRotation.x;
            }
            else
            {
                disableRotateRight = false;
            }
        }
        if (disableRotateLeft)
        {
            if (currentRotation.x > lastRotation.x)
            {
                currentRotation.x = lastRotation.x;
            }
            else
            {
                disableRotateLeft = false;
            }
        }


        // Calculated the next camera position
        Quaternion rotation = Quaternion.Euler(-currentRotation.y, currentRotation.x, 0);

        Vector3 newCameraOffset = rotation * cameraOffset * cameraDistance;

        Vector3 newCameraPos = transform.position + newCameraOffset;

        Vector3 direction = newCameraPos - lastCameraPos;

        // Check if the new position will collide with the camera
        float distance = direction.magnitude;
        if (Physics.Raycast(lastCameraPos, direction.normalized, out RaycastHit hit2, distance + wallDistance, LayerMask.GetMask("Wall")))
        {
            newCameraPos = hit2.point + hit2.normal * wallDistance;

            Vector3 hitDirection = hit2.point - lastCameraPos;
            float dotProduct = Vector3.Dot(hitDirection, cameraTransform.right);
            float dotProductUp = Vector3.Dot(hitDirection, cameraTransform.up);

            if (dotProduct > 0)
            {
                //If the raycast hit the right of the camera
                disableRotateRight = true;
            }
            else
            {
                //If the raycast hit the left of the camera
                disableRotateLeft = true;
            }
        }

        distanceToplayer = Vector3.Distance(cameraTransform.position, transform.position);
        if (!movingCamera && distanceToplayer < minCameraDistance)
        {
            currentRotation.x -= 180;

            rotation = Quaternion.Euler(-currentRotation.y, currentRotation.x, 0);

            newCameraOffset = rotation * cameraOffset * cameraDistance;

            newCameraPos = transform.position + newCameraOffset;

            movingCamera = true;
        }
        if (distanceToplayer > minCameraDistance)
        {
            movingCamera = false;
        }

        cameraTransform.position = Vector3.Slerp(cameraTransform.position, newCameraPos, smoothTime * Time.deltaTime);   
    }

    void UpdateCameraRotation()
    {
        Vector3 directionToPlayer = transform.position - cameraTransform.TransformPoint(cameraFocusOffset);

        Quaternion angleToPlayer = Quaternion.LookRotation(directionToPlayer);

        Quaternion targetRotation = Quaternion.Euler(angleToPlayer.eulerAngles.x, angleToPlayer.eulerAngles.y, 0);

        cameraTransform.rotation = targetRotation;
    }
}

/*
if (disableRotateUp)
{
    if (currentRotation.y < lastRotation.y)
    {
        currentRotation.y = lastRotation.y;
    }
    else
    {
        disableRotateUp = false;
    }
}
if (disableRotateDown)
{
    if (currentRotation.y > lastRotation.y)
    {
        currentRotation.y = lastRotation.y;
    }
    else
    {
        disableRotateDown = false;
    }
}

if (dotProductUp > 0)
{
    //If the raycast hit above of the camera
    disableRotateUp = true;
}
else
{
    //If the raycast hit below of the camera
    disableRotateDown = true;
}*/