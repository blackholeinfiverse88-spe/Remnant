using UnityEngine;
using TMPro;

public class VRPasswordDoor : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputField;

    [Header("Password")]
    public string correctPassword = "A1025";

    [Header("Door")]
    public Animator doorAnimator;
    public string openAnimation = "Open";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonSound;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip doorSound;


    private bool doorOpened = false;

    public void AddCharacter(string character)
    {
        if (doorOpened)
            return;

        inputField.text += character;

        if (audioSource != null && buttonSound != null)
            audioSource.PlayOneShot(buttonSound);
    }

    public void ClearPassword()
    {
        inputField.text = "";

        if (audioSource != null && buttonSound != null)
            audioSource.PlayOneShot(buttonSound);
    }

    public void CheckPassword()
    {
        if (doorOpened)
            return;

        if (inputField.text == correctPassword)
        {
            doorOpened = true;

            if (audioSource != null && correctSound != null)
                audioSource.PlayOneShot(correctSound);
                audioSource.PlayOneShot(doorSound);

            if (doorAnimator != null)
                doorAnimator.SetTrigger("OpenDoor");
        }
        else
        {
            if (audioSource != null && wrongSound != null)
                audioSource.PlayOneShot(wrongSound);

            inputField.text = "";
        }
    }
}