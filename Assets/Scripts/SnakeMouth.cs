using UnityEngine;

public class SnakeMouth : MonoBehaviour, ISwitchManager
{
    [Header("Settings")]
    [SerializeField] float openSpeed;
    [SerializeField] float openAngle;
    Quaternion openRotation;

    [Header("Settings for switches")]
    [SerializeField] int switches;
    [SerializeField] int switchesNeeded;
    [SerializeField] bool playSound;
    bool played;

    [Header("References")]
    PlayerMovement playerMovement;
    Interact interact;
    bool playedSound;
    GameObject cinematicCanvas;
    GameObject playerCanvas;


    [Header("Settings For Cameras")]
    [SerializeField] float cutSceneLength;
    bool playedCutScene;

    [Header("Camera References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera cutSceneCamera;
    [SerializeField] AudioListener playrtAudioListener;
    [SerializeField] AudioListener cameraAudioListener;

    private void Awake()
    {
        cinematicCanvas = GameObject.Find("CinemaCanvas");
        playerCanvas = GameObject.Find("CanvasPlayer");
    }

    void Start()
    {
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(openAngle, 0, 0));
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        interact = FindAnyObjectByType<Interact>();
        cinematicCanvas.SetActive(false);
    }
    void Update()
    {
        if (switches == switchesNeeded)
        {
            OpenDoor();
            if (cutSceneCamera != null && !playedCutScene)
            {
                ActivateCamera();
                Invoke(nameof(DisableActiveCamera), cutSceneLength);
                playedCutScene = true;
            }
        }
    }
    void OpenDoor()
    {
        if (Quaternion.Angle(transform.rotation, openRotation) > 0.5f)
        {
            if (!playedSound)
            {
                SoundFXManager.Instance.StartLoopFor(gameObject, SoundType.PushBlock, this.transform);
                playedSound = true;
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        }
        else
        {
            SoundFXManager.Instance.StopLoopFor(gameObject);
        }
    }
    void ActivateCamera()
    {
        cinematicCanvas.SetActive(true);
        playerCanvas.SetActive(false);
        playerCamera.enabled = false;
        cutSceneCamera.enabled = true;
        interact.disableInteract = true;
        playrtAudioListener.enabled = false;
        cameraAudioListener.enabled = true;
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
        SoundFXManager.Instance.StopLoopFor(gameObject);
        playrtAudioListener.enabled = true;
        cameraAudioListener.enabled = false;
        cutSceneCamera.enabled = false;
        playerCamera.enabled = true;
        interact.disableInteract = false;
        this.enabled = false;
        if (playerMovement != null)
        {
            playerMovement.cutscene = false;
        }
    }
    public void AddSwitch(int amount)
    {
        if (playSound && !played)
        {
            played = true;
            SoundFXManager.Instance.PlaySoundFX(SoundType.PuzzleSolvedFully);
        }
        switches += amount;
    }
    public void RemoveSwitch(int amount)
    {
        switches -= amount;
    }
}
