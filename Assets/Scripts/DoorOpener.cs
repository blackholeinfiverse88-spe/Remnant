using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public GameObject[] doorsToDisable;
    public AudioSource audioSource;
    public AudioClip doorSound;

    public void OpenDoors()
    {
        if (audioSource != null && doorSound != null)
        {
            audioSource.PlayOneShot(doorSound);
        }

        foreach (GameObject door in doorsToDisable)
        {
            if (door != null)
                door.SetActive(false);
        }
    }
}
