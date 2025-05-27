using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenu, optionsMenu,soundMenu,controllMenu;
    public PlayerInput playerInput;
    public PlayerMovement playerMovement;
    public CameraFollow cameraFollow;
    bool pauseButtonWasPressed = false;
    [SerializeField] GameObject resumeButton, backButton,backButtonOptions, backButtonOptionsSound;
    public Slider sliderControll;
    public Slider sliderMouse;
    bool isPausing;
    bool isGamepad;
    private bool justPaused = false;
    public InputActionAsset actions; // Drag in your InputActions asset in inspector
    private InputAction clickAction;
    public AudioMixerSnapshot pausedSnapshot;
    public AudioMixerSnapshot normalSnapshot;

    private void Awake()
    {
        pauseMenu.SetActive(false);
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
            SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);           
            isGamepad = playerInput.currentControlScheme == "Gamepad";
            EventSystem.current.SetSelectedGameObject(null);
            if(pausedSnapshot != null)
            {
                pausedSnapshot.TransitionTo(0.2f); // quickly lower volume or apply effect
            }
            playerInput.SwitchCurrentActionMap("UI");
            playerMovement.enabled = false;
            soundMenu.SetActive(false);
            optionsMenu.SetActive(false);
            pauseMenu.SetActive(true);
            controllMenu.SetActive(false);
            isPausing = true;
            Time.timeScale = 0f;
            justPaused = true; 
            if (isGamepad)
            {
                StartCoroutine(SetSelect(resumeButton)); 
            }
            else if (!isGamepad)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                justPaused = false;
            }
        }
        else if (isPaused)
        {
            SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            playerMovement.enabled = true;
            if (normalSnapshot != null)
            {
                normalSnapshot.TransitionTo(0.2f); // restore normal sound
            }
            playerInput.SwitchCurrentActionMap("Player");
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPausing = false;
            optionsMenu.SetActive(false);
            soundMenu.SetActive(false);
            controllMenu.SetActive(false);
        }
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
            if(normalSnapshot != null)
            {
                normalSnapshot.TransitionTo(0.2f); // restore normal sound
            }
            playerMovement.enabled = true;
            playerInput.SwitchCurrentActionMap("Player");
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            optionsMenu.SetActive(false);
            soundMenu.SetActive(false);
            controllMenu.SetActive(false);

        }
    }

    public void PauseMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);

        playerInput.SwitchCurrentActionMap("UI");
        playerMovement.enabled = false;
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        isPausing = true;
        Time.timeScale = 0f;
        justPaused = true; // <- block resume for 1 frame
        if (isGamepad)
        {
            Cursor.visible = false;
            StartCoroutine(SetSelect(resumeButton));
        }
        else if (!isGamepad)
        {
            justPaused = false;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
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

        soundMenu.SetActive(true);
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        controllMenu.SetActive(false);
        Time.timeScale = 0f;

        justPaused = true; // <- block resume for 1 frame
        if (isGamepad)
        {
            Cursor.visible = false;
            StartCoroutine(SetSelect(backButtonOptionsSound));
        }
        else if (!isGamepad)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            justPaused = false;
        }
    }
    public void ControllMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);

        soundMenu.SetActive(false);
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        controllMenu.SetActive(true);
        Time.timeScale = 0f;

        justPaused = true; // <- block resume for 1 frame
        if (isGamepad)
        {
            Cursor.visible = false;
            StartCoroutine(SetSelect(backButtonOptions));
        }
        else if (!isGamepad)
        {
            Cursor.lockState = CursorLockMode.Confined;
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
        
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
        soundMenu.SetActive(false);
        controllMenu.SetActive(false);
        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);

        Time.timeScale = 0f; 
        if (isGamepad)
        {
            Cursor.visible = false;
            StartCoroutine(SetSelect(backButton));
        }
        else if (!isGamepad)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            justPaused = false;
        }
    }

    public void QuitGame()
    {
        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);

        SceneManager.LoadScene("MainMenu");
    }
}
