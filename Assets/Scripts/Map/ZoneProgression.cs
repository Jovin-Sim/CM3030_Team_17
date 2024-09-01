using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ZoneProgression : MonoBehaviour
{
    PlayerProgression playerProgression;

    [SerializeField] int score;
    [SerializeField] List<int> scoresToNextZone;

    [SerializeField] int currentZone;
    [SerializeField] List<Vector3> zones;
    [SerializeField] List<ZoneDetails> zoneDetails = new List<ZoneDetails>();

    [SerializeField] List<GameObject> zoneColliders;
    [SerializeField] List<Image> nextZoneIndicators;

    [SerializeField] Vector2 minBounds, maxBounds;
    GridMap gridMap;
    Camera mainCamera;

    Transform playerTransform;
    bool indicatingNextZone;

    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            CheckZoneProgression();
        }
    }
    public int CurrentZone { get { return currentZone; } set { currentZone = value; } }
    public Vector2 MinBounds { get { return minBounds; } }
    public Vector2 MaxBounds { get { return maxBounds; } }

    void Awake()
    {
        currentZone = -1;
        gridMap = GetComponent<GridMap>();
        mainCamera = Camera.main;
        score = 0;
        indicatingNextZone = false;
    }

    private void Start()
    {
        playerProgression = FindObjectOfType<PlayerProgression>();
        playerProgression.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!indicatingNextZone) return;

        Vector3 playerPosition = playerTransform.position;
        if (minBounds.x > playerPosition.x || playerPosition.x > maxBounds.x ||
            minBounds.y > playerPosition.y || playerPosition.y > maxBounds.y)
            UpdateZone();
    }

    public void SetPlayerTransform(Transform transform) { playerTransform = transform; }

    void CheckZoneProgression()
    {
        if (scoresToNextZone.Count <= currentZone)
        {
            Debug.LogWarning("No next zone was found!");
            return;
        }

        if (score >= scoresToNextZone[currentZone] && !indicatingNextZone) EndZoneActivity();
    }

    void EndZoneActivity()
    {
        if (GameplayManager.instance.enemyManager == null)
        {
            Debug.LogError("Enemy manager not found!");
            return;
        }

        GameplayManager.instance.enemyManager.StopSpawnEnemyCoroutine();
        if (GameplayManager.instance.enemyManager.EnemyCount <= 1) IndicateNextZone();
    }

    public void IndicateNextZone()
    {
        if (zones.Count <= currentZone)
        {
            Debug.LogWarning("No next zone was found!");
            return;
        }
        if (nextZoneIndicators.Count == 0 || nextZoneIndicators[currentZone] == null)
        { 
            Debug.LogError("Next zone indicator could not be found!");
            return;
        }
        if (playerProgression == null)
        {
            Debug.LogError("Player progression script not found!");
            return;
        }

        playerProgression.gameObject.SetActive(true);
        playerProgression.ShowEffects();

        nextZoneIndicators[currentZone].enabled = true;
        if (zoneColliders[currentZone].transform.childCount > 0) zoneColliders[currentZone].transform.GetChild(0).gameObject.SetActive(false);

        indicatingNextZone = true;
    }

    public void UpdateZone()
    {
        ++currentZone;
        if (zones.Count <= currentZone)
        {
            Debug.LogWarning("No next zone was found.");
            return;
        }
        Vector3 zone = zones[currentZone];

        if (GameplayManager.instance.enemyManager == null)
        {
            Debug.LogError("Enemy manager not found!");
            return;
        }
        GameplayManager.instance.enemyManager.SetZoneDetails(zoneDetails[currentZone]);

        mainCamera.transform.position = zone;
        minBounds = mainCamera.ScreenToWorldPoint(mainCamera.pixelRect.min);
        maxBounds = mainCamera.ScreenToWorldPoint(mainCamera.pixelRect.max);

        zoneColliders[currentZone].SetActive(true);
        if (currentZone > 0)
        {
            zoneColliders[currentZone - 1].SetActive(false);
            nextZoneIndicators[currentZone - 1].enabled = false;
        }

        GameplayManager.instance.Player.MinBounds = minBounds;
        GameplayManager.instance.Player.MaxBounds = maxBounds;

        gridMap.InitializeNodes(minBounds, maxBounds);

        GameplayManager.instance.enemyManager.StartSpawnEnemyCoroutine();
        indicatingNextZone = false;
    }
}