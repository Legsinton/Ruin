using UnityEngine;

public class GateScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int switchesNeeded;
    [SerializeField] float pressDepth;
    [SerializeField] float moveSpeed;
    [SerializeField] float arriveThreshold;

    public int switches;
    bool playedCutScene;
    bool moveGate;
    Vector3 originalPosition;
    Vector3 targetPos;

    [Header("Settings For Cameras")]
    [SerializeField] float cutSceneLength;

    [Header("References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera cutSceneCamera;
    [SerializeField] Outline outlineScript;

    private void Start()
    {
        originalPosition = transform.position;
        targetPos = originalPosition;
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
        if (playerCamera != null) playerCamera.enabled = false;
        if (cutSceneCamera != null) cutSceneCamera.enabled = true;
    }

    void DisableActiveCamera()
    {
        if (playerCamera != null) playerCamera.enabled = true;
        if (cutSceneCamera != null) cutSceneCamera.enabled = false;
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

    void OnSwitchCountChanged()
    {
        if (switches == switchesNeeded)
        {
            if (!playedCutScene && cutSceneCamera != null)
            {
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
}
