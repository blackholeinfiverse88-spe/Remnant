using System.Collections;
using UnityEngine;
using TMPro;

public class AssistantDialogueController : MonoBehaviour
{
    [Header("Movement")]
    public AssistantSimpleMovement assistantMovement;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Assistant Voice")]
    public AudioSource voiceAudioSource;
    public AudioClip dialogueAudio;

    [TextArea(3, 6)]
    public string dialogue =
        "Wait! You need to listen to me. This facility is not safe anymore. The AI has released the test subjects. You need to get out of here before it finds you.";

    [Header("Animation")]
    public Animator animator;
    public string talkingParameter = "IsTalking";

    private bool dialogueStarted;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (animator == null)
            Debug.LogError("Assign the assistant Animator!", this);

        if (assistantMovement == null)
            Debug.LogError("Assign AssistantSimpleMovement!", this);

        if (voiceAudioSource == null)
            Debug.LogError("Assign the voice AudioSource!", this);
    }

    private void Update()
    {
        if (dialogueStarted || assistantMovement == null)
            return;

        // Begin talking only after she reaches the player.
        if (assistantMovement.HasArrived)
        {
            dialogueStarted = true;
            StartCoroutine(PlayDialogue());
        }
    }

    private IEnumerator PlayDialogue()
    {
        // Show dialogue.
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = dialogue;

        // Play talking animation.
        if (animator != null &&
            animator.isActiveAndEnabled)
        {
            animator.SetBool(talkingParameter, true);
        }

        // Play the assistant's voice.
        if (voiceAudioSource != null && dialogueAudio != null)
        {
            voiceAudioSource.clip = dialogueAudio;
            voiceAudioSource.Play();

            // Keep the text and talking animation active
            // until the voice finishes.
            yield return new WaitWhile(
                () => voiceAudioSource != null &&
                      voiceAudioSource.isPlaying
            );
        }
        else
        {
            Debug.LogWarning(
                "No dialogue audio assigned. " +
                "Displaying text for 5 seconds.", this);

            yield return new WaitForSeconds(5f);
        }

        // Stop talking animation.
        if (animator != null &&
            animator.isActiveAndEnabled)
        {
            animator.SetBool(talkingParameter, false);
        }

        // Hide dialogue.
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Move to the PC after speaking.
        if (assistantMovement != null)
            assistantMovement.GoToPC();
    }
}