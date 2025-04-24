using UnityEngine;
using UnityEngine.InputSystem;

public class PaintingScript : MonoBehaviour
{

    PlayerControls playerControls;
    bool done;

    private void Start()
    {
        playerControls = FindAnyObjectByType<PlayerControls>();
    }

    private void Update()
    {
        if (playerControls.IsPainting && playerControls.isInteracting && playerControls.haveInteracted == 1)
        {
            Debug.Log("Give Me Money");
        }

    }

}
