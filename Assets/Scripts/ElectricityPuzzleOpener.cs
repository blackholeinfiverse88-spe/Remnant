using UnityEngine;

public class ElectricityPuzzleOpener : MonoBehaviour
{
    [Header("Puzzle UI")]
    public GameObject electricityPuzzleCanvas;

    [Header("Clue Reference")]
    public ReadableClue requiredClue;

    [Header("Power System")]
    public Chapter2PowerFailure powerSystem;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    public string clueMissingDialogue =
        "Find the clue to solve the puzzle.";

    public void OpenPuzzle()
    {
        // Safety check
        if (powerSystem == null)
        {
            Debug.LogError(
                "Chapter2PowerFailure is not assigned!"
            );
            return;
        }

        // If puzzle was solved once, use this button
        // to toggle facility power instead.
        if (powerSystem.IsPuzzleSolved)
        {
            powerSystem.ToggleFacilityPower();
            return;
        }

        // Before the first solve, require the clue.
        if (requiredClue == null || !requiredClue.hasBeenRead)
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.PlayDialogue(
                    clueMissingDialogue
                );
            }

            return;
        }

        // Open the puzzle for the first time.
        if (electricityPuzzleCanvas != null)
        {
            electricityPuzzleCanvas.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "Electricity Puzzle Canvas is not assigned!"
            );
        }
    }

    public void ClosePuzzle()
    {
        if (electricityPuzzleCanvas != null)
        {
            electricityPuzzleCanvas.SetActive(false);
        }
    }
}

