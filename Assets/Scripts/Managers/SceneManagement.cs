using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement Instance;
    public string sceneToLoad;
    [Header("Game over UI")]
    public Image overlay;
    [Header("Win State UI")]
    public Image overlayWhite;
    public TMP_Text winText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnDeath()
    {
        StartCoroutine(DeathScreen());
    }

    public void OnWin()
    {
       StartCoroutine(WinScene());
    }

    private IEnumerator DeathScreen()
    {
        yield return StartCoroutine(FadeToBlack(1));
        yield return new WaitForSeconds(0.5f);
        overlay.gameObject.SetActive(false);
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator WinScene()
    {
        PlayerMovement playerMovement = FindAnyObjectByType<PlayerMovement>();
        SoundFXManager.Instance.PlaySoundFX(SoundType.Boom);
        if (playerMovement != null)
        {
            playerMovement.cutscene = true;
            playerMovement.PushBlock = null;
            playerMovement.movementInput = new Vector2(0, 0);
            playerMovement.movement = new Vector3(0, 0, 0);
        }
        yield return StartCoroutine(FadeToWhite(2));
        winText.gameObject.SetActive(true);
        yield return new WaitForSeconds(7f);
        winText.gameObject.SetActive(false);
        overlayWhite.gameObject.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator FadeToBlack(float duration)
    {
        Color color = overlay.color;
        float elapsedTime = 0f;

        overlay.gameObject.SetActive(true);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            overlay.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0f, 1f, elapsedTime / duration));
            yield return null;
        }

        overlay.color = new Color(color.r, color.g, color.b, 1f);
    }

    public IEnumerator FadeToWhite(float duration)
    {
        Color color = overlayWhite.color;
        float elapsedTime = 0f;

        overlayWhite.gameObject.SetActive(true);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            overlayWhite.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0f, 1f, elapsedTime / duration));
            yield return null;
        }

        overlayWhite.color = new Color(color.r, color.g, color.b, 1f);
    }
}