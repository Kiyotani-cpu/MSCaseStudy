using UnityEngine;
using System.Collections;

public class TikbalangMiniboss : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public Rigidbody rb;
    public Transform player;
    public Collider kickCollider; // Trigger collider on leg

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float chaseRange = 8f;   // Start running
    public float attackRange = 3f;  // Kick distance
    public float wanderRadius = 5f; // Max distance from spawn for wandering
    public float wanderSpeed = 1.5f;

    [Header("Attack Settings")]
    public float kickDuration = 1.2f;
    public float attackCooldown = 0.5f;

    private bool isAttacking = false;
    private Vector3 spawnPosition;
    private Vector3 wanderTarget;
    private bool isWandering = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (kickCollider != null)
            kickCollider.enabled = false;

        spawnPosition = transform.position;
        wanderTarget = transform.position;
    }

    void Update()
    {
        // If player is null, dead, or destroyed
        bool playerDeadOrMissing = (player == null || player.GetComponent<Health>()?.IsDead == true);

        if (playerDeadOrMissing)
        {
            Wander();
            return;
        }

        if (isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Rotate toward player
        RotateTowards(player.position);

        if (distance <= attackRange)
        {
            StartCoroutine(DoKick());
            animator.SetFloat("Speed", 0f);
        }
        else if (distance <= chaseRange)
        {
            MoveTowards(player.position, runSpeed);
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            MoveTowards(player.position, walkSpeed);
            animator.SetFloat("Speed", 0.5f);
        }
    }

    void RotateTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position);
        direction.y = 0;
        if (direction.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);
    }

    void MoveTowards(Vector3 targetPos, float speed)
    {
        Vector3 movePos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
        transform.position = Vector3.MoveTowards(transform.position, movePos, speed * Time.deltaTime);
    }

    System.Collections.IEnumerator DoKick()
    {
        isAttacking = true;
        animator.SetTrigger("Kick");

        yield return new WaitForSeconds(kickDuration);
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    // Called from animation events
    public void EnableKickCollider()
    {
        if (kickCollider != null)
            kickCollider.enabled = true;
    }

    public void DisableKickCollider()
    {
        if (kickCollider != null)
            kickCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (kickCollider != null && kickCollider.enabled && other.CompareTag("Player"))
        {
            Debug.Log("Player hit by Tikbalang kick!");
        }
    }

    // Wandering behavior when player is dead
    void Wander()
    {
        animator.SetFloat("Speed", 0.5f);

        if (!isWandering || Vector3.Distance(transform.position, wanderTarget) < 0.2f)
        {
            wanderTarget = spawnPosition + new Vector3(
                Random.Range(-wanderRadius, wanderRadius),
                0f,
                Random.Range(-wanderRadius, wanderRadius)
            );
            isWandering = true;
        }

        MoveTowards(wanderTarget, wanderSpeed);
        RotateTowards(wanderTarget);
    }
}
