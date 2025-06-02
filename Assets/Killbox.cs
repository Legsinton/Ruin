using UnityEngine;

public class Killbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Destroyed" + other.gameObject.name);
        Destroy(other.gameObject);
    }
}
