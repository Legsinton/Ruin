using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }
    public InputActionAsset inputActions;

    private InputActionMap playerMap;
    private InputActionMap uiMap;

    private void Awake()
    {
        Debug.Log("Agnes: Is this thing on?");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        playerMap = inputActions.FindActionMap("Player");
        uiMap = inputActions.FindActionMap("UI");
    }

    public void OnUIOpened()
    {
        playerMap.Disable();
        Debug.Log("Agnes: Player disabled");
        uiMap.Enable();
        Debug.Log("Agnes: UI enabled");
    }

    public void OnUIClosed()
    {
        uiMap.Disable();
        playerMap.Enable();
        Debug.Log("Agnes: UI disabled");
    }
}