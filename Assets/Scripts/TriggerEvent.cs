using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    public Animator targetAnimator;
    public string animationTrigger = "Play";

    public AudioSource audioSource;
    public AudioClip sound;

    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasTriggered = true;

        // Play cart animation
        if (targetAnimator != null)
        {
            targetAnimator.SetTrigger(animationTrigger);
        }

        // Play horror sound
        if (audioSource != null && sound != null)
        {
            audioSource.PlayOneShot(sound);
        }
    }
}