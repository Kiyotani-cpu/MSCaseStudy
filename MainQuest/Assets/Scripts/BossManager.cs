using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("UI References")]
    public GameObject mainUI;
    public GameObject victoryUI;
    public GameObject defeatUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (victoryUI != null)
            victoryUI.SetActive(false); // hide at start
    }

   public void ShowVictoryScreen()
{
    // 1. UI Management: Hide the main game UI and show the Victory UI.
    if (mainUI != null)
        mainUI.SetActive(false);
    if (victoryUI != null)
        victoryUI.SetActive(true);

    // 2. Level Index Calculation
    int currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    int nextLevel = currentLevel + 1;

    // --- FINAL LEVEL CHECK ---
    // Level 3 is the last level, which has a Build Index of 4.
    if (currentLevel == 4) 
    {
        Debug.Log("🎉 Game Completed! All levels finished.");
        
        // Optional: Add logic here to switch to a special "Game Complete" UI 
        // and/or disable interaction buttons on the standard victory UI.
        
        // We stop here to prevent saving '5' as the next unlocked level.
        return; 
    }
    // -------------------------

    // 3. Progress Saving (Only executed if it's NOT the final level)
    // Unlock the next level (nextLevel will be 3 for Level 1, 4 for Level 2)
    if (nextLevel > PlayerPrefs.GetInt("UnlockedLevel", 1))
    {
        PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
    }

    Debug.Log($"Victory UI shown! Unlocked up to level {nextLevel}");
}

    public void ShowDefeatScreen()
    {
        if (mainUI != null)
            mainUI.SetActive(false);
        if (defeatUI != null)
            defeatUI.SetActive(true);
        // Implement defeat UI similarly
        Debug.Log("Defeat UI shown!");
    }

    // Add this method to GameUIManager.cs
    public void LoadNextLevel()
    {
        int nextLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevel);
    }
}
