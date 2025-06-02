using UnityEngine;

public class Killbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            Debug.Log("Destroyed" + other.gameObject.name);
            Destroy(other.gameObject);
            SoundFXManager.Instance.PlaySoundFX(SoundType.Break, this.transform.position,5,500);
        }
    }
}
