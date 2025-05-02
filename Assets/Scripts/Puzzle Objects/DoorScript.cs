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
    bool playerOnTop;
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
            transform.position = Vector3.MoveTowards(transform.position, originalPosition , moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Pullable"))
        {
            pushBlock = other.GetComponent<PushBlock>();
        }

        if (other.CompareTag("Player"))
        {
            playerOnTop = true;
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pullable"))
        {
            pushBlock = null;
        }

        if (other.CompareTag("Player"))
        {
            playerOnTop = false;
        }

    }

    public void PressInteract()
    {
        if (!playerOnTop)
        {
            doorClosed = !doorClosed;
        }
        else if (playerOnTop)
        {
        }
    }

    public void ReleaseInteract() { }

    public void InteractInRange()
    {
        script.EnableUI();
        if (!doorClosed)
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
