using UnityEngine;
using System.Collections;

public class BGMController : MonoBehaviour
{
    public AudioSource bgmSource;      // Your BGM AudioSource
    public float fadeDuration = 1f;    // Seconds to fade in/out

    private float originalVolume;
    private Coroutine fadeCoroutine;

    void Start()
    {
        if (bgmSource != null)
            originalVolume = bgmSource.volume;
    }

    public void MuteBGM()
    {
        if (bgmSource == null) return;
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeVolume(0f));
    }

    public void UnmuteBGM()
    {
        if (bgmSource == null) return;
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeVolume(originalVolume));
    }

    private IEnumerator FadeVolume(float targetVolume)
    {
        float startVolume = bgmSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / fadeDuration);
            yield return null;
        }

        bgmSource.volume = targetVolume;
    }
}
