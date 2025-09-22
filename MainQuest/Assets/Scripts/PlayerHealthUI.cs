using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    public Health playerHealth;   // Drag your Player object here
    public Slider hpSlider;       // Drag your UI Slider here

    void Start()
    {
        if (playerHealth != null)
        {
            hpSlider.maxValue = playerHealth.maxHealth;
            hpSlider.value = playerHealth.currentHealth;
        }
    }

    void Update()
    {
        if (playerHealth != null)
        {
            hpSlider.value = playerHealth.currentHealth;
        }
    }
}
