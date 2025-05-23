using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenu, optionsMenu,soundMenu,controllMenu;
    public PlayerInput playerInput;
    public PlayerMovement playerMovement;
    public CameraFollow cameraFollow;
    [SerializeField] bool pauseButtonWasPressed = false;
    [SerializeField] GameObject resumeButton,backButton;
    [SerializeField] GameObject backButtonOptions, backButtonOptionsSound;
    [SerializeField] TextMeshProUGUI creditsText;
    public Slider sliderControll;
    public Slider sliderMouse;
    bool isPausing;
    bool isGamepad;
    private bool justPaused = false;
    public InputActionAsset actions; // Drag in your InputActions asset in inspector
    private InputAction clickAction;

    private void Awake()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsText.enabled = false;
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
            else if (selected != null && selected.name == "CreditsButton")
            {
                Credits();
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
            isGamepad = playerInput.currentControlScheme == "Gamepad";
            EventSystem.current.SetSelectedGameObject(null);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            playerInput.SwitchCurrentActionMap("UI");
            playerMovement.enabled = false;
            optionsMenu.SetActive(false);
            pauseMenu.SetActive(true);
            isPausing = true;
            Time.timeScale = 0f;
            creditsText.enabled = false;
            justPaused = true; // <- block resume for 1 frame
            if (isGamepad)
            {
                Debug.LogWarning("Hälloss");
                StartCoroutine(SetSelect(resumeButton)); 
            }
            else if (!isGamepad)
            {
                justPaused = false;
            }
        }
        else if (isPaused)
        {
            creditsText.enabled = false;
            Cursor.lockState = CursorLockMode.None;

            Cursor.visible = false;
            playerMovement.enabled = true;
            playerInput.SwitchCurrentActionMap("Player");
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPausing = false;
            optionsMenu.SetActive(false);
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

    public void ResumeGame()
    {
        if (justPaused)
        {
            return;
        }

        if (isPausing)
        {          
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            playerMovement.enabled = true;
            playerInput.SwitchCurrentActionMap("Player");
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            optionsMenu.SetActive(false);
            creditsText.enabled = false;

        }
    }

    public void PauseMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        playerInput.SwitchCurrentActionMap("UI");
        playerMovement.enabled = false;
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        isPausing = true;
        Time.timeScale = 0f;
        creditsText.enabled = false;
        justPaused = true; // <- block resume for 1 frame
        if (isGamepad)
        {
            Debug.LogWarning("Hälloss");
            StartCoroutine(SetSelect(resumeButton));
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
        pauseMenu.SetActive(false);
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
        pauseMenu.SetActive(false);
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
        isGamepad = playerInput.currentControlScheme == "Gamepad";
        creditsText.enabled = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
        soundMenu.SetActive(false);
        controllMenu.SetActive(false);
        Time.timeScale = 0f; 
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
        //UnityEditor.EditorApplication.isPlaying = false;
    }

    public void Credits()
    {
        isGamepad = playerInput.currentControlScheme == "Gamepad";
        if (isGamepad)
        {
            StartCoroutine(SetSelect(backButton));
        }
        creditsText.enabled = !creditsText.enabled;
    }
}
