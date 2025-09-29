using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBGMController : MonoBehaviour
{
    [Header("BGM Settings")]
    public AudioSource audioSource;       // Attach AudioSource
    public AudioClip normalBGM;           // Music at the start
    public AudioClip bossBGM;             // Music when Tikbalang appears
    public AudioClip victoryBGM;          // Music after victory

    [Header("Boss Reference")]
    public GameObject tikbalang;          // Drag your Tikbalang object here

    private bool bossMusicPlaying = false;
    private bool victoryMusicPlaying = false;

    void Start()
    {
        // Play the normal BGM at the start (loop ON)
        PlayMusic(normalBGM, true);
        Debug.Log("Normal BGM started.");
    }

    void Update()
    {
        // If Tikbalang is active, switch to boss music (loop ON)
        if (tikbalang != null && tikbalang.activeInHierarchy && !bossMusicPlaying && !victoryMusicPlaying)
        {
            PlayMusic(bossBGM, true);
            bossMusicPlaying = true;
            Debug.Log("Boss BGM started!");
        }

        // If Tikbalang is deactivated after fight, switch to victory music (loop OFF)
        if (tikbalang != null && !tikbalang.activeInHierarchy && bossMusicPlaying && !victoryMusicPlaying)
        {
            PlayMusic(victoryBGM, false); // loop = false
            victoryMusicPlaying = true;
            Debug.Log("Victory BGM started!");
        }
    }

    private void PlayMusic(AudioClip clip, bool loop)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.loop = loop; //  loop is set here
            audioSource.Play();
        }
    }
}
