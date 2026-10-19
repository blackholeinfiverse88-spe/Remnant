
using UnityEngine;
using TMPro;

public class RotationPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Tiles")]
    public RotatingPuzzleTile[] tiles;

    [Header("UI")]
    public TMP_Text statusText;
    public GameObject puzzleCanvas;

    [Header("Door")]
    public GameObject doorObject;

    [Header("Door Sound")]
    public AudioSource audioSource;
    public AudioClip doorSound;

    public bool IsSolved { get; private set; }

    private void Start()
    {
        IsSolved = false;

        if (tiles == null || tiles.Length != 9)
        {
            Debug.LogError(
                "RotationPuzzleManager: Assign exactly 9 tiles!",
                this
            );
            return;
        }

        // Keep the puzzle visible at startup.
        if (puzzleCanvas != null)
            puzzleCanvas.SetActive(true);

        // Connect and randomize all tiles.
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

        if (statusText != null)
            statusText.text = "Rotate all tiles to 0 degrees";

        // Do not check the solution at startup.
    }

    public void CheckPuzzle()
    {
        if (IsSolved)
            return;

        if (tiles == null || tiles.Length != 9)
            return;

        // Every tile must be at rotation step 0.
        foreach (RotatingPuzzleTile tile in tiles)
        {
            if (tile == null || !tile.IsCorrect())
            {
                if (statusText != null)
                    statusText.text =
                        "Image incomplete - keep rotating";

                return;
            }
        }

        // All nine tiles are correct.
        IsSolved = true;

        Debug.Log(
            "PUZZLE SOLVED! All tiles are at rotation step 0.",
            this
        );

        if (statusText != null)
            statusText.text = "ACCESS GRANTED";

        // Play sound before disabling any related objects.
        if (audioSource != null && doorSound != null)
            audioSource.PlayOneShot(doorSound);

        // Disable the door immediately. No animation.
        if (doorObject != null)
            doorObject.SetActive(false);
        else
            Debug.LogWarning("Door Object is not assigned!", this);

        // Close the puzzle UI.
        if (puzzleCanvas != null)
            puzzleCanvas.SetActive(false);
    }
}
