using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWalkingAround : MonoBehaviour
{
    public float roamRadius = 10f;
    public float roamDelay = 3f;

    private UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;
    private float timer;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>(); // Grab the Animator
        timer = roamDelay;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Inside Update()
        float movementSpeed = agent.velocity.magnitude;
        animator.SetFloat("Speed", movementSpeed);

        // Optional: scale animation playback so footsteps match
        animator.speed = movementSpeed > 0.1f ? movementSpeed / agent.speed : 1f;

        // Movement logic
        if (timer >= roamDelay)
        {
            Vector3 newPos = RandomNavSphere(transform.position, roamRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }

        // Animation control
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    }

    // Random roaming point
    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        UnityEngine.AI.NavMeshHit navHit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }
}
