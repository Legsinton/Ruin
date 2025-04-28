using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Interact : MonoBehaviour
{
    [SerializeField] int interactLayer;
    
    IInteracting currentInteractableObject;
    List<GameObject> interactableObjects = new List<GameObject>();

    bool interactInRange = false;
    bool interacting = false;
    bool multipleObjectsInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == interactLayer)
        {
            interactableObjects.Add(other.gameObject);

            if (interactableObjects.Count == 1)
            {
                currentInteractableObject = other.gameObject.GetComponent<IInteracting>();
            }
            else
            {
                multipleObjectsInRange = true;
            }

            interactInRange = true;
            currentInteractableObject.InteractInRange();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == interactLayer)
        {
            if (interactableObjects.Count >= 3)
            {
                other.GetComponent<IInteracting>().InteractNotInRange();
            }

            if (interactableObjects.Count == 2)
            {
                currentInteractableObject.InteractNotInRange();
                updateClosestObject();
                currentInteractableObject.InteractInRange();
                multipleObjectsInRange = false;
            }

            if (interactableObjects.Count == 1)
            {
                interactInRange = false;
                multipleObjectsInRange = false;
                currentInteractableObject.InteractNotInRange();
            }

            interactableObjects.Remove(other.gameObject);
        }
    }

    void Update()
    {
        if (multipleObjectsInRange)
        {
            currentInteractableObject.InteractNotInRange();

            updateClosestObject();

            currentInteractableObject.InteractInRange();
        }
    }

    void updateClosestObject()
    {
        float closestDistance = float.PositiveInfinity;

        for (int i = 0; i < interactableObjects.Count; i++)
        {
            if (Vector3.Distance(transform.position, interactableObjects[i].transform.position) < closestDistance)
            {
                closestDistance = Vector3.Distance(transform.position, interactableObjects[i].transform.position);
                currentInteractableObject = interactableObjects[i].GetComponent<IInteracting>();
            }
        }
    }

    private void OnInteract(InputValue value)
    {
        if (currentInteractableObject != null)
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
}

public interface IInteracting
{
    void PressInteract();

    void ReleaseInteract();

    void InteractInRange();

    void InteractNotInRange();
}