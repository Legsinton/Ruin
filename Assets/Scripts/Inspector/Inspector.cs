using UnityEngine;
using UnityEngine.InputSystem;

public class Inspector : MonoBehaviour
{
    Canvas canvas;
    //InputSystem_Actions inspectInput;
    PlayerInput playerInput;
    GameObject itemPrefab;
    bool isRotating = false;


    void Awake()
    {
        GameObject inspectorCanvas = GameObject.Find("InspectorCanvas");
        var actions = GetComponent<PlayerInput>().actions;
        if (inspectorCanvas != null)
        {
            canvas = inspectorCanvas.GetComponent<Canvas>();
            canvas.enabled = false;
        }
        else
        {
            Debug.Log("Could not locate Canvas component on " + inspectorCanvas.name);
        }

        //inspectInput = new InputSystem_Actions();
        playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("Player Input Component is null");
        }
    }

    public void InspectItem(ScriptableObject interactableObject)
    {
        Debug.Log("Agnes: Started inspection");
        canvas.enabled = true;
        Item item = (Item)interactableObject;
        if (item != null && item.prefab != null)
        {
            itemPrefab = Instantiate(item.prefab, new Vector3(1000, 1000, 1003), Quaternion.identity);
            //InspectInputController.Instance.OnEnable();
            Debug.Log("Agnes: OnEquip in run");
            playerInput.SwitchCurrentActionMap("Inspector");

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
        // if (itemPrefab != null)
        // {
        //     Destroy(itemPrefab);
        // }
    }

    public void OnEnable()
    {
        Debug.Log("Agnes: OnEquip in run");
        playerInput.SwitchCurrentActionMap("Inspector");
    }
    public void OnDisable()
    {
        Debug.Log("Agnes: OnBack is run");
        playerInput.SwitchCurrentActionMap("Player");
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        Debug.Log("Agnes: OnRotate is run");

        if (context.started || context.performed)
        {
            isRotating = true;
        }

        if (context.canceled)
        {
            isRotating = false;
        }
    }


    public void OnDelta(InputValue cc)
    {
        if (!isRotating || itemPrefab == null) return;

        Vector2 delta = cc.Get<Vector2>();

        itemPrefab.transform.Rotate(Vector3.up, -delta.x * 0.1f, Space.World);
        itemPrefab.transform.Rotate(Vector3.right, delta.y * 0.1f, Space.World);
    }



    private void OnBack()
    {
        OnDisable();
    }
    private void OnEquip()
    {
        OnEnable();
    }
}
