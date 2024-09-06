using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Dictionary containing all enemy types
    Dictionary<int, GameObject> enemyTypes = new Dictionary<int, GameObject>();

    [Tooltip("List of enemies in the game scene (Read Only)")]
    [SerializeField] List<GameObject> enemies = new List<GameObject>();

    [Tooltip("The maximum number of enemies that can exist")]
    [SerializeField] int maxEnemyCount;
    [Tooltip("The maximum number of enemies to spawn when spawning enemies")]
    [SerializeField] int maxSpawnAmount;
    [Tooltip("The time between spawning of newer enemies")]
    [SerializeField] float spawnIInterval;
    [Tooltip("The combined spawn chance of all enemies in the zone")]
    [SerializeField] int totalSpawnChance;

    Coroutine spawnEnemyCoroutine;

    #region Getters & Setters
    public Dictionary<int, GameObject> EnemyTypes {  get { return enemyTypes; } }
    public int EnemyCount { get { return enemies.Count; } }
    #endregion

    /// <summary>
    /// Set the enemy details of the zone
    /// </summary>
    /// <param name="detail">The details of the new zone</param>
    public void SetZoneDetails(ZoneDetails detail)
    {
        // Check if the lists in detail are null or are unequal
        if (detail.EnemySpawnChances == null || detail.EnemyPrefabs == null) Debug.LogError("No enemy names or prefabs were found");
        if (detail.EnemySpawnChances.Count != detail.EnemyPrefabs.Count) Debug.LogError("Enemy name and prefab mismatch");

        // Remove any existing enemy data
        enemyTypes.Clear();

        // Loop through all enemy types in the new zone and add them to enemyTypes
        totalSpawnChance = 0;
        for (int i = 0; i < detail.EnemySpawnChances.Count; ++i)
        {
            enemyTypes[detail.EnemySpawnChances[i]] = detail.EnemyPrefabs[i];
            if (totalSpawnChance < detail.EnemySpawnChances[i]) totalSpawnChance = detail.EnemySpawnChances[i];
        }

        // Set the stats for the new zone's enemy spawning
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

        int togetherChance = Random.Range(0, 100);
        bool together = togetherChance > 70 ? true : false;

        // Spawn a random number of a certain enemy
        SpawnSpecifiedEnemy(GetRandomEnemy(), Random.Range(2, maxSpawnAmount), together);
        // Restart the spawning coroutine
        StopSpawnEnemyCoroutine();
        StartSpawnEnemyCoroutine();
    }

    /// <summary>
    /// Start enemy spawning
    /// </summary>
    public void StartSpawnEnemyCoroutine()
    {
        if (spawnEnemyCoroutine != null) StopSpawnEnemyCoroutine();
        spawnEnemyCoroutine = StartCoroutine(SpawnEnemy());
    }

    /// <summary>
    /// Stop enemy spawning
    /// </summary>
    public void StopSpawnEnemyCoroutine()
    {
        if (spawnEnemyCoroutine == null) return;
        StopCoroutine(spawnEnemyCoroutine);
        spawnEnemyCoroutine = null;
    }

    /// <summary>
    /// Spawns a specific number of a specific enemy
    /// </summary>
    /// <param name="enemy">The enemy to spawn</param>
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
        Vector3 pos = GameplayManager.instance.gridMap.GetEmptyPosition();

        // Spawn the enemies
        for (int i = 0; i < amount; ++i)
        {
            // Spawn them in random locations
            pos = GameplayManager.instance.gridMap.GetEmptyPosition();

            // Spawn the enemy and add them to the list of enemies
            GameObject newEnemy = Instantiate(enemy, pos, Quaternion.identity);
            enemies.Add(newEnemy);
        }
    }

    /// <summary>
    /// Get a random enemy from the available enemy types
    /// </summary>
    /// <returns>The random enemy</returns>
    GameObject GetRandomEnemy()
    {
        // Return nothing if there are no enemies to choose from
        if (enemyTypes.Count <= 0) Debug.LogError("No enemies found!");

        // Get a random number that is between 0 and total spawn chance
        int rand = Random.Range(0, totalSpawnChance);

        // Find the enemy that corresponds to the number
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
        // Do nothing if the enemy is not within the list of enemies
        if (!enemies.Contains(enemy)) return;

        // Increase the player's score
        GameplayManager.instance.zoneProgression.Score += 10;

        GameplayManager.instance.powerupManager.TrySpawnCollectable(enemy.transform.position);

        // Remove the enemy from the list of enemies
        enemies.Remove(enemy);
    }
}
