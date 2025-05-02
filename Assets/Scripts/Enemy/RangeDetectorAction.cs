using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Range Detector",
    story: "Update Range Detector and assign Target + Last Known Position",
    category: "Action",
    id: "07ff086897f5ea1a515792e7dd58ff1c")]
public partial class RangeDetectorAction : Action
{
    [SerializeReference] public BlackboardVariable<RangeDetector> Detector;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownPosition;
    [SerializeReference] public BlackboardVariable<bool> HasNewLastKnownPosition;


    // Internal memory to detect transition
    private GameObject previousTarget;
    private bool wasTargetSeenLastFrame;

    protected override Status OnUpdate()
    {
        GameObject currentTarget = Detector.Value.UpdateDetector();
        Target.Value = currentTarget;

        if (previousTarget != null && currentTarget == null)
        {
            LastKnownPosition.Value = previousTarget.transform.position;
            Debug.Log("Agnes: Enemy lost player, last known position set to: " + LastKnownPosition.Value);
        }

        previousTarget = currentTarget;

        return currentTarget != null ? Status.Success : Status.Failure;
    }
}