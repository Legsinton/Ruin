using System.Collections;
using System.Collections.Generic;
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
    public Slider sliderControll;
    public Slider sliderMouse;
    bool pauseButtonWasPressed = false;
    bool isPausing;
    bool isGamepad;
    bool justPaused = false;
    public InputActionAsset actions; 
    private InputAction clickAction;
    private InputAction cancelAction;
    private GameObject currentMenu;

    Stack<GameObject> menuHistory = new Stack<GameObject>();

    [SerializeField]
    private GameObject defaultPauseButton, defaultOptionsButton, defaultSoundButton, defaultControlButton;

    private Dictionary<GameObject, GameObject> menuDefaultButtons;


    private void Awake()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        soundMenu.SetActive(false);
        controllMenu.SetActive(false);
        /*Hud_Ref1.SetActive(false);
        Hud_Ref2.SetActive(false);
        fishKey.SetActive(false);
        skullKey.SetActive(false);
        foot.SetActive(false);
        arm.SetActive(false);*/
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
            EventSystem.current.SetSelectedGameObject(null);
        
            playerInput.SwitchCurrentActionMap("UI");
            playerMovement.enabled = false;
            OpenMenu(pauseMenu);
            /*Hud_Ref1.SetActive(true);
            Hud_Ref2.SetActive(true);
            fishKey.SetActive(true);
            skullKey.SetActive(true);
            foot.SetActive(true);
            arm.SetActive(true);*/
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
                justPaused = false;
                isPausing = true;

            }
        }
        else if (isPaused)
        {
            SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = false;
            playerMovement.enabled = true;
            playerInput.SwitchCurrentActionMap("Player");
            Time.timeScale = 1f;
            isPausing = false;
            pauseMenu.SetActive(false);
            optionsMenu.SetActive(false);
            soundMenu.SetActive(false);
            controllMenu.SetActive(false);
            
            //currentMenu = null;
            menuHistory.Clear();
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
            //Cursor.lockState = CursorLockMode.Confined;
            //Cursor.visible = false; 

            Debug.Log("Unpause");
            playerMovement.enabled = true;
            playerInput.SwitchCurrentActionMap("Player");
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            optionsMenu.SetActive(false);
            soundMenu.SetActive(false);
            controllMenu.SetActive(false);
            /*Hud_Ref1.SetActive(false);
            Hud_Ref2.SetActive(false);
            fishKey.SetActive(false);
            skullKey.SetActive(false);
            foot.SetActive(false);
            arm.SetActive(false);*/
            isPausing = false;

            //currentMenu = null;
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
        
        if(!isGamepad)
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

        SceneManager.LoadScene("MainMenu");
    }
}
