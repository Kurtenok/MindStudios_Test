using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn options")]
    [SerializeField] List<GameObject> SpawnPoints = new List<GameObject>();
    [SerializeField] List<Wave> Waves = new List<Wave>();

    [Header("UI")]
    [SerializeField] TextMeshProUGUI CurrentWaveText;
    [SerializeField] TextMeshProUGUI EnemiesLeftText;

    [System.Serializable]
    struct Wave
    {
        public List<EnemyToSpawn> wave;
        public float nextWaveDelay;
    }

    [System.Serializable]
    struct EnemyToSpawn
    {
        public GameObject enemy;
        public int count;
        public float spawnInterval;
    }

    public Action AllWavesCompleted;

    private int currentWave = 1;

    private HashSet<IEnemyDeathNotifier> SpawnedEnemies = new HashSet<IEnemyDeathNotifier>();

    private void Start()
    {
        if (SpawnPoints.Count == 0)
        {
            Debug.LogError("EnemySpawner has no spawn points!");
            return;
        }

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (currentWave <= Waves.Count)
        {
            List<EnemyToSpawn> enemiesInWave = Waves[currentWave - 1].wave;

            UpdateCurrentWaveUI();

            foreach (EnemyToSpawn enemyToSpawn in enemiesInWave)
            {
                if (enemyToSpawn.enemy == null || enemyToSpawn.count <= 0)
                continue;

                for (int i = 0; i < enemyToSpawn.count; ++i)
                {
                    GameObject spawnedEnemy = Instantiate(
                        enemyToSpawn.enemy,
                        SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Count)].transform.position,
                        Quaternion.identity
                    );

                    IEnemyDeathNotifier deathNotifier = spawnedEnemy.GetComponent<IEnemyDeathNotifier>();

                    if (deathNotifier == null)
                    {
                        Debug.LogError($"{spawnedEnemy.name} does not implement IEnemyDeathNotifier!");
                        Destroy(spawnedEnemy);
                        continue;
                    }

                    deathNotifier.Died += OnEnemyDied;
                    SpawnedEnemies.Add(deathNotifier);

                    UpdateEnemiesLeftUI();

                    if (i < enemyToSpawn.count - 1 && enemyToSpawn.spawnInterval > 0)
                        yield return new WaitForSeconds(enemyToSpawn.spawnInterval);
                }
            }

            yield return new WaitUntil(() => SpawnedEnemies.Count == 0);

            if (currentWave == Waves.Count)
            {
                AllWavesCompleted?.Invoke();
                yield break;
            }

            yield return new WaitForSeconds(Mathf.Max(0, Waves[currentWave - 1].nextWaveDelay));

            currentWave++;
        }
    }

    private void OnEnemyDied(IEnemyDeathNotifier enemy)
    {
        enemy.Died -= OnEnemyDied;
        SpawnedEnemies.Remove(enemy);

        UpdateEnemiesLeftUI();
    }

    private void OnDestroy()
    {
        foreach (IEnemyDeathNotifier enemy in SpawnedEnemies)
        {
            enemy.Died -= OnEnemyDied;
        }

        SpawnedEnemies.Clear();
    }

    private void UpdateCurrentWaveUI()
    {
        if (CurrentWaveText == null)
        {
            Debug.LogError($"Assign CurrentWaveText in {gameObject.name}");
            return;
        }

        CurrentWaveText.text = $"Wave : {currentWave}";
    }

    private void UpdateEnemiesLeftUI()
    {
        if (EnemiesLeftText == null)
        {
            Debug.LogError($"Assign EnemiesLeftText in {gameObject.name}");
            return;
        }

        EnemiesLeftText.text = $"Enemies : {SpawnedEnemies.Count}";
    }

    public void HideAllUI()
    {
        if(EnemiesLeftText)
        EnemiesLeftText.gameObject.SetActive(false);

        if(CurrentWaveText)
        CurrentWaveText.gameObject.SetActive(false);
    }
}