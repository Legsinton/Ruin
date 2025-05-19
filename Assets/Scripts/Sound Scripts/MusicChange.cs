using UnityEngine;

public class MusicChange : MonoBehaviour
{
    bool played;
    public bool PlayingCalm { get { return played; } set { played = value; } }
    bool playedScary;
    public bool PlayingScary { get { return playedScary; } set { playedScary = value; } }
    [SerializeField] string music;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(music);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!PlayingScary)
            {
                PlayingScary = true;
                PlayingCalm = false;
                MusicManager.Instance.PlayMusic("Calm");
            }
            else if (!PlayingCalm)
            {
                PlayingCalm = true;
                PlayingScary = false;
                MusicManager.Instance.PlayMusic("Scary");
            }
        }
    }
}
