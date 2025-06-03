using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public TextMeshProUGUI introText;

    private void Awake()
    {
        introText.enabled = false;
    }
    private void Start()
    {
        StartCoroutine(LoadText());
        Invoke(nameof(LoadNewScene), 19);
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
}
