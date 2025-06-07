using UnityEngine;

public class ItemView : MonoBehaviour
{
    readonly float fishKeyId = 1;
    readonly float skullKeyId = 2;
    readonly float armId = 3;
    readonly float legId = 4;
    [SerializeField] GameObject arm, leg, fishKeyObject, skullKeyObject;
    public GameObject fishKey, skullKey, foot, hand;

    private void Awake()
    {
        hand.SetActive(false);
        foot.SetActive(false);
        skullKey.SetActive(false);
        fishKey.SetActive(false);
    }
    public void SymbolConfirm()
    {
        for (int i = 0; Inventory.Instance.inventoryItems.Count > i; i++)
        {
            Debug.Log("Im running here again");

            if (Inventory.Instance.inventoryItems[i].itemId == armId && !arm.activeInHierarchy)
            {
                hand.SetActive(true);
            }
            if (Inventory.Instance.inventoryItems[i].itemId == legId && !leg.activeInHierarchy)
            {
                foot.SetActive(true);
            }
            if (Inventory.Instance.inventoryItems[i].itemId == skullKeyId && !skullKeyObject.activeInHierarchy)
            {
                skullKey.SetActive(true);
            }
            if (Inventory.Instance.inventoryItems[i].itemId == fishKeyId && !fishKeyObject.activeInHierarchy)
            {
                fishKey.SetActive(true);
            }
        }
    }
}
