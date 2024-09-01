using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Dictionary containing all enemy types
    Dictionary<int, GameObject> enemyTypes = new Dictionary<int, GameObject>();

    // List of enemies in the game scene
    [SerializeField] List<GameObject> enemies = new List<GameObject>();

    // Maximum number of enemies that can exist
    [SerializeField] int maxEnemyCount;
    // Maximum number of enemies to spawn when spawning more enemies
    [SerializeField] int maxSpawnAmount;
    // Time between spawning of newer enemies
    [SerializeField] float spawnIInterval;
    [SerializeField] int totalSpawnChance;

    [SerializeField] Coroutine spawnEnemyCoroutine;
    
    public Dictionary<int, GameObject> EnemyTypes {  get { return enemyTypes; } }
    public int EnemyCount { get { return enemies.Count; } }

    public void SetZoneDetails(ZoneDetails detail)
    {
        if (detail.EnemySpawnChances == null || detail.EnemyPrefabs == null) Debug.LogError("No enemy names or prefabs were found");
        if (detail.EnemySpawnChances.Count != detail.EnemyPrefabs.Count) Debug.LogError("Enemy name and prefab mismatch");
        enemyTypes.Clear();

        totalSpawnChance = 0;
        for (int i = 0; i < detail.EnemySpawnChances.Count; ++i)
        {
            enemyTypes[detail.EnemySpawnChances[i]] = detail.EnemyPrefabs[i];
            if (totalSpawnChance < detail.EnemySpawnChances[i]) totalSpawnChance = detail.EnemySpawnChances[i];
        }

        this.maxEnemyCount = detail.MaxEnemyCount;
        this.maxSpawnAmount = detail.MaxSpawnAmount;
        this.spawnIInterval = detail.SpawnIInterval;
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
        SpawnSpecifiedEnemy(GetRandomEnemy(), Random.Range(2, maxSpawnAmount), true);
        // Restart the spawning coroutine
        StopSpawnEnemyCoroutine();
        StartSpawnEnemyCoroutine();
    }

    public void StartSpawnEnemyCoroutine()
    {
        if (spawnEnemyCoroutine != null) StopSpawnEnemyCoroutine();
        spawnEnemyCoroutine = StartCoroutine(SpawnEnemy());
    }

    public void StopSpawnEnemyCoroutine()
    {
        if (spawnEnemyCoroutine == null) return;
        StopCoroutine(spawnEnemyCoroutine);
        spawnEnemyCoroutine = null;
    }

    /// <summary>
    /// Spawns a specific number of a specific enemy
    /// </summary>
    /// <param name="enemyName">The name of the enemy to spawn</param>
    /// <param name="amount">The amount of enemies to spawn</param>
    /// <param name="together">Boolean for whether to spawn them together</param>
    void SpawnSpecifiedEnemy(GameObject enemy, int amount, bool together)
    {
        // Do nothing if the enemy does not exist or the enemy count is already at the max amount
        if (enemies.Count >= maxEnemyCount || enemy == null) return;

        // Adjust the spawn amount to prevent the number of enemies in the scene to exceed the max amount
        if (enemies.Count + amount >= maxEnemyCount) amount = maxEnemyCount - enemies.Count;

        // Get their size
        float enemySize = enemy.GetComponent<PathBasedMovement>().Tolerance;

        // Check the map for a random empty position to spawn them in that they can fit into
        Vector3 pos = GameplayManager.instance.gridMap.GetEmptyPosition(enemySize * 2);

        // Spawn the enemies
        for (int i = 0; i < amount; ++i)
        {
            // Check the map for a random empty position to spawn them in that they can fit into
            if (together)
            {
                // Add some randomness to the base position to avoid overlap
                Vector3 randomOffset = new Vector3(
                    Random.Range(-enemySize * 0.5f, enemySize * 0.5f),
                    Random.Range(-enemySize * 0.5f, enemySize * 0.5f),
                    0f
                );

                pos += randomOffset;
            }
            else pos = GameplayManager.instance.gridMap.GetEmptyPosition(enemySize * 2);

            // Spawn the enemy and add them to the list of enemies
            GameObject newEnemy = Instantiate(enemy, pos, Quaternion.identity);
            enemies.Add(newEnemy);
        }
    }

    /// <summary>
    /// Get a random enemy name from the available enemy types
    /// </summary>
    /// <returns>The random enemy name</returns>
    GameObject GetRandomEnemy()
    {
        // Return nothing if there are no enemies to choose from
        if (enemyTypes.Count <= 0) Debug.LogError("No enemies found!");

        int rand = Random.Range(0, totalSpawnChance);

        // Return a random enemy
        foreach (var enemyType in enemyTypes)
        {
            if (rand <= enemyType.Key) return enemyType.Value;
            rand -= enemyType.Key;
        }

        // Something went wrong
        return null;
    }
    #endregion

    /// <summary>
    /// Removes the enemy from the list of enemies
    /// </summary>
    /// <param name="enemy">The enemy to remove</param>
    public void DespawnEnemy(GameObject enemy)
    {
        if (!enemies.Contains(enemy)) return;

        ++GameplayManager.instance.zoneProgression.Score;
        enemies.Remove(enemy);
    }
}
