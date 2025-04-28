using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Interact : MonoBehaviour
{
    LayerMask layerMask;
    [SerializeField] Transform cameraPos;
    bool isPressed;
    bool isHolding;
    bool wasPressedThisFrame;
    bool wasReleasedThisFrame;

    public Canvas Canvas;

    public TextMeshProUGUI textMesh;

    private void Awake()
    {
        Canvas = GetComponent<Canvas>();
        layerMask = LayerMask.GetMask("Interactable", "Player");
    }

    private void FixedUpdate()
    {
        
        if (!isPressed || !isHolding) return;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, cameraPos.forward, out hit, 4, layerMask))

        {
            Debug.DrawRay(transform.position, cameraPos.forward * 4, Color.yellow);
            Debug.Log("Did Hit");
            textMesh.enabled = true;
            var interactable = hit.collider.GetComponent<IInteracting>();
            var interactableHold = hit.collider.GetComponent<IInteracting>();


            if (interactable != null)
            {
                if (wasPressedThisFrame)
                {
                    interactable.OnInteractTap();
                }
            }
            if (interactableHold != null)
            {
                interactableHold.OnInteractHold();

            }

        }
        else
        {
            Debug.DrawRay(transform.position, cameraPos.forward * 1000, Color.white);
            Debug.Log("Did not Hit");
        }

        wasPressedThisFrame = false;
        wasReleasedThisFrame = false;
    }
    private void OnInteract(InputValue value)
    {

        bool pressedHold = value.isPressed;

        bool newPressed = value.isPressed;

        // Detect "just pressed"
        wasPressedThisFrame = newPressed && !isPressed;

        // Detect "just released"
        wasReleasedThisFrame = !newPressed && isPressed;

        isPressed = newPressed;
        isHolding = pressedHold;


        Debug.Log($"OnIsHolding triggered: {isHolding}");

    }

    public interface IInteracting
    {
        void OnInteractTap();
        void OnInteractHold();


        
    }
}
