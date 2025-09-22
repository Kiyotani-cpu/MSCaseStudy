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

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalPosition;

    private bool isDragging = false;
    [Header("Card Visibility")]
    public CardVisibilityController visibilityController; // assign in Inspector

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.position;

        // Assign visibilityController automatically if not assigned
        if (visibilityController == null)
            visibilityController = GetComponent<CardVisibilityController>();

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0f;
    }


    void Update()
    {
        // Cooldown runs independently of visibility or dragging
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = Mathf.Clamp01(cooldownTimer / summonCooldown);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cooldownTimer > 0f) return;

        isDragging = true;
        if (visibilityController != null)
            visibilityController.isDragging = true;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        if (visibilityController != null)
            visibilityController.isDragging = false;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (cooldownTimer <= 0f)
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, groundLayer))
            {
                if (allowedAreaCenter == null || Vector3.Distance(hit.point, allowedAreaCenter.position) <= allowedAreaRadius)
                {
                    Instantiate(unitPrefab, hit.point, Quaternion.identity);

                    // Start cooldown
                    cooldownTimer = summonCooldown;
                    if (cooldownOverlay != null)
                        cooldownOverlay.fillAmount = 1f;
                }
                else
                {
                    Debug.Log("❌ Invalid summon: outside allowed area.");
                }
            }
            else
            {
                Debug.Log("❌ Invalid summon location: no ground hit.");
            }
        }

        rectTransform.position = originalPosition;
    }
}
