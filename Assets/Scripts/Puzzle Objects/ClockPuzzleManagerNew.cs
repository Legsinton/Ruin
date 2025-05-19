using System.Collections.Generic;
using UnityEngine;

public class ClockPuzzleManagerNew : MonoBehaviour
{

    [SerializeField] GateScript gate;
    [SerializeField] MovingPlatform plat;

    bool Solved;
    [SerializeField] Transform rotatingObjectBigTransform;
    [SerializeField] Transform rotatingObjectSmallTransform;
    [SerializeField] float bigMin, bigMax;
    [SerializeField] float smallMin, smallMax;

    private void Update()
    {
        float bigY = rotatingObjectBigTransform.eulerAngles.y;
        float smallY = rotatingObjectSmallTransform.eulerAngles.y;

        if (bigY >= bigMin && bigY <= bigMax && smallY >= smallMin && smallY <= smallMax)
        {
            if (!Solved)
            {
                PuzzleSolved();
            }
        }
        else
        {
            if (Solved)
            {
                PuzzleUnsolved();
            }
        }
    }
    private void PuzzleSolved()
    {
        Solved = true;
        if (gate != null)
        {
            gate.AddSwitch(1);

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
                gate.switches--;

            }
            if (plat != null)
            {
                plat.Switches--;
            }

            Solved = false;
        }
    }
}


