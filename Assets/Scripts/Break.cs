using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Break : MonoBehaviour
{
    public Rigidbody[] rb;

    public bool isBroken;
    void Awake()
    {
        rb = GetComponentsInChildren<Rigidbody>(); // Fills the array automatically

        foreach (Rigidbody r in rb)
        {
            r.isKinematic = true; // Makes all children kinematic at start
        }
    }

    void Update()
    {
        if (isBroken == true)
        {
            foreach (Rigidbody r in rb)
            {
                r.isKinematic = false; // Let them fall now
            }

            isBroken = false; // Prevent re-triggering
            Invoke(nameof(DestroyPiece), 5);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isBroken=true;
        }
    }
    
    void DestroyPiece() 
    {
        Destroy(this.gameObject);
    }    
}