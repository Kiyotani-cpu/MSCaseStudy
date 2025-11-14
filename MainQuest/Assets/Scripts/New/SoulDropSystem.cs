using UnityEngine;

public class SoulDropSystem : MonoBehaviour
{
    [Header("Soul Drop Settings")]
    public GameObject soulPrefab;
    public int minSouls = 1;
    public int maxSouls = 3;
    public float soulDropRadius = 2f;
    
    [Header("Drop Settings")]
    public bool isBoss = false;
    public Card[] bossCards; // Assign your 3 cards here for bosses
    
    private Health health;
    
    void Start()
    {
        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath += OnDeath;
        }
    }

    void OnDeath()
    {
        DropSouls();
    }

    void DropSouls()
    {
        if (soulPrefab == null) return;

        int soulCount = Random.Range(minSouls, maxSouls + 1);
        
        for (int i = 0; i < soulCount; i++)
        {
            CreateSoul(i);
        }
    }

    void CreateSoul(int index)
    {
        Vector2 randomCircle = Random.insideUnitCircle * soulDropRadius;
        Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 1f, randomCircle.y);
        
        GameObject soul = Instantiate(soulPrefab, spawnPos, Quaternion.identity);
        SoulCollectible soulScript = soul.GetComponent<SoulCollectible>();
        
        if (soulScript != null)
        {
            // Bosses drop cards, normal mobs drop XP
            if (isBoss && bossCards.Length > 0)
            {
                Card randomCard = bossCards[Random.Range(0, bossCards.Length)];
                soulScript.SetCardDrop(randomCard);
            }
            else
            {
                soulScript.SetXPDrop(10); // Normal mobs give 10 XP
            }
        }

        Destroy(soul, 30f); // Souls disappear after 30 seconds
    }
}