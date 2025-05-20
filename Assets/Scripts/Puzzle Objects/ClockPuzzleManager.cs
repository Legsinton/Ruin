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
    [SerializeField] GateScript[] gate;
    [SerializeField] MovingPlatform[] platforms;

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
                foreach (var g in gate)
                    g.AddSwitch(1);

                foreach (var p in platforms)
                    p.Switches++;
            }
            else
            {
                foreach (var g in gate)
                    g.RemoveSwitch(1);

                foreach (var p in platforms)
                    p.Switches--;
            }

            isAligned = newAligned;
        }
    }
}


