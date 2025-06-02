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

    public void PlayMusic(string trackName, float fadeDuration = 1f)
    {
        if (!isFading)
        {
            StartCoroutine(AnimateMusicCrossFade(library.GetClipFromName(trackName), fadeDuration));
        }
    }

    IEnumerator AnimateMusicCrossFade(AudioClip nextTrack, float fadeDuration = 2f)
    {
        isFading = true;
        Debug.Log("fade start for song " + nextTrack.name);
        float time = 0f;
        while (time < fadeDuration)
        {
            Debug.Log("Implaying");
            //percent += Time.deltaTime * 1/fadeDuration;
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(soundVolume,0.05f,time/fadeDuration);
            yield return null;  
        }

        musicSource.clip = nextTrack;
        musicSource.Play();

        time = 0f;
        while (time < fadeDuration)
        {
            Debug.Log("ImNotplaying");

            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0.05f, soundVolume, time/fadeDuration);
            yield return null;
        }
        Debug.Log("fade complete for song " + nextTrack.name);
        musicSource.volume = soundVolume; // ensure it's exactly at the target volume
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
