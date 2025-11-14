using UnityEngine;
using System.Collections;

public class NormalMob : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public Rigidbody rb;
    public Collider attackCollider;

    [Header("Settings")]
    public float walkSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float chaseRange = 6f;
    public float attackRange = 2f;
    public float visionRange = 8f;
    public float wanderRadius = 5f;

    [Header("Home")]
    public Transform homePoint;

    private Transform target;
    private Vector3 homePos;
    private Vector3 wanderPos;
    private bool isAttacking = false;
    private float idleTimer;

    void Start()
    {
        animator = animator ?? GetComponent<Animator>();
        rb = rb ?? GetComponent<Rigidbody>();
        if (attackCollider) attackCollider.enabled = false;

        homePos = homePoint ? homePoint.position : transform.position;
        PickNewWanderPoint();
        idleTimer = Random.Range(2f, 5f);
    }

    void Update()
    {
        if (isAttacking) return;

        FindTarget();

        if (target)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist <= attackRange)
            {
                StartCoroutine(Attack());
            }
            else if (dist <= chaseRange)
            {
                MoveTo(target.position, chaseSpeed);
                animator.SetFloat("Speed", 1f);
            }
            else
            {
                target = null; // lost sight
            }
        }
        else
        {
            Wander();
        }
    }

    void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange);
        float closest = Mathf.Infinity;
        Transform nearest = null;

        foreach (var h in hits)
        {
            Health health = h.GetComponent<Health>();
            if (health && (health.faction == Faction.Player || health.faction == Faction.Summon))
            {
                float d = Vector3.Distance(transform.position, h.transform.position);
                if (d < closest)
                {
                    closest = d;
                    nearest = h.transform;
                }
            }
        }

        target = nearest;
    }

    void Wander()
    {
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f)
        {
            PickNewWanderPoint();
            idleTimer = Random.Range(3f, 6f);
        }

        MoveTo(wanderPos, walkSpeed);
        animator.SetFloat("Speed", 0.5f);

        if (Vector3.Distance(transform.position, wanderPos) < 0.5f)
            idleTimer = 0f; // pick new point
    }

    void PickNewWanderPoint()
    {
        Vector2 rnd = Random.insideUnitCircle * wanderRadius;
        wanderPos = new Vector3(homePos.x + rnd.x, homePos.y, homePos.z + rnd.y);
    }

    void MoveTo(Vector3 targetPos, float speed)
    {
        targetPos.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        Vector3 dir = (targetPos - transform.position).normalized;

        if (dir.magnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1f); // attack animation
        isAttacking = false;
    }

    // Animation Events
    public void EnableAttackCollider() { if (attackCollider) attackCollider.enabled = true; }
    public void DisableAttackCollider() { if (attackCollider) attackCollider.enabled = false; }
}
