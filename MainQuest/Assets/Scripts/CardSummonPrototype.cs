using UnityEngine;
using UnityEngine.EventSystems;

public class CardSummonPrototype : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Unit Summon Settings")]
    public GameObject unitPrefab; // The unit to summon
    public float raycastDistance = 100f; // Max distance for raycast
    [Tooltip("Bitmask for ground layer. Default is layer 6.")]
    public int groundLayerMaskInt = 1 << 6;
    private LayerMask groundLayer => groundLayerMaskInt; // Layer for the ground plane (e.g., "Ground")

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalPosition;

    private bool isDragging = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        canvasGroup.alpha = 0.6f; // Make card semi-transparent while dragging
        canvasGroup.blocksRaycasts = false; // Allow raycasts through UI
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the card with the pointer
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Raycast from camera to the ground
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, groundLayer))
        {
            // Summon unit at hit point
            Instantiate(unitPrefab, hit.point, Quaternion.identity);
            
            // Optional: Disable card after summon (add logic here)
            // gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Invalid summon location - no ground hit");
            // Optional: Add feedback (e.g., UI text or sound)
        }

        // Reset card position
        rectTransform.position = originalPosition;
    }
}
