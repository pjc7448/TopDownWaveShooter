using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("----Enemy Settings----")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] int maxEnemies = 25;
    [SerializeField] float spawnInterval = 2f;

    [Header("----Spawn Area----")]
    [SerializeField] Vector3 spawnAreaCenter;
    [SerializeField] Vector3 spawnAreaSize = new Vector3(50, 0, 50);

    float spawnTimer;
    int spawnedEnemies;


    void Update()
    {
        if (spawnedEnemies >= maxEnemies) return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0;
        }
    }


    void SpawnEnemy()
    {
        Vector3 randomPos = spawnAreaCenter + new Vector3(
        Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2), 0, Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2));

        Instantiate(enemyPrefab, randomPos, Quaternion.identity);
        spawnedEnemies++;
    }
}
