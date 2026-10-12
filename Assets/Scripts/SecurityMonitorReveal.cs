using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class SecurityMonitorReveal : MonoBehaviour
{
    [Header("Security Monitor Video")]
    public VideoPlayer monitorVideo;

    [Header("Monitor Screen")]
    public GameObject monitorScreen;

    [Header("Timing")]
    public float delayAfterPowerRestoredDialogue = 1f;
    public float videoVisibleDuration = 5f;

    [Header("ASTER Dialogue")]
    [TextArea(2, 4)]
    public string aiResponse =
        "The image you observed was a corrupted recording. " +
        "There are no other living humans in this facility.";

    public AudioClip aiResponseVoice;
    public float aiResponseDuration = 7f;

    private bool hasPlayed;

    private void Awake()
    {
        // Hide the human figure before the reveal.
        if (monitorScreen != null)
            monitorScreen.SetActive(false);
    }

    public void PlayReveal()
    {
        Debug.Log("SECURITY VIDEO REVEAL CALLED");

        if (hasPlayed)
            return;

        hasPlayed = true;
        StartCoroutine(RevealSequence());
    }

    private IEnumerator RevealSequence()
    {
        yield return new WaitForSeconds(
            delayAfterPowerRestoredDialogue
        );

        if (monitorVideo == null)
        {
            Debug.LogWarning(
                "Security monitor VideoPlayer is not assigned."
            );
            yield break;
        }

        // Show the screen and play the video from the beginning.
        if (monitorScreen != null)
            monitorScreen.SetActive(true);

        monitorVideo.Stop();
        monitorVideo.isLooping = false;
        monitorVideo.time = 0;
        monitorVideo.Play();

        // Wait for the reveal duration.
        yield return new WaitForSeconds(videoVisibleDuration);

        // Stop playback and hide the human figure.
        monitorVideo.Stop();

        if (monitorScreen != null)
            monitorScreen.SetActive(false);

        // ASTER explains away what the player saw.
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.PlayDialogue(
                aiResponse,
                aiResponseVoice,
                aiResponseDuration
            );
        }
        else
        {
            Debug.LogWarning(
                "DialogueManager was not found."
            );
        }
    }
}

