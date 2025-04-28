using System.Runtime.CompilerServices;
using UnityEngine;

public class PushBlock : MonoBehaviour , IInteracting
{
    [SerializeField]
    private float forceMagnitude;
    [SerializeField] bool canMove;
    [SerializeField] Outline outline;
    /*private void OnCollisionStay(Collision hit)
    {
        Debug.Log("Hello");
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb != null)
        {
            Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
            forceDirection.y = 0;
            forceDirection.Normalize();
           
            rb.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);

        }
        
    }

    private void OnCollisionExit(Collision hit)
    {
        
    }*/

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
        outline.enabled = true;
    }

    public void InteractNotInRange()
    {
        outline.enabled = false;
    }

}
