using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerLevel : MonoBehaviour
{
    [Header("Level Settings")]
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public float xpMultiplier = 1.5f;

    [Header("Kill Tracking")]
    public Dictionary<string, int> enemyKills = new Dictionary<string, int>();

    // Events
    public static event Action<int> OnLevelUp;
    public static event Action<int, int> OnXPChanged;
    public static event Action<string, int> OnEnemyKilled; // enemyType, xpGained

    public void AddXP(int amount, string enemyType = "Unknown")
    {
        currentXP += amount;
        
        // Track kills
        if (enemyKills.ContainsKey(enemyType))
            enemyKills[enemyType]++;
        else
            enemyKills.Add(enemyType, 1);
        
        Debug.Log($"💀 Defeated {enemyType}! +{amount} XP");
        
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
        OnEnemyKilled?.Invoke(enemyType, amount);
        
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);
        
        Debug.Log($"🎉 Level Up! Now level {currentLevel}");
        
        ApplyLevelUpBenefits();
        OnLevelUp?.Invoke(currentLevel);
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);

        // Check for multiple level-ups
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void ApplyLevelUpBenefits()
    {
        Health playerHealth = GetComponent<Health>();
        if (playerHealth != null) 
        {
            playerHealth.IncreaseMaxHealth(10);
            playerHealth.Heal(10);
        }
    }
}