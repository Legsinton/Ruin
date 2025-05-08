using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI textMeshHold;
    [SerializeField] Image imageHold;

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

    public void EnableUIHold()
    {
        textMeshHold.enabled = true;
        imageHold.enabled = true;
    }

    public void DisebleUIHold()
    {
        textMeshHold.enabled = false;
        imageHold.enabled = false;
    }
}