using System.Collections.Generic;
using UnityEngine;

public class Inspector : MonoBehaviour
{
    Canvas canvas;

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
    }

    public void InspectItem(ScriptableObject interactableObject)
    {
        Debug.Log("Agnes: Started inspection");
        canvas.enabled = true;
        Item item = (Item)interactableObject;
        if (item != null && item.prefab != null)
        {
            Instantiate(item.prefab, new Vector3(1000, 1000, 1003), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Cannot spawn: Missing component, data, or prefab.");
        }
    }

    public void StopInspection()
    {
        Debug.Log("Agnes: Stopped inspection");
        // if (itemPrefab != null)
        // {
        //     Destroy(itemPrefab);
        // }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
