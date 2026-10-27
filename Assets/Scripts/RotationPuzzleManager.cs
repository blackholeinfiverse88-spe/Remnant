using UnityEngine;
using TMPro;

public class RotationPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Tiles")]
    public RotatingPuzzleTile[] tiles;

    [Header("UI")]
    public TMP_Text statusText;
    public GameObject puzzleCanvas;

    [Header("Clue Settings")]
    public bool clueRead = false;
    public string clueRequiredMessage =
        "Find the clue to solve the puzzle and open the door.";

    [Header("Door")]
    public GameObject doorObject;

    [Header("Door Sound")]
    public AudioSource audioSource;
    public AudioClip doorSound;

    [Header("Puzzle Settings")]
    public int gridSize = 4;

    public bool IsSolved { get; private set; }

    private void Start()
    {
        IsSolved = false;

        int expectedTileCount = gridSize * gridSize;

        if (tiles == null || tiles.Length != expectedTileCount)
        {
            Debug.LogError(
                $"Assign exactly {expectedTileCount} tiles!",
                this
            );
            return;
        }

        if (puzzleCanvas != null)
            puzzleCanvas.SetActive(true);

        foreach (RotatingPuzzleTile tile in tiles)
        {
            if (tile == null)
            {
                Debug.LogError("A puzzle tile is missing!", this);
                continue;
            }

            tile.puzzleManager = this;
            tile.RandomizeRotation();
        }

        UpdateStatus();
    }

    public void MarkClueAsRead()
    {
        clueRead = true;

        if (statusText != null && !IsSolved)
            statusText.text = "Clue found! Solve the puzzle to open the door.";

        Debug.Log("Puzzle clue has been read.");
    }

    public void CheckPuzzle()
    {
        if (IsSolved)
            return;

        // Prevent solving before reading the clue.
        if (!clueRead)
        {
            if (statusText != null)
                statusText.text = clueRequiredMessage;

            return;
        }

        int expectedTileCount = gridSize * gridSize;

        if (tiles == null || tiles.Length != expectedTileCount)
            return;

        foreach (RotatingPuzzleTile tile in tiles)
        {
            if (tile == null || !tile.IsCorrect())
            {
                if (statusText != null)
                    statusText.text = "Image incomplete - keep rotating.";

                return;
            }
        }

        IsSolved = true;

        Debug.Log("PUZZLE SOLVED!");

        if (statusText != null)
            statusText.text = "ACCESS GRANTED";

        if (audioSource != null && doorSound != null)
            audioSource.PlayOneShot(doorSound);

        if (doorObject != null)
            doorObject.SetActive(false);
        else
            Debug.LogWarning("Door Object is not assigned!", this);

        if (puzzleCanvas != null)
            puzzleCanvas.SetActive(false);
    }

    private void UpdateStatus()
    {
        if (statusText == null)
            return;

        if (!clueRead)
            statusText.text = clueRequiredMessage;
        else
            statusText.text = "Clue found! Solve the puzzle to open the door.";
    }
}
