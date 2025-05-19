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
    bool multipleObjectsInRange = false;
    private bool isChecking = false;

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

            //currentInteractableObject.GetComponent<IInteracting>().InteractInRange();
            // Added stuff here
            if (currentInteractableObject != null && currentInteractableObject.TryGetComponent(out IInteracting comp))
            {
                comp.InteractInRange();
            }
            else
            {
                Debug.LogWarning("Trying to interact with a missing component or something");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == interactLayer)
        {
            if (other.gameObject != null && other.gameObject.TryGetComponent(out IInteracting comp))
            {
                comp.InteractNotInRange();
            }

            interactableObjects.Remove(other.gameObject);

            if (interactableObjects.Count == 0)
            {
                interactInRange = false;
                multipleObjectsInRange = false;
                currentInteractableObject = null;
            }
            else if (interactableObjects.Count == 1)
            {
                currentInteractableObject = interactableObjects[0];
                multipleObjectsInRange = false;

                if (currentInteractableObject.TryGetComponent(out IInteracting newComp))
                {
                    newComp.InteractInRange();
                }
            }
            else
            {
                multipleObjectsInRange = true;
                GetCurrentObject();
            }
        }

        /*if (other.gameObject.layer == interactLayer)
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
        }*/
    }

    void Update()
    {
        CheckIfObjectStillExists(); // Add this line

        if (multipleObjectsInRange && currentInteractableObject != null)
        {
            currentInteractableObject.GetComponent<IInteracting>().InteractNotInRange();

            GetCurrentObject();

            if (currentInteractableObject != null)
            {
                currentInteractableObject.GetComponent<IInteracting>().InteractInRange();
            }
        }

        /*if (multipleObjectsInRange)
        {
            currentInteractableObject.GetComponent<IInteracting>().InteractNotInRange();

            getCurrentObject();

            currentInteractableObject.GetComponent<IInteracting>().InteractInRange();
        }*/
    }

    void GetCurrentObject()
    {
        CheckIfObjectStillExists(); // Add this!

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
            if (interactableObjects[i] != null && Vector3.Distance(distancePoint, interactableObjects[i].transform.position) < closestDistance)
            {
                closestDistance = Vector3.Distance(distancePoint, interactableObjects[i].transform.position);
                currentInteractableObject = interactableObjects[i];

            }

                /*if (Vector3.Distance(distancePoint, interactableObjects[i].transform.position) < closestDistance)
            {
            }*/
        }
    }

    private void OnInteract(InputValue value)
    {
        if (currentInteractableObject != null)
        {
            CheckIfObjectStillExists();

            if (interactInRange && value.isPressed)
            {
                currentInteractableObject.GetComponent<IInteracting>().PressInteract();
            }
            if (!value.isPressed)
            {
                currentInteractableObject.GetComponent<IInteracting>().ReleaseInteract();
            }
        }
    }

    void CheckIfObjectStillExists()
    {
        /*interactableObjects.RemoveAll(item => item == null || !item.TryGetComponent<IInteracting>(out _));
        // Added this
        if (currentInteractableObject == null || !currentInteractableObject.TryGetComponent<IInteracting>(out _))
        {
            currentInteractableObject = null;
            interactInRange = false;
            multipleObjectsInRange = false;
        }*/



        if (isChecking) return;
        isChecking = true;

        // Your checking logic here

        

        interactableObjects.RemoveAll(item => item == null || !item.TryGetComponent<IInteracting>(out _));

        if (currentInteractableObject == null || !currentInteractableObject.TryGetComponent<IInteracting>(out _))
        {
            currentInteractableObject = null;
            interactInRange = false;
            multipleObjectsInRange = interactableObjects.Count > 1;

            if (interactableObjects.Count > 0)
            {
                GetCurrentObject(); // Select a new object based on raycast distance
                if (currentInteractableObject != null && currentInteractableObject.TryGetComponent(out IInteracting comp))
                {
                    comp.InteractInRange(); // Notify new object
                    interactInRange = true;
                }
            }
        }
        isChecking = false;
    }
}

public interface IInteracting
{
    void PressInteract();

    void ReleaseInteract();

    void InteractInRange();

    void InteractNotInRange();
}