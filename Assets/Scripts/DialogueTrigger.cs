
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string dialogueText = "Where am I?";

    public AudioClip voiceClip;
    public float duration = 3f;

    [Header("Playback Settings")]
    [Tooltip("Checked = play once. Unchecked = play every time the player enters.")]
    public bool playOnlyOnce = true;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (playOnlyOnce && triggered)
            return;

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("DialogueManager not found!");
            return;
        }

        triggered = true;

        DialogueManager.Instance.PlayDialogue(
            dialogueText,
            voiceClip,
            duration
        );
    }

    public void ResetTrigger()
    {
        triggered = false;
    }
}