using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialogueCanvas;
    public TMP_Text dialogueText;

    [Header("Audio")]
    public AudioSource voiceSource;

    [Header("Settings")]
    public float defaultTextDuration = 3f;

    private Coroutine currentDialogue;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        dialogueCanvas.SetActive(false);
    }

    public void PlayDialogue(string text)
    {
        PlayDialogue(text, null, defaultTextDuration);
    }

    public void PlayDialogue(string text, AudioClip voiceClip)
    {
        PlayDialogue(text, voiceClip, defaultTextDuration);
    }

    public void PlayDialogue(string text, AudioClip voiceClip, float duration)
    {
        if (currentDialogue != null)
            StopCoroutine(currentDialogue);

        currentDialogue = StartCoroutine(DisplayDialogue(text, voiceClip, duration));
    }

    private IEnumerator DisplayDialogue(
        string text,
        AudioClip voiceClip,
        float duration)
    {
        dialogueCanvas.SetActive(true);

        dialogueText.text = text;

        // Play voice if available
        if (voiceClip != null && voiceSource != null)
        {
            voiceSource.Stop();
            voiceSource.clip = voiceClip;
            voiceSource.Play();

            // Use voice length instead of fixed duration
            duration = Mathf.Max(duration, voiceClip.length);
        }

        yield return new WaitForSeconds(duration);

        dialogueCanvas.SetActive(false);

        if (voiceSource != null)
            voiceSource.Stop();

        dialogueText.text = "";

        currentDialogue = null;
    }

    public void HideDialogue()
    {
        if (currentDialogue != null)
        {
            StopCoroutine(currentDialogue);
            currentDialogue = null;
        }

        dialogueCanvas.SetActive(false);

        if (voiceSource != null)
            voiceSource.Stop();

        dialogueText.text = "";
    }
}