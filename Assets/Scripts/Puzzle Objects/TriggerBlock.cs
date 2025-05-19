using UnityEngine;

public class TriggerBlock : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float correctAngle;

    [Header("References")]
    [SerializeField] RotatingObject rotatingObject;
    [SerializeField] GateScript[] gate;
    [SerializeField] MovingPlatform[] platforms;

    bool isAligned;

    void OnEnable()
    {
        rotatingObject.UpdateTriggerBlocks += checkIfObjectAligned;
    }

    void OnDisable()
    {
        rotatingObject.UpdateTriggerBlocks -= checkIfObjectAligned;
    }

    void checkIfObjectAligned(float currentAngle)
    {
        if (Mathf.Round(currentAngle) == correctAngle)
        {
            if (!isAligned)
            {
                for (int i = 0; i < gate.Length; i++)
                    gate[i].AddSwitch(1);

                for (int i = 0; i < platforms.Length; i++)
                    platforms[i].Switches++;

                isAligned = true;
            }
        }
        else
        {
            if (isAligned)
            {
                for (int i = 0; i < gate.Length; i++)
                    gate[i].RemoveSwitch(1);

                for (int i = 0; i < platforms.Length; i++)
                    platforms[i].Switches--;

                isAligned = false;
            }
        }
    }
}