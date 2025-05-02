using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagement : MonoBehaviour
{
    public Image overlay;
    public TMP_Text gameOverText;

    public void OnDeath()
    {
        StartCoroutine(DeathScreen());
    }

    private IEnumerator DeathScreen()
    {
        yield return StartCoroutine(FadeToBlack(1));
        gameOverText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        gameOverText.gameObject.SetActive(false);
        SceneManager.LoadScene("Playtest_1");
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
}