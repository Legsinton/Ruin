using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PushBlock : MonoBehaviour , IInteracting
{
    [SerializeField]
    private float forceMagnitude;
    public PlayerMovement playerMove;
    [SerializeField] bool canMove;
    [SerializeField] UIScript script;
    [SerializeField] Outline outlineScript;
    Vector3 forceDirection;
    Rigidbody rb;
    private void Awake()
    {
        script = FindAnyObjectByType<UIScript>();
        canMove = FindAnyObjectByType<PlayerMovement>();
        playerMove = FindAnyObjectByType<PlayerMovement>();
    }

    private void Update()
    {
        if (rb != null && canMove && playerMove.movement.magnitude > 0)
        {
            
            forceDirection.y = 0;
            forceDirection.Normalize();

            rb.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
        }
        else if (rb != null && canMove && playerMove.movement.magnitude < 0)
        {   
            forceDirection.y = 0;
            forceDirection.Normalize();

            rb.AddForceAtPosition(-forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
        }
    }

    private void OnCollisionStay(Collision hit)
    {
        rb = hit.gameObject.GetComponent<Rigidbody>();
        forceDirection = transform.position - hit.gameObject.transform.position;

    }

    private void OnCollisionExit(Collision hit)
    {
       // rb = null;
    }


    public void PressInteract()
    {
        canMove = true;
    }

    public void ReleaseInteract() 
    {
        canMove = false;
    }

    public void InteractInRange()
    {
        script.EnableUI();
        outlineScript.enabled = true;
    }

    public void InteractNotInRange()
    {
        script.DisebleUI();
        outlineScript.enabled = false;
    }

}
