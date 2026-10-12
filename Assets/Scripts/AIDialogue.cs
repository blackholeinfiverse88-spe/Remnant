
using UnityEngine;

public class AIActionDialogue : MonoBehaviour
{
    [Header("Editable ASTER Dialogue")]
    [TextArea(2, 5)]
    public string dialogueText =
        "Do not trust this clue. It has been corrupted by someone pretending to be human. You are the only living human left in this facility.";

    [Header("Voice Audio")]
    public AudioClip voiceClip;
    public float duration = 3f;

    [Header("Playback")]
    public bool playOnlyOnce = true;

    private bool hasPlayed;

    // Call this after reading a clue or completing an action.
    public void PlayDialogue()
    {
        if (playOnlyOnce && hasPlayed)
            return;

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("DialogueManager not found!");
            return;
        }

        hasPlayed = true;

        DialogueManager.Instance.PlayDialogue(
            dialogueText,
            voiceClip,
            duration
        );
    }

    public void ResetDialogue()
    {
        hasPlayed = false;
    }
}