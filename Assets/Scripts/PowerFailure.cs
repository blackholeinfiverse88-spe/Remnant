using System.Collections;
using UnityEngine;

public class Chapter2PowerFailure : MonoBehaviour
{
    [Header("Player")]
    public string playerTag = "Player";

    [Header("Warehouse Lights")]
    public GameObject[] warehouseLights;

    [Header("Blackout")]
    public float blackoutAmbientIntensity = 0f;
    public float blackoutFadeTime = 1f;

    [Header("Emergency Lights")]
    public Light[] emergencyLights;

    [Header("Security Monitor Reveal")]
    public SecurityMonitorReveal securityMonitorReveal;

    [Header("AI Power Failure Dialogue")]
    [TextArea(2, 5)]
    public string aiDialogue =
        "Power failure detected. Find the electrical room and restore the power.";

    public AudioClip aiVoice;
    public float dialogueDuration = 5f;

    [Header("AI Power Restored Dialogue")]
    [TextArea(2, 5)]
    public string powerRestoredDialogue =
        "Power restored. All systems are operational.";

    public AudioClip powerRestoredVoice;
    public float powerRestoredDialogueDuration = 5f;

    [Header("Sequence Delays")]
    public float delayBeforeBlackout = 0.5f;
    public float delayBeforeDialogue = 1f;

    [Header("Power Failure Audio")]
    public AudioSource audioSource;
    public AudioClip powerFailureSound;

    private bool hasTriggered;
    private bool powerRestored;
    private bool firstRestorationComplete;
    private float originalAmbientIntensity;

    public bool IsPuzzleSolved => firstRestorationComplete;
    public bool PowerIsOn { get; private set; }

    private void Start()
    {
        originalAmbientIntensity = RenderSettings.ambientIntensity;
        SetEmergencyLights(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (hasTriggered || powerRestored)
            return;

        if (!other.CompareTag(playerTag))
            return;

        hasTriggered = true;
        StartCoroutine(PowerFailureSequence());
    }

    private IEnumerator PowerFailureSequence()
    {
        Debug.Log("Player left Room 1. Starting power failure.");

        yield return new WaitForSeconds(delayBeforeBlackout);

        if (audioSource != null && powerFailureSound != null)
            audioSource.PlayOneShot(powerFailureSound);

        SetWarehouseLights(false);
        SetEmergencyLights(false);

        yield return StartCoroutine(
            FadeAmbientLighting(
                RenderSettings.ambientIntensity,
                blackoutAmbientIntensity,
                blackoutFadeTime
            )
        );

        PowerIsOn = false;

        Debug.Log("COMPLETE BLACKOUT");

        yield return new WaitForSeconds(delayBeforeDialogue);

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.PlayDialogue(
                aiDialogue,
                aiVoice,
                dialogueDuration
            );
        }
        else
        {
            Debug.LogWarning("DialogueManager not found!");
        }
    }

    private IEnumerator FadeAmbientLighting(
        float startIntensity,
        float endIntensity,
        float duration)
    {
        if (duration <= 0f)
        {
            RenderSettings.ambientIntensity = endIntensity;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            RenderSettings.ambientIntensity = Mathf.Lerp(
                startIntensity,
                endIntensity,
                Mathf.Clamp01(elapsed / duration)
            );

            yield return null;
        }

        RenderSettings.ambientIntensity = endIntensity;
    }

    private void SetWarehouseLights(bool state)
    {
        if (warehouseLights == null)
            return;

        foreach (GameObject lightObject in warehouseLights)
        {
            if (lightObject != null)
                lightObject.SetActive(state);
        }
    }

    private void SetEmergencyLights(bool state)
    {
        if (emergencyLights == null)
            return;

        foreach (Light emergencyLight in emergencyLights)
        {
            if (emergencyLight != null)
                emergencyLight.enabled = state;
        }
    }

    // Called when the electricity puzzle is solved for the first time.
    public void PowerRestored()
    {
        bool isFirstRestoration = !firstRestorationComplete;

        powerRestored = true;
        firstRestorationComplete = true;
        PowerIsOn = true;

        StopAllCoroutines();

        RenderSettings.ambientIntensity = originalAmbientIntensity;
        SetWarehouseLights(true);
        SetEmergencyLights(false);

        Debug.Log("POWER RESTORED");

        // Story dialogue and monitor reveal happen only once.
        if (isFirstRestoration)
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.PlayDialogue(
                    powerRestoredDialogue,
                    powerRestoredVoice,
                    powerRestoredDialogueDuration
                );
            }

            if (securityMonitorReveal != null)
            {
                StartCoroutine(PlaySecurityRevealAfterDialogue());
            }
            else
            {
                Debug.LogWarning(
                    "SecurityMonitorReveal is not assigned."
                );
            }
        }
    }

    private IEnumerator PlaySecurityRevealAfterDialogue()
    {
        float waitTime = powerRestoredDialogueDuration;

        if (powerRestoredVoice != null)
            waitTime = Mathf.Max(waitTime, powerRestoredVoice.length);

        yield return new WaitForSeconds(waitTime);

        if (securityMonitorReveal != null)
            securityMonitorReveal.PlayReveal();
    }

    // Used by the puzzle button after the puzzle has already been solved.
    public void ToggleFacilityPower()
    {
        if (!firstRestorationComplete)
        {
            Debug.Log("Solve the electricity puzzle first.");
            return;
        }

        if (PowerIsOn)
        {
            TurnPowerOff();
        }
        else
        {
            TurnPowerOnAgain();
        }
    }

    public void TurnPowerOff()
    {
        if (!firstRestorationComplete || !PowerIsOn)
            return;

        PowerIsOn = false;

        SetWarehouseLights(false);
        SetEmergencyLights(false);
        RenderSettings.ambientIntensity = blackoutAmbientIntensity;

        Debug.Log("POWER TURNED OFF");
    }

    private void TurnPowerOnAgain()
    {
        PowerIsOn = true;

        RenderSettings.ambientIntensity = originalAmbientIntensity;
        SetWarehouseLights(true);
        SetEmergencyLights(false);

        Debug.Log("POWER TURNED ON AGAIN");
    }
}

