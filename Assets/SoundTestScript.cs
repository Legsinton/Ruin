using UnityEngine;

public class SoundTestScript : MonoBehaviour
{
    bool started;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!started)
        {
            started = true;
            SoundFXManager.Instance.Start3DLoop(SoundType.Coin, transform.position);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        SoundFXManager.Instance.StopLoop();
        started = false;
    }
}
