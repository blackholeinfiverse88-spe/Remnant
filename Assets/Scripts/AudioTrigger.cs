using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public AudioClip audioClip;
    public float volume = 1f;
    public bool playOnce = true;

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (playOnce && hasPlayed)
            return;

        if (other.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(
                audioClip,
                transform.position,
                volume
            );

            hasPlayed = true;
        }
    }
}