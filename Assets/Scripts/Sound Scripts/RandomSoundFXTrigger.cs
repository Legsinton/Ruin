using UnityEngine;

public class RandomSoundFXTrigger : MonoBehaviour
{
    bool played;

    private void OnTriggerEnter(Collider other)
    {
        if (!played)
        {
            played = true;
            SoundFXManager.Instance.PlaySoundFX(SoundType.RandomScary);
            Invoke(nameof(PlaySound), 1);
        }
    }

    void PlaySound()
    {
        SoundFXManager.Instance.PlaySoundFX(SoundType.Goddess);

    }
}
