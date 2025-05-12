using UnityEngine;

public class MusicChange : MonoBehaviour
{
    bool played;
    [SerializeField] string music;

    private void Start()
    {
        MusicManager.Instance.PlayMusic("Scary");

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!played)
            {
                played = true;
                MusicManager.Instance.PlayMusic(music);
            }
        }
    }
}
