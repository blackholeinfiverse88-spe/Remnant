using UnityEngine;
using UnityEngine.UI;

public class RotatingPuzzleTile : MonoBehaviour
{
    [Header("Puzzle Manager")]
    public RotationPuzzleManager puzzleManager;

    [Header("Tile Image")]
    public RectTransform imageToRotate;

    private Button button;
    private int rotationStep = 0;

    public int RotationStep => rotationStep;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
            button = GetComponentInParent<Button>();

        if (imageToRotate == null)
        {
            Image[] images = GetComponentsInChildren<Image>(true);

            foreach (Image img in images)
            {
                if (img.gameObject != gameObject)
                {
                    imageToRotate = img.rectTransform;
                    break;
                }
            }

            if (imageToRotate == null)
            {
                Image ownImage = GetComponent<Image>();

                if (ownImage != null)
                    imageToRotate = ownImage.rectTransform;
            }
        }

        if (button == null)
            Debug.LogError(name + ": Button component missing!", this);

        if (imageToRotate == null)
            Debug.LogError(name + ": Assign Image To Rotate!", this);
    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(RotateTile);
            button.onClick.AddListener(RotateTile);
        }
    }

    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(RotateTile);
    }

    public void RandomizeRotation()
    {
        rotationStep = Random.Range(0, 4);
        ApplyRotation();
    }

    public void RotateTile()
    {
        if (puzzleManager == null)
        {
            Debug.LogWarning(name + ": Puzzle Manager is not assigned!", this);
            return;
        }

        if (puzzleManager.IsSolved)
            return;

        // Prevent rotation until the clue has been read.
        if (!puzzleManager.clueRead)
        {
            if (puzzleManager.statusText != null)
            {
                puzzleManager.statusText.text =
                    "Find the clue to solve the puzzle and open the door.";
            }

            Debug.Log("Read the clue before solving the puzzle.");
            return;
        }

        if (imageToRotate == null)
        {
            Debug.LogError(name + ": Image To Rotate is not assigned!", this);
            return;
        }

        // Rotate 90 degrees clockwise.
        rotationStep = (rotationStep + 1) % 4;

        ApplyRotation();

        Debug.Log(name + ": Rotation step = " + rotationStep, this);

        puzzleManager.CheckPuzzle();
    }

    private void ApplyRotation()
    {
        if (imageToRotate == null)
            return;

        imageToRotate.localRotation =
            Quaternion.Euler(0f, 0f, -90f * rotationStep);
    }

    public bool IsCorrect()
    {
        return rotationStep == 0;
    }
}
