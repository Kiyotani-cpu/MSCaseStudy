using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using System.Collections;

public class MiniBossCutScene : MonoBehaviour
{
    [Header("Timeline")]
    public PlayableDirector director; // assign CutsceneDirector's PlayableDirector

    [Header("Boss Movement/Anim")]
    public NavMeshAgent bossAgent;
    public Transform stopPoint;
    public Animator bossAnimator;

    [Header("Settings")]
    public float walkTimeout = 10f; // fallback if navmesh stalls

    // call this to start the full cutscene
    public void Start()
    {
        StartCoroutine(CutsceneRoutine());
    }

    IEnumerator CutsceneRoutine()
    {
        // Start walk animation and movement
        bossAnimator.SetTrigger("Walk");
        bossAgent.isStopped = false;
        bossAgent.SetDestination(stopPoint.position);

        // Start the camera timeline (starts playing Cinemachine shots)
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

        // (optional) wait for timeline to finish before doing anything else
        if (director != null)
            yield return new WaitWhile(() => director.state == PlayState.Playing);
        else
            yield return null;
    }
}
