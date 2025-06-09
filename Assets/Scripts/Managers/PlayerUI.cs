using TMPro;
using UnityEngine;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI displayText;

    private Coroutine currentTextCoroutine;
    readonly float fishKeyId = 1;
    readonly float skullKeyId = 2;
    readonly float armId = 3;
    readonly float legId = 4;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void DisplayText(string text, float duration)
    {
        if (currentTextCoroutine != null)
        {
            StopCoroutine(currentTextCoroutine);
        }

        currentTextCoroutine = StartCoroutine(DisplayTextCoroutine(text, duration));
    }

    private IEnumerator DisplayTextCoroutine(string text, float duration)
    {
        displayText.text = text;
        displayText.enabled = true;

        yield return new WaitForSeconds(duration);

        displayText.text = string.Empty;
        displayText.enabled = false;
        currentTextCoroutine = null;
    }
}
