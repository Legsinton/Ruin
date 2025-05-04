using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GateScript : MonoBehaviour, IInteracting
{
    [SerializeField] int switches;
    [SerializeField] Outline outlineScript;
    public int Switches { get { return switches; } set { switches = value; } }
    public int switchAmount;
    Vector3 targetPosition;
    Vector3 originalPosition;
    public bool solved;
    public float pressDepth;
    public float moveSpeed;
    public bool gateClosed = false;

    private void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (Switches >= switchAmount && !solved)
        {
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            Invoke(nameof(EnableSolved), 5);
        }

        else if (Switches != switchAmount && solved)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            Invoke(nameof(EnableUnSolved), 5);
        }

        if (solved)
        {
            if (!gateClosed)
            {
                targetPosition = originalPosition - Vector3.up * pressDepth;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else if (gateClosed)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            }
        }
    }

    void EnableSolved()
    {
         solved = true;
    }

    void EnableUnSolved()
    {
            solved = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (solved)
            {
                gateClosed = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gateClosed = true;
        }
    }

    public void AddSwitch()
    {
        switches++;
    }

    public void PressInteract()
    {
        if (solved && !gateClosed)
        {
            gateClosed = true;
        }
        else
        {

            gateClosed = !gateClosed;

        }
    }

    public void ReleaseInteract() { }

    public void InteractInRange()
    {
        if (solved && gateClosed)
        {
            outlineScript.enabled = true;
        }
    }

    public void InteractNotInRange()
    {
        if (solved && !gateClosed)
        {
            outlineScript.enabled = false;
        }
    }
}
