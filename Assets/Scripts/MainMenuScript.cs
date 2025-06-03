using System.Collections;
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
    [SerializeField] GameObject creditsButton, startButton, backButton, backButtonOptions, backButtonOptionsSound;
    public Slider sliderControll;
    public Slider sliderMouse;
    [SerializeField] TextMeshProUGUI creditsText;
    bool isGamepad;
    private bool justPaused = false;
    public InputActionAsset actions; // Drag in your InputActions asset in inspector
    private InputAction clickAction;

    private void Awake()
    {
        optionsMenu.SetActive(false);
        soundMenu.SetActive(false);
        controllMenu.SetActive(false);
    }
    private void OnEnable()
    {
        var actionMap = actions.FindActionMap("UI"); // Or whichever map contains your Click
        clickAction = actionMap.FindAction("Click");
        clickAction.Enable();
        clickAction.performed += OnClickPerformed;
    }

    private void OnDisable()
    {
        clickAction.performed -= OnClickPerformed;
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
        mainMenu.SetActive(false);
        Time.timeScale = 1f;
        optionsMenu.SetActive(false);
        soundMenu.SetActive(false);
        controllMenu.SetActive(false);
        Invoke(nameof(LoadScene), 2);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void PauseMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        playerInput.SwitchCurrentActionMap("UI");
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
        Time.timeScale = 0f;
        creditsText.enabled = false;

        justPaused = true; // <- block resume for 1 frame
        if (isGamepad)
        {
            Debug.LogWarning("Hälloss");
            StartCoroutine(SetSelect(startButton));
        }
        else if (!isGamepad)
        {
            justPaused = false;
        }
    }

    public void BackButton()
    {
        PauseMenu();
    }

    public void SoundMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Confined;
        soundMenu.SetActive(true);
        optionsMenu.SetActive(false);
        mainMenu.SetActive(false);
        controllMenu.SetActive(false);
        Time.timeScale = 0f;
        creditsText.enabled = false;

        justPaused = true; // <- block resume for 1 frame
        if (isGamepad)
        {
            Cursor.visible = false;
            StartCoroutine(SetSelect(backButtonOptionsSound));
        }
        else if (!isGamepad)
        {
            Cursor.visible = true;
            justPaused = false;
        }
    }
    public void ControllMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Confined;
        soundMenu.SetActive(false);
        optionsMenu.SetActive(false);
        mainMenu.SetActive(false);
        controllMenu.SetActive(true);
        Time.timeScale = 0f;
        creditsText.enabled = false;
        justPaused = true; // <- block resume for 1 frame
        if (isGamepad)
        {
            Cursor.visible = false;
            StartCoroutine(SetSelect(backButtonOptions));
        }
        else if (!isGamepad)
        {
            Cursor.visible = true;
            justPaused = false;
        }
    }
    public void BackButtonOptions()
    {
        Options();
    }

    public void Options()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        soundMenu.SetActive(false);
        controllMenu.SetActive(false);
        Time.timeScale = 0f;
        creditsText.enabled = false;
        if (isGamepad)
        {
            StartCoroutine(SetSelect(backButton));
        }
        else if (!isGamepad)
        {
            justPaused = false;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void CreditsMenu()
    {
        creditsText.enabled = !creditsText.enabled;
        if (isGamepad)
        {
            StartCoroutine(SetSelect(backButton));
        }
    }
}
