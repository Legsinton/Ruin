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
        }
    }
}
