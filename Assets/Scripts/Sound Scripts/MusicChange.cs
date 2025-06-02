using UnityEngine;

public class MusicChange : MonoBehaviour
{
    [SerializeField] string music;
    [SerializeField] float fadeDuration;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!MusicManager.Instance.IsTrackPlaying(music))
            {
                MusicManager.Instance.PlayMusic(music, fadeDuration);
            }
            else
            {
                Debug.Log("Song already playing");
            }
        }
    }
}
