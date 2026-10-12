
using UnityEngine;

public class PowerOffButton : MonoBehaviour
{
    public Chapter2PowerFailure powerManager;

    public void TurnPowerOff()
    {
        if (powerManager != null)
        {
            powerManager.TurnPowerOff();
        }
    }
}