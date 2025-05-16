using System.Threading;
using UnityEngine;

public class RandomSoundFX : MonoBehaviour
{
    [SerializeField] float timer;
    [SerializeField] float smallVal;
    [SerializeField] float largeVal;

    bool played;
    float random;

    private void Awake()
    {
        random = UnityEngine.Random.Range(smallVal, largeVal);
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
                SoundFXManager.Instance.PlaySoundFX(SoundType.RandomScary, this.transform.position,4f);
                Invoke(nameof(UnPlay), 0.4f);
            }
        }
    }

    void NewRandom()
    {
        random = UnityEngine.Random.Range(smallVal, largeVal);
    }

    void UnPlay()
    {
        played = false;
    }
}
