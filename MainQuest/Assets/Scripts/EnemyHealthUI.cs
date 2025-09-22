using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("References")]
    public Health enemyHealth;   // Assign in Inspector
    public Slider hpSlider;      // Assign in Inspector (or use prefab)
    public Vector3 offset = Vector3.zero; // Optional if using prefab

    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (enemyHealth != null)
        {
            // Subscribe to health changes
            enemyHealth.OnHealthChanged += UpdateHealthBar;
            
            // Initialize slider immediately
            if (hpSlider != null)
            {
                hpSlider.maxValue = enemyHealth.maxHealth;
                hpSlider.value = enemyHealth.currentHealth; // <-- important
            }
        }
    }

    void UpdateHealthBar(int currentHealth)
    {
        if (hpSlider != null)
            hpSlider.value = currentHealth;
    }


    void Update()
    {
        // Optional: make the slider face the camera
        if (hpSlider != null && cam != null)
        {
            Vector3 lookDir = hpSlider.transform.position + cam.transform.rotation * Vector3.forward;
            hpSlider.transform.LookAt(lookDir, cam.transform.rotation * Vector3.up);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (enemyHealth != null)
            enemyHealth.OnHealthChanged -= UpdateHealthBar;
    }
}
