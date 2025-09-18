using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OLD1AnimationController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    private float talkDuration = 100f; // seconds
    private float talkTimer = 0f;
    private bool isTalking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Press E once to start talking
        if (Input.GetKeyDown(KeyCode.E) && !isTalking)
        {
            StartTalking();
        }

        // Countdown timer
        if (isTalking)
        {
            talkTimer -= Time.deltaTime;
            if (talkTimer <= 0f)
            {
                StopTalking();
            }
        }
    }

    void StartTalking()
    {
        animator.SetBool("isTalking", true);
        isTalking = true;
        talkTimer = talkDuration;
    }

    void StopTalking()
    {
        animator.SetBool("isTalking", false);
        isTalking = false;
    }
}
