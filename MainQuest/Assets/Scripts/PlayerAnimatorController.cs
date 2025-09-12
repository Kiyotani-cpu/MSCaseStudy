using UnityEngine;
using UnityEngine.UI;
using Terresquall;

public class PlayerAnimatorController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button rollButton;
    [SerializeField] private Button weaponButton;

    [Header("Components")]
    public Animator animator;
    public Rigidbody rb;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Roll Settings")]
    public float rollSpeed = 8f;
    private bool isRolling = false;
    private float rollDuration = 0.8f;
    private float rollTimer = 0f;
    public float rollCooldown = 1.5f;
    private float rollCooldownTimer = 0f;
    public bool IsEvading { get; private set; } = false;

    [Header("Attack Settings")]
    public float attackCooldown = 0.8f;
    private bool isAttacking = false;
    private float attackTimer = 0f;

    [Header("Weapon Settings")]
    public bool IsWeaponDrawn = false;
    [SerializeField] private GameObject swordInHand;

    private bool isBusy = false;

    void Start()
    {
        // Hook up UI buttons to their actions
        if (attackButton != null)
            attackButton.onClick.AddListener(() => TryAttack());

        if (rollButton != null)
            rollButton.onClick.AddListener(() => TryRoll());

        if (weaponButton != null)
            weaponButton.onClick.AddListener(() => ToggleWeapon());
    }

    void Update()
    {
        HandleRollCooldown();

        if (isRolling)
            HandleRoll();
        else
        {
            HandleMovement();
        }

        HandleAttackCooldown();
    }

    void HandleAttackCooldown()
    {
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                isAttacking = false;
                animator.ResetTrigger("Attack");
            }
        }
    }

    void TryAttack()
    {
        if (!isBusy && !isAttacking && IsWeaponDrawn)
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

    void HandleMovement()
    {
        if (isRolling || isBusy) return;

        float inputX = VirtualJoystick.GetAxis("Horizontal");
        float inputZ = VirtualJoystick.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(inputX, 0f, inputZ).normalized;

        float speed = moveDirection.magnitude;
        animator.SetFloat("Speed", speed);

        if (speed > 0f)
        {
            Vector3 moveVelocity = moveDirection * moveSpeed;
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
    }

    void HandleRollCooldown()
    {
        if (rollCooldownTimer > 0f)
            rollCooldownTimer -= Time.deltaTime;
    }

    void HandleRoll()
    {
        rollTimer += Time.deltaTime;
        if (rollTimer >= rollDuration)
        {
            isRolling = false;
            animator.SetBool("IsRolling", false);
            IsEvading = false;
            rb.velocity = Vector3.zero;
            isBusy = false;
        }
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
        Vector3 rollDirection = transform.forward;
        rb.velocity = rollDirection * rollSpeed;
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
        if (isBusy) return;
        isBusy = true;
        animator.SetTrigger("Sheath");
        IsWeaponDrawn = false;
        animator.SetBool("IsWeaponDrawn", false);
        UnequipSword();
    }

    public void UnsheathWeapon()
    {
        if (isBusy) return;
        isBusy = true;
        animator.SetTrigger("Unsheath");
        IsWeaponDrawn = true;
        animator.SetBool("IsWeaponDrawn", true);
        EquipSword();
    }

    public void EquipSword() => swordInHand.SetActive(true);
    public void UnequipSword() => swordInHand.SetActive(false);

    public void EndAction()
    {
        isBusy = false;
        Debug.Log("Action ended, controls unlocked.");
    }
}
