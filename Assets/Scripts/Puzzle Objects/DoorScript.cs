using UnityEngine;

public class DoorEasyScript : MonoBehaviour, IInteracting
{
    [Header("Settings")]
    [SerializeField] float openSpeed;
    [SerializeField] float openAngle;
    [SerializeField] float closingDistance;
    [SerializeField] bool locked;
    [SerializeField] int itemIdToUnlock;

    [Header("Reference")]
    [SerializeField] GameObject[] buttonPrompts;
    [SerializeField] Outline outlineScript;

    GameObject player;
    bool openingDoor;
    bool closingDoor;
    bool isDoorOpen;
    bool played;
    bool inInteractRange;
    Quaternion closedRotation;
    Quaternion openRotation;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
            if (played)
            {
                played = false;
                SoundFXManager.Instance.StartLoopFor(gameObject, SoundType.DoorOpen, this.transform);
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, closedRotation, Time.deltaTime * openSpeed);
        }
        else
        {
            SoundFXManager.Instance.StopLoopFor(gameObject);
            closingDoor = false;
            isDoorOpen = false;
            if (inInteractRange)
            {
                outlineScript.enabled = true;

                foreach (var item in buttonPrompts)
                {
                    item.SetActive(true);
                }
            }
        }
    }

    void OpenDoor()
    {
        if (Quaternion.Angle(transform.rotation, openRotation) > 0.5f)
        {
            if (!played)
            {
                played = true;
                SoundFXManager.Instance.StartLoopFor(gameObject,SoundType.DoorOpen,this.transform);
                foreach (var item in buttonPrompts)
                {
                    item.SetActive(false);
                }
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        }
        else
        {
            SoundFXManager.Instance.StopLoopFor(gameObject);
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
            if (locked)
            {
                PlayerUI.Instance.DisplayText("It appears to be locked", 3);
            }
        }

        if (!locked && !isDoorOpen && !openingDoor)
        {
            Vector3 toPlayer = player.transform.position - transform.position;
            float dot = Vector3.Dot(transform.forward, toPlayer.normalized);

            if (dot > 0)
            {
                openAngle = -Mathf.Abs(openAngle);
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
        inInteractRange = true;

        if (!isDoorOpen && !openingDoor && !closingDoor)
        {
            outlineScript.enabled = true;

            foreach(var item in buttonPrompts)
            {
                item.SetActive(true);
            }
        }
    }

    public void InteractNotInRange()
    {
        inInteractRange = false;

        outlineScript.enabled = false;
        foreach (var item in buttonPrompts)
        {
            item.SetActive(false);
        }
    }

    public bool shouldObjectBeDestroyed()
    {
        return false;
    }
}