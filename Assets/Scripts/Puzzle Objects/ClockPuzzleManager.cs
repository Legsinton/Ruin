using System.Collections.Generic;
using UnityEngine;

public class ClockPuzzleManager : MonoBehaviour
{
    [SerializeField] List<ClockPuzzle> buttons;
    [SerializeField] List<int> correctSequence;

    [SerializeField] GateScript gate;
    [SerializeField] MovingPlatform plat;

    bool Solved;

    List<int> playerInput = new List<int>();

    public void RegisterButtonPress(int buttonID)
    {

        playerInput.Add(buttonID);

        if (playerInput.Count == correctSequence.Count)
        {
            PuzzleSolved();
        }
    }

    public void UnRegisterButtonPress(int buttonID)
    {
        playerInput.Remove(buttonID);

        PuzzleUnsolved();
    }
    private void PuzzleSolved()
    {
        foreach (var button in buttons)
        {
            button.PuzzleComplete();
        }
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

