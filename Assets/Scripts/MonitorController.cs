using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class MonitorController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage screen;

    public void TurnOnMonitor()
    {
        screen.enabled = true;
        videoPlayer.Play();
    }
}