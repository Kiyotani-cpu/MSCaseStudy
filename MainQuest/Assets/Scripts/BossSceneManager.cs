using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossSceneManager : MonoBehaviour
{
    public static BossSceneManager Instance;

    private Transform playerTransform;
    private GameObject bossPrefab;

    [Header("Intro Settings")]
    public float introDuration = 5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayBossIntro(Transform player, GameObject boss)
    {
        playerTransform = player;
        bossPrefab = boss;
        StartCoroutine(LoadCutScene());
    }

    private IEnumerator LoadCutScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MiniBossCutScene");
        while (!asyncLoad.isDone)
            yield return null;

        // Wait for intro duration
        yield return new WaitForSeconds(introDuration);

        // Load boss fight scene
        AsyncOperation asyncFight = SceneManager.LoadSceneAsync("MiniBossFightScene");
        while (!asyncFight.isDone)
            yield return null;

        // Move player to spawn point near boss
        GameObject spawnPoint = GameObject.FindWithTag("BossSpawnPoint");
        if (spawnPoint != null && playerTransform != null)
        {
            playerTransform.position = spawnPoint.transform.position;
            playerTransform.rotation = spawnPoint.transform.rotation;
        }

        // Enable boss prefab
        if (bossPrefab != null)
            bossPrefab.SetActive(true);
    }
}
