using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "SearchForPlayerAction",
    story: "[Self] searches for [Target]",
    category: "Action",
    id: "6e1912763089f6ca95fae18d0f5b47c0")]
public partial class SearchForPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Duration;
    private float timer;

    protected override Status OnStart()
    {
        timer = 0f;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        timer += Time.deltaTime;

        // TODO: Add rotate while searching

        Debug.Log("Agnes: The enemy now searches for the player");

        return timer >= Duration.Value ? Status.Success : Status.Running;
    }
}
