using UnityEngine;
public enum Faction
{
    Player,
    Enemy,
    Summon
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
    public bool isTikbalang; // Special case for Tikbalang
    public bool isPlayer;     // Tick if this is the player
    [Header("Floating Text Settings")]
    public Vector3 textBaseOffset = new Vector3(0, 2f, 0);   // Default above head
    public float textRandomRadius = 0.5f;                    // Random horizontal spread
    public event System.Action<int> OnHealthChanged;

    private bool isDead = false;
    public bool IsDead => isDead;
    // Cache reference to text data
    private DynamicTextData textData;

    void Start()
    {
        currentHealth = maxHealth;
        
        OnHealthChanged?.Invoke(currentHealth);
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

        // Apply crit first
        if (isCrit) damage *= 2;

        // Hurt animation
        if (animator != null)
            animator.SetTrigger("Hurt");

        // Reduce health
        // Reduce health
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Notify UI immediately
        OnHealthChanged?.Invoke(currentHealth);

        // Death check
        if (currentHealth <= 0)
            Die();

        // Floating text
        if (textData != null)
        {
            // Add random horizontal spread
            Vector3 randomOffset = new Vector3(
                Random.Range(-textRandomRadius, textRandomRadius),
                0f, // keep Y consistent
                Random.Range(-textRandomRadius, textRandomRadius)
            );

            Vector3 spawnPos = transform.position + textBaseOffset + randomOffset;

            // Crit text
            if (isCrit)
                DynamicTextManager.CreateText(spawnPos + Vector3.up * 0.5f, "CRIT!", textData);

            // Damage text
            DynamicTextManager.CreateText(spawnPos, damage.ToString(), textData);
        }
    }


    void Die()
    {
        if (isDead) return;
        isDead = true;
        OnHealthChanged?.Invoke(0);
        // Play death animation
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
        else if (faction == Faction.Summon)
        {
            // Summon death
            SummonAI summonAI = GetComponent<SummonAI>();
            if (summonAI != null)
            {
                summonAI.Die();
            }
        }
        else if (isTikbalang)
        {
            // Special case for Tikbalang
            TikbalangMiniboss tikbalang = GetComponent<TikbalangMiniboss>();
            tikbalang.enabled = false;
            GameUIManager.Instance.ShowVictoryScreen();
        }
        else // Enemy
        {
            // Disable enemy AI / attack scripts
            NormalMob enemyAI = GetComponent<NormalMob>();
            if (enemyAI != null) enemyAI.enabled = false;

            Destroy(gameObject, 3f); // wait for death anim
        }

    }
}