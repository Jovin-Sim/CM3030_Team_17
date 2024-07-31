using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<string> enemyNames = new List<string>();
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    Dictionary<string, GameObject> enemyTypes = new Dictionary<string, GameObject>();

    [SerializeField] List<GameObject> enemies = new List<GameObject>();

    [SerializeField] int maxEnemyCount;
    [SerializeField] int maxSpawnAmount;
    [SerializeField] float spawnIInterval;

    // Start is called before the first frame update
    void Start()
    {
        if (enemyNames == null || enemyPrefabs == null) Debug.LogError("No enemy names or prefabs found!");
        if (enemyNames.Count != enemyPrefabs.Count) Debug.LogError("Unequal number of enemy names and prefabs!");

        for (int i = 0; i < enemyNames.Count; ++i)
            enemyTypes[enemyNames[i]] = enemyPrefabs[i];

        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(spawnIInterval);
        SpawnSpecifiedEnemy(GetRandomEnemyName(), Random.Range(1, maxSpawnAmount), true);
        StartCoroutine(SpawnEnemy());
    }

    void SpawnSpecifiedEnemy(string enemyName, int amount, bool together)
    {
        if (!enemyTypes.ContainsKey(enemyName)) return;
        if (enemies.Count >= maxEnemyCount) return;
        if (enemies.Count + amount >= maxEnemyCount) amount = maxEnemyCount - enemies.Count;

        for (int i = 0; i < amount; ++i)
        {
            float enemySize = enemyTypes[enemyName].GetComponent<BaseEnemy>().Tolerance;
            Vector3 pos = GameplayManager.instance.gridNodes.GetEmptyPosition(enemySize);
            if (pos == Vector3.zero) continue;

            GameObject enemy = Instantiate(enemyTypes[enemyName], pos, Quaternion.identity);
            enemies.Add(enemy);
        }

        Debug.Log("Spawned " + amount + " " + enemyName);
    }

    string GetRandomEnemyName()
    {
        if (enemyTypes.Count <= 0) return "";

        List<string> keys = new List<string>(enemyTypes.Keys);
        return keys[Random.Range(0, keys.Count)];
    }
}
