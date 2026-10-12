
using UnityEngine;

public class GameBGM : MonoBehaviour
{
    [Header("Background Music")]
    public AudioClip bgmClip;

    [Range(0f, 1f)]
    public float volume = 0.3f;

    public bool loop = true;
    public bool playOnStart = true;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f; // 2D background music
    }

    private void Start()
    {
        if (playOnStart)
            PlayMusic();
    }

    public void PlayMusic()
    {
        if (bgmClip == null)
        {
            Debug.LogWarning("BGM AudioClip is not assigned!");
            return;
        }

        audioSource.clip = bgmClip;
        audioSource.volume = volume;
        audioSource.loop = loop;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }
}