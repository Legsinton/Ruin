using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "SearchForPlayerAction",
    story: "[Self] searches in multiple directions",
    category: "Action",
    id: "6e1912763089f6ca95fae18d0f5b47c0")]
public partial class SearchForPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float angleThreshold = 2f;
    [SerializeField] private float pauseDuration = 0.5f;

    private Vector3[] directions;
    private int currentDirectionIndex;
    private Quaternion targetRotation;
    private float pauseTimer;
    private bool isPausedAfterTurn;

    protected override Status OnStart()
    {
        currentDirectionIndex = 0;
        directions = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            float angle = UnityEngine.Random.Range(0f, 360f);
            directions[i] = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        }

        isPausedAfterTurn = false;
        pauseTimer = 0f;
        SetNextRotationTarget();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Self.Value == null)
            return Status.Failure;

        Transform selfTransform = Self.Value.transform;

        if (!isPausedAfterTurn)
        {
            selfTransform.rotation = Quaternion.RotateTowards(
                selfTransform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);

            float angle = Quaternion.Angle(selfTransform.rotation, targetRotation);
            if (angle <= angleThreshold)
            {
                isPausedAfterTurn = true;
                pauseTimer = 0f;
            }

            return Status.Running;
        }
        else
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseDuration)
            {
                currentDirectionIndex++;
                isPausedAfterTurn = false;

                if (currentDirectionIndex >= directions.Length)
                    return Status.Success;

                SetNextRotationTarget();
            }

            return Status.Running;
        }
    }

    private void SetNextRotationTarget()
    {
        if (Self.Value == null) return;
        Vector3 direction = directions[currentDirectionIndex];
        if (direction != Vector3.zero)
            targetRotation = Quaternion.LookRotation(direction, Vector3.up);

    }
}
