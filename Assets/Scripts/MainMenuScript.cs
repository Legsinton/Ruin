using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public GameObject mainMenu, optionsMenu, soundMenu, controllMenu;
    public PlayerInput playerInput;
    public CameraFollow cameraFollow;
    bool pauseButtonWasPressed = false;
    public Slider sliderControll;
    public Slider sliderMouse;
    [SerializeField] TextMeshProUGUI creditsText;
    bool isGamepad;
    private bool justPaused = false;
    public InputActionAsset actions; // Drag in your InputActions asset in inspector
    private InputAction clickAction;
    private InputAction cancelAction;
    private GameObject currentMenu;

    Stack<GameObject> menuHistory = new Stack<GameObject>();

    [SerializeField]
    private GameObject defaultStartButton, defaultOptionsButton, defaultSoundButton, defaultControlButton;

    private Dictionary<GameObject, GameObject> menuDefaultButtons;

    private void Awake()
    {
        optionsMenu.SetActive(false);
        soundMenu.SetActive(false);
        controllMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        playerInput.SwitchCurrentActionMap("UI");
        isGamepad = playerInput.currentControlScheme == "Gamepad";
        currentMenu = mainMenu;
        menuDefaultButtons = new Dictionary<GameObject, GameObject>
        {
            { mainMenu, defaultStartButton },
            { optionsMenu, defaultOptionsButton },
            { soundMenu, defaultSoundButton },
            { controllMenu, defaultControlButton }
        };
    }
    private void OnEnable()
    {
        var actionMap = actions.FindActionMap("UI"); // Or whichever map contains your Click
        clickAction = actionMap.FindAction("Click");
        cancelAction = actionMap.FindAction("Cancel");
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

       
        if (selected != null && selected.name == "StartButton")
        {
            StartGame();
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
        else if (selected != null && selected.name == "CreditsButton")
        {
            CreditsMenu();
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

    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        var selected = EventSystem.current.currentSelectedGameObject;

        
        if (context.action.name == "Cancel")
        {
            GoBack();
        }
        else
        {

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
            BackButton();
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
        isGamepad = playerInput.currentControlScheme == "Gamepad";

        if (isGamepad && menuDefaultButtons.ContainsKey(menu))
        {
            StartCoroutine(SetSelect(menuDefaultButtons[menu]));
        }
        else if (!isGamepad)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
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
   
    IEnumerator SetSelect(GameObject gameObject)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
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

    public void StartGame()
    {
        if (justPaused)
        {
            return;
        }

        creditsText.enabled = false;

        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.Boom);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        playerInput.SwitchCurrentActionMap("Player");
        Time.timeScale = 1f;
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        soundMenu.SetActive(false);
        controllMenu.SetActive(false);
        MusicManager.Instance.StopMusic(1.5f);
        Invoke(nameof(LoadScene), 2);
    }

    void LoadScene()
    {
        SceneManager.LoadScene("LoadScene");
    }

    public void PauseMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        playerInput.SwitchCurrentActionMap("UI");
        Time.timeScale = 0f;
        creditsText.enabled = false;
        OpenMenu(mainMenu);
        justPaused = true; // <- block resume for 1 frame
    }

    public void BackButton()
    {
        PauseMenu();
    }

    public void SoundMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Confined;
        OpenMenu(soundMenu);

        Time.timeScale = 0f;
        creditsText.enabled = false;

        justPaused = true; // <- block resume for 1 frame

    }
    public void ControllMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Confined;
        OpenMenu(controllMenu);

        Time.timeScale = 0f;
        creditsText.enabled = false;
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
        Application.Quit();
    }

    public void CreditsMenu()
    {
        creditsText.enabled = !creditsText.enabled;
    }
}
