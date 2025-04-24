using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public PressurePlate pressurePlate;
    Vector3 targetPosition;
    Vector3 currentPoisition;
    Vector3 originalPosition;
    public float pressDepth;
    public float moveSpeed;

    void Update()
    {
        Debug.Log("Door Move");
        if (pressurePlate.triggerd == true)
        {
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        }
    }

}
