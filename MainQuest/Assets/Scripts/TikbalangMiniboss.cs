using UnityEngine;

public class TikbalangMiniboss : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public Rigidbody rb;
    public Transform player;
    public Collider kickCollider; // Assign a trigger collider on the leg in inspector

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float chaseRange = 8f;   // When to run
    public float attackRange = 3f;  // Kick distance

    private bool isAttacking = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (kickCollider != null)
            kickCollider.enabled = false; // make sure off by default
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Rotate toward player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction.magnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);

        if (distance <= attackRange)
        {
            // Start attack
            StartCoroutine(DoKick());
            animator.SetFloat("Speed", 0f); // idle during attack
        }
        else if (distance <= chaseRange)
        {
            // Run
            MoveTowardsPlayer(runSpeed);
            animator.SetFloat("Speed", 1f); // full run
        }
        else
        {
            // Walk
            MoveTowardsPlayer(walkSpeed);
            animator.SetFloat("Speed", 0.5f); // mid-value = walk
        }
    }

    void MoveTowardsPlayer(float speed)
    {
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    System.Collections.IEnumerator DoKick()
    {
        isAttacking = true;
        animator.SetTrigger("Kick");

        // Wait until animation finishes (set duration same as anim length)
        yield return new WaitForSeconds(1.2f);

        // small cooldown
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    // Called from Animation Events in Kick clip
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
        if (other.CompareTag("Player") && kickCollider.enabled)
        {
            Debug.Log("Player hit by Tikbalang kick!");
        }
    }
}
