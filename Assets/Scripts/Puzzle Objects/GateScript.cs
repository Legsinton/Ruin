using UnityEngine;

public class GateScript : MonoBehaviour
{
    [SerializeField] int switches;
    [SerializeField] Outline outlineScript;
    public int Switches { get { return switches; } set { switches = value; } }
    public int switchAmount;
    Vector3 targetPosition;
    Vector3 originalPosition;
    Vector3 previousPosition;
    float movementThreshold = 0.001f;
    public bool solved;
    public float pressDepth;
    public float moveSpeed;
    bool played;
    bool playedCutScene;

    [Header("Settings For Cameras")]
    [SerializeField] float cutSceneLength;

    [Header("Camera References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera cutSceneCamera;

    private void Start()
    {
        originalPosition = transform.position;
        previousPosition = transform.position;
    }

    void Update()
    {
        if (Switches == switchAmount)
        {
            if (!played)
            {
                PlaySoundFX();
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
                PlaySoundFX();
                played = false;
            }
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        }
        // Check movement
        Vector3 movement = transform.position - previousPosition;

        if (movement.magnitude > movementThreshold)
        {
            if (!played)
            {
                played = true;
                SoundFXManager.Instance.StartLoopFor(gameObject, SoundType.Chain, transform.position, this.transform);
            }
        }
        else
        {
            if (played)
            {
                played = false;
                SoundFXManager.Instance.StopLoopFor(gameObject);
            }
        }

        previousPosition = transform.position;
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
        SoundFXManager.Instance.PlaySoundFX(0.5f, SoundType.Chain, transform.position);
    }
    public void AddSwitch()
    {
        switches++;
    }
}
