using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class LoadScene : MonoBehaviour
{
    public TextMeshProUGUI introText;
    public TextMeshProUGUI scipText;
    public TextMeshProUGUI scipTextKeyboard;
    private InputAction skipAction;

    bool justPressed;
    bool pressedOnce;


    private void Awake()
    {
        introText.enabled = false;
        scipText.enabled = false;
        scipTextKeyboard.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        skipAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
        skipAction.AddBinding("<Gamepad>/buttonSouth"); 
        skipAction.performed += OnSkipPressed;
        skipAction.Enable();
    }
    private void Start()
    {
        StartCoroutine(LoadText());
        Invoke(nameof(LoadNewScene), 19);
    }

    private void OnSkipPressed(InputAction.CallbackContext context)
    {
        // Detect what control was used
        string controlPath = context.control.path;

        if (!pressedOnce)
        {
            if (controlPath.Contains("buttonSouth")) // Gamepad
            {
                scipText.enabled = true;
                StartCoroutine(FadeMenu(scipText, 1.5f, true));

                if (scipTextKeyboard.enabled)
                {
                    StartCoroutine(FadeMenu(scipTextKeyboard, 1.5f, false));
                }
            }
            else if (controlPath.Contains("space")) // Keyboard
            {
                scipTextKeyboard.enabled = true;
                StartCoroutine(FadeMenu(scipTextKeyboard, 1.5f, true));

                if (scipText.enabled)
                {
                    StartCoroutine(FadeMenu(scipText, 1.5f, false));
                }
            }

            pressedOnce = true;
            Invoke(nameof(StopPressed), 2f);
        }
        else
        {
            StopAllCoroutines();
            LoadNewScene();
        }
    }

    private void OnDestroy()
    {
        skipAction.Disable();
        skipAction.Dispose();
    }

    IEnumerator LoadText()
    {
        yield return new WaitForSeconds(10.5f);
        introText.enabled = true;
        StartCoroutine(FadeMenu(introText, 1.5f, true));
        
        yield return new WaitForSeconds(6f);
        StartCoroutine(FadeMenu(introText, 1.5f, false));
    }

    IEnumerator FadeMenu(TextMeshProUGUI menu, float duration, bool fadeIn)
    {
        float time = 0f;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        // Get all UI components you want to fade
        var texts = menu.GetComponentsInChildren<TextMeshProUGUI>(true);

        while (time < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);

            foreach (var txt in texts)
            {
                Color c = txt.color;
                c.a = alpha;
                txt.color = c;
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
    }

    void LoadNewScene()
    {
        SceneManager.LoadScene("Final_Build");
    }

    void StopPressed()
    {
        pressedOnce = true;
    }
}
