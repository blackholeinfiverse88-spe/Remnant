using UnityEngine;
using TMPro;

public class ClueReadManager : MonoBehaviour
{
    public static ClueReadManager Instance;

    [Header("Reading UI")]
    public GameObject readingCanvas;
    public TMP_Text clueText;

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
    }

    public void OpenClue(string text)
    {
        readingCanvas.SetActive(true);
        clueText.text = text;
    }

    public void CloseClue()
    {
        readingCanvas.SetActive(false);
        clueText.text = "";
    }
}