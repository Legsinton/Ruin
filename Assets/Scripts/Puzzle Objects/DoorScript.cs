using UnityEngine;

public class DoorEasyScript : MonoBehaviour, IInteracting
{
    public Interact interact;
    public PushBlock pushBlock;
    Vector3 targetPosition;
    Vector3 originalPosition;

    public float pressDepth;
    public float moveSpeed;
    public bool doorClosed = false;

    [SerializeField] bool locked;
    [SerializeField] int itemIdToUnlock;
    [SerializeField] UIScript script;
    [SerializeField] Outline outlineScript;

    private void Start()
    {
        originalPosition = transform.position;
        script = FindAnyObjectByType<UIScript>();
        interact = FindAnyObjectByType<Interact>();
    }

    void Update()
    {
        if (!locked)
        {
            CloseDoor();
            OpenDoor();            
        }
    }

    void CloseDoor()
    {
        if (doorClosed)
        {
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void OpenDoor()
    {
        if (!doorClosed)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Pullable"))
        {
            pushBlock = other.GetComponent<PushBlock>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pullable"))
        {
            pushBlock = null;
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

        if (!locked)
        {
            if (interact.Interacting && pushBlock != null && !pushBlock.CanMove)
            {
                doorClosed = !doorClosed;
            }
            else
            {
                doorClosed = !doorClosed;
            }
        }
    }

    public void ReleaseInteract() { }

    public void InteractInRange()
    {
        if (script != null)
        {
            script.EnableUI();
        }
        outlineScript.enabled = true;
    }

    public void InteractNotInRange()
    {
        if (script != null)
        {
            script.DisebleUI();
        }
        outlineScript.enabled = false;
    }
}