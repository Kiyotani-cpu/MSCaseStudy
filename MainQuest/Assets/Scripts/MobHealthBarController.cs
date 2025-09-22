using UnityEngine;

public class MobHealthBarController : MonoBehaviour
{
    private Canvas healthBarCanvas;

    void Awake()
    {
        healthBarCanvas = GetComponentInChildren<Canvas>();
        if (healthBarCanvas != null)
        {
            // Hide health bar at start
            healthBarCanvas.enabled = false;
        }
    }

    void OnEnable()
    {
        if (healthBarCanvas != null)
            healthBarCanvas.enabled = true;
    }

    void OnDisable()
    {
        if (healthBarCanvas != null)
            healthBarCanvas.enabled = false;
    }
}
