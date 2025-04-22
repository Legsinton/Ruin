using UnityEngine;

public class CameraTest : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] Vector3 cameraOffset;
    [SerializeField] float smoothSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float currentAngle;

    void LateUpdate()
    {
        

        Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);

        Vector3 rotatedOffset = rotation * cameraOffset;

        Vector3 newCameraPos = transform.position + rotatedOffset;

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, newCameraPos, smoothSpeed * Time.deltaTime);

        cameraTransform.LookAt(transform);
    }
}
