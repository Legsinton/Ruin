using System.Threading;
using UnityEngine;

public class RandomSoundFX : MonoBehaviour
{
    PlayerMovement playerMovement;
    [SerializeField] float timer;
    bool played;
    float random;

    private void Awake()
    {
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        random = UnityEngine.Random.Range(60f, 100f);
    }

    private void Update()
    {
        timer += Time.deltaTime;
         
        if (timer >= random)
        {
            timer = 0;
            if (!played)
            {
                NewRandom();
                played = true;
                SoundFXManager.Instance.PlaySoundFX(SoundType.RandomScary, playerMovement.transform.position);
                Invoke(nameof(UnPlay), 0.4f);
            }
        }
    }

    void NewRandom()
    {
        random = UnityEngine.Random.Range(60f, 100f);
    }

    void UnPlay()
    {
        played = false;
    }
}
