using UnityEngine;

[System.Serializable]
public class XPReward : MonoBehaviour
{
    [Header("XP Settings")]
    public int baseXP = 30;
    public bool scaleWithLevel = true;
    public int level = 1;
    
    [Header("Multipliers")]
    public float eliteMultiplier = 2f;
    public float bossMultiplier = 5f;
    
    public int GetXPReward()
    {
        int xp = baseXP;
        
        // Scale with level
        if (scaleWithLevel)
        {
            xp = Mathf.RoundToInt(xp * (1 + (level - 1) * 0.2f)); // 20% more per level
        }
        
        // Apply multipliers based on enemy type
        if (gameObject.CompareTag("Elite"))
            xp = Mathf.RoundToInt(xp * eliteMultiplier);
        else if (gameObject.CompareTag("Boss"))
            xp = Mathf.RoundToInt(xp * bossMultiplier);
            
        return xp;
    }
}