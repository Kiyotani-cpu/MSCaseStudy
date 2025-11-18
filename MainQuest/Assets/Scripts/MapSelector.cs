using UnityEngine;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
    [System.Serializable]
    public class LevelButton
    {
        public Button button;
        public int levelNumber;
        public GameObject objectToHide; // New variable
        public Image imageToColor; // New variable
    }

    public LevelButton[] levelButtons;

    void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        foreach (LevelButton lb in levelButtons)
        {
            lb.button.interactable = (lb.levelNumber <= unlockedLevel);
            UpdateLevelButtonState(lb, unlockedLevel); // Call the new function
        }
    }

    // New function to handle hiding and color changes
    void UpdateLevelButtonState(LevelButton lb, int unlockedLevel)
    {
        if (lb.levelNumber <= unlockedLevel)
        {
            // If the level is unlocked
            if (lb.objectToHide != null)
            {
                lb.objectToHide.SetActive(false); // Hide the object
            }
            if (lb.imageToColor != null)
            {
                lb.imageToColor.color = Color.white; // Change the image color to white
            }
        }
    }
}