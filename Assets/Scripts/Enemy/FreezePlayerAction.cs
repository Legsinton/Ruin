using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FreezePlayer", story: "[DetectedTarget] loses its movement", category: "Action", id: "b11eec29a5caf4ebec68035ea073aaf6")]
public partial class FreezePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> DetectedTarget;
    public float freezeDuration = 5.0f;

    private Rigidbody targetRb;
    private float timer;

    protected override Status OnStart()
    {
        GameObject target = DetectedTarget?.Value;

        Debug.Log("Target: " + target);

        if (target != null)
        {
            Debug.Log("Target isn't null");
            // targetRb = target.GetComponent<Rigidbody>();
            targetRb = target.GetComponentInParent<Rigidbody>();

            if (targetRb != null)
            {
                Debug.Log("RigidBody is found");
                targetRb.isKinematic = true;
                Debug.Log("Player is frozen!");
            }
            else
            {
                Debug.Log("Can't find rigidbody!");
            }
        }

        timer = 0f;
        return Status.Running;
    }

    // protected override void OnEnd()
    // {
    //     if (targetRb != null)
    //     {
    //         targetRb.isKinematic = false; // Unfreeze
    //     }
    // }

    protected override Status OnUpdate()
    {
        timer += Time.deltaTime;
        return (timer >= freezeDuration) ? Status.Success : Status.Running;
    }
}


















//     [SerializeReference] public BlackboardVariable<GameObject> Target;
//     public GameObject player;
//     public float freezeDuration = 1.0f;
//     private Rigidbody playerRb;
//     private float timer;

//     private void Awake()
//     {

//     }

//     protected override Status OnStart()
//     {
//         if (player != null)
//         {
//             playerRb = player.GetComponent<Rigidbody>();
//             if (playerRb != null)
//             {
//                 playerRb.isKinematic = true;
//             }
//         }

//         timer = 0f;

//         return Status.Running;
//     }

//     protected override Status OnUpdate()
//     {
//         timer += Time.deltaTime;

//         if (timer >= freezeDuration)
//         {
//             return Status.Success;

//         }

//         return Status.Running;
//     }

//     protected override void OnEnd()
//     {
//         if (playerRb != null)
//         {
//             playerRb.isKinematic = false;
//         }
//     }
// }