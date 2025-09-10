using UnityEngine;

public class EnvironmentMover : MonoBehaviour
{
    public Transform environmentRoot; // Assign your environment parent here
    public float moveSpeed = 5f;

    void Update()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down

        // Calculate movement vector relative to camera direction
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical);
        if (inputDirection.magnitude > 1)
            inputDirection.Normalize();

        // Get camera forward and right vectors on the horizontal plane
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        // Calculate movement direction relative to camera
        Vector3 moveDirection = camForward * inputDirection.z + camRight * inputDirection.x;

        // Move the environment in the opposite direction to simulate player movement
        environmentRoot.position -= moveDirection * moveSpeed * Time.deltaTime;
    }
}
