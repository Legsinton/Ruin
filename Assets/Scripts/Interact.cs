using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    LayerMask layerMask;
    [SerializeField] Transform cameraPos;
    bool interactPressed;

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Interactable", "Player");
    }

    private void FixedUpdate()
    {
        if (!interactPressed) return;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, cameraPos.forward, out hit, 4, layerMask))

        {
            Debug.DrawRay(transform.position, cameraPos.forward * 4, Color.yellow);
            Debug.Log("Did Hit");
            var interactable = hit.collider.GetComponent<IInteracting>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
        else
        {
            Debug.DrawRay(transform.position, cameraPos.forward * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }
    private void OnInteract(InputValue value)
    {
        interactPressed = value.isPressed;
    }

    public interface IInteracting
    {
        void Interact();
    }
}
