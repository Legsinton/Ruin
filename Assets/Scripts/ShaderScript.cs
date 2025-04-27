using UnityEngine;
using static Interact;

public class ShaderScript : MonoBehaviour, IInteracting
{
    public static int PosID = Shader.PropertyToID("_Player_Position");
    public static int SizeID = Shader.PropertyToID("_Size");

    public Material material;
    public Camera cam;
    public LayerMask mask;

    public PlayerMovement player;

    GameObject block;

    float distance;

    [SerializeField] bool canSee;

    void Update()
    {
        /// var playerVar = player.transform.position - transform.position;

        // var dir = cam.transform.position - transform.position;
        //var ray = new Ray(transform.position, dir.normalized);



        if (canSee)
        {
            material.SetFloat(SizeID, 3);
        }

        //if (Physics.Raycast(ray, 3000, mask))
        else
        {
            material.SetFloat(SizeID, 0);
        }

        /*var view = cam.WorldToViewportPoint(transform.position);
        material.SetVector(PosID, view);*/


    }

    public void OnInteractTap()
    {
        canSee = !canSee;
    }

    public void OnInteractHold()
    {

    }
}
