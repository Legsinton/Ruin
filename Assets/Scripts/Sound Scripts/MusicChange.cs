using UnityEngine;

public class MusicChange : MonoBehaviour
{
    [SerializeField] string music;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!MusicManager.Instance.IsTrackPlaying(music))
            {
                MusicManager.Instance.PlayMusic(music);
            }
            else
            {
                Debug.Log("Song already playing");
            }
        }
    }
}
