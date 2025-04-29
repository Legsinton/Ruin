using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Line of Sight Check", story: "Check [Target] with Line of Sight [Detector]", category: "Conditions", id: "b6b883e803219449098ac7bf3dbf47ef")]
public partial class LineOfSightCheckCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<LineOfSightDetector> Detector;

    public override bool IsTrue()
    {
        var detected = Detector.Value.PerformDetection(Target.Value);
        if (detected != null)
        {
            Blackboard.ReferenceEquals("LastKnownPosition", detected.transform.position);
            Blackboard.ReferenceEquals("MemoryTimer", Detector.Value); // Example usage
            return true;
        }

        return false;
    }


    // OLD LOGIC
    // public override bool IsTrue()
    // {
    //     return Detector.Value.PerformDetection(Target.Value) != null;
    // }
}
