using UnityEngine;

public class NPCTalkController : MonoBehaviour
{
    private Animator animator;
    private bool isTalking = false;

    [Header("Player Settings")]
    public Transform player;          // Assign your Player here in Inspector
    public float maxTalkDistance = 3f; // Distance to trigger talk

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null) Debug.LogError("Animator missing on NPC!");
        if (player == null) Debug.LogError("Player not assigned in Inspector!");
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // Start talking if close and E pressed
        if (!isTalking && distance <= maxTalkDistance && Input.GetKeyDown(KeyCode.E))
        {
            StartTalking();
        }

        // Stop talking if player moves away
        if (isTalking && distance > maxTalkDistance)
        {
            StopTalking();
        }
    }

    void StartTalking()
    {
        animator.SetBool("isTalking", true); // Idle -> Talk
        isTalking = true;
        Debug.Log("NPC started talking!");
    }

    void StopTalking()
    {
        animator.SetBool("isTalking", false); // Talk -> Idle
        isTalking = false;
        Debug.Log("NPC stopped talking!");
    }
}
