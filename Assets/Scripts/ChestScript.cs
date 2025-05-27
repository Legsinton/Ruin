using UnityEngine;

public class ChestScript : MonoBehaviour, IInteracting
{
    [Header("Settings")]
    [SerializeField] float openSpeed;
    [SerializeField] float openAngle;

    [Header("Reference")]
    [SerializeField] PlayerUI playerUI;
    [SerializeField] Outline outlineScript;
    [SerializeField] Collider colliderLid;
    PlayerMovement PlayerMovement;
    Interact interact;

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
    [SerializeField] float spinSpeed;

    [Header("Settings For Cameras")]
    [SerializeField] float cutSceneLength;

    [Header("Camera References")]
    [SerializeField] Camera playerCamera;
    [SerializeField] Camera cutSceneCamera;

    void Start()
    {
        playerUI = FindAnyObjectByType<PlayerUI>();
        PlayerMovement = FindAnyObjectByType<PlayerMovement>();
        interact = FindAnyObjectByType<Interact>();
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, openAngle));
    }
    void Update()
    {
        if (isDoorOpen)
        {
            colliderLid.enabled = false;
            outlineScript.enabled = false;
            if (!played)
            {
                SoundFXManager.Instance.PlaySoundFX(SoundType.ChestOpen, transform.position);
                SoundFXManager.Instance.PlaySoundFX(SoundType.ChestCreak, transform.position);
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
        if (spawnedItem != null)
        {
            SpinKey();
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
        }
    }

    void SpinKey()
    {
        if (spawnedItem != null)
        {
            spawnedItem.transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
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
        //this.enabled = false;
    }

    public void PressInteract()
    {
        if (!isDoorOpen)
        {
            isDoorOpen = true;
            gameObject.layer = default;
        }
    }

    public void ReleaseInteract() { }

    public void InteractInRange()
    {
        if (!isDoorOpen)
        {
            outlineScript.enabled = true;
        }
    }

    public void InteractNotInRange()
    {
        outlineScript.enabled = false;
    }

    public bool shouldObjectBeDestroyed()
    {
        return true;
    }
}
