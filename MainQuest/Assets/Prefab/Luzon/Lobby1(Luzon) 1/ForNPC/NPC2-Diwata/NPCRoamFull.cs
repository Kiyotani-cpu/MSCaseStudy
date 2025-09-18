using UnityEngine;
using UnityEngine.AI;

public class NPCRoamFull : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float roamRange = 10f;         // roaming radius
    [SerializeField] float waitTime = 2f;           // seconds to wait before moving again
    [SerializeField] float detectionRange = 2f;     // distance to detect walls
    [SerializeField] LayerMask obstacleLayer;       // walls/obstacles

    [Header("Animation")]
    [SerializeField] Animator animator;             // NPC Animator

    private NavMeshAgent agent;
    private Vector3 destPoint;
    private bool walkpointSet;
    private float waitTimer;
    private float wallAvoidCooldown;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
            Debug.LogError("No NavMeshAgent on " + gameObject.name);

        if (animator == null)
            animator = GetComponent<Animator>(); // auto grab if not set
    }

    void Update()
    {
        Patrol();
        WallCheck();
        HandleAnimations();

        if (waitTimer > 0)
            waitTimer -= Time.deltaTime;

        if (wallAvoidCooldown > 0)
            wallAvoidCooldown -= Time.deltaTime;
    }

    void Patrol()
    {
        if (!walkpointSet && waitTimer <= 0)
            SearchForDest();

        if (walkpointSet && agent.enabled)
            agent.SetDestination(destPoint);

        if (walkpointSet && !agent.pathPending && agent.remainingDistance < 1.5f)
        {
            walkpointSet = false;
            waitTimer = waitTime; // pause before next roam
        }
    }

    void WallCheck()
    {
        // Raycast from chest height forward
        if (wallAvoidCooldown <= 0 &&
            Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, detectionRange, obstacleLayer))
        {
            // Rotate NPC naturally left or right
            float randomTurn = Random.value > 0.5f ? 90f : -90f;
            transform.Rotate(Vector3.up * randomTurn);

            CancelPath();
            SearchForDest();

            wallAvoidCooldown = 1f; // prevent flickering
        }
    }

    void SearchForDest()
    {
        float z = Random.Range(-roamRange, roamRange);
        float x = Random.Range(-roamRange, roamRange);

        Vector3 potentialPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (NavMesh.SamplePosition(potentialPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            destPoint = hit.position;
            walkpointSet = true;
        }
    }

    void CancelPath()
    {
        agent.ResetPath();
        walkpointSet = false;
    }

    void HandleAnimations()
    {
        // Check if NPC is moving
        bool isWalking = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isWalking);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 1f, transform.forward * detectionRange);
    }
}
