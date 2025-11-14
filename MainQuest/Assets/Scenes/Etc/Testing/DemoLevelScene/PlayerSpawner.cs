using UnityEngine;
using Vuforia;

public class PlayerSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject playerPrefab;     // Assign your Player prefab
    public GameObject terrain;          // Assign your terrain object
    public Transform spawnPoint;        // Drag the empty spawn point here in Inspector

    private bool playerSpawned = false;

    void Start()
    {
        if (terrain == null)  // fallback if not assigned
        {
            terrain = GameObject.FindGameObjectWithTag("Terrain");
        }

        if (terrain != null && playerPrefab != null && spawnPoint != null)
        {
            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("✅ Player spawned on terrain");
        }
        else
        {
            Debug.LogError("❌ Missing reference(s) in PlayerSpawner!");
        }
    }

    private void OnTerrainStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (!playerSpawned && (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED))
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        if (playerPrefab != null && spawnPoint != null)
        {
            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            playerSpawned = true;
        }
        else
        {
            Debug.LogWarning("PlayerPrefab or SpawnPoint not assigned!");
        }
    }
}
