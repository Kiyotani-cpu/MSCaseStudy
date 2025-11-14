using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    public Transform target;           // Assign your player
    public Vector3 offset = new Vector3(1.5f, 2.5f, -4f);
    public float followSpeed = 5f;
    public float rotationSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // Desired camera position (behind and above the player)
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);

        // Smoothly move camera to target position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Smoothly rotate to look at the player
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
