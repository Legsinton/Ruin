using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    [SerializeField] MusicLibrary library;
    public AudioSource musicSource;
    [SerializeField] float soundVolume;
    [SerializeField] AudioClip startSong;
    string currentTrackName;
    bool isFading = false;
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

    private void Start()
    {
        musicSource.clip = startSong;
        musicSource.Play();
        currentTrackName = startSong.name;
    }

    public void PlayMusic(string trackName, float fadeDuration = 20f)
    {
        if (!isFading)
        {
            StartCoroutine(AnimateMusicCrossFade(library.GetClipFromName(trackName), fadeDuration));
        }
    }

    IEnumerator AnimateMusicCrossFade(AudioClip nextTrack, float fadeDuration = 20f)
    {
        isFading = true;
        Debug.Log("fade start for song " + nextTrack.name);
        float percent = 0;
        while (percent < soundVolume)
        {
            Debug.Log("Implaying");
            percent += Time.deltaTime * 1/fadeDuration;
            musicSource.volume = Mathf.Lerp(soundVolume,0.05f,percent);
            yield return null;  
        }

        musicSource.clip = nextTrack;
        musicSource.Play();

        percent = 0;
        while (percent < soundVolume)
        {
            Debug.Log("ImNotplaying");

            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0.05f, soundVolume, percent);
            yield return null;
        }
        Debug.Log("fade complete for song " + nextTrack.name);
        currentTrackName = nextTrack.name;
        isFading = false;
    }

    public bool IsTrackPlaying(string trackName)
    {
        var boolean = musicSource.isPlaying && currentTrackName == trackName;
        Debug.Log("isTrackPlaying " + trackName + " " + boolean);
        return boolean;
    }
}
