using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    // Static instance to implement the singleton pattern
    public static GameplayManager instance = null;

    // References to managers in the game
    public MenuManager menuManager = null;
    public GridMap gridMap = null;
    public ZoneProgression zoneProgression = null;
    public Pathfinding pathfinding = null;
    public EnemyManager enemyManager = null;
    public PowerupManager powerupManager = null;

    PlayerController player = null;

    public PlayerController Player { get { return player; } }

    private void Awake()
    {
        // Destroy any other instance of gameplay manager
        if (GameObject.Find("GameplayManager") && GameObject.Find("GameplayManager") != gameObject)
        {
            Destroy(gameObject);
            return;
        }
        // Assign this as the instance if there are no other instances
        if (instance == null)
            instance = this;
        // If the current instance is not the same as this, destroy the current object
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        menuManager = GetComponent<MenuManager>();
        gridMap = GetComponent<GridMap>();
        zoneProgression = GetComponent<ZoneProgression>();
        pathfinding = GetComponent<Pathfinding>();
        enemyManager = GetComponent<EnemyManager>();
        powerupManager = GetComponent<PowerupManager>();

        player = FindObjectOfType<PlayerController>();
        zoneProgression.SetPlayerTransform(player.transform);
    }

    void Start()
    {
        zoneProgression.UpdateZone();
    }

    /// <summary>
    /// End the game
    /// </summary>
    /// <param name="win">The win status</param>
    public void EndGame(bool win)
    {
        menuManager.EndGame(win);
    }    
}
