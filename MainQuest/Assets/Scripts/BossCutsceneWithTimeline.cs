using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossCutsceneWithTimeline : MonoBehaviour
{
    [Header("Timeline")]
    public PlayableDirector director; // assign CutsceneDirector's PlayableDirector

    [Header("Boss Movement/Anim")]
    public NavMeshAgent bossAgent;
    public Transform stopPoint;
    public Animator bossAnimator;

    [Header("Settings")]
    public float walkTimeout = 10f; // fallback if navmesh stalls
    public string bossNameInMainScene = "Tikbalang MiniBoss"; // name of boss object in main scene

    void Start()
    {
        StartCoroutine(CutsceneRoutine());
    }

    IEnumerator CutsceneRoutine()
    {
        // Start walk animation and movement
        bossAnimator.SetTrigger("Walk");
        bossAgent.isStopped = false;
        bossAgent.SetDestination(stopPoint.position);

        // Start the camera timeline
        if (director != null) director.Play();

        // Wait until boss reaches stop point or timeout
        float t = 0f;
        while (Vector3.Distance(bossAgent.transform.position, stopPoint.position) > bossAgent.stoppingDistance + 0.1f && t < walkTimeout)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // stop movement and play roar animation
        bossAgent.isStopped = true;
        bossAnimator.SetTrigger("Roar");

        // Wait for timeline to finish
        if (director != null)
            yield return new WaitWhile(() => director.state == PlayState.Playing);

        // Small buffer
        yield return new WaitForSeconds(0.5f);

        // Return to original scene
        string returnScene = PlayerPrefs.GetString("ReturnScene", "MainScene");
        SceneManager.sceneLoaded += OnMainSceneLoaded;
        SceneManager.LoadScene(returnScene);
    }

    void OnMainSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Enable boss when we’re back
        GameObject boss = GameObject.Find(bossNameInMainScene);
        if (boss != null)
        {
            boss.SetActive(true);
            Debug.Log("🔥 Boss enabled after cutscene!");
        }

        SceneManager.sceneLoaded -= OnMainSceneLoaded;
    }
}
