using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class Inspector : MonoBehaviour
{
    [Header("Inspector Sensitivity")]
    [SerializeField] public float mouseSensitivity;
    [SerializeField] public float controllerSensitivity;

    Canvas canvas;
    PostProcessManager postProcessManager;
    PlayerInput playerInput;
    GameObject itemPrefab;
    bool isRotating = false;
    bool isGamepad = false;
    float sensitivity;
    Vector2 currentRotation;


    void Awake()
    {
        GameObject inspectorCanvas = GameObject.Find("InspectorCanvas");

        if (inspectorCanvas != null)
        {
            canvas = inspectorCanvas.GetComponent<Canvas>();
            canvas.enabled = false;
        }
        else
        {
            Debug.Log("Could not locate Canvas component on " + inspectorCanvas.name);
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

        itemPrefab.transform.Rotate(Vector3.up, -currentRotation.x * sensitivity * Time.deltaTime, Space.World);
        itemPrefab.transform.Rotate(Vector3.right, currentRotation.y * sensitivity * Time.deltaTime, Space.World);
    }

    public void InspectItem(ScriptableObject interactableObject)
    {
        canvas.enabled = true;
        postProcessManager.ToggleDepthOfField();

        Item item = (Item)interactableObject;
        if (item != null && item.prefab != null)
        {
            playerInput.SwitchCurrentActionMap("Inspector");
            itemPrefab = Instantiate(item.prefab, new Vector3(1000, 1000, 1003), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Cannot spawn: Missing component, data, or prefab.");
        }
    }

    // ACTION MAPS / INPUT HANDLING
    public void StopInspection()
    {
        Debug.Log("Agnes: Stopped inspection");
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
        playerInput.SwitchCurrentActionMap("Player");
        if (itemPrefab != null)
        {
            Destroy(itemPrefab);
        }
        canvas.enabled = false;
        postProcessManager.ToggleDepthOfField();
    }

    private void OnEquip()
    {
        //TODO: Add equip logic
    }
}
