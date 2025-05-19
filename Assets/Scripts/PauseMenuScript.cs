using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenu, optionsMenu;
    bool pressed;

    private void Awake()
    {
        pauseMenu.SetActive(false);
    }

    private void OnClick(InputValue value)
    {
        pressed = value.isPressed;
    }

    private void OnSubmit(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("Pause triggered!");
            PauseUnPause();
        }
    }

    void PauseUnPause()
    {
        if (!pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            optionsMenu.SetActive(false);
        }
    }
}
