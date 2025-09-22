using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 20;

    [Header("Ownership")]
    public Faction ownerFaction; // Assign in inspector (Player / Enemy)

    private void OnTriggerEnter(Collider other)
    {
        Health target = other.GetComponent<Health>();

        if (target != null && target.faction != ownerFaction)
        {
            target.TakeDamage(damageAmount);
        }
    }
}
