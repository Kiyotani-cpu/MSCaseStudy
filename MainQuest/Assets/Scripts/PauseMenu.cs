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
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f; // unfreeze
        isPaused = false;
    }

    public void NewGame()
    {
        Time.timeScale = 1f; // reset time
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // reload current scene
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f; // reset time
        SceneManager.LoadScene("MainMenu"); // replace with your main menu scene name
    }
}
