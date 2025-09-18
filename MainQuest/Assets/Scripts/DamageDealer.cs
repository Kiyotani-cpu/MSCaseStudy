using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 20;
    public bool destroyOnHit = false; // Useful for projectiles

    [Header("Ownership")]
    public Faction ownerFaction; // Assign in inspector (Player / Enemy)

    private void OnTriggerEnter(Collider other)
    {
        Health target = other.GetComponent<Health>();

        if (target != null)
        {
            if (target.faction != ownerFaction)
            {
                target.TakeDamage(damageAmount);

                if (destroyOnHit)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
