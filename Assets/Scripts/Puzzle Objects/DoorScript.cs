using UnityEngine;
using UnityEngine.UIElements;

public class DoorEasyScript : MonoBehaviour, IInteracting
{
    [Header("Settings")]
    [SerializeField] float openSpeed;
    [SerializeField] float openAngle;
    [SerializeField] float closingDistance;
    [SerializeField] bool locked;
    [SerializeField] int itemIdToUnlock;

    [Header("Reference")]
    [SerializeField] UIScript UIScript;
    [SerializeField] Outline outlineScript;

    GameObject player;
    bool openingDoor;
    bool closingDoor;
    bool isDoorOpen;
    Quaternion closedRotation;
    Quaternion openRotation;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        UIScript = FindAnyObjectByType<UIScript>();
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    void Update()
    {
        if (locked) return;

        if (openingDoor)
        {
            OpenDoor();
        }
        if (isDoorOpen && !openingDoor)
        {
            if (!closingDoor)
            {
                if (Vector3.Distance(transform.position, player.transform.position) > closingDistance)
                {
                    closingDoor = true;
                }
            }

            if (closingDoor)
            {
                CloseDoor();
            }
        }
    }

    void CloseDoor()
    {
        if (Quaternion.Angle(transform.rotation, closedRotation) > 0.5f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, closedRotation, Time.deltaTime * openSpeed);
        }
        else
        {
            closingDoor = false;
            isDoorOpen = false;
        }
    }

    void OpenDoor()
    {
        if (Quaternion.Angle(transform.rotation, openRotation) > 0.5f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        }
        else
        {
            openingDoor = false;
            isDoorOpen = true;
        }
    }

    public void PressInteract()
    {
        if (locked)
        {
            for (int i = 0; Inventory.Instance.inventoryItems.Count > i; i++)
            {
                if (Inventory.Instance.inventoryItems[i].itemId == itemIdToUnlock)
                {
                    locked = false;
                }
            }
        }

        if (!locked && !isDoorOpen && !openingDoor)
        {
            Vector3 toPlayer = player.transform.position - transform.position;
            float dot = Vector3.Dot(transform.forward, toPlayer.normalized);

            if (dot > 0)
            {
                openAngle = -openAngle;
            }
            else
            {
                openAngle = Mathf.Abs(openAngle);
            }

            openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));

            openingDoor = true;
            InteractNotInRange();
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