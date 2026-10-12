
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElectricityPuzzle : MonoBehaviour
{
    [Header("Puzzle Buttons")]
    public Button[] buttons;
    public Image[] buttonImages;

    [Header("Brightness")]
    [Range(1f, 3f)]
    public float brightnessMultiplier = 2f;

    [Header("Correct Sequence")]
    // 0=Red, 1=Blue, 2=Green, 3=Yellow,
    // 4=Purple, 5=Orange, 6=White, 7=Pink
    public int[] correctSequence = { 0, 1, 4, 2, 3, 7, 5, 6 };

    [Header("UI")]
    public TMP_Text statusText;

    [Header("Power System")]
    public Chapter2PowerFailure powerFailureManager;
    public GameObject electricityPuzzleCanvas;
    private Color[] originalColors;
    private int currentStep = 0;
    private bool puzzleCompleted = false;

    private void Awake()
    {
        originalColors = new Color[buttonImages.Length];

        for (int i = 0; i < buttonImages.Length; i++)
        {
            if (buttonImages[i] != null)
                originalColors[i] = buttonImages[i].color;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => PressButton(index));
        }
    }

    private void Start()
    {
        ResetPuzzle();
    }

    private void PressButton(int index)
    {
        if (puzzleCompleted)
            return;

        if (currentStep >= correctSequence.Length)
            return;

        if (index == correctSequence[currentStep])
        {
            if (buttonImages[index] != null)
            {
                Color brighter = originalColors[index] * brightnessMultiplier;
                brighter.a = originalColors[index].a;
                buttonImages[index].color = brighter;
            }

            currentStep++;

            if (statusText != null)
                statusText.text = "Power sequence correct: " +
                                  currentStep + "/" + correctSequence.Length;

            if (currentStep >= correctSequence.Length)
            {
                puzzleCompleted = true;

                if (statusText != null)
                    statusText.text = "POWER RESTORED!";

                if (powerFailureManager != null)
                    powerFailureManager.PowerRestored();

                electricityPuzzleCanvas.SetActive(false);
            }
        }
        else
        {
            ResetPuzzle();

            if (statusText != null)
                statusText.text = "Wrong sequence! Try again.";
        }
    }

    private void ResetPuzzle()
    {
        currentStep = 0;

        for (int i = 0; i < buttonImages.Length; i++)
        {
            if (buttonImages[i] != null)
                buttonImages[i].color = originalColors[i];
        }

        if (statusText != null)
            statusText.text = "Follow the clue and press the buttons in order.";
    }
}