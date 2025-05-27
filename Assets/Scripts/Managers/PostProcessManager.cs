using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessManager : MonoBehaviour
{
    public PostProcessVolume volume;
    private DepthOfField depthOfField = null;

    void Start()
    {
        volume.profile.TryGetSettings(out depthOfField);
    }

    public void ToggleDepthOfField()
    {
        depthOfField.enabled.value = !depthOfField.enabled.value;
    }
}
