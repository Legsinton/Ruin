using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToLastKnownPositionAction", story: "[Target] moves to [LastKnownPosition]", category: "Action", id: "c886b4d88f977ea6313f4d8e8beaa912")]
public partial class MoveToLastKnownPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<Vector3> LastKnownPosition;
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;

    protected override Status OnUpdate()
    {
        if (LastKnownPosition == null || Agent == null)
            return Status.Failure;

        Agent.Value.SetDestination(LastKnownPosition.Value);

        if (!Agent.Value.pathPending && Agent.Value.remainingDistance <= Agent.Value.stoppingDistance)
        {
            if (!Agent.Value.hasPath || Agent.Value.velocity.sqrMagnitude < 0.01f)
                return Status.Success;
        }
        return Status.Running;
    }
}

