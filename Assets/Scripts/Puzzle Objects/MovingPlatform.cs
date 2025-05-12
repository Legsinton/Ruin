using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Switch Settings")]
    [SerializeField] int switches;
    public int Switches { get { return switches; } set { switches = value; } }
    public int switchAmount;

    Vector3 targetPosition;
    Vector3 originalPosition;
    bool played;
    bool playedCutScene;

    [Header("Settings To Move Platform")]

    [SerializeField] bool down;
    [SerializeField] bool sideZ;
    [SerializeField] bool sideX;
    public float pressDepth;
    public float moveSpeed;

    [Header("Settings For Cameras")]
    [SerializeField] float cutSceneLength;

    [Header("Camera References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera cutSceneCamera;

    private void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        MovementZ();
        MovementX();
        MovementUp();
    }

    void MovementUp()
    {
        if (down)
        {
            if (Switches == switchAmount)
            {
                if (!played)
                {
                    //PlaySoundFX();
                    played = true;
                    if (cutSceneCamera != null && !playedCutScene)
                    {
                        ActivateCamera();
                        Invoke(nameof(DisableActiveCamera), cutSceneLength);
                        playedCutScene = true;
                    }
                }

                targetPosition = originalPosition - Vector3.up * pressDepth;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            else if (Switches != switchAmount)
            {
                if (played)
                {
                    //PlaySoundFX();
                    played = false;
                }
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            }
        }
    }

    void MovementZ()
    {
        if (sideZ)
        {
            if (Switches == switchAmount)
            {
                if (!played)
                {
                    //PlaySoundFX();
                    if (cutSceneCamera != null && !playedCutScene)
                    {
                        ActivateCamera();
                        Invoke(nameof(DisableActiveCamera), cutSceneLength);
                        playedCutScene = true;
                    }
                }
                targetPosition = originalPosition - Vector3.forward * pressDepth;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            else if (Switches == switchAmount)
            {
                if (played)
                {
                    //PlaySoundFX();
                    played = false;
                }
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            }
        }
    }

    void MovementX()
    {
        if (sideX)
        {
            if (Switches == switchAmount)
            {
                if (!played)
                {
                    //PlaySoundFX();
                    played = true;
                    if (cutSceneCamera != null && !playedCutScene)
                    {
                        ActivateCamera();
                        Invoke(nameof(DisableActiveCamera), cutSceneLength);
                        playedCutScene = true;
                    }
                }
                targetPosition = originalPosition - Vector3.right * pressDepth;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }

            else if (Switches == switchAmount)
            {
                if (played)
                {
                    //PlaySoundFX();
                    played = false;
                }
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            }
        }
    }

    void ActivateCamera()
    {
        playerCamera.enabled = false;
        cutSceneCamera.enabled = true;
    }

    void DisableActiveCamera()
    {
        playerCamera.enabled = true;
        cutSceneCamera.enabled = false;
    }

    void PlaySoundFX()
    {
        SoundFXManager.Instance.PlaySoundFX(SoundType.Chain, transform.position);
    }
    public void AddSwitch()
    {
        switches++;
    }
}
