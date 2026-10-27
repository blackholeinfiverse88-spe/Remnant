using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System;

public class CCTVVideoSwitcher : MonoBehaviour
{
    [Header("Video Player")]
    public VideoPlayer videoPlayer;
    public VideoClip[] recordings;
    public RenderTexture cctvRenderTexture;

    [Header("CCTV UI")]
    public GameObject cctvPanel;
    public RawImage cctvScreen;
    public TMP_Text cameraName;
    public TMP_Text recIndicator;
    public TMP_Text timestamp;

    [Header("Buttons")]
    public Button previousButton;
    public Button nextButton;
    public Button closeButton;

    [Header("Player Proximity Trigger")]
    public string playerTag = "Player";
    public bool requirePlayerNearby = true;

    [Header("Timestamp")]
    public bool useRealtimeClock = false;
    public string timestampPrefix = "REC DATE: ";

    public string[] recordingDates =
    {
        "2049/08/12 03:42:15",
        "2049/08/12 03:44:09",
        "2049/08/12 03:51:32",
        "2049/08/12 04:02:18"
    };

    private readonly string[] cameraNames =
    {
        "CAM 01 - MAIN ENTRANCE",
        "CAM 02 - MAIN CORRIDOR",
        "CAM 03 - LABORATORY",
        "CAM 04 - RESTRICTED AREA"
    };

    private int currentIndex;
    private float blinkTimer;

    private bool recVisible = true;
    private bool isOpen;
    private bool playerNearby;
    private bool isPreparing;

    private VideoClip requestedClip;

    private void Awake()
    {
        if (cctvPanel != null)
            cctvPanel.SetActive(true);

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = true;
            videoPlayer.waitForFirstFrame = true;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;

            if (cctvRenderTexture != null)
                videoPlayer.targetTexture = cctvRenderTexture;
        }

