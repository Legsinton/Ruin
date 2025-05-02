using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorEasyScript : MonoBehaviour, IInteracting
{
    public Interact interact;
    public PushBlock pushBlock;
    Vector3 targetPosition;
    Vector3 currentPoisition;
    Vector3 originalPosition;
    public float pressDepth;
    public float moveSpeed;
    public bool doorClosed = false;
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
        if (doorClosed)
        {
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        OpenDoor();
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
        if (interact.Interacting && pushBlock != null && !pushBlock.CanMove)
        {
            doorClosed = !doorClosed;
        }
        else
        {
            doorClosed = !doorClosed;
        }
    }

    public void ReleaseInteract() { }

    public void InteractInRange()
    {
        script.EnableUI();
        if (!doorClosed && pushBlock != null && !pushBlock.CanMove)
        {
            outlineScript.enabled = true;
        }
    }

    public void InteractNotInRange()
    {
        script.DisebleUI();
        outlineScript.enabled = false;
    }
}