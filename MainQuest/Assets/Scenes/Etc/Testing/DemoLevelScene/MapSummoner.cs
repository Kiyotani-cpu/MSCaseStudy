using UnityEngine;
using Vuforia;

public class ARWorldAnchor : MonoBehaviour
{
    [Header("World Setup")]
    public GameObject worldPrefab;   // Drag your world prefab here
    public Transform anchorParent;   // Drag your WorldAnchor empty object here

    private bool worldSpawned = false;

    void Start()
    {
        var observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
        {
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (!worldSpawned && status.Status == Status.TRACKED)
        {
            SpawnWorld(behaviour.transform);
        }
    }

    private void SpawnWorld(Transform imageTarget)
    {
        // Spawn at the image target position
        GameObject world = Instantiate(worldPrefab, imageTarget.position, imageTarget.rotation);

        // Re-parent under anchor so it no longer follows the image
        if (anchorParent != null)
            world.transform.SetParent(anchorParent, true);

        worldSpawned = true;
        Debug.Log("🌍 World spawned and anchored!");
    }
}
