using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorScript : MonoBehaviour, IInteracting
{
    public Interact interact;
    public PushBlock pushBlock;
    Vector3 targetPosition;
    Vector3 originalPosition;
    public float pressDepth;
    public float moveSpeed;
    public bool doorClosed = false;
    bool doorMoving;
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

        DoorMoving();

        OpenDoor();
    }

    void OpenDoor()
    {
        if (!doorClosed)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    void DoorMoving()
    {
        if (doorClosed && Vector3.Distance(transform.position, targetPosition) > 0.01f || doorClosed && Vector3.Distance(transform.position, originalPosition) < 0.01f) 
        {
            doorMoving = true;
        }
        else
        {
            doorMoving = false;
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
        if (!doorMoving)
        {
            doorClosed = !doorClosed;

        }
    }

    public void ReleaseInteract() { }

    public void InteractInRange()
    {
        script.EnableUI();
        if (!doorMoving)
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
