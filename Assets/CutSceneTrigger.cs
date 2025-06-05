using UnityEngine;
using System.Collections;

public class CutSceneTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool playSound;
    [SerializeField] Transform target;
    [SerializeField] string music;
    [SerializeField] float fadeDuration;
    bool played;
    bool playedCutScene;

    [Header("Settings For Cameras")]
    public Transform[] waypoints; // Set these in the Inspector
    public float moveDuration = 2f; // Time between waypoints
    [SerializeField] float cutSceneLength;

    [Header("References")]
    public bool cutscene;
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera cutSceneCamera;
    [SerializeField] AudioListener playrtAudioListener;
    [SerializeField] AudioListener cameraAudioListener;
    PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!MusicManager.Instance.IsTrackPlaying(music))
            {
                MusicManager.Instance.PlayMusic(music, fadeDuration);
            }
            else
            {
                Debug.Log("Song already playing");
            }

            Debug.Log("Im triggered");
            if (!playedCutScene && cutSceneCamera != null)
            {
                if (playSound && !played)
                {
                    played = true;
                    SoundFXManager.Instance.PlaySoundFX(SoundType.PuzzleSolvedFully);
                }
                ActivateCamera();
                StartCoroutine(PlayCutscene());

                playedCutScene = true;
            }
        }
    }

    void ActivateCamera()
    {
        if (playerCamera != null) playerCamera.enabled = false;
        if (cutSceneCamera != null) cutSceneCamera.enabled = true;
        if (playrtAudioListener != null && cutSceneCamera != null)
        {
            cutSceneCamera.enabled = true;
            playrtAudioListener.enabled = false;
            cameraAudioListener.enabled = true;
            cutscene = true;


        }
        if (playerMovement != null)
        {
            playerMovement.cutscene = true;
            playerMovement.PushBlock = null;
            playerMovement.movementInput = new Vector2(0, 0);
            playerMovement.movement = new Vector3(0, 0, 0);
        }
    }


    void DisableActiveCamera()
    {
        if (playerCamera != null) playerCamera.enabled = true;
        if (cutSceneCamera != null) cutSceneCamera.enabled = false;
        if (playrtAudioListener != null && cutSceneCamera != null)
        {
            cutSceneCamera.enabled = false;
            playrtAudioListener.enabled = true;
            cameraAudioListener.enabled = false;
            cutscene = false;
        }
        if (playerMovement != null)
        {
            playerMovement.cutscene = false;
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
        DisableActiveCamera(); // call this when the cutscene finishes
    }
}
