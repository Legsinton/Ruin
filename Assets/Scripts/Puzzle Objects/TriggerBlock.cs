using System.Collections;
using TMPro;
using UnityEngine;

public class TriggerBlock : MonoBehaviour
{

    public bool triggerd = false;
    public GateScript[] gate;
    public MovingPlatform[] platforms;
    Vector3 targetPosition;
    Vector3 originalPosition;
    public float pressDepth;
    public float moveSpeed;
    bool added = false;

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (triggerd)
        {
            targetPosition = originalPosition - Vector3.up * pressDepth;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        }
        else if (!triggerd)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("RotatingTag"))
        {
            Debug.Log("Rotating object on pressure plate!");

            if (!added)
            {
                triggerd = true;
                added = true;
                for (int i = 0; i < platforms.Length; i++)
                {
                    platforms[i].Switches++;
                }
                for (int i = 0; i < gate.Length; i++)
                {
                    gate[i].Switches++;
                }

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("RotatingTag"))
        {
            if (added)
            {
                triggerd = false;
                added = false;

                for (int i = 0; i < gate.Length; i++)
                {
                    gate[i].Switches--;
                }
                for (int i = 0; i < platforms.Length; i++)
                {
                    platforms[i].Switches--;
                }
            }
        }
    }
}





