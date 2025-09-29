using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    void Awake()
    {
        // Auto-assign if not set in Inspector
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (targetCamera != null)
        {
            // Face the camera
            transform.LookAt(transform.position + targetCamera.transform.forward);
        }
    }
}
