using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("UI / Text")]
    public DynamicTextData textData; // Assign in Inspector (damage number style)

    [Header("References")]
    public Health health;      // Assign your Health component

    private void Awake()
    {
        // Auto-grab Health if not manually assigned
        if (health == null)
            health = GetComponent<Health>();
    }

    // This can be called by player attacks, projectiles, etc.
    public void TakeHit(int damage, bool isCrit = false)
    {
        if (health != null)
            health.TakeDamage(damage, isCrit);
    }
}
