using UnityEngine;
using Vuforia;
using System.Collections.Generic;

/// <summary>
/// Activates terrain prefabs when Ground Plane content is placed,
/// and triggers wave spawning logic once terrain is visible.
/// </summary>
public class GroundPlaneActivator : MonoBehaviour
{
    [Header("Assign all prefabs or terrains to activate")]
    public List<GameObject> objectsToActivate = new List<GameObject>();

    [Header("Optional Position Adjustment")]
    public Vector3 positionOffset = Vector3.zero;   // 👈 move all terrains in X, Y, Z

    [Header("References")]
    [Tooltip("Reference to WaveManager (for starting waves).")]
    public WaveManager waveManager;

    private ContentPositioningBehaviour contentPositioning;

    private Vector3[] initialOffsets;
    private Quaternion[] initialRotations;

    private bool triggered = false; // prevent duplicate start

    private void Awake()
    {
        initialOffsets = new Vector3[objectsToActivate.Count];
        initialRotations = new Quaternion[objectsToActivate.Count];

        if (objectsToActivate.Count > 0)
        {
            Vector3 referencePos = objectsToActivate[0].transform.position;

            for (int i = 0; i < objectsToActivate.Count; i++)
            {
                if (objectsToActivate[i] != null)
                {
                    initialOffsets[i] = objectsToActivate[i].transform.position - referencePos;
                    initialRotations[i] = objectsToActivate[i].transform.rotation;

                    // Hide terrain until AR placement happens
                    objectsToActivate[i].SetActive(false);
                }
            }
        }
    }

    private void Start()
    {
        contentPositioning = FindObjectOfType<ContentPositioningBehaviour>();

        if (contentPositioning != null)
            contentPositioning.OnContentPlaced.AddListener(OnContentPlaced);
    }

    private void OnDestroy()
    {
        if (contentPositioning != null)
            contentPositioning.OnContentPlaced.RemoveListener(OnContentPlaced);
    }

    private void OnContentPlaced(GameObject placedContent)
    {
        if (triggered) return; // already started once
        if (objectsToActivate.Count == 0) return;

        Vector3 basePos = placedContent.transform.position;
        Quaternion baseRot = placedContent.transform.rotation;

        for (int i = 0; i < objectsToActivate.Count; i++)
        {
            if (objectsToActivate[i] != null)
            {
                // Apply initial offset and user-defined XYZ offset
                Vector3 finalPos = basePos + (baseRot * initialOffsets[i]) + positionOffset;

                objectsToActivate[i].transform.position = finalPos;
                objectsToActivate[i].transform.rotation = baseRot * initialRotations[i];

                objectsToActivate[i].SetActive(true);
            }
        }

        Debug.Log($"Neighbor terrains activated with XYZ offset: {positionOffset}");

        // ✅ Start summon/wave logic now
        if (waveManager != null)
        {
            triggered = true;
            waveManager.StartWaves();
            Debug.Log("WaveManager triggered because terrain is active/visible.");
        }
    }
}
