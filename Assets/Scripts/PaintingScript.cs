using UnityEngine;
using UnityEngine.InputSystem;
using static Interact;

public class PaintingScript : MonoBehaviour, IInteracting
{
    bool interact;
    bool done;

    private void Start()
    {
    }

    private void Update()
    {
        if (interact && !done)
        {
            done = true;
            Debug.Log("Give Me Money");
        }
        else if (!interact && done) 
        {
            done = false;
        }

    }

    public void OnInteractHold()
    {
    }

    public void OnInteractTap()
    {
        interact = !interact;

    }

}
