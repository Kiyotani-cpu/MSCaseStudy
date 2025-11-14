using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public Rigidbody rb;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Roll Settings")]
    public float rollSpeed = 8f;
    public float rollDistance = 5f;
    public float rollDuration = 0.8f;
    public float rollCooldown = 1.5f;

    private bool isRolling = false;
    private float rollTimer = 0f;
    private float rollCooldownTimer = 0f;
    private Vector3 rollStartPos;
    private Vector3 rollTargetPos;

    public bool IsEvading { get; private set; } = false;

    [Header("Attack Settings")]
    public float attackCooldown = 0.8f;
    private bool isAttacking = false;
    private float attackTimer = 0f;

    [Header("Weapon Settings")]
    [SerializeField]
    private Collider swordCollider;

    [SerializeField]
    private GameObject swordInHand;
    public bool IsWeaponDrawn = false;

    private bool isBusy = false;

    void Start()
    {
        DisableSwordCollider();
        UnequipSword();
    }

    void Update()
    {
        HandleCooldowns();

        if (isRolling)
            HandleRoll();
        else if (!isBusy)
            HandleMovement();

        HandleInputs();
    }

    void HandleInputs()
    {
        // Attack
        if (IsWeaponDrawn && Input.GetMouseButtonDown(0))
            TryAttack();

        // Roll
        if (Input.GetKeyDown(KeyCode.Space))
            TryRoll();

        // Toggle Weapon
        if (Input.GetKeyDown(KeyCode.Q))
            ToggleWeapon();
    }

    void HandleCooldowns()
    {
        if (rollCooldownTimer > 0f)
            rollCooldownTimer -= Time.deltaTime;
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                isAttacking = false;
                animator.ResetTrigger("Attack");
                EndAction();
            }
        }
    }

    void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 camForward = Vector3
            .Scale(Camera.main.transform.forward, new Vector3(1, 0, 1))
            .normalized;
        Vector3 camRight = Camera.main.transform.right;
        Vector3 moveDirection = (camForward * inputZ + camRight * inputX).normalized;

        float speed = moveDirection.magnitude;
        animator.SetFloat("Speed", speed);

        if (speed > 0.1f)
        {
            Vector3 targetVelocity = moveDirection * moveSpeed;
            rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
    }

    void TryAttack()
    {
        if (!isBusy && !isAttacking)
            StartAttack();
    }

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = 0f;
        isBusy = true;

        animator.SetTrigger("Attack");
        rb.velocity = Vector3.zero;
    }

    void TryRoll()
    {
        if (!isBusy && rollCooldownTimer <= 0f)
            StartRoll();
    }

    void StartRoll()
    {
        isRolling = true;
        rollTimer = 0f;
        rollCooldownTimer = rollCooldown;

        animator.SetBool("IsRolling", true);
        IsEvading = true;
        isBusy = true;

        rollStartPos = transform.position;
        rollTargetPos = transform.position + transform.forward * rollDistance;
    }

    void HandleRoll()
    {
        rollTimer += Time.deltaTime;
        float t = rollTimer / rollDuration;
        transform.position = Vector3.Lerp(rollStartPos, rollTargetPos, t);

        if (rollTimer >= rollDuration)
        {
            isRolling = false;
            animator.SetBool("IsRolling", false);
            IsEvading = false;
            rb.velocity = Vector3.zero;
            EndAction();
        }
    }

    void ToggleWeapon()
    {
        if (IsWeaponDrawn)
            SheathWeapon();
        else
            UnsheathWeapon();
    }

    public void SheathWeapon()
    {
        if (isBusy)
            return;
        isBusy = true;
        animator.SetTrigger("Sheath");
        IsWeaponDrawn = false;
        animator.SetBool("IsWeaponDrawn", false);
    }

    public void UnsheathWeapon()
    {
        if (isBusy)
            return;
        isBusy = true;
        animator.SetTrigger("Unsheath");
        IsWeaponDrawn = true;
        animator.SetBool("IsWeaponDrawn", true);
    }

    // Animation Event Functions
    public void EquipSword() => swordInHand.SetActive(true);

    public void UnequipSword() => swordInHand.SetActive(false);

    public void EnableSwordCollider()
    {
        if (swordCollider)
            swordCollider.enabled = true;
    }

    public void DisableSwordCollider()
    {
        if (swordCollider)
            swordCollider.enabled = false;
    }

    public void EndAction()
    {
        isBusy = false;
        Debug.Log("Action ended, controls unlocked.");
    }
}
