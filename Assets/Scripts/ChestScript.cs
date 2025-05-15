using UnityEngine;
using UnityEngine.UIElements;

public class ChestScript : MonoBehaviour, IInteracting
{
    [Header("Settings")]
    [SerializeField] float openSpeed;
    [SerializeField] float openAngle;

    [Header("Reference")]
    [SerializeField] UIScript UIScript;
    [SerializeField] Outline outlineScript;
    [SerializeField] Collider colliderLid;

    bool isDoorOpen;
    bool spawned;
    bool played;
    bool playedCutScene;
    Quaternion openRotation;
    GameObject spawnedItem;

    [Header("Settings for Item")]

    public Transform spawnPoisition;
    public Transform spawnPoint; // Assign the SpawnPoint in Inspector
    public GameObject itemPrefab; // Assign your item prefab
    public float launchForce = 5f; // Tune for the desired "pop" effect

    [Header("Settings For Cameras")]
    [SerializeField] float cutSceneLength;

    [Header("Camera References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera cutSceneCamera;

    void Start()
    {
        UIScript = FindAnyObjectByType<UIScript>();
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, openAngle));
    }

    void Update()
    {
        if (isDoorOpen)
        {
            colliderLid.enabled = false;
            outlineScript.enabled = false;
            gameObject.layer = default;
            if (!played)
            {
                SoundFXManager.Instance.PlaySoundFX(0.6f, SoundType.ChestOpen, transform.position);
                SoundFXManager.Instance.PlaySoundFX(0.5f, SoundType.ChestCreak, transform.position);
                played = true;
            }
            OpenDoor();
            if (cutSceneCamera != null && !playedCutScene)
            {
                ActivateCamera();
                Invoke(nameof(DisableActiveCamera), cutSceneLength);
                playedCutScene = true;
            }
            if (!spawned)
            {
                spawned = true;
                SpawnItem();
            }

            MoveItem();
        }
    }

    public void SpawnItem()
    {
        spawnedItem = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
        spawnedItem.transform.localScale = Vector3.one; // Force it to correct scale
    }

    public void MoveItem()
    {
        if (spawnedItem != null)
        {
            if (spawnedItem.transform.position.y < spawnPoisition.position.y)
            {
                spawnedItem.transform.position += new Vector3(0, launchForce, 0) * Time.deltaTime;
            }
            else
            {
                
                this.enabled = false;
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
    }

    void DisableActiveCamera()
    {
        playerCamera.enabled = true;
        cutSceneCamera.enabled = false;
    }

    public void PressInteract()
    {
        if (!isDoorOpen)
        {
            isDoorOpen = true;
        }
    }

    public void ReleaseInteract() { }

    public void InteractInRange()
    {
        if (UIScript != null)
        {
            UIScript.EnableUI();
        }
        if (!isDoorOpen)
        {
            outlineScript.enabled = true;
        }
    }

    public void InteractNotInRange()
    {
        if (UIScript != null)
        {
            UIScript.DisebleUI();
        }
        outlineScript.enabled = false;
    }

}
