
using UnityEngine;

public class ReadableClue : MonoBehaviour
{
    [TextArea(5, 20)]
    public string clueText;

    public bool hasBeenRead = false;

    public void OpenClue()
    {
        hasBeenRead = true;

        if (ClueReadManager.Instance != null)
        {
            ClueReadManager.Instance.OpenClue(clueText);
        }
    }
}