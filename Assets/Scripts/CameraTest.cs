using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTest : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] Vector3 cameraOffset;
    [SerializeField] float smoothSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float distanceToMove;

    Vector3 velocity = Vector3.zero;
    Vector2 mouseDelta;
    Vector2 currentRotation;
    
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    void UpdateCameraPosition()
    {
        mouseDelta = Mouse.current.delta.ReadValue() * rotationSpeed;

        currentRotation += mouseDelta;

        currentRotation.y = Mathf.Clamp(currentRotation.y, -20, 60);

        Quaternion rotation = Quaternion.Euler(-currentRotation.y, currentRotation.x, 0);

        Vector3 newCameraOffset = rotation * cameraOffset;

        Vector3 newCameraPos = transform.position + newCameraOffset;

        cameraTransform.position = newCameraPos;

        //cameraTransform.position = Vector3.Lerp(cameraTransform.position, newCameraPos, smoothSpeed * Time.deltaTime);
    }

    void UpdateCameraRotation()
    {
        Vector3 directionToPlayer = transform.position - cameraTransform.localPosition;

        Quaternion angleToPlayer = Quaternion.LookRotation(directionToPlayer);

        Quaternion targetRotation = Quaternion.Euler(angleToPlayer.eulerAngles.x, angleToPlayer.eulerAngles.y, 0);

        cameraTransform.rotation = targetRotation;

        /*
        Vector3 moveDirection = directionToPlayer.normalized;

        Vector3 targetCameraPosition = transform.position + moveDirection * distanceToMove;

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetCameraPosition, smoothSpeed * Time.deltaTime);*/
    }
}
