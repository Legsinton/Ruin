using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] Image image;

    private void Awake()
    {
        textMesh.enabled = false;
        image.enabled = false;
    }

    public void EnableUI()
    {
        textMesh.enabled = true;
        image.enabled = true;
    }

    public void DisebleUI()
    {
        textMesh.enabled = false;
        image.enabled = false;
    }

}
