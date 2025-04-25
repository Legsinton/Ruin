using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    LayerMask layerMask;
    [SerializeField] Transform cameraPos;

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Interactable", "Player");
    }

    private void FixedUpdate()
    {
        
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, cameraPos.forward, out hit, 4, layerMask))

        {
            Debug.DrawRay(transform.position, cameraPos.forward * 4, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, cameraPos.forward * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }

    private void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("Interact Pressed");
        }

        else
        {
            Debug.Log("Interact Released");
        }
    }
}
