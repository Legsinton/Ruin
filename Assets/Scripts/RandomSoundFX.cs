using System.Threading;
using UnityEngine;

public class RandomSoundFX : MonoBehaviour
{
    [SerializeField] float timer;
    bool played;
    float random;

    private void Awake()
    {
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
                SoundFXManager.Instance.PlaySoundFX(SoundType.RandomScary, this.transform.position);
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
