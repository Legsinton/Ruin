using UnityEngine;
using System.Collections;

public class ChaseTarget : MonoBehaviour
{
    [SerializeField]
    private RangeDetector RangeDetector;
    [SerializeField]
    private GameObject temporaryTarget;
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField, Range(0, 1)]
    private float m_volume = 0.8f;
    public void PerformHit()
    {
        Debug.Log("Performing hit");
        GameObject target = null;
        if (RangeDetector.DetectedTarget == null)
        {
            target = temporaryTarget;
        }
        else
        {
            target = RangeDetector.DetectedTarget.gameObject;
        }
        ChaseHandler handler = target.GetComponent<ChaseHandler>();
        if (handler != null)
        {
            handler.GetHit(gameObject);
        }
        StopAllCoroutines();
    }


}