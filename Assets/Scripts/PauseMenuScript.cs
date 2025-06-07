using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    [Header("Menu References")]
    public Slider sliderControll;
    public GameObject pauseMenu, optionsMenu, soundMenu, controllMenu;
    public Slider sliderMouse;
   
    [Header("Icon References")]

    [SerializeField]
    private GameObject defaultPauseButton, defaultOptionsButton, defaultSoundButton, defaultControlButton;
    public GameObject Hud_Ref,goddesSymbol/*, fishKey, skullKey, foot, hand*/;

    [Header("Settings")]
    public PlayerInput playerInput;
    public PlayerMovement playerMovement;
    public CameraFollow cameraFollow;
    public InputActionAsset actions;
    private InputAction clickAction;
    private InputAction cancelAction;
    private GameObject currentMenu;
    bool pauseButtonWasPressed = false;
    bool isPausing;
    bool isGamepad;
    bool justPaused = false;
    readonly float fishKeyId = 1;
    readonly float skullKeyId = 2;
    readonly float armId = 3;
    readonly float legId = 4;
    //[SerializeField] GameObject arm,leg,fishKeyObject,skullKeyObject;


    Stack<GameObject> menuHistory = new Stack<GameObject>();


    private Dictionary<GameObject, GameObject> menuDefaultButtons;


    private void Awake()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        soundMenu.SetActive(false);
        controllMenu.SetActive(false);
        goddesSymbol.SetActive(false);
        Hud_Ref.SetActive(false);

        //fishKey.SetActive(false); skullKey.SetActive(false); foot.SetActive(false); hand.SetActive(false);
        menuDefaultButtons = new Dictionary<GameObject, GameObject>
        {
            { pauseMenu, defaultPauseButton },
            { optionsMenu, defaultOptionsButton },
            { soundMenu, defaultSoundButton },
            { controllMenu, defaultControlButton }
        };
    }
    private void OnEnable()
    {
        var actionMap = actions.FindActionMap("UI");
        clickAction = actionMap.FindAction("Click");
        cancelAction = actionMap.FindAction("Cancel");
        playerInput.onControlsChanged += OnControlsChanged;
        clickAction.Enable();
        clickAction.performed += OnClickPerformed;
        cancelAction.performed += OnCancelPerformed; // Define this handler
    }

    private void OnDisable()
    {
        clickAction.performed -= OnClickPerformed;
        cancelAction.performed -= OnCancelPerformed; // Define this handler
        clickAction.Disable();
    }

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        var selected = EventSystem.current.currentSelectedGameObject;

        if (isPausing)
        {
            if (selected != null && selected.name == "ResumeButton")
            {
                ResumeGame();
            }
            else if (selected != null && selected.name == "OptionsButton")
            {
                Options();
            }
            else if (selected != null && selected.name == "QuitButton")
            {
                QuitGame();
            }
            else if (selected != null && selected.name == "BackButton")
            {
                BackButton();
            }
            else if (selected != null && selected.name == "BackButtonOptions")
            {
                BackButtonOptions();
            }
            else if (selected != null && selected.name == "SoundSettings")
            {
                SoundMenu();
            }
            else if (selected != null && selected.name == "ControllSettings")
            {
                ControllMenu();
            }
            else if (selected != null && selected.name == "BackButtonOptionsSound")
            {
                BackButtonOptions();
            }
            else
            {

            }
        }
    }

    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        var selected = EventSystem.current.currentSelectedGameObject;

        if (isPausing)
        {
            if (context.action.name == "Cancel")
            {
                GoBack();
            }
            else
            {

            }
        }
    }

    void OnControlsChanged(PlayerInput input)
    {
        string scheme = input.currentControlScheme;
        isGamepad = scheme == "Gamepad";

        // Find your pointer action — adjust the action map and action name as per your setup
        var uiActionMap = actions.FindActionMap("UI");
        var pointerAction = uiActionMap?.FindAction("Point");

        if (pointerAction != null)
        {
            if (isGamepad)
                pointerAction.Disable();  // Disable pointer input when using gamepad
            else
                pointerAction.Enable();   // Enable pointer input when using mouse/keyboard
        }

        // Now set the EventSystem selected object accordingly
        EventSystem.current.SetSelectedGameObject(null);

        if (isGamepad)
        {
            if (currentMenu != null && menuDefaultButtons.ContainsKey(currentMenu))
            {
                EventSystem.current.SetSelectedGameObject(menuDefaultButtons[currentMenu]);
            }
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; // optional: lock cursor center for gamepad
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined; // free cursor
        }
    }


    private void OnSubmit(InputValue value)
    {
        if (value.isPressed && !pauseButtonWasPressed)
        {
            EventSystem.current.SetSelectedGameObject(null);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("Pressed");
            pauseButtonWasPressed = true;
            PauseUnPause();
        }
        else
        {
            Debug.Log("Not Pressed");
            pauseButtonWasPressed = false;
        }
    }

    void OpenMenu(GameObject menu)
    {
        if (currentMenu != null)
        {
            menuHistory.Push(currentMenu);
            currentMenu.SetActive(false);
        }

        menu.SetActive(true);
        currentMenu = menu;

        if (isGamepad && menuDefaultButtons.ContainsKey(menu))
        {
            StartCoroutine(SetSelect(menuDefaultButtons[menu]));
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);

            /*Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;*/
            justPaused = false;
            isPausing = true;
        }
    }

    void GoBack()
    {
        if (menuHistory.Count > 0)
        {
            currentMenu.SetActive(false);
            currentMenu = menuHistory.Pop();
            currentMenu.SetActive(true);
            if (isGamepad && menuDefaultButtons.ContainsKey(currentMenu))
            {
                StartCoroutine(SetSelect(menuDefaultButtons[currentMenu]));
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                justPaused = false;
                isPausing = true;
            }
        }
    }

    public void ChangeCameraSensetivetyControl()
    {
        cameraFollow.controllerSensitivity = sliderControll.value;
    }
    public void ChangeCameraSensetivetyMouse()
    {
        cameraFollow.mouseSensitivity = sliderMouse.value;
    }
    public void PauseUnPause()
    {

        bool isPaused = pauseMenu.activeInHierarchy;
        if (!isPaused)
        {
            SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);
            isGamepad = playerInput.currentControlScheme == "Gamepad";
            EventSystem.current.SetSelectedGameObject(null);

            playerInput.SwitchCurrentActionMap("UI");
            playerMovement.enabled = false;
            Hud_Ref.SetActive(true);
            goddesSymbol.SetActive(true);
            //SymbolConfirm();
            //fishKey.SetActive(true); skullKey.SetActive(true); foot.SetActive(true); hand.SetActive(true);
            OpenMenu(pauseMenu);
            isPausing = true;
            Time.timeScale = 0f;
            justPaused = true;
            if (isGamepad && menuDefaultButtons.ContainsKey(currentMenu))
            {
                StartCoroutine(SetSelect(menuDefaultButtons[currentMenu]));
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                justPaused = false;
                isPausing = true;

            }
        }
        else if (isPaused)
        {
            SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            playerMovement.enabled = true;
            playerInput.SwitchCurrentActionMap("Player");
            Time.timeScale = 1f;
            isPausing = false;
            pauseMenu.SetActive(false);
            optionsMenu.SetActive(false);
            soundMenu.SetActive(false);
            controllMenu.SetActive(false);
            Hud_Ref.SetActive(false);
            goddesSymbol.SetActive(false);
           // fishKey.SetActive(false); skullKey.SetActive(false); foot.SetActive(false); hand.SetActive(false);
            currentMenu = null;
           
            menuHistory.Clear();
        }
    }

    IEnumerator SetSelect(GameObject gameObject)
    {
        yield return new WaitUntil(() =>
        {
            bool startReleased = Gamepad.current == null || !Gamepad.current.startButton.isPressed;
            bool escapeReleased = Keyboard.current == null || !Keyboard.current.escapeKey.isPressed;
            bool buttonSouthReleased = Gamepad.current == null || !Gamepad.current.buttonSouth.isPressed;
            bool enterReleased = Keyboard.current == null || !Keyboard.current.enterKey.isPressed;

            return startReleased && escapeReleased && buttonSouthReleased && enterReleased;
        });

        yield return null; // Let one frame pass

        justPaused = false;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

   /* public void SymbolConfirm()
    {
        for (int i = 0; Inventory.Instance.inventoryItems.Count > i; i++)
        {
            if (Inventory.Instance.inventoryItems[i].itemId == armId && !arm.activeInHierarchy)
            {
                hand.SetActive(true);
            }
            if (Inventory.Instance.inventoryItems[i].itemId == legId && !leg.activeInHierarchy)
            {
                foot.SetActive(true);
            }
            if (Inventory.Instance.inventoryItems[i].itemId == skullKeyId && !skullKeyObject.activeInHierarchy)
            {
                skullKey.SetActive(true);
            }
            if (Inventory.Instance.inventoryItems[i].itemId == fishKeyId && !fishKeyObject.activeInHierarchy)
            {
                fishKey.SetActive(true);
            }
        }
    }
*/
    public void ResumeGame()
    {
        if (justPaused)
        {
            return;
        }

        if (isPausing)
        {
            SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            Debug.Log("Unpause");
            playerMovement.enabled = true;
            playerInput.SwitchCurrentActionMap("Player");
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            Hud_Ref.SetActive(false);
            goddesSymbol.SetActive(false);
            //fishKey.SetActive(false); skullKey.SetActive(false); foot.SetActive(false); hand.SetActive(false);
            optionsMenu.SetActive(false);
            soundMenu.SetActive(false);
            controllMenu.SetActive(false);
            isPausing = false;
            currentMenu = null;
            menuHistory.Clear();
        }
    }

    public void PauseMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);

        playerInput.SwitchCurrentActionMap("UI");
        playerMovement.enabled = false;
        OpenMenu(pauseMenu);

        isPausing = true;
        Time.timeScale = 0f;
        justPaused = true; // <- block resume for 1 frame

        if (!isGamepad)
        {
            EventSystem.current.SetSelectedGameObject(null);

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            justPaused = false;
            isPausing = true;
        }
    }

    public void BackButton()
    {
        PauseMenu();
    }

    public void SoundMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);
        OpenMenu(soundMenu);

        Time.timeScale = 0f;

        justPaused = true; // <- block resume for 1 frame
    }
    public void ControllMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);

        OpenMenu(controllMenu);

        Time.timeScale = 0f;

        justPaused = true; // <- block resume for 1 frame
    }

    public void BackButtonOptions()
    {
        Options();
    }

    public void Options()
    {
        OpenMenu(optionsMenu);

        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);

        Time.timeScale = 0f;
    }

    public void QuitGame()
    {
        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);
        MainMenuScript.cameFromPauseMenu = true;
        SceneManager.LoadScene("MainMenu");
    }
}
