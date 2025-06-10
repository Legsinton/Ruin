using UnityEngine;

public class GateScript : MonoBehaviour, ISwitchManager
{
    [Header("Settings")]
    [SerializeField] int switches;
    [SerializeField] int switchesNeeded;
    [SerializeField] float pressDepth;
    [SerializeField] float moveSpeed;
    [SerializeField] float arriveThreshold;
    [SerializeField] bool playSound;
    public bool cutscene;
    bool played;

    bool playedCutScene;
    [SerializeField] bool moveGate;
    Vector3 originalPosition;
    Vector3 targetPos;
    GameObject cinematicCanvas;
    GameObject playerCanvas;

    [Header("Settings For Cameras")]
    [SerializeField] float cutSceneLength;

    [Header("References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera cutSceneCamera;
    [SerializeField] AudioListener playrtAudioListener;
    [SerializeField] AudioListener cameraAudioListener;
    [SerializeField] Outline outlineScript;
    PlayerMovement playerMovement;

    private void Awake()
    {
        cinematicCanvas = GameObject.Find("CinemaCanvas");
        playerCanvas = GameObject.Find("CanvasPlayer");
    }

    private void Start()
    {
        originalPosition = transform.position;
        targetPos = originalPosition;
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        cinematicCanvas.SetActive(false);
    }

    private void Update()
    {
        if (moveGate)
        {
            MoveGate();
        }
    }
    void MoveGate()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) <= arriveThreshold)
        {
            transform.position = targetPos;
            moveGate = false;
            SoundFXManager.Instance.StopLoopFor(gameObject);
        }
        else
        {
            SoundFXManager.Instance.StartLoopFor(gameObject, SoundType.Chain, transform);
        }
    }

    void ActivateCamera()
    {
        cinematicCanvas.SetActive(true);
        playerCanvas.SetActive(false);
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
        cinematicCanvas.SetActive(false);
        playerCanvas.SetActive(true);
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
    void OnSwitchCountChanged()
    {
        if (switches == switchesNeeded)
        {
            if (!playedCutScene && cutSceneCamera != null)
            {
                if (playSound && !played)
                {
                    played = true;
                    SoundFXManager.Instance.PlaySoundFX(SoundType.PuzzleSolvedFully);
                }
                ActivateCamera();
                Invoke(nameof(DisableActiveCamera), cutSceneLength);
                playedCutScene = true;
            }

            targetPos = originalPosition - Vector3.up * pressDepth;
        }
        else
        {
            targetPos = originalPosition;
        }

        moveGate = true;
    }

    public void AddSwitch(int amount)
    {

        switches += amount;
        OnSwitchCountChanged();
    }

    public void RemoveSwitch(int amount)
    {
        switches -= amount;
        OnSwitchCountChanged();
    }
}
