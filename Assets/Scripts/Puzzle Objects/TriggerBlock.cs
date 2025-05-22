using UnityEngine;

public class TriggerBlock : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float correctAngle;

    [Header("Clock Puzzel Settings")]
    [SerializeField] bool clockPuzzle;
    [SerializeField] float moveSpeed;
    [SerializeField] float arriveThreshold;
    [SerializeField] float angleThreshold;
    [SerializeField] float pressDepth;
    [SerializeField] RotatingObject extraRotatingObject;
    float rotatingObjectAngle;
    float extraRotatingObjectAngle;

    [Header("References")]
    [SerializeField] RotatingObject rotatingObject;
    [SerializeField] GateScript[] gate;
    [SerializeField] MovingPlatform[] platforms;

    bool isAligned;
    bool moveTriggerBlock;

    Vector3 originalPosition;
    Vector3 targetPos;

    void Start()
    {
        originalPosition = transform.position;
        targetPos = originalPosition;
    }

    void Update()
    {
        MoveTriggerBlock();
    }
    void MoveTriggerBlock()
    {
        if (moveTriggerBlock)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) <= arriveThreshold)
            {
                transform.position = targetPos;
                moveTriggerBlock = false;
                SoundFXManager.Instance.StopLoopFor(gameObject);
            }
            else
            {
                SoundFXManager.Instance.StartLoopFor(gameObject, SoundType.PressurePlate, transform);
            }            
        }
    }

    void OnEnable()
    {
        rotatingObject.UpdateTriggerBlocks += checkIfObjectAligned;

        if (clockPuzzle)
        {
            extraRotatingObject.UpdateTriggerBlocks += checkIfObjectAligned;
        }
    }

    void OnDisable()
    {
        rotatingObject.UpdateTriggerBlocks -= checkIfObjectAligned;
        
        if (clockPuzzle)
        {
            extraRotatingObject.UpdateTriggerBlocks -= checkIfObjectAligned;
        }
    }

    void checkIfObjectAligned(RotatingObject sender, float currentAngle)
    {
        if (sender == rotatingObject)
        {
            rotatingObjectAngle = currentAngle;
        }
        else if (sender == extraRotatingObject)
        {
            extraRotatingObjectAngle = currentAngle;
        }

        bool newAligned;

        if (clockPuzzle)
        {
            bool mainAligned = Mathf.Abs(Mathf.DeltaAngle(rotatingObjectAngle, correctAngle)) <= angleThreshold;
            bool extraAligned = Mathf.Abs(Mathf.DeltaAngle(extraRotatingObjectAngle, correctAngle)) <= angleThreshold;

            newAligned = mainAligned || extraAligned;
        }
        else
        {
            newAligned = Mathf.Abs(Mathf.DeltaAngle(rotatingObjectAngle, correctAngle)) <= angleThreshold;
        }

        if (newAligned != isAligned)
        {
            if (newAligned)
            {
                foreach (var g in gate)
                    g.AddSwitch(1);

                foreach (var p in platforms)
                    p.Switches++;

                if (clockPuzzle)
                {
                    moveTriggerBlock = true;
                    targetPos = originalPosition - Vector3.up * pressDepth;
                }
            }
            else
            {
                foreach (var g in gate)
                    g.RemoveSwitch(1);

                foreach (var p in platforms)
                    p.Switches--;

                if (clockPuzzle)
                {
                    moveTriggerBlock = true;
                    targetPos = originalPosition;
                }
            }

            isAligned = newAligned;
        }
    }
}