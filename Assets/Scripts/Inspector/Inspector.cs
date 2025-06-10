using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


public class Inspector : MonoBehaviour
{
    [Header("Inspector Sensitivity")]
    [SerializeField] public float mouseSensitivity;
    [SerializeField] public float controllerSensitivity;
    [SerializeField] public GameObject equipUI;
    [SerializeField] public TMP_Text itemInfoText;
    [SerializeField] public GameObject playerCanvas;

    Canvas canvas;
    Camera camera;
    PostProcessManager postProcessManager;
    PlayerInput playerInput;
    GameObject itemPrefab;
    [HideInInspector] public InspectableItem inspectableItem;
    bool isRotating = false;
    bool isGamepad = false;
    float sensitivity;
    Vector2 currentRotation;
    int itemId;
    bool equipable;


    void Awake()
    {
        GameObject inspectorCanvas = GameObject.Find("CenteredInspectorCanvas");
        GameObject inspectorCamera = GameObject.Find("InspectorCamera");

        if (inspectorCanvas != null)
        {
            canvas = inspectorCanvas.GetComponent<Canvas>();
            canvas.enabled = false;
        }
        else
        {
            Debug.Log("Could not locate Canvas component on " + inspectorCanvas.name);
        }

        if (inspectorCamera != null)
        {
            camera = inspectorCamera.GetComponent<Camera>();
            camera.enabled = false;
        }
        else
        {
            Debug.Log("Could not locate Canvas component on " + inspectorCamera.name);
        }

        postProcessManager = FindFirstObjectByType<PostProcessManager>();
        playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("Player Input Component is null");
        }
    }

    private void Update()
    {
        sensitivity = isGamepad ? controllerSensitivity : mouseSensitivity;
        if (itemPrefab)
        {
            Renderer renderer = itemPrefab.GetComponentInChildren<Renderer>();
            Vector3 center = renderer.bounds.center;

            itemPrefab.transform.RotateAround(center, Vector3.up, -currentRotation.x * sensitivity * Time.deltaTime);
            itemPrefab.transform.RotateAround(center, Vector3.right, currentRotation.y * sensitivity * Time.deltaTime);
        }
    }

    public void InspectItem(ScriptableObject interactableObject)
    {
        canvas.enabled = true;
        camera.enabled = true;
        postProcessManager.ToggleDepthOfField();
        playerCanvas.SetActive(false);

        Item item = (Item)interactableObject;
        itemId = item.itemId;

        itemInfoText.text = item.itemInfo;

        if (item.equipable == true)
        {
            equipable = true;
            equipUI.SetActive(true);
        }
        else
        {
            // TODO: Hide UI for equip
            equipable = false;
            equipUI.SetActive(false);
        }


        if (item != null && item.prefab != null)
        {
            playerInput.SwitchCurrentActionMap("Inspector");
            itemPrefab = Instantiate(item.prefab, Vector3.zero, Quaternion.Euler(0, 90, 0));
            Renderer[] renderers = itemPrefab.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                Debug.LogWarning("No Renderer found in prefab: " + item.prefab.name);
            }
            else
            {
                Bounds bounds = renderers[0].bounds;
                foreach (Renderer r in renderers)
                {
                    bounds.Encapsulate(r.bounds);
                }

                Vector3 centerOffset = bounds.center - itemPrefab.transform.position;
                Vector3 desiredCenterPosition = new Vector3(1000, 1000, 1003);
                itemPrefab.transform.position = desiredCenterPosition - centerOffset;
            }

        }
        else
        {
            Debug.LogWarning("Cannot spawn: Missing component, data, or prefab.");
        }
    }

    // ACTION MAPS / INPUT HANDLING

    public void StopInspection()
    {

    }

    public void OnRotateStart()
    {
        isRotating = true;
    }

    public void OnRotateEnd()
    {
        isRotating = false;
    }

    public void OnDelta(InputValue value)
    {
        if (playerInput.currentControlScheme == "Gamepad")
        {
            isGamepad = true;
        }

        if ((!isGamepad && !isRotating) || itemPrefab == null) return;

        currentRotation = value.Get<Vector2>();

    }

    private void OnBack()
    {
        if (itemPrefab != null)
        {
            Destroy(itemPrefab);
        }
        canvas.enabled = false;
        camera.enabled = false;
        postProcessManager.ToggleDepthOfField();
        playerInput.SwitchCurrentActionMap("Player");
        playerCanvas.SetActive(true);
    }

    private void OnEquip()
    {
        if (!equipable)
        {
            return;
        }

        Inventory.Instance.AddItem(itemId);
        inspectableItem.DestroyOnEquip();
        OnBack();
    }
}
