using Unity.VisualScripting;
using UnityEngine;

public class Inspector : MonoBehaviour
{
    // [SerializeField] private Inventory inventory;
    public Item item;
    public GameObject itemPrefab;

    public void InspectItem()
    {
        Debug.Log("Agnes: Started inspection");
        itemPrefab = Instantiate(item.prefab, new Vector3(1000, 1000, 1000), Quaternion.identity);
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
