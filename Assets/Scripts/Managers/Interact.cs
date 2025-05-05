using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Interact : MonoBehaviour
{
    [SerializeField] int interactLayer;
    [SerializeField] Transform cameraTransform;

    GameObject currentInteractableObject;
    List<GameObject> interactableObjects = new List<GameObject>();

    bool interactInRange = false;
    bool interacting = false;
    public bool Interacting { get { return interacting; } set { interacting = value; } }
    bool multipleObjectsInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == interactLayer)
        {
            CheckIfObjectStillExists();

            interactableObjects.Add(other.gameObject);

            if (interactableObjects.Count == 1)
            {
                currentInteractableObject = other.gameObject;
            }
            else
            {
                multipleObjectsInRange = true;
            }

            interactInRange = true;
            currentInteractableObject.GetComponent<IInteracting>().InteractInRange();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == interactLayer)
        {
            other.gameObject.GetComponent<IInteracting>().InteractNotInRange();

            if (interactableObjects.Count == 2)
            {
                multipleObjectsInRange = false;
            }

            if (interactableObjects.Count == 1)
            {
                interactInRange = false;
                multipleObjectsInRange = false;
            }

            interactableObjects.Remove(other.gameObject.gameObject);

            if (interactableObjects.Count == 1)
            {
                currentInteractableObject = interactableObjects[0];
                currentInteractableObject.GetComponent<IInteracting>().InteractInRange();
            }
        }
    }

    void Update()
    {
        if (multipleObjectsInRange)
        {
            currentInteractableObject.GetComponent<IInteracting>().InteractNotInRange();

            getCurrentObject();

            currentInteractableObject.GetComponent<IInteracting>().InteractInRange();
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
                currentInteractableObject = interactableObjects[i];
            }
        }
    }

    private void OnInteract(InputValue value)
    {
        if (currentInteractableObject != null)
        {
            CheckIfObjectStillExists();

            if (!interacting && interactInRange)
            {
                interacting = true;
                currentInteractableObject.GetComponent<IInteracting>().PressInteract();
            }
            else
            {
                interacting = false;
                currentInteractableObject.GetComponent<IInteracting>().ReleaseInteract();
            }
        }
    }

    void CheckIfObjectStillExists()
    {
        interactableObjects.RemoveAll(item => item == null);
    }
}

public interface IInteracting
{
    void PressInteract();

    void ReleaseInteract();

    void InteractInRange();

    void InteractNotInRange();
}