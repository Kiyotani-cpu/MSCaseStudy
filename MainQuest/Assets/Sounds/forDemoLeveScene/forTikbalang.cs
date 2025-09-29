using UnityEngine;

public class forTikbalang : MonoBehaviour
{
    [Header("BGM Settings")]
    public AudioSource bgmSource;   // Attach your AudioSource here
    public AudioClip bossBGM;       // Drag the BGM clip here
    public float delayBeforeBGM = 2f; // Time to wait before playing

    void Start()
    {
        // Start the coroutine when boss spawns or scene starts
        StartCoroutine(PlayBossBGMWithDelay());
    }

    private System.Collections.IEnumerator PlayBossBGMWithDelay()
    {
        // Wait for the delay
        yield return new WaitForSeconds(delayBeforeBGM);

        // Play the BGM if not already playing
        if (bgmSource != null && bossBGM != null)
        {
            bgmSource.clip = bossBGM;
            bgmSource.Play();
            Debug.Log("Boss BGM started!");
        }
        else
        {
            Debug.LogWarning("BGM Source or Clip is missing!");
        }
    }
}
