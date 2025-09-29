using UnityEngine;
using Vuforia;

public class MapSummoner : MonoBehaviour
{
    public GameObject mapPrefab;   // Assign your map prefab in Inspector
    public Vector3 offset = new Vector3(0, 0, 0); // Optional offset from the target

    private bool mapSpawned = false;

    private void Start()
    {
        var observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
        {
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (!mapSpawned && (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED))
        {
            SummonMap();
        }
    }

    private void SummonMap()
    {
        if (mapPrefab != null)
        {
            // Spawn at the Image Target’s position, but not parented to it
            Vector3 spawnPos = transform.position + transform.TransformVector(offset);
            Quaternion spawnRot = transform.rotation;

            Instantiate(mapPrefab, spawnPos, spawnRot);

            mapSpawned = true; // only summon once
        }
    }
}
