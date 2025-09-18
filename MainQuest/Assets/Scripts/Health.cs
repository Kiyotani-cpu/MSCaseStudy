using UnityEngine;
public enum Faction
{
    Player,
    Enemy
}

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    [Header("Faction Settings")]
    public Faction faction;
    [Header("References")]
    public Animator animator; // Assign in Inspector
    public bool isPlayer;     // Tick if this is the player

    private bool isDead = false;

    // Cache reference to text data
    private DynamicTextData textData;

    void Start()
    {
        currentHealth = maxHealth;

        // Try to get textData from either Player or Enemy
        if (isPlayer)
        {
            Player player = GetComponent<Player>();
            if (player != null) textData = player.textData;
        }
        else
        {
            Enemy enemy = GetComponent<Enemy>();
            if (enemy != null) textData = enemy.textData;
        }
    }

    public void TakeDamage(int damage, bool isCrit = false)
    {
        if (isDead) return;

        // Hurt animation
        if (animator != null)
            animator.SetTrigger("Hurt");

        // Floating text
        if (textData != null)
        {
            Vector3 destination = transform.position + Vector3.up * 2f;

            if (isCrit)
            {
                damage *= 2;
                DynamicTextManager.CreateText(destination + Vector3.up * 0.5f, "CRIT!", textData);
            }
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            DynamicTextManager.CreateText(destination, damage.ToString(), textData);
        }

        // Death check
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Die");

        if (isPlayer)
        {
            // Disable player controller when dead
            PlayerAnimatorController controller = GetComponent<PlayerAnimatorController>();
            if (controller != null)
                controller.enabled = false;

            Debug.Log("Player is dead!");
        }
        else
        {
            // Enemy dies after delay
            Destroy(gameObject, 5f);
        }
    }
}
