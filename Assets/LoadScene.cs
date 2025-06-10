using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class LoadScene : MonoBehaviour
{
    public TextMeshProUGUI introText;
    public TextMeshProUGUI scipText;
    private InputAction skipAction;

    bool justPressed;
    bool pressedOnce;


    private void Awake()
    {
        introText.enabled = false;
        scipText.enabled = false;
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
        if (!pressedOnce)
        {
            scipText.enabled = true;
            StartCoroutine(FadeMenu(scipText, 1.5f, true));
            pressedOnce = true;
            Invoke(nameof(StopPressed), 2f);
        }
        else
        {
            StopAllCoroutines();
            SceneManager.LoadScene("Playtest_4");
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
        SceneManager.LoadScene("Playtest_4");
    }

    void StopPressed()
    {
        pressedOnce = true;
    }
}
