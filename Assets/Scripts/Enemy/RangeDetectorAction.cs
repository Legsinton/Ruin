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
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownPosition;

    // Internal memory to detect transition
    private GameObject previousTarget;

    protected override Status OnUpdate()
    {
        GameObject currentTarget = Detector.Value.UpdateDetector();
        Target.Value = currentTarget;

        // Transition: target lost
        if (previousTarget != null && currentTarget == null)
        {
            if (previousTarget != null)
            {
                LastKnownPosition.Value = previousTarget.transform.position;
            }
        }

        previousTarget = currentTarget;

        return currentTarget != null ? Status.Success : Status.Failure;
    }
}
