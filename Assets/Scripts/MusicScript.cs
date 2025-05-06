using UnityEngine;

public class MusicScript : MonoBehaviour
{
    
    AudioSource musicSource;
    AudioSource scaryMusicSource;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        scaryMusicSource = GetComponent<AudioSource>();
        musicSource.enabled = true;
        scaryMusicSource.enabled = false;
    }

    private void Update()
    {
        EnableOtherSong();
        DisableOtherSong();
    }

    void EnableOtherSong()
    {
        musicSource.enabled = false;
        scaryMusicSource.enabled = true;
    }

    void DisableOtherSong()
    {
        musicSource.enabled = true; 
        scaryMusicSource.enabled = false;
    }
}
