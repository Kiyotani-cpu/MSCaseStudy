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
        if (mainUI != null)
            mainUI.SetActive(false);
        if (victoryUI != null)
            victoryUI.SetActive(true);

        // Unlock next level
        int currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int nextLevel = currentLevel + 1;
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
