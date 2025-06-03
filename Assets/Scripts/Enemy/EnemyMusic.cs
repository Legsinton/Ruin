using Unity.Behavior;
using UnityEngine;

public class EnemyMusic : MonoBehaviour
{
    [SerializeField]
    private GameObject agent;
    [SerializeField]
    private BlackboardVariable chaseSequence;
    public string previousSongName;
    AudioClip previousSong;
    bool played;

    private void Start()
    {
        BehaviorGraphAgent behaviorAgent = agent.GetComponent<BehaviorGraphAgent>();
        if (behaviorAgent == null)
        {
            return;
        }
        behaviorAgent.BlackboardReference.GetVariable("ChaseSequence", out chaseSequence);

    }

    void Update()
    {
        /*AudioClip*/
        previousSong = MusicManager.Instance.musicSource.clip;

        if (previousSong != null && previousSong.name != "Chase")
        {
            previousSongName = previousSong.name;
        }

        if (chaseSequence == null)
        {
            return;
        }

        bool playChaseMusic = (bool)chaseSequence.ObjectValue;

        if (playChaseMusic)
        {
            if (!played)
            {
                played = true;
                Debug.Log("Agnes: Should play Angry!");
                SoundFXManager.Instance.PlaySoundFX(SoundType.GoddessAngry, this.transform.position);
            }

            MusicManager.Instance.PlayMusic("Chase", 1);
        }

        if (!playChaseMusic && !string.IsNullOrEmpty(previousSongName))
        {
            played = false;

            MusicManager.Instance.PlayMusic(previousSongName, 1);
        }
        else
        {
            Debug.LogWarning("No previous song stored, cannot resume music.");
        }
    }
}