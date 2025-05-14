using UnityEngine;

public class TriggerBlock : MonoBehaviour
{
    [SerializeField] float correctAngle;

    [SerializeField] RotatingObject rotatingObject;
    [SerializeField] GateScript[] gate;
    [SerializeField] MovingPlatform[] platforms;

    void Start()
    {
        
    }

    void checkIfObjectAligned()
    {
        if (correctAngle == rotatingObject.currentAngle)
        {
            for (int i = 0; i < gate.Length; i++)
            {
                gate[i].Switches++;
            }
            for (int i = 0; i < platforms.Length; i++)
            {
                platforms[i].Switches++;
            }
        }
        else
        {
            for (int i = 0; i < gate.Length; i++)
            {
                gate[i].Switches--;
            }
            for (int i = 0; i < platforms.Length; i++)
            {
                platforms[i].Switches--;
            }
        }
    }
}





