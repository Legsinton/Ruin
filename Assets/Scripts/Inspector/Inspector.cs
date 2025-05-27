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

        if (inspectorCanvas != null)
        {
            canvas = inspectorCanvas.GetComponent<Canvas>();
            canvas.enabled = false;
        }
        else
        {
            Debug.Log("Could not locate Canvas component on " + inspectorCanvas.name);
        }

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
            playerInput.SwitchCurrentActionMap("Inspector");
            itemPrefab = Instantiate(item.prefab, new Vector3(1000, 1000, 1003), Quaternion.identity);
            Debug.Log("Agnes: OnEquip in run");

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

    public void OnRotate()
    {
        Debug.Log("Agnes: OnRotate is run");

        isRotating = true;
    }

    public void OnDelta(InputValue cc)
    {
        Debug.Log("Agnes: OnDelta is run");

        //if (!isRotating || itemPrefab == null) return;

        Vector2 delta = cc.Get<Vector2>();

        itemPrefab.transform.Rotate(Vector3.up, -delta.x * 0.2f, Space.World);
        itemPrefab.transform.Rotate(Vector3.right, delta.y * 0.2f, Space.World);
    }



    private void OnBack()
    {
        playerInput.SwitchCurrentActionMap("Player");
        if (itemPrefab != null)
        {
            Destroy(itemPrefab);
        }
        canvas.enabled = false;
    }

    private void OnEquip()
    {
        //TODO: Add equip logic
    }
}
