using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneDetails : MonoBehaviour
{
    // Spawn chances of all enemies
    [SerializeField] List<int> enemySpawnChances = new List<int>();
    // Prefabs of all enemy types
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    // Maximum number of enemies that can exist
    [SerializeField] int maxEnemyCount;
    // Maximum number of enemies to spawn when spawning more enemies
    [SerializeField] int maxSpawnAmount;
    // Time between spawning of newer enemies
    [SerializeField] float spawnIInterval;

    public List<int> EnemySpawnChances { get { return enemySpawnChances; } }
    public List<GameObject> EnemyPrefabs { get { return enemyPrefabs; } }
    public int MaxEnemyCount { get { return maxEnemyCount; } }
    public int MaxSpawnAmount { get { return maxSpawnAmount; } }
    public float SpawnIInterval { get { return spawnIInterval; } }
}