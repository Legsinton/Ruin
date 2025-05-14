using System.Collections.Generic;
using UnityEngine;

public class ClockPuzzleManagerNew : MonoBehaviour
{

    [SerializeField] GateScript gate;
    [SerializeField] MovingPlatform plat;

    bool Solved;
    [SerializeField] Transform rotatingObjectBigTransform;
    [SerializeField] Transform rotatingObjectSmallTransform;
    [SerializeField] Quaternion Big;
    [SerializeField] Quaternion Small;
    [SerializeField] float smallValueLittle;
    [SerializeField] float bigValueLittle;
    [SerializeField] float smallValueBig;
    [SerializeField] float bigValueBig;

    private void Update()
    {
        Big = rotatingObjectBigTransform.rotation;
        Small = rotatingObjectSmallTransform.rotation;

        if (Small.y < smallValueLittle && Small.y > bigValueLittle && Big.y < smallValueBig && Big.y > bigValueBig)
        {
            PuzzleSolved();
        }
        else
        {
            PuzzleUnsolved();
        }
    }

    private void PuzzleSolved()
    {
     
        Solved = true;
        if (gate != null)
        {
            gate.AddSwitch();

        }
        if (plat != null)
        {
            plat.AddSwitch();
        }
    }

    private void PuzzleUnsolved()
    {
        if (Solved)
        {
            if (gate != null)
            {
                gate.Switches--;

            }
            if (plat != null)
            {
                plat.Switches--;
            }

            Solved = false;
        }
    }
}


