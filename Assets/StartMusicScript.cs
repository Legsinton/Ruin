using UnityEngine;

public class StartMusicScript : MonoBehaviour
{
    [SerializeField] string music;
    [SerializeField] float volume;
    void Start()
    {
        MusicManager.Instance.StartMusic(music,volume);
    }
}
