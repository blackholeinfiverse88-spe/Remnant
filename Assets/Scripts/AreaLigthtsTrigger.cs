using UnityEngine;

public class AreaLightTrigger : MonoBehaviour
{
    public Light[] lightsToTurnOn;
    public float delay = 0f;
    public bool turnOffAfterLeaving = false;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            if (delay > 0)
                Invoke(nameof(TurnOnLights), delay);
            else
                TurnOnLights();
        }
    }

    private void TurnOnLights()
    {
        foreach (Light light in lightsToTurnOn)
        {
            if (light != null)
                light.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!turnOffAfterLeaving)
            return;

        if (other.CompareTag("Player"))
        {
            foreach (Light light in lightsToTurnOn)
            {
                if (light != null)
                    light.enabled = false;
            }
        }
    }
}