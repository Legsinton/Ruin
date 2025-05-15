using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Break : MonoBehaviour,IInteracting
{
    public Rigidbody[] rb;
    public MeshRenderer[] rbMesh;
    public bool isBroken;
    [SerializeField] Collider collision;

    [SerializeField] Outline outlineScript;

    bool played;
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
            collision.enabled = false;
            outlineScript.enabled = false;

            if (!played)
            {
                played = true;
                SoundFXManager.Instance.PlaySoundFX(SoundType.Break, transform.position);
            }
            foreach (Rigidbody r in rb)
            {
                r.isKinematic = false; // Let them fall now
            }

            StartCoroutine(FadeOut(3));
            Invoke(nameof(DestroyPiece), 6);
        }
    }

    void DestroyPiece() 
    {
        played = false;
        // Cleanup to prevent reference issues
        rb = null;
        rbMesh = null;
        outlineScript = null;
        Destroy(gameObject);
    }

    IEnumerator FadeOut(float duration)
    {
        if (rbMesh != null)
        {
            yield return new WaitForSeconds(3);
            float elapsedTime = 0f;

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

    public void PressInteract()
    {
        if (this == null || outlineScript == null) return;

        isBroken = true;
    }

    public void ReleaseInteract()
    {
        if (this == null || outlineScript == null) return;

    }

    public void InteractInRange()
    {
        if (this == null || outlineScript == null) return;
        else if (!isBroken)     
        {
            outlineScript.enabled = true;
        }
    }

    public void InteractNotInRange()
    {
        if (this == null || outlineScript == null) return;

        else if (isBroken )
        {
            outlineScript.enabled = false;
        }
    }
}