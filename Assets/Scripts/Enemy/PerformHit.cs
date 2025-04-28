using Unity.Behavior;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PerformHit : MonoBehaviour
{
    [SerializeField]
    private GameObject agent;
    [SerializeField]
    private BlackboardVariable isHit;
    private SceneManagement sceneManagement;

    private void Start()
    {
        sceneManagement = FindFirstObjectByType<SceneManagement>();

        BehaviorGraphAgent behaviorAgent = agent.GetComponent<BehaviorGraphAgent>();
        if (behaviorAgent == null)
        {
            return;
        }
        behaviorAgent.BlackboardReference.GetVariable("IsHit", out isHit);
    }
    void Update()
    {
        if (isHit == null)
        {
            return;
        }

        bool targetDetected = (bool)isHit.ObjectValue;

        if (targetDetected)
        {
            Debug.Log("Target killed!");
            sceneManagement.OnDeath();
        }
    }
}
