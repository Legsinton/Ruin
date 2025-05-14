using UnityEngine;
using UnityEngine.UIElements;

public class ChestScript : MonoBehaviour,IInteracting
{
    [Header("Settings")]
    [SerializeField] float openSpeed;
    [SerializeField] float openAngle;

    [Header("Reference")]
    [SerializeField] UIScript UIScript;
    [SerializeField] Outline outlineScript;

    bool isDoorOpen;
    Quaternion closedRotation;
    Quaternion openRotation;

    void Start()
    {
        UIScript = FindAnyObjectByType<UIScript>();
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(openAngle, 0, 0));
    }

    void Update()
    {
        if (!isDoorOpen)
        {
            CloseDoor();
        }
        else if (isDoorOpen)
        {
            OpenDoor();
        }
    }

    void CloseDoor()
    {
        if (Quaternion.Angle(transform.rotation, closedRotation) > 0.5f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, closedRotation, Time.deltaTime * openSpeed);
        }
    }

    void OpenDoor()
    {
        if (Quaternion.Angle(transform.rotation, openRotation) > 0.5f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        }
    }

    public void PressInteract()
    {
        if (!isDoorOpen)
        {
          isDoorOpen = true;
        }
        else if (isDoorOpen)
        {
            isDoorOpen = false;
        }
    }

    public void ReleaseInteract() { }

    public void InteractInRange()
    {
        if (UIScript != null)
        {
            UIScript.EnableUI();
        }
        outlineScript.enabled = true;
    }

    public void InteractNotInRange()
    {
        if (UIScript != null)
        {
            UIScript.DisebleUI();
        }
        outlineScript.enabled = false;
    }

}
