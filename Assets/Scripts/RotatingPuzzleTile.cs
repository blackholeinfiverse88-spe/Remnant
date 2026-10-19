
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
        // Find Button on this object or a parent.
        button = GetComponent<Button>();

        if (button == null)
            button = GetComponentInParent<Button>();

        // Find the image to rotate.
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
        if (puzzleManager != null && puzzleManager.IsSolved)
            return;

        if (imageToRotate == null)
        {
            Debug.LogError(name + ": Image To Rotate is not assigned!", this);
            return;
        }

        // Rotate by 90 degrees each click.
        rotationStep = (rotationStep + 1) % 4;

        ApplyRotation();

        Debug.Log(
            name + ": Rotation step = " + rotationStep,
            this
        );

        if (puzzleManager != null)
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
