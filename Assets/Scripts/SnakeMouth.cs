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

    [Header("References")]
    PlayerMovement PlayerMovement;
    Interact interact;

    [Header("Settings For Cameras")]
    [SerializeField] float cutSceneLength;
    bool playedCutScene;

    [Header("Camera References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera cutSceneCamera;

    void Start()
    {
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(openAngle, 0,0 ));
        PlayerMovement = FindAnyObjectByType<PlayerMovement>();
        interact = FindAnyObjectByType<Interact>();
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
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        }
    }
    void ActivateCamera()
    {
        playerCamera.enabled = false;
        cutSceneCamera.enabled = true;
        interact.disableInteract = true;
        PlayerMovement.enabled = false;
        PlayerMovement.movement = new Vector3(0, 0, 0);
    }
    void DisableActiveCamera()
    {
        playerCamera.enabled = true;
        interact.disableInteract = false;
        cutSceneCamera.enabled = false;
        PlayerMovement.enabled = true;
        this.enabled = false;
    }
    public void AddSwitch(int amount)
    {
        if (playSound)
        {
            SoundFXManager.Instance.PlaySoundFX(SoundType.PuzzleSolvedFully);
        }
        switches += amount;
    }
    public void RemoveSwitch(int amount)
    {
        switches -= amount;
    }
}
