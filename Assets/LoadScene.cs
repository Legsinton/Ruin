using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public TextMeshProUGUI introText;
    public TextMeshProUGUI introDot;

    public AudioSource soundClip;

    private void Awake()
    {
        introText.enabled = false;
        introDot.enabled = false;
    }
    private void Start()
    {
        StartCoroutine(LoadText());
        Invoke(nameof(LoadNewScene), 19);
    }

    IEnumerator LoadText()
    {
        yield return new WaitForSeconds(10.5f);
        //introDot.enabled = true;
        introText.enabled = true;
        
       
        yield return new WaitForSeconds(6f);
        introText.enabled = false;
    }

    void LoadNewScene()
    {
        SceneManager.LoadScene("Playtest_4");
    }
}
