using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [HideInInspector] private bool deathScreenStarted = false;
    [HideInInspector] public bool playerHasDied = false;
    [SerializeField] string music;
    [SerializeField] float fadeDuration;

    PlayerMovement playerMovement;
    GameObject player;
    CameraFollow cameraFollow;
    GameObject playerCamera;
    [SerializeField] Camera playerCameraCutscene;

    [Header("Settings For Cameras")]
    public Transform[] waypoints; // Set these in the Inspector
    public float moveDuration = 2f; // Time between waypoints
    [SerializeField] float cutSceneLength;
    [SerializeField] Transform target;

    [Header("References")]
    public bool cutscene;
    [SerializeField] Camera cutSceneCamera;
    [SerializeField] AudioListener playrtAudioListener;
    [SerializeField] AudioListener cameraAudioListener;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        cameraFollow = FindFirstObjectByType<CameraFollow>();
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        player = playerGO.transform.root.gameObject;
        playerCamera = GameObject.Find("PlayerCamera");
    }

    public void OnDeath()
    {
        if (playerHasDied || deathScreenStarted) return;

        Debug.Log("Im dying");
        //playerHasDied = true;
        deathScreenStarted = true;
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
        playerMovement.enabled = false;
        player.transform.position = playerMovement.playerStartPosition;

        playerCamera.transform.position = cameraFollow.cameraStartPosition;
        cameraFollow.StartFollowing();
        cameraFollow.DisableLockCamera();
        MusicManager.Instance.PlayMusic(music, fadeDuration);

        yield return null;

        yield return StartCoroutine(FadeOut(2));
        playerMovement.enabled = true;
        playerHasDied = false;
        deathScreenStarted = false;
        Debug.Log("Im finished dying");

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
        ActivateCamera();
        StartCoroutine(PlayCutscene());
        yield return StartCoroutine(FadeToWhite(2));
        winText.gameObject.SetActive(true);
        yield return new WaitForSeconds(7f);
        /*winText.gameObject.SetActive(false);
        overlayWhite.gameObject.SetActive(false);*/
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

    IEnumerator FadeOut(float duration)
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            overlay.color = Color.Lerp(Color.black, Color.clear, timer / duration);
            yield return 0;
        }
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
    void ActivateCamera()
    {
        if (playerCameraCutscene != null) playerCameraCutscene.enabled = false;
        if (cutSceneCamera != null) cutSceneCamera.enabled = true;
        if (playrtAudioListener != null && cutSceneCamera != null)
        {
            cutSceneCamera.enabled = true;
            playrtAudioListener.enabled = false;
            cameraAudioListener.enabled = true;
            cutscene = true;
        }
    }
    private IEnumerator PlayCutscene()
    {
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            cutSceneCamera.transform.LookAt(target);
            Vector3 startPos = waypoints[i].position;
            Quaternion startRot = waypoints[i].rotation;
            Vector3 endPos = waypoints[i + 1].position;
            Quaternion endRot = waypoints[i + 1].rotation;

            Debug.Log("Started Moving");

            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                Debug.Log("Is Moving");

                float t = elapsed / moveDuration;
                cutSceneCamera.transform.position = Vector3.Lerp(startPos, endPos, t);
                cutSceneCamera.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                cutSceneCamera.transform.LookAt(target);
                elapsed += Time.deltaTime;
                yield return null;
            }

            cutSceneCamera.transform.position = endPos;
            cutSceneCamera.transform.LookAt(target);
        }

        yield return new WaitForSeconds(cutSceneLength); // example timing, or use actual animation/movement
        Debug.Log("Im done");
    }
}