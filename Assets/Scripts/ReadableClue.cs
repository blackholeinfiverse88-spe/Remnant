using UnityEngine;

public class ReadableClue : MonoBehaviour
{
    [TextArea(5, 20)]
    public string clueText;

    public bool hasBeenRead = false;

    [Header("Optional Clue Image")]
    public GameObject clueImageObject;

    [Header("Optional Rotation Puzzle")]
    public RotationPuzzleManager puzzleManager;

    public void OpenClue()
    {
        hasBeenRead = true;

        if (ClueReadManager.Instance != null)
        {
            ClueReadManager.Instance.OpenClue(clueText);
        }

        // Show the clue image if one is assigned.
        if (clueImageObject != null)
        {
            clueImageObject.SetActive(true);
        }

        // Unlock tile rotation after opening this clue.
        if (puzzleManager != null)
        {
            puzzleManager.MarkClueAsRead();
        }
    }
}