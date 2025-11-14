using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel; // Assign in Inspector

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false); // hide on start
    }

    void Update()
    {
        // ESC key handling is now in PlayerCameraController to avoid conflicts
        // This method is kept for other input methods if needed
    }

    // Call this when pause button is clicked
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f; // freeze game
        isPaused = true;
        
        // Show cursor when paused
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f; // unfreeze
        isPaused = false;
        
        // Hide cursor when resuming
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void NewGame()
    {
        Time.timeScale = 1f; // reset time
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // reload current scene
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f; // reset time
        SceneManager.LoadScene("Menu"); // replace with your main menu scene name
    }

    public void QuitGame()
    {
        Application.Quit();
        
        // For testing in editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // Public method to check if game is paused
    public bool IsPaused()
    {
        return isPaused;
    }
}