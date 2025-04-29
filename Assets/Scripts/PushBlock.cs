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
    [SerializeField] Rigidbody rb;
    private void Awake()
    {
        script = FindAnyObjectByType<UIScript>();
        //canMove = FindAnyObjectByType<PlayerMovement>();
        playerMove = FindAnyObjectByType<PlayerMovement>();
    }

    private void Update()
    {
        Vector3 moveDir = playerMove.movement.normalized;

       /* if (rb != null)
        {


            if (Vector3.Dot(moveDir, playerMove.transform.forward) > 0.5f)
            {
                Debug.Log("Moving forward");
                forceDirection.y = 0;
                forceDirection.Normalize();

                rb.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
            }
            else if (Vector3.Dot(moveDir, playerMove.transform.forward) < -0.5f)
            {
                Debug.Log("Moving backward");
                /* forceDirection.y = 0;
                 forceDirection.Normalize();

                 rb.AddForceAtPosition(-forceDirection * forceMagnitude, rb.transform.position, ForceMode.Impulse);
            }
        }

        /*if (rb != null && Vector3.Dot(playerMove.movement.normalized, playerMove.transform.forward) > 0.5f)
        {
            Debug.Log("Moving forward");
            forceDirection.y = 0;
            forceDirection.Normalize();

            rb.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
        }
        else if (rb != null && Vector3.Dot(playerMove.movement.normalized, playerMove.transform.forward) < -0.5f)
        {
            Debug.Log("Moving backward");
            forceDirection.y = 0;
            forceDirection.Normalize();

            rb.AddForceAtPosition(-forceDirection * forceMagnitude, rb.transform.position, ForceMode.Impulse);
        }*/
    }

    private void OnCollisionStay(Collision hit)
    {
     
            rb = hit.gameObject.GetComponent<Rigidbody>();
            forceDirection = transform.position - hit.gameObject.transform.position;
            playerMove.gameObject.transform.forward = hit.gameObject.transform.forward;
        
            
        
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
