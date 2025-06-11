using System.Collections;
using UnityEngine;

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
            gameObject.layer = default;
            collision.enabled = false;
            if (outlineScript != null)
            {
                outlineScript.enabled = false;
            }

            if (!played)
            {
                played = true;
                SoundFXManager.Instance.PlaySoundFX(SoundType.Break, transform.position);
            }
            if(rb != null)
            {
                foreach (Rigidbody r in rb)
                {
                    r.isKinematic = false; // Let them fall now
                }
            }
            //StartCoroutine(FadeOut(3));
            StartCoroutine(DestroyPiece());
        }
    }

    IEnumerator DestroyPiece() 
    {
        // Cleanup to prevent reference issues
        yield return new WaitForSeconds(6);
        rb = null;
        rbMesh = null;
        outlineScript = null;
        Invoke(nameof(DestroyObject), 1);
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isBroken = true;
        }
    }

    IEnumerator FadeOut(float duration)
    {
        if (rbMesh != null)
        {
            yield return new WaitForSeconds(3);
            float elapsedTime = 0f;

            Color[] startColors = new Color[rbMesh.Length];
            for (int i = 0; i < rbMesh.Length; i++)
            {
                if (rbMesh[i] != null && rbMesh[i].material != null)
                {
                    SetupMaterialForFade(rbMesh[i].material);
                    startColors[i] = rbMesh[i].material.color;
                }
            }

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

                for (int i = 0; i < rbMesh.Length; i++)
                {
                    if (rbMesh[i] != null && rbMesh[i].material != null)
                    {
                        Color c = startColors[i];
                        c.a = newAlpha;
                        rbMesh[i].material.color = c;
                    }
                }

                yield return null;
            }

            // Ensure final alpha is 0
            for (int i = 0; i < rbMesh.Length; i++)
            {
                if (rbMesh[i] != null && rbMesh[i].material != null)
                {
                    Color c = rbMesh[i].material.color;
                    c.a = 0f;
                    rbMesh[i].material.color = c;
                }
            }
        }
    }
    void SetupMaterialForFade(Material mat)
    {
        if (mat == null) return;

        mat.SetFloat("_Mode", 2); // Fade mode
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
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

        outlineScript.enabled = false;
    }

    public bool shouldObjectBeDestroyed()
    {
        return true;
    }
}