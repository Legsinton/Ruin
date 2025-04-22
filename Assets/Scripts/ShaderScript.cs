using UnityEngine;

public class ShaderScript : MonoBehaviour 
{
    public static int PosID = Shader.PropertyToID("_Player_Position");
    public static int SizeID = Shader.PropertyToID("_Size");

    public Material material;
    public Camera cam;
    public LayerMask mask;

    void Update()
    {
        var dir = cam.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, 3000, mask))
        {
            material.SetFloat(SizeID, 3);
        }
        else
        {
            material.SetFloat(SizeID, 0);
        }
            var view = cam.WorldToViewportPoint(transform.position);
        material.SetVector(PosID, view);
    }
}
