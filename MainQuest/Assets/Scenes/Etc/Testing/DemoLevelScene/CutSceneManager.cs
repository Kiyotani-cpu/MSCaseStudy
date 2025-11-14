using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

public class CutsceneManager : MonoBehaviour
{
    [Header("Video Settings")]
    public string videoFileName = "boss_intro.mp4"; // Must be inside StreamingAssets
    public VideoPlayer videoPlayer;

    [Header("UI Settings")]
    public GameObject cutsceneUI; // Canvas or RawImage that displays video

    [Header("Events")]
    public UnityEvent OnCutsceneStart;
    public UnityEvent OnCutsceneEnd;

    private bool isPlaying = false;

    void Start()
    {
        cutsceneUI.SetActive(false);
        
        if (videoPlayer == null)
            videoPlayer = gameObject.AddComponent<VideoPlayer>();

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void PlayCutscene()
    {
        if (isPlaying) return;

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.url = path;

        if (cutsceneUI != null)
            cutsceneUI.SetActive(true);   // ✅ Show UI

        videoPlayer.Play();
        isPlaying = true;

        Debug.Log("Cutscene started: " + path);
        OnCutsceneStart?.Invoke();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        isPlaying = false;
        Debug.Log("Cutscene finished");

        if (cutsceneUI != null)
            cutsceneUI.SetActive(false);  // ✅ Hide UI

        OnCutsceneEnd?.Invoke();
    }
}