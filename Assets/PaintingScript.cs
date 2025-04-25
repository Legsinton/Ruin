using UnityEngine;
using UnityEngine.InputSystem;

public class PaintingScript : MonoBehaviour
{

    PlayerMovement playerMovement;
    bool done;

    private void Start()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement.IsPainting && playerMovement.isInteracting && playerMovement.haveInteracted == 1)
        {
            Debug.Log("Give Me Money");
        }

    }

}
