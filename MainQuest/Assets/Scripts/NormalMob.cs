using UnityEngine;

public class NormalMob : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public Rigidbody rb;
    public Transform player;
    public Collider attackCollider; // Assign in inspector (hand, claw, weapon)

    [Header("Movement Settings")]
    public float walkSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float chaseRange = 6f;
    public float attackRange = 2f;

    [Header("Wandering Settings")]
    public float wanderRadius = 5f;
    public float idleTimeMin = 2f;
    public float idleTimeMax = 5f;

    [Header("Vision Settings")]
    public float visionDistance = 8f;   // ✅ only distance now
    public LayerMask visionMask;        // optional raycast check (can remove if not needed)

    [Header("Home Settings")]
    public Transform homePoint;
    public float returnSpeed = 2f;

    private bool isAttacking = false;
    private bool hasSpottedPlayer = false;
    private Vector3 wanderTarget;
    private Vector3 homePosition;

    private enum WanderState { Idle, Walking }
    private WanderState currentState = WanderState.Idle;
    private float stateTimer = 0f;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (attackCollider != null)
            attackCollider.enabled = false;

        homePosition = (homePoint != null) ? homePoint.position : transform.position;

        EnterIdle();
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        // ✅ Just check distance instead of FOV
        hasSpottedPlayer = CanSeePlayer();

        float distance = Vector3.Distance(transform.position, player.position);

        if (hasSpottedPlayer && distance <= attackRange)
        {
            StartCoroutine(DoAttack());
            animator.SetFloat("Speed", 0f);
        }
        else if (hasSpottedPlayer && distance <= chaseRange)
        {
            MoveTowards(player.position, chaseSpeed);
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            float homeDist = Vector3.Distance(transform.position, homePosition);
            if (homeDist > wanderRadius * 2f)
            {
                MoveTowards(homePosition, returnSpeed);
                animator.SetFloat("Speed", 0.5f);
            }
            else
            {
                HandleWandering();
            }
        }
    }

    void HandleWandering()
    {
        stateTimer -= Time.deltaTime;

        if (currentState == WanderState.Idle)
        {
            animator.SetFloat("Speed", 0f);

            if (stateTimer <= 0f)
            {
                PickNewWanderTarget();
                EnterWalking();
            }
        }
        else if (currentState == WanderState.Walking)
        {
            MoveTowards(wanderTarget, walkSpeed);
            animator.SetFloat("Speed", 0.5f);

            if (Vector3.Distance(transform.position, wanderTarget) < 0.5f || stateTimer <= 0f)
            {
                EnterIdle();
            }
        }
    }

    void EnterIdle()
    {
        currentState = WanderState.Idle;
        stateTimer = Random.Range(idleTimeMin, idleTimeMax);
    }

    void EnterWalking()
    {
        currentState = WanderState.Walking;
        stateTimer = Random.Range(2f, 5f);
    }

    void PickNewWanderTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        wanderTarget = new Vector3(homePosition.x + randomCircle.x, homePosition.y, homePosition.z + randomCircle.y);
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 targetPos = new Vector3(target.x, transform.position.y, target.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction.magnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);
    }

    // ✅ Simplified "vision" → distance only
    bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= visionDistance;
    }

    System.Collections.IEnumerator DoAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f); // attack anim duration
        yield return new WaitForSeconds(0.5f); // cooldown
        isAttacking = false;
    }

    // Animation Events
    public void EnableAttackCollider()
    {
        if (attackCollider != null)
            attackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && attackCollider.enabled)
        {
            Debug.Log("Player hit by Normal Mob attack!");
            // TODO: Apply damage
        }
    }
}
