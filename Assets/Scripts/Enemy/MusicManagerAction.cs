using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Music Manager", story: "Update Music [Manager] and set [Bool] as true", category: "Action", id: "2cbc5a9f528c85b4307dd351ae364748")]
public partial class MusicManagerAction : Action
{
    [SerializeReference] public BlackboardVariable<MusicManager> Manager;
    [SerializeReference] public BlackboardVariable<bool> Bool;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        //Manager.Value.PlayMusic(Chase, 1);
        return Status.Success;
    }

    protected override void OnEnd()
    {

    }
}

