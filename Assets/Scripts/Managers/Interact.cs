using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Interact : MonoBehaviour
{
    [SerializeField] int interactLayer;
    [SerializeField] Transform cameraTransform;

    GameObject currentInteractableObject;
    List<GameObject> interactableObjects = new List<GameObject>();

    bool interacting;
    bool getClosestObejct;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == interactLayer)
        {
            interactableObjects.Add(other.gameObject);

            GetCurrentInteractableObject();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == interactLayer)
        {
            other.gameObject.GetComponent<IInteracting>().InteractNotInRange();

            interactableObjects.Remove(other.gameObject.gameObject);

            GetCurrentInteractableObject();
        }
    }

    void GetCurrentInteractableObject()
    {
        if (!interacting)
        {
            switch (interactableObjects.Count)
            {
                case 0:
                    currentInteractableObject = null;
                    getClosestObejct = false;
                    break;
                case 1:
                    currentInteractableObject = interactableObjects[0];
                    getClosestObejct = false;
                    break;
                case > 1:
                    getClosestObejct = true;
                    GetClosestObject();
                    break;
            }

            if (currentInteractableObject != null)
            {
                currentInteractableObject.GetComponent<IInteracting>().InteractInRange();
            }
        }
    }

    void Update()
    {
        GetClosestObject();
    }

    void GetClosestObject()
    {
        if (getClosestObejct && !interacting)
        {
            float closestDistance = float.PositiveInfinity;
            GameObject closestObject = null;

            interactableObjects.RemoveAll(item => item == null);

            for (int i = 0; i < interactableObjects.Count; i++)
            {
                currentInteractableObject.GetComponent<IInteracting>().InteractNotInRange();

                float distanceToObject = Vector3.Distance(transform.position, interactableObjects[i].transform.position);
                if (distanceToObject < closestDistance)
                {
                    closestDistance = distanceToObject;
                    currentInteractableObject = interactableObjects[i];
                }
            }

            if (currentInteractableObject != null)
            {
                currentInteractableObject.GetComponent<IInteracting>().InteractInRange();
            }
        }
    }

    void OnInteract(InputValue value)
    {
        if (currentInteractableObject != null)
        {
            if (value.isPressed) // Interact button press
            {
                currentInteractableObject.GetComponent<IInteracting>().PressInteract();
                if (currentInteractableObject.GetComponent<IInteracting>().shouldObjectBeDestroyed())
                {
                    RemoveObject(currentInteractableObject);
                }
                interacting = true;
            }
        }
        if (!value.isPressed) // Interact button release
        {
            if (currentInteractableObject != null)
            {
                currentInteractableObject.GetComponent<IInteracting>().ReleaseInteract();
            }
            interacting = false;
            GetCurrentInteractableObject();
        }
    }

    void RemoveObject(GameObject obj)
    {
        interactableObjects.Remove(obj);
        GetCurrentInteractableObject();
    }
}

public interface IInteracting
{
    void PressInteract();

    void ReleaseInteract();

    void InteractInRange();

    void InteractNotInRange();

    bool shouldObjectBeDestroyed();
}