using UnityEngine;

public class ClockPuzzleManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float correctAngleMinute;
    [SerializeField] float correctAngleHour;
    [SerializeField] float angleThreshold;

    [Header("References")]
    [SerializeField] RotatingObject minuteRotatingObject;
    [SerializeField] RotatingObject hourRotatingObject;
    public MonoBehaviour[] switchTargets;


    bool isAligned;
    float minuteRotatingAngle;
    float hourRotatingAngle;

    void OnEnable()
    {
        minuteRotatingObject.UpdateTriggerBlocks += checkIfObjectAligned;

        hourRotatingObject.UpdateTriggerBlocks += checkIfObjectAligned;
    }

    void OnDisable()
    {
        minuteRotatingObject.UpdateTriggerBlocks -= checkIfObjectAligned;

        hourRotatingObject.UpdateTriggerBlocks -= checkIfObjectAligned;
    }

    void checkIfObjectAligned(RotatingObject sender, float currentAngle)
    {
        if (sender == minuteRotatingObject)
        {
            minuteRotatingAngle = currentAngle;
        }
        else if (sender == hourRotatingObject)
        {
            hourRotatingAngle = currentAngle;
        }

        bool newAligned;

        bool minuteAligned = Mathf.Abs(Mathf.DeltaAngle(minuteRotatingAngle, correctAngleMinute)) <= angleThreshold;
        bool hourAligned = Mathf.Abs(Mathf.DeltaAngle(hourRotatingAngle, correctAngleHour)) <= angleThreshold;

        newAligned = minuteAligned && hourAligned;

        if (newAligned != isAligned)
        {
            if (newAligned)
            {
                foreach (MonoBehaviour target in switchTargets)
                {
                    if (target is ISwitchManager switchable)
                    {
                        switchable.AddSwitch(1);  // or RemoveSwitch(1)
                    }
                }
            }
            else
            {
                foreach (MonoBehaviour target in switchTargets)
                {
                    if (target is ISwitchManager switchable)
                    {
                        switchable.RemoveSwitch(1);  // or RemoveSwitch(1)
                    }
                }
            }

            isAligned = newAligned;
        }
    }
}


