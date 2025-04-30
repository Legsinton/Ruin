using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PushBlock : MonoBehaviour , IInteracting
{
    private float pullSpeed = 0.87f;
    [SerializeField] UIScript script;
    [SerializeField] Outline outlineScript;
    PlayerMovement playerMove;
    Transform playerTransform;
    bool canMove;
    public bool CanMove { get { return canMove; } }
    private void Awake()
    {   
        script = FindAnyObjectByType<UIScript>();
        playerMove = FindAnyObjectByType<PlayerMovement>();
    }

    private void Update()
    {

        if (canMove && playerTransform != null && playerMove != null)
        {
            Vector3 moveDir = playerMove.movement;
            moveDir.y = 0;
            
            if (moveDir.magnitude > 0)
            {
                transform.position += moveDir.normalized * pullSpeed * Time.deltaTime;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerTransform = collision.transform;
            playerMove = collision.gameObject.GetComponent<PlayerMovement>();
        }
    }

    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }


    public void PressInteract()
    {
        canMove = true;
        //playerMove.CurrentSpeed = 1;

    }

    public void ReleaseInteract() 
    {
        canMove = false;
        //playerMove.CurrentSpeed = 10;
    }

    public void InteractInRange()
    {
        script.EnableUI();
        outlineScript.enabled = true;
    }

    public void InteractNotInRange()
    {
        script.DisebleUI();
        outlineScript.enabled = false;
    }

}
