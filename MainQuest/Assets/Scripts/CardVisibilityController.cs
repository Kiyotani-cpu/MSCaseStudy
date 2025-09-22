using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardVisibilityController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject cardUI;   // The card to show/hide
    public Button summonButton; // The button that triggers the card
    public float visibleDuration = 3f; // Time the card stays visible

    [HideInInspector] public bool isDragging = false; // Flag set by CardSummonPrototype

    private Coroutine hideRoutine;

    void Start()
    {
        if (cardUI != null)
            cardUI.SetActive(false); // Hide card at start
        if (summonButton != null)
            summonButton.gameObject.SetActive(true); // Button visible at start
    }

    // Call this from Button OnClick()
    public void ShowCard()
    {
        if (cardUI == null || summonButton == null) return;

        // Show card & hide button
        cardUI.SetActive(true);
        summonButton.gameObject.SetActive(false);

        // Cancel previous routine if running
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        // Start auto-hide
        hideRoutine = StartCoroutine(HideAfterDelay());
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
            if (cardUI != null)
                cardUI.SetActive(false);

            if (summonButton != null)
                summonButton.gameObject.SetActive(true);
        }
    }
}
