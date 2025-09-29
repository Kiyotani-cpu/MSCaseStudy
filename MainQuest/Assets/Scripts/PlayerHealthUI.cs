using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    public Health playerHealth;   // Auto-assigned via tag "Player"
    public Slider hpSlider;       // Auto-assigned via tag "HealthSlider"

    void Awake()
    {
        // --- Auto-assign PlayerHealth ---
        if (playerHealth == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerHealth = playerObj.GetComponent<Health>();
            }
            else
            {
                Debug.LogWarning("⚠️ No object with tag 'Player' found in the scene!");
            }
        }

        // --- Auto-assign Slider ---
        if (hpSlider == null)
        {
            GameObject sliderObj = GameObject.FindGameObjectWithTag("PlayerSlider");
            if (sliderObj != null)
            {
                hpSlider = sliderObj.GetComponent<Slider>();
            }
            else
            {
                Debug.LogWarning("⚠️ No object with tag 'PlayerSlider' found in the scene!");
            }
        }
    }

    void Start()
    {
        if (playerHealth != null && hpSlider != null)
        {
            hpSlider.maxValue = playerHealth.maxHealth;
            hpSlider.value = playerHealth.currentHealth;
        }
    }

    void Update()
    {
        if (playerHealth != null && hpSlider != null)
        {
            hpSlider.value = playerHealth.currentHealth;
        }
    }
}
