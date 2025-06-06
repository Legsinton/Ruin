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
        skipAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
        skipAction.AddBinding("<Gamepad>/buttonSouth"); // A on Xbox, X on PS
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
        
        yield return new WaitForSeconds(7f);
        introText.enabled = false;
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
