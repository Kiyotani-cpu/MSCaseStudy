using UnityEngine;
using UnityEngine.AI;

public class SummonAI : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;

    [Header("References")]
    public Transform player;
    public float followDistance = 2f;
    public float idleRadius = 1.5f;
    public float detectionRadius = 10f;
    public float attackRange = 2f;

    [Header("Combat Settings")]
    public float attackCooldown = 1.5f;
    private float attackTimer = 0f;
    public Collider attackCollider;        // ✅ assign collider on weapon/hand
    private Transform currentEnemy;
    private Vector3 formationOffset;

    private enum State { Follow, IdleNearPlayer, Chase, Attack, Dead }
    private State currentState = State.Follow;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        Vector2 circle = Random.insideUnitCircle.normalized * followDistance;
        formationOffset = new Vector3(circle.x, 0f, circle.y);

        if (attackCollider != null)
            attackCollider.enabled = false; // start disabled
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        FindClosestEnemy();

        switch (currentState)
        {
            case State.Follow:
            case State.IdleNearPlayer:
                HandleFollowOrIdle();
                break;
            case State.Chase:
                HandleChase();
                break;
            case State.Attack:
                HandleAttack();
                break;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void HandleFollowOrIdle()
    {
        if (currentEnemy != null)
        {
            currentState = State.Chase;
            return;
        }

        Vector3 targetPos = player.position + formationOffset;
        float dist = Vector3.Distance(transform.position, targetPos);

        if (dist > followDistance + 0.5f)
        {
            agent.SetDestination(targetPos);
            currentState = State.Follow;
        }
        else
        {
            if (agent.remainingDistance < 0.5f)
            {
                Vector3 offset = Random.insideUnitSphere * idleRadius;
                offset.y = 0f;
                Vector3 idlePos = targetPos + offset;
                agent.SetDestination(idlePos);
            }
            currentState = State.IdleNearPlayer;
        }
    }

    void HandleChase()
    {
        if (currentEnemy == null)
        {
            currentState = State.Follow;
            return;
        }

        float dist = Vector3.Distance(transform.position, currentEnemy.position);

        if (dist > detectionRadius)
        {
            currentEnemy = null;
            currentState = State.Follow;
        }
        else if (dist <= attackRange)
        {
            currentState = State.Attack;
            agent.ResetPath();
        }
        else
        {
            agent.SetDestination(currentEnemy.position);
        }
    }

    void HandleAttack()
    {
        if (currentEnemy == null)
        {
            currentState = State.Follow;
            return;
        }

        transform.LookAt(currentEnemy);

        float dist = Vector3.Distance(transform.position, currentEnemy.position);

        if (dist > attackRange)
        {
            currentState = State.Chase;
            return;
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            animator.SetTrigger("Attack"); // triggers animation events
            attackTimer = attackCooldown;
        }
    }
    // Use if the Enemy dont have a NavMeshAgent
    // void FindClosestEnemy()
    // {
    //     Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, LayerMask.GetMask("Enemy"));

    //     float closestDist = Mathf.Infinity;
    //     Transform closest = null;

    //     foreach (Collider hit in hits)
    //     {
    //         float dist = Vector3.Distance(transform.position, hit.transform.position);
    //         if (dist < closestDist)
    //         {
    //             closestDist = dist;
    //             closest = hit.transform;
    //         }
    //     }

    //     currentEnemy = closest;
    // }
    void FindClosestEnemy()
    {
        // Find all enemies in the scene (tagged as "Enemy")
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (var enemy in allEnemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < detectionRadius && dist < closestDist)
            {
                closestDist = dist;
                closest = enemy.transform;
            }
        }

        currentEnemy = closest;
    }

    // ✅ Animation Events
    public void EnableAttackCollider()
    {
        if (currentState == State.Dead) return;
        if (attackCollider != null)
            attackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        if (attackCollider != null)
            attackCollider.enabled = false;
    }
    public void Die()
    {
        if (currentState == State.Dead) return;

        currentState = State.Dead;
        agent.isStopped = true;

        DisableAttackCollider();

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Death");
        }

        Destroy(gameObject, 3f);
    }


}
