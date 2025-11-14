using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages mob wave spawning, miniboss cutscene, and spawn intervals.
/// Next wave only starts when all mobs from the previous wave are cleared.
/// Attach to an empty GameObject (WaveManager).
/// </summary>
public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class MobEntry
    {
        [Tooltip("Mob prefab to spawn.")]
        public GameObject mobPrefab;

        [Tooltip("Spawn location for this mob.")]
        public Transform spawnPoint;
    }

    [System.Serializable]
    public class Wave
    {
        [Tooltip("Name of this wave (for debugging).")]
        public string waveName = "Wave";

        [Tooltip("List of mobs in this wave (can include boss).")]
        public List<MobEntry> mobs = new List<MobEntry>();

        [Tooltip("Delay between each mob spawn in this wave.")]
        public float spawnInterval = 1.5f;

        [Tooltip("Is this wave a boss/miniboss wave?")]
        public bool isBossWave = false;
    }


    [Header("Wave Settings")]
    [Tooltip("List of waves. Each wave has its own mob array and spawn points.")]
    public List<Wave> waves = new List<Wave>();

    [Tooltip("Delay between waves after all mobs are cleared.")]
    public float delayBetweenWaves = 2f;

    [Header("References")]
    [Tooltip("Reference to CutsceneManager for cutscene playback.")]
    public CutsceneManager cutsceneManager;

    private int currentWave = 0;
    private List<GameObject> activeMobs = new List<GameObject>();

    public void StartWaves()
    {
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (currentWave < waves.Count)
        {
            currentWave++;
            Wave wave = waves[currentWave - 1];
            Debug.Log($"▶ Starting {wave.waveName}");

            if (wave.isBossWave)
            {
                if (cutsceneManager != null)
                {
                    // Make sure no duplicate listeners
                    cutsceneManager.OnCutsceneEnd.RemoveAllListeners();
                    cutsceneManager.OnCutsceneEnd.AddListener(() =>
                    {
                        // Spawn the boss mob(s) after cutscene
                        if (wave.mobs.Count > 0)
                        {
                            foreach (var mob in wave.mobs)
                            {
                                if (mob.mobPrefab != null && mob.spawnPoint != null)
                                    Instantiate(mob.mobPrefab, mob.spawnPoint.position, mob.spawnPoint.rotation);
                            }
                            Debug.Log("⚔ Boss spawned after cutscene!");
                        }
                    });

                    cutsceneManager.PlayCutscene();
                }
                else
                {
                    // No cutscene → spawn boss immediately
                    foreach (var mob in wave.mobs)
                    {
                        if (mob.mobPrefab != null && mob.spawnPoint != null)
                            Instantiate(mob.mobPrefab, mob.spawnPoint.position, mob.spawnPoint.rotation);
                    }
                }

                yield break; // stop after boss wave
            }
            else
            {
                yield return StartCoroutine(SpawnWave(wave));
                yield return StartCoroutine(WaitUntilWaveCleared());
                yield return new WaitForSeconds(delayBetweenWaves);
            }
        }
    }


    private IEnumerator SpawnWave(Wave wave)
    {
        foreach (var mob in wave.mobs)
        {
            if (mob.mobPrefab != null && mob.spawnPoint != null)
            {
                GameObject newMob = Instantiate(mob.mobPrefab, mob.spawnPoint.position, mob.spawnPoint.rotation);
                activeMobs.Add(newMob);

                // Cleanup when mob dies
                MobDeathHandler handler = newMob.AddComponent<MobDeathHandler>();
                handler.onDeath += () => activeMobs.Remove(newMob);
            }
            else
            {
                Debug.LogWarning("WaveManager: Missing prefab or spawn point in " + wave.waveName);
            }

            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private IEnumerator WaitUntilWaveCleared()
    {
        Debug.Log("⏳ Waiting for wave to be cleared...");
        while (activeMobs.Count > 0)
        {
            yield return null;
        }
        Debug.Log("✅ Wave cleared!");
    }
}

public class MobDeathHandler : MonoBehaviour
{
    public System.Action onDeath;

    private void OnDestroy()
    {
        onDeath?.Invoke();
    }
}
