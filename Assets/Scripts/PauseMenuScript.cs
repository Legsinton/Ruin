using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenu, optionsMenu;
    public PlayerInput playerInput;
    public PlayerMovement playerMovement;
    [SerializeField] bool pauseButtonWasPressed = false;
    [SerializeField] GameObject resumeButton;
    [SerializeField] GameObject backButton;
    [SerializeField] bool clickButtonWasPressed;
    [SerializeField] TextMeshProUGUI creditsText;
    bool isPausing;
    private bool justPaused = false;
    public InputActionAsset actions; // Drag in your InputActions asset in inspector
    private InputAction clickAction;

    private void Awake()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsText.enabled = false;
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
                // Handle options click here
            }
            else if (selected != null && selected.name == "QuitButton")
            {
                // Handle options click here
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

    public void PauseUnPause()
    {
        
        bool isPaused = pauseMenu.activeInHierarchy;
        if (!isPaused)
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
            StartCoroutine(SetSelect(resumeButton)); 
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

    public void BackButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        isPausing = true;
        Time.timeScale = 0f;
        creditsText.enabled = false;

        justPaused = true; // <- block resume for 1 frame
        StartCoroutine(SetSelect(resumeButton));
    }

    public void Options()
    {
        creditsText.enabled = false;
        Cursor.lockState = CursorLockMode.Confined;

        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
        StartCoroutine(SetSelect(backButton));
        Time.timeScale = 0f; 
    }

    public void QuitGame()
    {

    }

    public void Credits()
    {
        creditsText.enabled = !creditsText.enabled;
    }
}
