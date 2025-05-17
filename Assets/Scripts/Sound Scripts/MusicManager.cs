using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    [SerializeField] MusicLibrary library;
    [SerializeField] AudioSource musicSource;
    [SerializeField] float soundVolume;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayMusic(string trackName, float fadeDuration = 1f)
    {
        StartCoroutine(AnimateMusicCrossFade(library.GetClipFromName(trackName), fadeDuration));
    }

    IEnumerator AnimateMusicCrossFade(AudioClip nextTrack, float fadeDuration = 1f)
    {
        float percent = 0;
        while (percent < soundVolume)
        {
            percent += Time.deltaTime * 1/fadeDuration;
            musicSource.volume = Mathf.Lerp(1f,0,percent);
            yield return null;  
        }

        musicSource.clip = nextTrack;
        musicSource.Play();

        percent = 0;
        while (percent < soundVolume)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, 1f, percent);
            yield return null;
        }

    }
}
