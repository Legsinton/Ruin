using UnityEngine;

public class TriggerBlock : MonoBehaviour
{
    [SerializeField] float correctAngle;

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
                    gate[i].Switches++;

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
                    gate[i].Switches--;

                for (int i = 0; i < platforms.Length; i++)
                    platforms[i].Switches--;

                isAligned = false;
            }
        }
    }
}