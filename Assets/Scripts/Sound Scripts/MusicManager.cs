using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    [SerializeField] MusicLibrary library;
    public AudioSource musicSource;
    [SerializeField] float soundVolume;
    string currentTrackName;
    bool isFading = false;
    readonly string Chase;
    readonly string Scary;


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

    public void StopMusic(float fadeDuration)
    {
        StartCoroutine(AnimateMusicStopCrossFade(fadeDuration));
    }

    public void StartMusic(string track,float volume)
    {
        musicSource.clip = library.GetClipFromName(track);
        musicSource.volume = volume;
        musicSource.Play();
    }

    public void PlayMusic(string trackName, float fadeDuration)
    {    
        if (isFading && currentTrackName == Scary)
        {
            StartCoroutine(AnimateMusicCrossFade(library.GetClipFromName(trackName), fadeDuration));

        }
        else if (isFading && currentTrackName == Chase)
        {
            StartCoroutine(AnimateMusicCrossFade(library.GetClipFromName(trackName), fadeDuration));

        }
        else if (!isFading)
        {
            StartCoroutine(AnimateMusicCrossFade(library.GetClipFromName(trackName), fadeDuration));
        }
    }

    IEnumerator AnimateMusicCrossFade(AudioClip nextTrack, float fadeDuration)
    {
        isFading = true;
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(soundVolume,0.05f,time/fadeDuration);
            yield return null;  
        }

        musicSource.clip = nextTrack;
        musicSource.Play();

        time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0.05f, soundVolume, time/fadeDuration);
            yield return null;
        }
        musicSource.volume = soundVolume; // ensure it's exactly at the target volume
        currentTrackName = nextTrack.name;
        isFading = false;
    }

    IEnumerator AnimateMusicStopCrossFade(float fadeDuration)
    {
        isFading = true;
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(soundVolume, 0, time / fadeDuration);
            yield return null;
        }

        musicSource.Stop();

        
        isFading = false;
    }

    public bool IsTrackPlaying(string trackName)
    {
        var boolean = musicSource.isPlaying && currentTrackName == trackName;
        Debug.Log("isTrackPlaying " + trackName + " " + boolean);
        return boolean;
    }
}
