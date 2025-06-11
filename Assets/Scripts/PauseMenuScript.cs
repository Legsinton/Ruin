using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    public GameObject Hud_Ref, goddesSymbol, panel;
    GameObject playerCanvas;

    [Header("Settings")]
    public PlayerInput playerInput;
    public PlayerMovement playerMovement;
    public CameraFollow cameraFollow;
    public InputActionAsset actions;
    private InputAction clickAction;
    private InputAction cancelAction;
    private GameObject currentMenu;
    bool pauseButtonWasPressed = false;
    [SerializeField] bool isPausing;
    bool isGamepad;
    [SerializeField] bool justPaused = false;

    Stack<GameObject> menuHistory = new Stack<GameObject>();


    private Dictionary<GameObject, GameObject> menuDefaultButtons;


    private void Awake()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        soundMenu.SetActive(false);
        controllMenu.SetActive(false);
        goddesSymbol.SetActive(false);
        panel.SetActive(false);
        Hud_Ref.SetActive(false);
        playerCanvas = GameObject.Find("CanvasPlayer");

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
        Debug.Log($"Control scheme changed to: {scheme}");

        var uiActionMap = actions.FindActionMap("UI");
        var pointAction = uiActionMap?.FindAction("Point");

        if (pointAction != null)
        {
            if (isGamepad)
            {
                pointAction.Disable();
            }
            else
            {
                pointAction.Enable();
                Debug.Log("Mouse Point action re-enabled");
            }
        }

        // Reset selection
        EventSystem.current.SetSelectedGameObject(null);

        if (isGamepad)
        {
            if (currentMenu != null && menuDefaultButtons.ContainsKey(currentMenu))
            {
                EventSystem.current.SetSelectedGameObject(menuDefaultButtons[currentMenu]);
            }
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            justPaused = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }



    private void OnSubmit(InputValue value)
    {
        if (value.isPressed && !pauseButtonWasPressed)
        {
            Debug.Log("Pressed");
            pauseButtonWasPressed = true;
            PauseUnPause();
        }
    }

    void StopButton()
    {
        Debug.Log("Stopped pressed");
        pauseButtonWasPressed = false;

    }

    private void OnUnClick(InputValue value)
    {
        if (value.isPressed && pauseButtonWasPressed)
        {
            Debug.Log("Un Pressed");
            Invoke(nameof(StopButton), 1);

            PauseUnPause();
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

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
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
            playerInput.SwitchCurrentActionMap("UI");
            playerMovement.enabled = false;
            Hud_Ref.SetActive(true);
            panel.SetActive(true);
            goddesSymbol.SetActive(true);
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
            playerCanvas.SetActive(false);
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
            panel.SetActive(false);
            goddesSymbol.SetActive(false);
            currentMenu = null;

            playerCanvas.SetActive(true);

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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
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
            panel.SetActive(false);
            goddesSymbol.SetActive(false);
            optionsMenu.SetActive(false);
            soundMenu.SetActive(false);
            controllMenu.SetActive(false);
            isPausing = false;
            currentMenu = null;
            menuHistory.Clear();
            pauseButtonWasPressed = false;
            playerCanvas.SetActive(true);
        }
    }

    public void PauseMenu()
    {
        //EventSystem.current.SetSelectedGameObject(null);
        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);

        playerInput.SwitchCurrentActionMap("UI");
        playerMovement.enabled = false;
        OpenMenu(pauseMenu);

        isPausing = true;
        Time.timeScale = 0f;
        justPaused = false; // <- block resume for 1 frame
    }

    public void BackButton()
    {
        PauseMenu();
    }

    public void SoundMenu()
    {
        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);
        OpenMenu(soundMenu);

        Time.timeScale = 0f;

        justPaused = true; // <- block resume for 1 frame
    }
    public void ControllMenu()
    {
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
