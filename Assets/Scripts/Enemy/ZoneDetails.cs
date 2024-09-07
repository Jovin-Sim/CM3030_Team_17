using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneDetails : MonoBehaviour
{
    [Tooltip("The spawn chances of all enemies")]
    [SerializeField] List<int> enemySpawnChances = new List<int>();
    [Tooltip("The prefabs of all enemy types")]
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    [Tooltip("The maximum number of enemies that can exist")]
    [SerializeField] int maxEnemyCount;
    [Tooltip("The maximum number of enemies to spawn when spawning enemies")]
    [SerializeField] int maxSpawnAmount;
    [Tooltip("The time between spawning of newer enemies")]
    [SerializeField] float spawnIInterval;

    #region Getters & Setters
    public List<int> EnemySpawnChances { get { return enemySpawnChances; } }
    public List<GameObject> EnemyPrefabs { get { return enemyPrefabs; } }
    public int MaxEnemyCount { get { return maxEnemyCount; } }
    public int MaxSpawnAmount { get { return maxSpawnAmount; } }
    public float SpawnIInterval { get { return spawnIInterval; } }
    #endregion
}