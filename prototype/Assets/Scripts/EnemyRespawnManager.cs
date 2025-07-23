using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        public Vector2 spawnPosition;
    }

    private List<EnemySpawnData> spawnData = new List<EnemySpawnData>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Find all enemies in the scene and register them
        Enemy[] existingEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy e in existingEnemies)
        {
            if (e.enemyPrefabReference != null)
            {
                RegisterEnemy(e.enemyPrefabReference, e.transform.position, e.gameObject);
            }
            else
            {
                Debug.LogWarning($"Enemy '{e.name}' is missing its prefab reference.");
            }
        }
    }

    public void RegisterEnemy(GameObject enemyPrefab, Vector2 spawnPos, GameObject instance = null)
    {
        spawnData.Add(new EnemySpawnData { enemyPrefab = enemyPrefab, spawnPosition = spawnPos });

        if (instance != null)
            spawnedEnemies.Add(instance);
    }

    public void RespawnAllEnemies()
    {
        // Destroy ALL enemies, including scene instances
        var allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (var enemy in allEnemies)
        {
            Destroy(enemy.gameObject);
        }

        spawnedEnemies.Clear();

        foreach (var data in spawnData)
        {
            GameObject enemy = Instantiate(data.enemyPrefab, data.spawnPosition, Quaternion.identity);
            spawnedEnemies.Add(enemy);
        }
    }
}
