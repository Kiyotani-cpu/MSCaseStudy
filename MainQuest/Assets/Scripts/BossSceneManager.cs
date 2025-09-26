using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class BossSceneManager : MonoBehaviour
{
    public static BossSceneManager Instance;

    private Transform playerTransform;
    private GameObject bossPrefab;
    private MonoBehaviour playerController; // assign movement script dynamically

    [Header("Intro Settings")]
    public VideoPlayer videoPlayer; // Assign in Inspector
    public string videoFileName = "boss_intro.mp4"; // must be inside StreamingAssets

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayBossIntro(Transform player, GameObject boss)
    {
        playerTransform = player;
        bossPrefab = boss;

        // Find player movement script (replace with your actual script type if possible)
        playerController = player.GetComponent<PlayerMovement>()
                        ?? player.GetComponent<PlayerAnimatorController>()
                        ?? player.GetComponent<MonoBehaviour>(); // fallback

        StartCoroutine(PlayIntroVideo());
    }

    private IEnumerator PlayIntroVideo()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer not assigned!");
            yield break;
        }

        // Build video path
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.url = videoPath;
        Debug.Log("Video path: " + videoPlayer.url);

        // Disable player control
        if (playerController != null)
            playerController.enabled = false;

        // Make sure the video object is active
        videoPlayer.gameObject.SetActive(true);

        // Prepare video before playing
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
        Debug.Log("Video started!");

        // Wait until video finishes
        while (videoPlayer.isPlaying)
            yield return null;

        // Hide video after playback
        videoPlayer.gameObject.SetActive(false);

        // Enable boss
        if (bossPrefab != null)
        {
            bossPrefab.SetActive(true);
            Debug.Log("Boss enabled after intro video!");
        }

        // Re-enable player control
        if (playerController != null)
            playerController.enabled = true;
    }
}
