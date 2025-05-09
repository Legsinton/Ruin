using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Break : MonoBehaviour
{
    public Rigidbody[] rb;

    public MeshRenderer[] rbMesh;

    public bool isBroken;
    void Awake()
    {
        rb = GetComponentsInChildren<Rigidbody>(); // Fills the array automatically
        rbMesh = GetComponentsInChildren<MeshRenderer>();

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
            StartCoroutine(FadeOut(3));
            Invoke(nameof(DestroyPiece), 6);
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

    IEnumerator FadeOut(float duration)
    {
        if (rbMesh != null)
        {
            yield return new WaitForSeconds(3);
            float elapsedTime = 0f;

            Debug.Log("Hellooooo");
            // Store the original colors
            Color[] startColors = new Color[rbMesh.Length];
            for (int i = 0; i < rbMesh.Length; i++)
            {
                startColors[i] = rbMesh[i].material.color;
            }

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

                for (int i = 0; i < rbMesh.Length; i++)
                {
                    Color c = startColors[i];
                    c.a = newAlpha;
                    rbMesh[i].material.color = c;
                }

                yield return null;
            }

            // Ensure final alpha is 0
            for (int i = 0; i < rbMesh.Length; i++)
            {
                Color c = rbMesh[i].material.color;
                c.a = 0f;
                rbMesh[i].material.color = c;
            }
        }
    }
}