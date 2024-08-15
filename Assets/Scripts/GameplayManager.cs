using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance = null;

    public GridNodes gridNodes = null;
    public Pathfinding pathfinding = null;
    public EnemyManager enemySpawner = null;

    PlayerController player = null;

    private void Awake()
    {
        if (GameObject.Find("GameplayManager") && GameObject.Find("GameplayManager") != gameObject)
        {
            Destroy(gameObject);
            return;
        }
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        gridNodes = GetComponent<GridNodes>();
        pathfinding = GetComponent<Pathfinding>();
        enemySpawner = GetComponent<EnemyManager>();

        player = FindObjectOfType<PlayerController>();
    }

    public PlayerController Player { get { return player; } }

    public void GameOver()
    {
        if (player == null) return;
        player.GameOver();
    }    
}
