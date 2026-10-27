using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClueReadManager : MonoBehaviour
{
    public static ClueReadManager Instance;

    [Header("Reading UI")]
    public GameObject readingCanvas;
    public TMP_Text clueText;

    [Header("Image UI")]
    public Image clueImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        readingCanvas.SetActive(false);

        if (clueImage != null)
            clueImage.gameObject.SetActive(false);
    }

    // Existing text clue functionality
    public void OpenClue(string text)
    {
        readingCanvas.SetActive(true);

        if (clueText != null)
        {
            clueText.text = text;
            clueText.gameObject.SetActive(true);
        }

        if (clueImage != null)
            clueImage.gameObject.SetActive(false);
    }

    // Display an image inside the reading canvas
    public void OpenImageClue(Sprite image)
    {
        readingCanvas.SetActive(true);

        if (clueText != null)
        {
            clueText.text = "";
            clueText.gameObject.SetActive(false);
        }

        if (clueImage != null)
        {
            clueImage.sprite = image;
            clueImage.preserveAspect = true;
            clueImage.gameObject.SetActive(image != null);
        }
    }

    public void CloseClue()
    {
        readingCanvas.SetActive(false);

        if (clueText != null)
        {
            clueText.text = "";
            clueText.gameObject.SetActive(true);
        }

        if (clueImage != null)
        {
            clueImage.sprite = null;
            clueImage.gameObject.SetActive(false);
        }
    }
}
