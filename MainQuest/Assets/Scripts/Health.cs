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
    public event System.Action OnDeath; // Added OnDeath event

    private bool isDead = false;
    public bool IsDead => isDead;
    // Cache reference to text data
    private DynamicTextData textData;
    private PlayerLevel playerlevel;
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
        OnDeath?.Invoke();

        if (animator != null)
            animator.SetTrigger("Die");

        if (isPlayer)
        {
            PlayerAnimatorController controller = GetComponent<PlayerAnimatorController>();
            if (controller != null)
                controller.enabled = false;
            GameUIManager.Instance.ShowDefeatScreen();
        }
        else if (faction == Faction.Summon)
        {
            SummonAI summonAI = GetComponent<SummonAI>();
            if (summonAI != null)
                summonAI.Die();
        }
        else if (isTikbalang)
        {
            TikbalangMiniboss tikbalang = GetComponent<TikbalangMiniboss>();
            tikbalang.enabled = false;
            GiveXPToPlayer(200, "Tikbalang Boss");
            GameUIManager.Instance.ShowVictoryScreen();
        }
        else // Enemy
        {
            string enemyType = gameObject.name;
            int xpAmount = GetXPReward();
            GiveXPToPlayer(xpAmount, enemyType);

            NormalMob enemyAI = GetComponent<NormalMob>();
            if (enemyAI != null) enemyAI.enabled = false;

            Destroy(gameObject, 3f);
        }
    }

    private void GiveXPToPlayer(int xpAmount, string enemyType)
    {
        PlayerLevel playerLevel = FindObjectOfType<PlayerLevel>();
        if (playerLevel != null)
        {
            playerLevel.AddXP(xpAmount, enemyType);
        }
    }

    private int GetXPReward()
    {
        // Your logic for determining XP based on enemy type
        return gameObject.name switch
        {
            string name when name.Contains("Tikbalang") => 100,
            string name when name.Contains("Aswang") => 20,
            _ => 15
        };
    }

    // New method to increase max health
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount; // Also heal the increased amount

        OnHealthChanged?.Invoke(currentHealth);

        Debug.Log($"Max health increased by {amount}. New max: {maxHealth}");
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);

        // Show healing text - FIXED: Using only 3 arguments
        if (textData != null)
        {
            Vector3 spawnPos = transform.position + textBaseOffset;
            // Create a new text data for healing color or use existing one
            DynamicTextManager.CreateText(spawnPos, $"+{amount}", textData);
        }
    }
}