using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public static bool cameFromPauseMenu = false;
    public GameObject mainMenu, creditsMenu;
    public PlayerInput playerInput;
    [SerializeField] TextMeshProUGUI creditsText;

    bool isGamepad;
    private bool justPaused = false;
    public InputActionAsset actions; // Drag in your InputActions asset in inspector
    private InputAction clickAction;
    private InputAction cancelAction;
    private GameObject currentMenu;
    private bool blockInput = false;
    bool pressed;

    Stack<GameObject> menuHistory = new Stack<GameObject>();

    [SerializeField]
    private GameObject defaultStartButton, defaultCreditsButton;

    private Dictionary<GameObject, GameObject> menuDefaultButtons;

    private void Awake()
    {
        StartCoroutine(Start());
        creditsMenu.SetActive(false);
       
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        playerInput.SwitchCurrentActionMap("UI");
        isGamepad = playerInput.currentControlScheme == "Gamepad";
        currentMenu = mainMenu;
        menuDefaultButtons = new Dictionary<GameObject, GameObject>
        {
            { mainMenu, defaultStartButton },
            { creditsMenu, defaultCreditsButton },
        };
    }
    private void OnEnable()
    {
        var actionMap = actions.FindActionMap("UI"); // Or whichever map contains your Click
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
        if (blockInput)
            return;

        var selected = EventSystem.current.currentSelectedGameObject;

       
        if (selected != null && selected.name == "StartButton")
        {
            StartGame();
        }
        else if (selected != null && selected.name == "CreditsButton")
        {
            Credits();
        }
        else if (selected != null && selected.name == "QuitButton")
        {
            QuitGame();
        }
        else if (selected != null && selected.name == "BackButton")
        {
            BackButton();
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

  /*  private void OnSubmit(InputValue value)
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
    }*/

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
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            justPaused = false;
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

    private IEnumerator Start()
    {
        if (cameFromPauseMenu)
        {
            blockInput = true;
            yield return new WaitForSecondsRealtime(0.5f); // Wait until input is released
            cameFromPauseMenu = false;
            blockInput = false;
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

    IEnumerator FadeMenu(GameObject menu, float duration, bool fadeIn)
    {
        float time = 0f;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        // Get all UI components you want to fade
        var texts = menu.GetComponentsInChildren<TextMeshProUGUI>(true);
        var images = menu.GetComponentsInChildren<UnityEngine.UI.Image>(true);

        while (time < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);

            foreach (var txt in texts)
            {
                Color c = txt.color;
                c.a = alpha;
                txt.color = c;
            }

            foreach (var img in images)
            {
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }

            time += Time.deltaTime;
            yield return null;
        }

        // Ensure final alpha
        foreach (var txt in texts)
        {
            Color c = txt.color;
            c.a = endAlpha;
            txt.color = c;
        }

        foreach (var img in images)
        {
            Color c = img.color;
            c.a = endAlpha;
            img.color = c;
        }

        // Optional: disable menu when fully faded out
        if (!fadeIn)
            OpenMenu(menu);
    }

    public void StartGame()
    {
        if (justPaused)
        {
            return;
        }
        else if (!pressed)
        {
            pressed = true;
            SoundFXManager.Instance.PlayButtonSoundFX(SoundType.Boom);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            playerInput.SwitchCurrentActionMap("Player");
            Time.timeScale = 1f;
            //mainMenu.SetActive(false);
            creditsMenu.SetActive(false);
            StartCoroutine(FadeMenu(mainMenu, 1.5f, false));  
            MusicManager.Instance.StopMusic(1.5f);
            Invoke(nameof(LoadScene), 2);
        }
    }

    void LoadScene()
    {
        SceneManager.LoadScene("LoadScene");
    }

    public void PauseMenu()
    {
        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);

        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        playerInput.SwitchCurrentActionMap("UI");
        Time.timeScale = 0f;
        creditsText.enabled = false;
        OpenMenu(mainMenu);
    }

    public void BackButton()
    {
        PauseMenu();
    }

    public void Credits()
    {
        OpenMenu(creditsMenu);

        SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);

        Time.timeScale = 0f;


    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
