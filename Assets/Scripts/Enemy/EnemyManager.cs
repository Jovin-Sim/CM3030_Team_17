using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Names of all enemy types
    [SerializeField] List<string> enemyNames = new List<string>();
    // Prefabs of all enemy types
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    // Dictionary containing all enemy types
    Dictionary<string, GameObject> enemyTypes = new Dictionary<string, GameObject>();

    // List of enemies in the game scene
    [SerializeField] List<GameObject> enemies = new List<GameObject>();

    // Maximum number of enemies that can exist
    [SerializeField] int maxEnemyCount;
    // Maximum number of enemies to spawn when spawning more enemies
    [SerializeField] int maxSpawnAmount;
    // Time between spawning of newer enemies
    [SerializeField] float spawnIInterval;
    
    void Start()
    {
        if (enemyNames == null || enemyPrefabs == null) Debug.LogError("No enemy names or prefabs found!");
        if (enemyNames.Count != enemyPrefabs.Count) Debug.LogError("Unequal number of enemy names and prefabs!");

        // Populate enemyTypes with the names and prefabs
        for (int i = 0; i < enemyNames.Count; ++i)
            enemyTypes[enemyNames[i]] = enemyPrefabs[i];

        // Start a coroutine to spawn enemies
        StartCoroutine(SpawnEnemy());
    }

    #region Spawning Functions
    /// <summary>
    /// Spawns a random number of enemies at regular intervals
    /// </summary>
    IEnumerator SpawnEnemy()
    {
        // Wait for a number of seconds equal to spawnIInterval before proceeding with spawning
        yield return new WaitForSeconds(spawnIInterval);
        // Spawn a random number of a certain enemy
        SpawnSpecifiedEnemy(GetRandomEnemyName(), Random.Range(1, maxSpawnAmount), true);
        // Restart the spawning coroutine
        StartCoroutine(SpawnEnemy());
    }

    /// <summary>
    /// Spawns a specific number of a specific enemy
    /// </summary>
    /// <param name="enemyName">The name of the enemy to spawn</param>
    /// <param name="amount">The amount of enemies to spawn</param>
    /// <param name="together">Boolean for whether to spawn them together</param>
    void SpawnSpecifiedEnemy(string enemyName, int amount, bool together)
    {
        // Do nothing if the enemy does not exist or the enemy count is already at the max amount
        if (enemies.Count >= maxEnemyCount || !enemyTypes.ContainsKey(enemyName)) return;

        // Adjust the spawn amount to prevent the number of enemies in the scene to exceed the max amount
        if (enemies.Count + amount >= maxEnemyCount) amount = maxEnemyCount - enemies.Count;

        // Spawn the enemies
        for (int i = 0; i < amount; ++i)
        {
            // Get their size
            float enemySize = enemyTypes[enemyName].GetComponent<PathBasedMovement>().Tolerance;
            // Check the map for a random empty position to spawn them in that they can fit into
            Vector3 pos = GameplayManager.instance.gridMap.GetEmptyPosition(enemySize);
            // No empty positions were found, do nothing
            if (pos == Vector3.zero) continue;

            // Spawn the enemy and add them to the list of enemies
            GameObject enemy = Instantiate(enemyTypes[enemyName], pos, Quaternion.identity);
            enemies.Add(enemy);
        }
    }

    /// <summary>
    /// Get a random enemy name from the available enemy types
    /// </summary>
    /// <returns>The random enemy name</returns>
    string GetRandomEnemyName()
    {
        // Return nothing if there are no enemies to choose from
        if (enemyTypes.Count <= 0) return "";

        // Return a random enemy name
        List<string> keys = new List<string>(enemyTypes.Keys);
        return keys[Random.Range(0, keys.Count)];
    }
    #endregion

    /// <summary>
    /// Removes the enemy from the list of enemies
    /// </summary>
    /// <param name="enemy">The enemy to remove</param>
    public void DespawnEnemy(GameObject enemy)
    {
        if (enemies.Contains(enemy)) enemies.Remove(enemy);
    }
}
