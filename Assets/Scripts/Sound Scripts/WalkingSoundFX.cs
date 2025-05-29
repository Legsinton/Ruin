using UnityEngine;

public class WalkingSoundFX: MonoBehaviour
{
    public Animator animator;
    float lastFootStep;

    private void OnValidate()
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        var footstep = animator.GetFloat("Footstep");
        if (Mathf.Abs(footstep) < 0.0001f)
        {
            footstep = 0f;
        }
        if (lastFootStep > 0 && footstep < 0 || lastFootStep < 0 && footstep > 0)
        {
            Debug.Log("Im walking here!");
            SoundFXManager.Instance.PlaySoundFX(SoundType.Walk);
        }

        lastFootStep = footstep; 
    }

}
