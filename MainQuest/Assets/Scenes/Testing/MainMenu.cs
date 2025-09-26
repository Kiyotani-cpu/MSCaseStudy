using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Function to play the game
    public void PlayGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Function to exit the game
    public void ExitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }

    // Function to go back to main menu
    public void BackToMenu(string menuSceneName)
    {
        SceneManager.LoadScene(menuSceneName);
    }

    public void MapOption(string mapName)
    {
        SceneManager.LoadScene(mapName);
    }
}
