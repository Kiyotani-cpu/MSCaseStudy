using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobTriggerManager : MonoBehaviour
{
    [System.Serializable]
    public class MobWave
    {
        public string waveName;
        public Transform triggerZone;
        public GameObject[] mobs;

        [Header("Wave Type")]
        public bool isBeforeBossWave = false;
    }

    [Header("Wave Settings")]
    public List<MobWave> waves = new List<MobWave>();
    public float nextWaveDelay = 2f; // delay before enabling next wave trigger

    [Header("Boss Settings")]
    public GameObject bossObject;

    private int currentWaveIndex = 0;
    private List<GameObject> activeMobs = new List<GameObject>();
    private bool waveInProgress = false;

    void Awake()
    {
        foreach (var wave in waves)
        {
            foreach (var mob in wave.mobs)
                if (mob != null) mob.SetActive(false);

            if (wave.triggerZone != null)
                wave.triggerZone.gameObject.SetActive(false);
        }

        if (bossObject != null)
            bossObject.SetActive(false);
    }

    void Start()
    {
        if (waves.Count > 0 && waves[0].triggerZone != null)
            waves[0].triggerZone.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!waveInProgress) return;

        activeMobs.RemoveAll(mob => mob == null || (mob.GetComponent<Health>()?.IsDead ?? false));

        if (activeMobs.Count == 0)
        {
            waveInProgress = false;

            if (waves[currentWaveIndex].isBeforeBossWave)
            {
                // Start coroutine to trigger boss intro after a delay
                StartCoroutine(TriggerBossIntroAfterDelay(3));
            }
            else
            {
                // Enable next wave after delay
                StartCoroutine(EnableNextWaveAfterDelay(nextWaveDelay));
            }
        }
    }

    public void StartWave(int waveIndex)
    {
        if (waveInProgress || waveIndex != currentWaveIndex) return;

        if (waves[waveIndex].triggerZone != null)
            waves[waveIndex].triggerZone.gameObject.SetActive(false);

        activeMobs.Clear();
        foreach (var mob in waves[waveIndex].mobs)
        {
            if (mob != null)
            {
                mob.SetActive(true);
                activeMobs.Add(mob);
            }
        }

        waveInProgress = true;
        Debug.Log($"Wave Started: {waves[waveIndex].waveName}");
    }

    private IEnumerator EnableNextWaveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentWaveIndex++;
        if (currentWaveIndex < waves.Count && waves[currentWaveIndex].triggerZone != null)
            waves[currentWaveIndex].triggerZone.gameObject.SetActive(true);
    }

    private IEnumerator TriggerBossIntroAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Player playerObj = FindObjectOfType<Player>();
        if (playerObj != null && bossObject != null)
        {
            Debug.Log("Pre-boss wave cleared! Triggering boss intro...");
            BossSceneManager.Instance.PlayBossIntro(playerObj.transform, bossObject);
        }
    }
}
