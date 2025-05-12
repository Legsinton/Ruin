using System.Collections;
using System.Collections.Generic;
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
            if (!played)
            {
                played = true;
                SoundFXManager.Instance.PlaySoundFX(SoundType.Break, transform.position);
            }
            foreach (Rigidbody r in rb)
            {
                r.isKinematic = false; // Let them fall now
            }

            isBroken = false; // Prevent re-triggering
            StartCoroutine(FadeOut(3));
            Invoke(nameof(DestroyPiece), 6);
        }
    }

    void DestroyPiece() 
    {
        played = false;
        Destroy(this.gameObject);
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
        isBroken = true;
    }

    public void ReleaseInteract()
    {

    }

    public void InteractInRange()
    {
        if (!isBroken && this.gameObject != null)
        {
            outlineScript.enabled = true;
        }
    }

    public void InteractNotInRange()
    {
        if (!isBroken && this.gameObject != null)
        {
            outlineScript.enabled = false;
        }
    }
}