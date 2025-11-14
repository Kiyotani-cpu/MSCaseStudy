using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSummonPrototype : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Unit Summon Settings")]
    public GameObject unitPrefab;
    public float raycastDistance = 100f;
    public int groundLayerMaskInt = 1 << 6;
    private LayerMask groundLayer => groundLayerMaskInt;

    [Header("Restriction Settings")]
    public Transform allowedAreaCenter;
    public float allowedAreaRadius = 10f;

    [Header("Cooldown Settings")]
    public float summonCooldown = 5f;
    private float cooldownTimer = 0f;

    [Header("UI References")]
    public Image cooldownOverlay;
    public Image validSummonIndicator;
    public Image invalidSummonIndicator;
    public GameObject cardUI; // Added this missing reference

    [Header("Keyboard Shortcuts")]
    public KeyCode summonKey = KeyCode.Q;
    public KeyCode cancelKey = KeyCode.Escape;

    [Header("Visual Feedback")]
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;
    public float indicatorDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalPosition;

    private bool isDragging = false;
    private bool isValidSummonLocation = false;
    private Vector3 currentSummonPosition;

    [Header("Card Visibility")]
    public CardVisibilityController visibilityController;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.position;

        // Assign visibilityController automatically if not assigned
        if (visibilityController == null)
            visibilityController = GetComponent<CardVisibilityController>();

        // Auto-assign cardUI if not set
        if (cardUI == null)
            cardUI = gameObject; // Use this GameObject as cardUI

        // Initialize UI elements
        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0f;

        if (validSummonIndicator != null)
        {
            validSummonIndicator.color = validColor;
            validSummonIndicator.gameObject.SetActive(false);
        }

        if (invalidSummonIndicator != null)
        {
            invalidSummonIndicator.color = invalidColor;
            invalidSummonIndicator.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Handle cooldown
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = Mathf.Clamp01(cooldownTimer / summonCooldown);
        }

        // Keyboard shortcuts
        HandleKeyboardInput();

        // Update summon location validation during drag
        if (isDragging)
        {
            UpdateSummonLocationValidation();
        }
    }

    void HandleKeyboardInput()
    {
        // Quick summon with keyboard (when card is visible)
        if (Input.GetKeyDown(summonKey) && IsCardVisible() && cooldownTimer <= 0f)
        {
            AttemptQuickSummon();
        }

        // Cancel drag with Escape
        if (Input.GetKeyDown(cancelKey) && isDragging)
        {
            CancelDrag();
        }
    }

    bool IsCardVisible()
    {
        return cardUI != null && cardUI.activeInHierarchy;
    }

    void AttemptQuickSummon()
    {
        // Raycast from center of screen for quick summon
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, groundLayer))
        {
            if (IsValidSummonLocation(hit.point))
            {
                SummonUnit(hit.point);
                if (visibilityController != null)
                    visibilityController.HideCardImmediately();
            }
            else
            {
                ShowVisualFeedback(false, "❌ Invalid summon location");
            }
        }
        else
        {
            ShowVisualFeedback(false, "❌ No ground found for summoning");
        }
    }

    void UpdateSummonLocationValidation()
    {
        // Raycast from current mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, groundLayer))
        {
            currentSummonPosition = hit.point;
            isValidSummonLocation = IsValidSummonLocation(hit.point);

            // Update visual indicators
            UpdateDragIndicators(isValidSummonLocation);
        }
        else
        {
            isValidSummonLocation = false;
            UpdateDragIndicators(false);
        }
    }

    void UpdateDragIndicators(bool valid)
    {
        if (validSummonIndicator != null)
            validSummonIndicator.gameObject.SetActive(valid);

        if (invalidSummonIndicator != null)
            invalidSummonIndicator.gameObject.SetActive(!valid);
    }

    bool IsValidSummonLocation(Vector3 position)
    {
        return allowedAreaCenter == null || Vector3.Distance(position, allowedAreaCenter.position) <= allowedAreaRadius;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cooldownTimer > 0f) return;

        isDragging = true;
        if (visibilityController != null)
            visibilityController.isDragging = true;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // Reset position to ensure smooth dragging
        originalPosition = rectTransform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        if (visibilityController != null)
            visibilityController.isDragging = false;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Hide indicators
        UpdateDragIndicators(false);

        if (cooldownTimer <= 0f && isValidSummonLocation)
        {
            SummonUnit(currentSummonPosition);
            if (visibilityController != null)
                visibilityController.HideCardImmediately();
        }
        else if (!isValidSummonLocation)
        {
            ShowVisualFeedback(false, "❌ Invalid summon location");
        }

        rectTransform.position = originalPosition;
    }

    void CancelDrag()
    {
        if (!isDragging) return;

        isDragging = false;
        if (visibilityController != null)
            visibilityController.isDragging = false;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Hide indicators
        UpdateDragIndicators(false);

        rectTransform.position = originalPosition;

        ShowVisualFeedback(false, "Summon cancelled");
    }

    void SummonUnit(Vector3 position)
    {
        Instantiate(unitPrefab, position, Quaternion.identity);

        // Start cooldown
        cooldownTimer = summonCooldown;
        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 1f;

        ShowVisualFeedback(true, "Unit summoned!");
    }

    void ShowVisualFeedback(bool success, string message)
    {
        Debug.Log(message);

        // Start visual feedback coroutine
        StartCoroutine(ShowIndicatorRoutine(success));
    }

    private System.Collections.IEnumerator ShowIndicatorRoutine(bool success)
    {
        Image indicator = success ? validSummonIndicator : invalidSummonIndicator;

        if (indicator != null)
        {
            indicator.gameObject.SetActive(true);
            yield return new WaitForSeconds(indicatorDuration);
            indicator.gameObject.SetActive(false);
        }
    }

    // Public method to check if card can be used
    public bool CanSummon()
    {
        return cooldownTimer <= 0f && IsCardVisible();
    }

    // Public method for external summon calls
    public void AttemptSummonAtPosition(Vector3 worldPosition)
    {
        if (CanSummon() && IsValidSummonLocation(worldPosition))
        {
            SummonUnit(worldPosition);
            if (visibilityController != null)
                visibilityController.HideCardImmediately();
        }
    }

    // Public method to force cooldown reset (for testing/cheats)
    public void ResetCooldown()
    {
        cooldownTimer = 0f;
        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0f;
    }
}