        ConnectRenderTexture();
    }

    private void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.errorReceived -= OnVideoError;
            videoPlayer.started -= OnVideoStarted;
            videoPlayer.loopPointReached -= OnVideoFinished;

            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.errorReceived += OnVideoError;
            videoPlayer.started += OnVideoStarted;
            videoPlayer.loopPointReached += OnVideoFinished;

            videoPlayer.Stop();
        }

        if (previousButton != null)
        {
            previousButton.onClick.RemoveListener(PreviousVideo);
            previousButton.onClick.AddListener(PreviousVideo);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(NextVideo);
            nextButton.onClick.AddListener(NextVideo);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseCCTV);
            closeButton.onClick.AddListener(CloseCCTV);
        }

        UpdateCameraDetails();

        if (recIndicator != null)
        {
            recIndicator.enabled = true;
            recIndicator.text = "STANDBY";
        }
    }

    private void Update()
    {
        if (!isOpen)
            return;

        blinkTimer += Time.unscaledDeltaTime;

        if (blinkTimer >= 0.5f)
        {
            blinkTimer = 0f;
            recVisible = !recVisible;

            if (recIndicator != null)
                recIndicator.enabled = recVisible;
        }

        if (timestamp != null)
        {
            string dateText = useRealtimeClock
                ? DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                : GetRecordingDate();

            timestamp.text = timestampPrefix + dateText;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayerCollider(other))
            return;

        playerNearby = true;

        Debug.Log("CCTV: Player entered trigger.");

        if (requirePlayerNearby)
            OpenCCTV();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayerCollider(other))
            return;

        playerNearby = false;

        Debug.Log("CCTV: Player left trigger.");

        if (requirePlayerNearby)
            StopCCTVPlayback();
    }

    private bool IsPlayerCollider(Collider other)
    {
        if (other == null)
            return false;

        if (other.CompareTag(playerTag))
            return true;

        return other.transform.root.CompareTag(playerTag);
    }

    public void OpenCCTV()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("CCTV: VideoPlayer is not assigned.");
            return;
        }

        if (recordings == null || recordings.Length == 0)
        {
            Debug.LogError("CCTV: No recordings assigned.");
            return;
        }

        if (requirePlayerNearby && !playerNearby)
        {
            Debug.LogWarning("CCTV: Player is outside the trigger.");
            return;
        }

        isOpen = true;

        if (cctvPanel != null)
            cctvPanel.SetActive(true);

        currentIndex = Mathf.Clamp(
            currentIndex, 0, recordings.Length - 1);

        RequestSelectedVideo();
    }

    public void NextVideo()
    {
        if (!CanSwitchVideo())
            return;

        if (isPreparing)
        {
            Debug.Log("CCTV: Please wait for the current video to prepare.");
            return;
        }

        currentIndex = (currentIndex + 1) % recordings.Length;

        RequestSelectedVideo();
    }

    public void PreviousVideo()
    {
        if (!CanSwitchVideo())
            return;

        if (isPreparing)
        {
            Debug.Log("CCTV: Please wait for the current video to prepare.");
            return;
        }

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = recordings.Length - 1;

        RequestSelectedVideo();
    }

    private bool CanSwitchVideo()
    {
        if (!isOpen)
        {
            Debug.LogWarning("CCTV: Open CCTV before switching videos.");
            return false;
        }

        if (requirePlayerNearby && !playerNearby)
        {
            Debug.LogWarning("CCTV: Player is outside the trigger.");
            return false;
        }

        if (recordings == null || recordings.Length == 0)
        {
            Debug.LogError("CCTV: No recordings assigned.");
            return false;
        }

        return true;
    }

    private void RequestSelectedVideo()
    {
        if (videoPlayer == null ||
            recordings == null ||
            currentIndex < 0 ||
            currentIndex >= recordings.Length)
        {
            return;
        }

        VideoClip selectedClip = recordings[currentIndex];

        if (selectedClip == null)
        {
            Debug.LogError(
                "CCTV: Recording slot " + currentIndex + " is empty.");
            return;
        }

        // Remember the exact clip requested.
        requestedClip = selectedClip;
        isPreparing = true;

        // Stop old playback and clear its selected clip.
        videoPlayer.Stop();
        videoPlayer.clip = null;

        // Ensure we are using the assigned VideoClip source.
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.isLooping = true;
        videoPlayer.waitForFirstFrame = true;

        ConnectRenderTexture();
        ClearRenderTexture();

        // Update the UI to match the selected recording.
        UpdateCameraDetails();

        // Assign the new recording only after stopping the old one.
        videoPlayer.clip = selectedClip;

        if (recIndicator != null)
        {
            recIndicator.enabled = true;
            recIndicator.text = "LOADING";
        }

        Debug.Log(
            "CCTV REQUESTED: Index " + currentIndex +
            " | Clip: " + selectedClip.name);

        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        if (source != videoPlayer)
            return;

        // Ignore callbacks if the selected clip has changed.
        if (source.clip == null || source.clip != requestedClip)
        {
            Debug.LogWarning("CCTV: Ignored callback for an old clip.");
            return;
        }

        if (!isOpen)
        {
            isPreparing = false;
            return;
        }

        if (requirePlayerNearby && !playerNearby)
        {
            isPreparing = false;
            return;
        }

        ConnectRenderTexture();
        ClearRenderTexture();

        Debug.Log(
            "CCTV PREPARED: " + source.clip.name +
            " | Index: " + currentIndex);

        // Begin playback of the selected recording.
        source.Play();
    }

    private void OnVideoStarted(VideoPlayer source)
    {
        if (source != videoPlayer)
            return;

        if (source.clip != requestedClip)
            return;

        isPreparing = false;

        Debug.Log(
            "CCTV ACTUALLY STARTED: " + source.clip.name +
            " | Index: " + currentIndex);

        if (recIndicator != null)
        {
            recIndicator.enabled = true;
            recIndicator.text = "● REC";
        }
    }

    private void OnVideoError(VideoPlayer source, string message)
    {
        if (source != videoPlayer)
            return;

        isPreparing = false;

        Debug.LogError(
            "CCTV ERROR: " +
            (source.clip != null ? source.clip.name : "Unknown clip") +
            " | " + message);
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        if (source != videoPlayer)
            return;

        if (isOpen &&
            source.isLooping &&
            (!requirePlayerNearby || playerNearby))
        {
            source.Play();
        }
    }

    private void ConnectRenderTexture()
    {
        if (videoPlayer != null && cctvRenderTexture != null)
        {
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = cctvRenderTexture;
        }

        if (cctvScreen != null && cctvRenderTexture != null)
        {
            cctvScreen.texture = cctvRenderTexture;
        }
    }

    private void ClearRenderTexture()
    {
        if (cctvRenderTexture == null)
            return;

        RenderTexture previousTexture = RenderTexture.active;

        RenderTexture.active = cctvRenderTexture;

        GL.Clear(true, true, Color.black);

        RenderTexture.active = previousTexture;
    }

    private void UpdateCameraDetails()
    {
        if (cameraName != null)
        {
            cameraName.text = currentIndex < cameraNames.Length
                ? cameraNames[currentIndex]
                : "CAM " + (currentIndex + 1);
        }

        if (timestamp != null)
        {
            string dateText = useRealtimeClock
                ? DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                : GetRecordingDate();

            timestamp.text = timestampPrefix + dateText;
        }
    }

    private string GetRecordingDate()
    {
        if (recordingDates != null &&
            currentIndex >= 0 &&
            currentIndex < recordingDates.Length)
        {
            return recordingDates[currentIndex];
        }

        return "DATE UNKNOWN";
    }

    private void StopCCTVPlayback()
    {
        isOpen = false;
        isPreparing = false;
        requestedClip = null;

        if (videoPlayer != null)
            videoPlayer.Stop();

        blinkTimer = 0f;
        recVisible = true;

        if (recIndicator != null)
        {
            recIndicator.enabled = true;
            recIndicator.text = "STANDBY";
        }

        // Keep the Canvas and CCTV panel enabled.
    }

    public void CloseCCTV()
    {
        StopCCTVPlayback();
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.errorReceived -= OnVideoError;
            videoPlayer.started -= OnVideoStarted;
            videoPlayer.loopPointReached -= OnVideoFinished;
        }

        if (previousButton != null)
            previousButton.onClick.RemoveListener(PreviousVideo);

        if (nextButton != null)
            nextButton.onClick.RemoveListener(NextVideo);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseCCTV);
    }
}