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

    bool isDoorOpen;
    Quaternion closedRotation;
    Quaternion openRotation;
    bool spawned;
    GameObject spawnedItem;

    public Transform spawnPoisition;
    public Transform spawnPoint; // Assign the SpawnPoint in Inspector
    public GameObject itemPrefab; // Assign your item prefab
    public float launchForce = 5f; // Tune for the desired "pop" effect

    void Start()
    {
        UIScript = FindAnyObjectByType<UIScript>();
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(openAngle, 0, 0));
    }

    void Update()
    {
        if (isDoorOpen)
        {
            outlineScript.enabled = false;
            gameObject.layer = default;
            OpenDoor();
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
