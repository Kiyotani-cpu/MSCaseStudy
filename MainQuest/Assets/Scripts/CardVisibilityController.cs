using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardVisibilityController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject cardUI; // The card to show/hide
    public Button summonButton; // The button that triggers the card
    public float visibleDuration = 3f; // Time the card stays visible

    [Header("Keyboard Shortcuts")]
    public KeyCode showCardKey = KeyCode.Escape;
    public KeyCode hideCardKey = KeyCode.Escape;

    [Header("Visual Effects")]
    public Animator cardAnimator;
    public string showAnimation = "CardShow";
    public string hideAnimation = "CardHide";

    [HideInInspector]
    public bool isDragging = false;

    private Coroutine hideRoutine;
    private bool isCardVisible = false;

    void Start()
    {
        // Auto-assign references if not set
        if (cardUI == null)
        {
            cardUI = gameObject;
            Debug.LogWarning("CardUI not assigned, using GameObject: " + gameObject.name);
        }

        if (summonButton == null)
        {
            summonButton = GetComponentInChildren<Button>();
            if (summonButton != null)
                Debug.LogWarning(
                    "SummonButton not assigned, found in children: " + summonButton.name
                );
        }

        if (cardUI != null)
            cardUI.SetActive(false);

        if (summonButton != null)
            summonButton.gameObject.SetActive(true);

        isCardVisible = false;
    }

    void Update()
    {
        HandleKeyboardInput();
    }

    void HandleKeyboardInput()
    {
        // Show card with keyboard shortcut
        if (Input.GetKeyDown(showCardKey) && !isCardVisible)
        {
            ShowCard();
        }

        // Hide card with keyboard shortcut
        if (Input.GetKeyDown(hideCardKey) && isCardVisible && !isDragging)
        {
            HideCardImmediately();
        }
    }

    public void ShowCard()
    {
        if (cardUI == null)
        {
            Debug.LogError("CardUI reference is missing!");
            return;
        }

        // Show card
        cardUI.SetActive(true);
        isCardVisible = true;

        // Hide button if available
        if (summonButton != null)
            summonButton.gameObject.SetActive(false);

        // Play show animation
        if (cardAnimator != null && !string.IsNullOrEmpty(showAnimation))
        {
            cardAnimator.Play(showAnimation);
        }

        // Cancel previous routine if running
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        // Start auto-hide
        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    public void HideCardImmediately()
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        if (cardUI != null)
        {
            // Play hide animation if available
            if (cardAnimator != null && !string.IsNullOrEmpty(hideAnimation))
            {
                cardAnimator.Play(hideAnimation);
                // Deactivate after animation
                StartCoroutine(DeactivateAfterAnimation(0.3f)); // Adjust timing as needed
            }
            else
            {
                cardUI.SetActive(false);
            }
        }

        // Show button if available
        if (summonButton != null)
            summonButton.gameObject.SetActive(true);

        isCardVisible = false;
        isDragging = false;
    }

    private IEnumerator HideAfterDelay()
    {
        float timer = 0f;

        while (timer < visibleDuration)
        {
            // Only count time if not dragging
            if (!isDragging)
                timer += Time.deltaTime;

            yield return null;
        }

        // Only hide if not dragging
        if (!isDragging)
        {
            HideCardImmediately();
        }
    }

    private IEnumerator DeactivateAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (cardUI != null)
            cardUI.SetActive(false);
    }

    // Public method to check if card is currently visible
    public bool IsCardVisible()
    {
        return isCardVisible;
    }

    // Public method to extend visibility time
    public void ExtendVisibility(float additionalTime)
    {
        if (hideRoutine != null && isCardVisible)
        {
            StopCoroutine(hideRoutine);
            visibleDuration += additionalTime;
            hideRoutine = StartCoroutine(HideAfterDelay());
        }
    }

    // Public method to toggle card visibility
    public void ToggleCard()
    {
        if (isCardVisible)
        {
            HideCardImmediately();
        }
        else
        {
            ShowCard();
        }
    }

    // Called when the script is added or reset in inspector
    void Reset()
    {
        // Auto-populate common references
        cardUI = gameObject;
        summonButton = GetComponentInChildren<Button>();
        cardAnimator = GetComponent<Animator>();
    }
}
