using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] List<GameObject> SpawnPoints = new List<GameObject>();
    [SerializeField] List<Wave> Waves = new List<Wave>();

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
    }

    int currentWave = 1;

    HashSet<IEnemyDeathNotifier> SpawnedEnemies = new HashSet<IEnemyDeathNotifier>();

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

            foreach (EnemyToSpawn enemyToSpawn in enemiesInWave)
            {
                if(enemyToSpawn.enemy==null || enemyToSpawn.count==0)
                continue;
                
                for (int i = 0; i < enemyToSpawn.count; ++i)
                {
                    GameObject spawnedEnemy = Instantiate(
                        enemyToSpawn.enemy,
                        SpawnPoints[Random.Range(0, SpawnPoints.Count)].transform.position,
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
                }
            }

            yield return new WaitUntil(() => SpawnedEnemies.Count == 0);

            if (currentWave == Waves.Count)
                yield break;

            yield return new WaitForSeconds(Mathf.Max(0, Waves[currentWave - 1].nextWaveDelay));

            currentWave++;
        }
    }

    private void OnEnemyDied(IEnemyDeathNotifier enemy)
    {
        enemy.Died -= OnEnemyDied;
        SpawnedEnemies.Remove(enemy);
    }

    private void OnDestroy()
    {
        foreach (IEnemyDeathNotifier enemy in SpawnedEnemies)
        {
            enemy.Died -= OnEnemyDied;
        }

        SpawnedEnemies.Clear();
    }
}