using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PushBlock : MonoBehaviour , IInteracting
{
    [SerializeField]
    private float forceMagnitude;
    [SerializeField] bool canMove;
    [SerializeField] UIScript script;
    [SerializeField] Outline outlineScript;
    Rigidbody rb;
    private void Awake()
    {
        script = FindAnyObjectByType<UIScript>();
    }

    private void OnCollisionStay(Collision hit)
    {
        Debug.Log("Hello");
        rb = hit.gameObject.GetComponent<Rigidbody>();
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
        rb = null;
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
