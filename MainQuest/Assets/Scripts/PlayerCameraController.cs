using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Camera References")]
    public Transform playerTransform;
    public Transform cameraFollowTarget;
    public Camera playerCamera;

    [Header("Camera Settings")]
    public float cameraDistance = 5f;
    public float cameraHeight = 2f;
    public float cameraSmoothSpeed = 5f;
    public float cameraRotationSpeed = 2f;
    public float cameraCollisionOffset = 0.3f;

    [Header("Camera Limits")]
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 70f;
    public float minCameraDistance = 1f;
    public float maxCameraDistance = 8f;

    [Header("Input Settings")]
    public string mouseXInput = "Mouse X";
    public string mouseYInput = "Mouse Y";
    public bool invertY = false;

    private float mouseX;
    private float mouseY;
    private float xRotation = 0f;
    private float currentCameraDistance;
    private Vector3 cameraVelocity = Vector3.zero;

    [Header("Collision Detection")]
    public LayerMask collisionLayers = -1;
    public float collisionSphereRadius = 0.3f;

    // Reference to pause menu
    private PauseMenu pauseMenu;

    void Start()
    {
        // Initialize references if not set
        if (playerTransform == null)
            playerTransform = transform;

        if (playerCamera == null)
            playerCamera = Camera.main;

        // Create camera follow target if not assigned
        if (cameraFollowTarget == null)
        {
            CreateCameraFollowTarget();
        }

        currentCameraDistance = cameraDistance;

        // Find pause menu in scene
        pauseMenu = FindObjectOfType<PauseMenu>();

        // Lock and hide cursor initially
        LockCursor();
    }

    void Update()
    {
        if (playerTransform == null || cameraFollowTarget == null) return;

        // Check for ESC key to toggle pause menu
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePauseMenu();
        }

        // Only handle camera input when not paused
        if (!IsGamePaused())
        {
            HandleCameraInput();
            UpdateCameraPosition();
            HandleCameraCollision();
        }
    }

    void HandleCameraInput()
    {
        // Get mouse input
        mouseX = Input.GetAxis(mouseXInput) * cameraRotationSpeed;
        mouseY = Input.GetAxis(mouseYInput) * cameraRotationSpeed * (invertY ? 1 : -1);

        // Rotate player horizontally with mouse
        playerTransform.Rotate(Vector3.up * mouseX);

        // Handle vertical camera rotation
        xRotation += mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        // Apply vertical rotation to camera target
        cameraFollowTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void UpdateCameraPosition()
    {
        if (playerCamera == null) return;

        // Calculate desired camera position
        Vector3 desiredPosition = cameraFollowTarget.position - 
                                cameraFollowTarget.forward * currentCameraDistance;

        // Smoothly move camera to desired position
        playerCamera.transform.position = Vector3.SmoothDamp(
            playerCamera.transform.position,
            desiredPosition,
            ref cameraVelocity,
            1f / cameraSmoothSpeed
        );

        // Make camera look at follow target
        playerCamera.transform.LookAt(cameraFollowTarget.position);
    }

    void HandleCameraCollision()
    {
        if (cameraFollowTarget == null || playerCamera == null) return;

        // Raycast from camera target to camera position
        Vector3 cameraDirection = (playerCamera.transform.position - cameraFollowTarget.position).normalized;
        float targetDistance = cameraDistance;

        RaycastHit hit;
        if (Physics.SphereCast(
            cameraFollowTarget.position,
            collisionSphereRadius,
            -cameraFollowTarget.forward,
            out hit,
            cameraDistance + collisionSphereRadius,
            collisionLayers))
        {
            // Adjust camera distance if collision detected
            targetDistance = hit.distance - collisionSphereRadius - cameraCollisionOffset;
            targetDistance = Mathf.Clamp(targetDistance, minCameraDistance, maxCameraDistance);
        }

        // Smoothly adjust current camera distance
        currentCameraDistance = Mathf.Lerp(currentCameraDistance, targetDistance, Time.deltaTime * cameraSmoothSpeed);
    }

    void CreateCameraFollowTarget()
    {
        GameObject target = new GameObject("CameraFollowTarget");
        cameraFollowTarget = target.transform;
        cameraFollowTarget.SetParent(playerTransform);
        cameraFollowTarget.localPosition = new Vector3(0, 1.5f, 0);
        cameraFollowTarget.localRotation = Quaternion.identity;
    }

    public void SetCameraTarget(Transform target)
    {
        playerTransform = target;
        if (cameraFollowTarget != null)
        {
            cameraFollowTarget.SetParent(playerTransform);
            cameraFollowTarget.localPosition = new Vector3(0, 1.5f, 0);
        }
    }

    public void ToggleCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            UnlockCursor();
        }
        else
        {
            LockCursor();
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetCameraDistance(float distance, bool instant = false)
    {
        cameraDistance = Mathf.Clamp(distance, minCameraDistance, maxCameraDistance);
        if (instant)
            currentCameraDistance = cameraDistance;
    }

    private void TogglePauseMenu()
    {
        if (pauseMenu != null)
        {
            pauseMenu.TogglePause();
            
            // Update cursor state based on pause state
            if (IsGamePaused())
            {
                UnlockCursor();
            }
            else
            {
                LockCursor();
            }
        }
    }

    private bool IsGamePaused()
    {
        return pauseMenu != null && pauseMenu.IsPaused();
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        if (cameraFollowTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(cameraFollowTarget.position, collisionSphereRadius);
            Gizmos.DrawLine(cameraFollowTarget.position, cameraFollowTarget.position - cameraFollowTarget.forward * currentCameraDistance);
        }
    }

    // Cleanup
    void OnDestroy()
    {
        // Reset cursor state when object is destroyed
        UnlockCursor();
    }
}