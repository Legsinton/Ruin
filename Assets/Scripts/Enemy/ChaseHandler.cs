using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class ChaseHandler : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float rotationDuration = 0.5f; // Duration of rotation in seconds

    [SerializeField]
    private PlayerControls playerControls;
    [SerializeField, Range(0, 1)]
    private float m_volume = 0.8f;

    [SerializeField]
    private ParticleSystem m_impactParticles;

    public void GetHit(GameObject sender)
    {
        Debug.Log("Enemy hits player!");
        if (animator != null)
        {
            // Start rotation
            StopAllCoroutines();
            StartCoroutine(StartRotation(sender));
            animator.SetTrigger("GetHit");
        }

    }

    private IEnumerator StartRotation(GameObject sender)
    {
        // Store the starting rotation
        Quaternion m_startRotation = transform.rotation;
        // Calculate direction to this enemy (the attacker)
        Vector3 directionToEnemy = (sender.transform.position - transform.position).normalized;
        directionToEnemy.y = 0; // Keep rotation only in XZ plane
                                // Calculate target rotation
        Quaternion m_targetRotation = Quaternion.LookRotation(directionToEnemy);
        float m_rotationTime = 0;
        //Perform rotation
        while (m_rotationTime < rotationDuration)
        {
            m_rotationTime += Time.deltaTime;
            float progress = m_rotationTime / rotationDuration;
            transform.rotation = Quaternion.Lerp(m_startRotation, m_targetRotation, progress);
            yield return null;
        }
        // Ensure we end at exact target rotation
        transform.rotation = m_targetRotation;
    }
}