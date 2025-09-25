using UnityEngine;
using Cinemachine;

public class NPCTalkController : MonoBehaviour
{
    private Animator animator;
    private bool isTalking = false;

    // Fix: Declare the cameraTransform variable here
    public Transform cameraTransform;

    [Header("Player Settings")]
    public Transform player;
    public float maxTalkDistance = 3f;

    [Header("Cinemachine Cameras")]
    public CinemachineVirtualCamera playerCam; // AR default
    public CinemachineVirtualCamera npcCam;    // NPC close-up

    void Start()
    {
        // Add a null check for the player, in case it wasn't assigned in the Inspector.
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned!");
        }

        // Initialize the animator component.
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator component not found on the NPC.");
        }

        // The rest of your existing Start() logic
        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                cameraTransform = mainCam.transform;
                Debug.Log("Auto-assigned Main Camera to cameraTransform: " + cameraTransform.name);
            }
            else
            {
                Debug.LogError("No Main Camera found in the scene! Ensure a camera is tagged as 'MainCamera'.");
            }
        }
    }


    void Update()
    {
        // Add a null check to prevent errors if the player isn't assigned.
        if (player == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (!isTalking && distance <= maxTalkDistance && Input.GetKeyDown(KeyCode.E))
        {
            StartTalking();
        }

        if (isTalking && (distance > maxTalkDistance || Input.GetKeyDown(KeyCode.E)))
        {
            StopTalking();
        }
    }

    void StartTalking()
    {
        if (animator != null)
        {
            animator.SetBool("isTalking", true);
        }

        isTalking = true;

        if (npcCam != null && playerCam != null)
        {
            npcCam.Priority = 30;   // Switch to NPC close-up
            playerCam.Priority = 10;
        }
        else
        {
            Debug.LogError("Cinemachine Cameras not assigned! Please assign them in the Inspector.");
        }
    }

    void StopTalking()
    {
        if (animator != null)
        {
            animator.SetBool("isTalking", false);
        }

        isTalking = false;

        if (npcCam != null && playerCam != null)
        {
            npcCam.Priority = 10;   // Back to AR camera
            playerCam.Priority = 20;
        }
    }
}