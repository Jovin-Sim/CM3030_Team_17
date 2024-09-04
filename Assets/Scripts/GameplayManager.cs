using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance = null;

    public MenuManager menuManager = null;
    public GridMap gridMap = null;
    public ZoneProgression zoneProgression = null;
    public Pathfinding pathfinding = null;
    public EnemyManager enemyManager = null;

    PlayerController player = null;

    public PlayerController Player { get { return player; } }

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

        menuManager = gameObject.GetComponent<MenuManager>();
        gridMap = GetComponent<GridMap>();
        zoneProgression = GetComponent<ZoneProgression>();
        pathfinding = GetComponent<Pathfinding>();
        enemyManager = GetComponent<EnemyManager>();

        player = FindObjectOfType<PlayerController>();
        zoneProgression.SetPlayerTransform(player.transform);
    }

    void Start()
    {
        zoneProgression.UpdateZone();
    }

    public void EndGame(bool win)
    {
        menuManager.EndGame(win);
    }    
}
