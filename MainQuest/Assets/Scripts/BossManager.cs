using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("UI References")]
    public GameObject mainUI;
    public GameObject victoryUI;

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
        if (mainUI != null) mainUI.SetActive(false);
        if (victoryUI != null) victoryUI.SetActive(true);

        Debug.Log("Victory UI shown!");
    }
}
