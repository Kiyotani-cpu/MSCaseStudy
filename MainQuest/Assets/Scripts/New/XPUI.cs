using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider xpBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public GameObject levelUpEffect; // Optional particle effect

    void Start()
    {
        // Subscribe to events
        PlayerLevel.OnLevelUp += OnLevelUp;
        PlayerLevel.OnXPChanged += OnXPChanged;
        
        // Initialize UI
        PlayerLevel playerLevel = FindObjectOfType<PlayerLevel>();
        if (playerLevel != null)
        {
            UpdateUI(playerLevel.currentLevel, playerLevel.currentXP, playerLevel.xpToNextLevel);
        }
    }

    void OnLevelUp(int newLevel)
    {
        // Update level text
        if (levelText != null)
            levelText.text = $"Level {newLevel}";
        
        // Show level up effect
        if (levelUpEffect != null)
            Instantiate(levelUpEffect, transform.position, Quaternion.identity);
        
        Debug.Log($"UI: Level updated to {newLevel}");
    }

    void OnXPChanged(int currentXP, int xpToNextLevel)
    {
        UpdateUI(0, currentXP, xpToNextLevel); // 0 for level since we don't have it here
    }

    void UpdateUI(int level, int currentXP, int xpToNextLevel)
    {
        // Update XP bar
        if (xpBar != null)
        {
            xpBar.maxValue = xpToNextLevel;
            xpBar.value = currentXP;
        }
        
        // Update XP text
        if (xpText != null)
            xpText.text = $"{currentXP}/{xpToNextLevel}";
        
        // Update level text if provided
        if (level > 0 && levelText != null)
            levelText.text = $"Level {level}";
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        PlayerLevel.OnLevelUp -= OnLevelUp;
        PlayerLevel.OnXPChanged -= OnXPChanged;
    }
}