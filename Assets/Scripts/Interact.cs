using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Interact : MonoBehaviour
{
    [SerializeField] Transform cameraPos;

    LayerMask layerMask;

    bool interactInRange;
    bool interacting;
    IInteracting currentInteractableObject;

    public Canvas Canvas;

    public TextMeshProUGUI textMesh;

    private void Awake()
    {
        Canvas = GetComponent<Canvas>();
        layerMask = LayerMask.GetMask("Interactable");
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, cameraPos.forward, out hit, 4, layerMask))
        {
            Debug.DrawRay(transform.position, cameraPos.forward * 4, Color.yellow);
            textMesh.enabled = true;
            var interactable = hit.collider.GetComponent<IInteracting>();
            var interactableHold = hit.collider.GetComponent<IInteracting>();


            /*if (interactable != null)
            {
                if (wasPressedThisFrame)
                {
                    interactable.OnInteractTap();
                }
            }
            if (interactableHold != null)
            {
                interactableHold.OnInteractHold();

            }*/

            currentInteractableObject = hit.collider.GetComponent<IInteracting>();
            interactInRange = true;
            currentInteractableObject.InteractInRange();
        }
        else
        {
            interactInRange = false;
            if (currentInteractableObject != null)
            {
                currentInteractableObject.InteractNotInRange();
            }

            Debug.DrawRay(transform.position, cameraPos.forward * 1000, Color.white);
        }
    }
    private void OnInteract(InputValue value)
    {
        if (!interacting && interactInRange)
        {
            interacting = true;
            currentInteractableObject.PressInteract();
        }
        else
        {
            interacting = false;
            currentInteractableObject.ReleaseInteract();
        }
    }
}

public interface IInteracting
{
    void PressInteract();

    void ReleaseInteract();

    void InteractInRange();

    void InteractNotInRange();
}