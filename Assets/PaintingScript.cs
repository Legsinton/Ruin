using UnityEngine;
using UnityEngine.InputSystem;
using static Interact;

public class PaintingScript : MonoBehaviour, IInteracting
{
    bool interact;

    private void Start()
    {
    }

    private void Update()
    {
        if (interact)
        {
            Debug.Log("Give Me Money");
        }

    }

    public void Interact()
    {
        interact = !interact;
    }

}
