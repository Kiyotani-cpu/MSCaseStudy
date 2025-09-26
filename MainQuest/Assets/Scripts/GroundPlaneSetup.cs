using UnityEngine;
using Vuforia;

public class GroundPlaneSetup : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;                // root (with AR Camera inside)
    public GameObject playerMesh;            // only the visual mesh
    public MonoBehaviour[] movementScripts;  // e.g. joystick controller

    [Header("Vuforia")]
    public ContentPositioningBehaviour contentPosBehaviour;

    void Start()
    {
        // Hide visuals, disable movement
        if (playerMesh != null)
            playerMesh.SetActive(false);

        foreach (var s in movementScripts)
            if (s != null) s.enabled = false;

        // Find the ContentPositioningBehaviour if not assigned
        if (contentPosBehaviour == null)
            contentPosBehaviour = FindObjectOfType<ContentPositioningBehaviour>();

        if (contentPosBehaviour != null)
            contentPosBehaviour.OnContentPlaced.AddListener(HandleContentPlaced);
        else
            Debug.LogWarning("ContentPositioningBehaviour not found!");
    }

    private void HandleContentPlaced(GameObject placedContent)
    {
        // Re-enable mesh
        if (playerMesh != null)
            playerMesh.SetActive(true);

        // Place player on map
        player.transform.position = placedContent.transform.position + Vector3.up * 1f;

        // Re-enable movement
        foreach (var s in movementScripts)
            if (s != null) s.enabled = true;
    }

    void OnDestroy()
    {
        if (contentPosBehaviour != null)
            contentPosBehaviour.OnContentPlaced.RemoveListener(HandleContentPlaced);
    }
}
