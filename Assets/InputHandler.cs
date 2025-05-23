using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }
    public InputActionAsset InputActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public void OnEnable()
    {
        InputActions.FindActionMap("UI").Enable();
        InputSwitch();
        Debug.Log("Agnes: UI enabled");
    }

    private void InputSwitch()
    {
        InputActions.FindAction("UI/Navigate");
    }

    // private void OnDisable()
    // {
    //     InputActions.FindActionMap("UI").Disable();
    //     Debug.Log("Agnes: UI disabled");

    // }


}