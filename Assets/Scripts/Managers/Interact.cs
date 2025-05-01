using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Interact : MonoBehaviour
{
    [SerializeField] int interactLayer;
    [SerializeField] Transform cameraTransform;

    IInteracting currentInteractableObject;
    List<GameObject> interactableObjects = new List<GameObject>();

    bool interactInRange = false;
    bool interacting = false;
    public bool Interacting { get { return interacting; } set { interacting = value; } }
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
            other.GetComponent<IInteracting>().InteractNotInRange();

            if (interactableObjects.Count == 2)
            {
                multipleObjectsInRange = false;
            }

            if (interactableObjects.Count == 1)
            {
                interactInRange = false;
                multipleObjectsInRange = false;
            }

            interactableObjects.Remove(other.gameObject);

            if (interactableObjects.Count == 1)
            {
                currentInteractableObject = interactableObjects[0].GetComponent<IInteracting>();
                currentInteractableObject.InteractInRange();
            }
        }
    }

    void Update()
    {
        if (multipleObjectsInRange)
        {
            currentInteractableObject.InteractNotInRange();

            getCurrentObject();

            currentInteractableObject.InteractInRange();
        }
    }

    void getCurrentObject()
    {
        RaycastHit hit;
        Vector3 distancePoint;
        float closestDistance = float.PositiveInfinity;

        Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, 20);

        if (hit.collider == null)
        {
            distancePoint = transform.position;
        }
        else
        {
            distancePoint = hit.point;
        }

        for (int i = 0; i < interactableObjects.Count; i++)
        {
            if (Vector3.Distance(distancePoint, interactableObjects[i].transform.position) < closestDistance)
            {
                closestDistance = Vector3.Distance(distancePoint, interactableObjects[i].transform.position);
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