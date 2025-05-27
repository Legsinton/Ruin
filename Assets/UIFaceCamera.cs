using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
        else
        {
            Debug.LogError("Assign camera to: " + gameObject.transform.parent.parent.name);
        }
    }
}
