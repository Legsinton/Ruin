using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] bool checkPressSequence;
    [SerializeField] List<PuzzleButton> buttons;
    [SerializeField] List<int> correctSequence;
    [SerializeField] GateScript gate;
    [SerializeField] MovingPlatform movingPlatform;

    readonly List<int> playerInput = new List<int>();

    public void RegisterButtonPress(int buttonID)
    {
        playerInput.Add(buttonID);

        if (playerInput.Count == correctSequence.Count)
        {
            if (checkPressSequence)
            {
                for (int i = 0; i < playerInput.Count; i++)
                {
                    if (playerInput[i] != correctSequence[i])
                    {
                        ResetPuzzle();
                        return;
                    }
                }
            }
            else
            {
                bool foundCorrectButton = false;
                for (int i = 0; i < playerInput.Count; i++)
                {
                    foundCorrectButton = false;
                    for(int j = 0; j < playerInput.Count; j++)
                    {
                        if (playerInput[i] == correctSequence[j])
                        {
                            foundCorrectButton = true;
                        }
                    }
                    if (!foundCorrectButton)
                    {
                        ResetPuzzle();
                        return;
                    }
                }
            }

            PuzzleSolved();
        }
    }

    public void UnRegisterButtonPress(int buttonID)
    {
        playerInput.Remove(buttonID);
    }
    private void PuzzleSolved()
    {
        GetComponent<Renderer>().material.color = Color.green;
        foreach (var button in buttons)
        {
            button.PuzzleComplete();
        }

        if(gate != null)
        {
            gate.AddSwitch(1);
        }

        if(movingPlatform != null)
        {
            movingPlatform.Switches++;
        }
    }
    private void ResetPuzzle()
    {
        playerInput.Clear();
        foreach (var button in buttons)
        {
            button.ResetButton();
        }
    }
}
