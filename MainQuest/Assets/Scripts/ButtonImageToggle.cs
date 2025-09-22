using UnityEngine;
using UnityEngine.UI;

public class ButtonImageToggle : MonoBehaviour
{
    [Header("Button & Sprites")]
    public Button targetButton;   // The button to click
    public Sprite onSprite;       // Image when ON
    public Sprite offSprite;      // Image when OFF

    private Image buttonImage;
    private bool isOn = false;

    void Start()
    {
        buttonImage = targetButton.GetComponent<Image>();

        // Set initial state (OFF)
        buttonImage.sprite = offSprite;

        // Add click listener
        targetButton.onClick.AddListener(ToggleImage);
    }

    void ToggleImage()
    {
        isOn = !isOn;  // Flip state
        buttonImage.sprite = isOn ? onSprite : offSprite;
    }
}
