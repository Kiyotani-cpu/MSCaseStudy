using UnityEngine;
using Terresquall;
using UnityEngine.InputSystem;

public class PlayerAnimatorController : MonoBehaviour
{
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

 
    // NEW: General lock for movement/inputs
    private bool isBusy = false;

    void Update()
    {
        HandleRollCooldown();

        if (isRolling)
        {
            HandleRoll();
        }
        else
        {
            HandleInputs();
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
        if (!isBusy && !isAttacking && IsWeaponDrawn && Input.GetMouseButtonDown(0))
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = 0f;
        isBusy = true; // lock movement
        animator.SetTrigger("Attack");
        rb.velocity = Vector3.zero;
    }

    void HandleMovement()
    {
        if (isRolling || isBusy) return; // can't move if busy

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
            isBusy = false; // unlock controls after roll ends
        }
    }

    void StartRoll()
    {
        if (isBusy) return; // can't roll if busy
        isRolling = true;
        rollTimer = 0f;
        rollCooldownTimer = rollCooldown;
        animator.SetBool("IsRolling", true);
        IsEvading = true;
        isBusy = true; // lock controls during roll
        Vector3 rollDirection = transform.forward;
        rb.velocity = rollDirection * rollSpeed;
    }

    void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space) && rollCooldownTimer <= 0f)
        {
            StartRoll();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (IsWeaponDrawn)
                SheathWeapon();
            else
                UnsheathWeapon();
        }
    }

    public void SheathWeapon()
    {
        if (isBusy) return;
        isBusy = true; // lock controls
        animator.SetTrigger("Sheath");
        IsWeaponDrawn = false;
        animator.SetBool("IsWeaponDrawn", false);
        UnequipSword();
    }

    public void UnsheathWeapon()
    {
        if (isBusy) return;
        isBusy = true; // lock controls
        animator.SetTrigger("Unsheath");
        IsWeaponDrawn = true;
        animator.SetBool("IsWeaponDrawn", true);
        EquipSword();
    }

    public void EquipSword()
    {
        swordInHand.SetActive(true);
    }

    public void UnequipSword()
    {
        swordInHand.SetActive(false);
    }


    public void EndAction()
    {
        isBusy = false;
        Debug.Log("Action ended, controls unlocked.");
    }
}
