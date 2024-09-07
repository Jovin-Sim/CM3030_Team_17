using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ZoneProgression : MonoBehaviour
{
    // Stores the player progression script that will be used
    PlayerProgression playerProgression;

    [Header("Score Variables")]
    [Tooltip("The text that shows the score")]
    [SerializeField] Text scoreText;
    [Tooltip("The current score (Read Only)")]
    [SerializeField] int score;
    [Tooltip("The scores that the player needs to hit before progressing")]
    [SerializeField] List<int> scoresToNextZone;

    [Header("Zone Variables")]
    [Tooltip("The current zone the player is in (Read Only)")]
    [SerializeField] int currentZone;
    [Tooltip("The center of every zone")]
    [SerializeField] List<Vector3> zones;
    [Tooltip("The details of every zone")]
    [SerializeField] List<ZoneDetails> zoneDetails = new List<ZoneDetails>();

    [Tooltip("The colliders of every zone")]
    [SerializeField] List<GameObject> zoneColliders;
    [Tooltip("The indicators for the next zone")]
    [SerializeField] List<Image> nextZoneIndicators;

    [Tooltip("The bounds of the current zone")]
    [SerializeField] Bounds bounds;

    GridMap gridMap;
    Camera mainCamera;

    Transform playerTransform;
    // A boolean to check if the next zone is being indicated
    bool indicatingNextZone;

    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            // Check if the player can progress
            CheckZoneProgression();
        }
    }
    public int CurrentZone { get { return currentZone; } set { currentZone = value; } }

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
        // Do nothing if the next zone is not being indicated
        if (!indicatingNextZone) return;

        // Check if the player has went to the next zone and update the zone if they have
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(playerTransform.position);
        if (0 > viewportPosition.x || viewportPosition.x > 1 ||
            0 > viewportPosition.y || viewportPosition.y > 1) 
            UpdateZone();
    }

    /// <summary>
    /// Set the player transform
    /// </summary>
    /// <param name="transform">The transform of the player</param>
    public void SetPlayerTransform(Transform transform) { playerTransform = transform; }

    /// <summary>
    /// Check if the player can progress to the next zone
    /// </summary>
    void CheckZoneProgression()
    {
        // Update the score text
        if (scoreText != null) scoreText.text = "Score: " + score.ToString();

        // Do nothing if there are no next zone
        if (scoresToNextZone.Count <= currentZone)
        {
            Debug.LogWarning("No next zone was found!");
            return;
        }

        // Check if the player can progress,
        // Stop activity within the current zone to prepare for player progression to the next zone
        if (score >= scoresToNextZone[currentZone] && !indicatingNextZone) EndZoneActivity();
    }

    /// <summary>
    /// Stop activity within the current zone to prepare for player progression to the next zone
    /// </summary>
    void EndZoneActivity()
    {
        // Log an error and return if the enemy manager cannot be found
        if (GameplayManager.instance.enemyManager == null)
        {
            Debug.LogError("Enemy manager not found!");
            return;
        }

        // Stop enemy spawning
        GameplayManager.instance.enemyManager.StopSpawnEnemyCoroutine();
        // Indicate the direction to the next zone once all enemies in the current zone have been killed
        if (GameplayManager.instance.enemyManager.EnemyCount <= 1) IndicateNextZone();
    }

    /// <summary>
    /// Indicate the direction to the next zone
    /// </summary>
    public void IndicateNextZone()
    {
        // End the game if the player has completed the last zone
        if (zones.Count <= currentZone + 1)
        {
            GameplayManager.instance.EndGame(true);
            return;
        }
        // Log an error and return if no next zone indicators are found
        if (nextZoneIndicators.Count == 0 || nextZoneIndicators[currentZone] == null)
        { 
            Debug.LogError("Next zone indicator could not be found!");
            return;
        }
        // Log an error and return if player progression cannot be found
        if (playerProgression == null)
        {
            Debug.LogError("Player progression script not found!");
            return;
        }

        // Enable the player upgrade panel and show the upgrades they could get
        playerProgression.ShowEffects();

        // Enable the indicator to the next zone
        nextZoneIndicators[currentZone].enabled = true;
        // Allow the player to travel to the next zone
        if (zoneColliders[currentZone].transform.childCount > 0) zoneColliders[currentZone].transform.GetChild(0).gameObject.SetActive(false);

        indicatingNextZone = true;
    }

    /// <summary>
    /// Updates the zone details
    /// </summary>
    public void UpdateZone()
    {
        // Increment currentZone
        ++currentZone;
        // Log a warning and return if the next zone cannot be found
        if (zones.Count <= currentZone)
        {
            Debug.LogWarning("No next zone was found!");
            return;
        }
        // Get the position of the current zone's center
        Vector3 zone = zones[currentZone];

        // Log an error and return if the enemy manager cannot be found
        if (GameplayManager.instance.enemyManager == null)
        {
            Debug.LogError("Enemy manager not found!");
            return;
        }
        // Set the enemy spawning details of the current zone
        GameplayManager.instance.enemyManager.SetZoneDetails(zoneDetails[currentZone]);

        // Enable the zone collider of the current zone and disable the zone collider for the previous zone
        zoneColliders[currentZone].SetActive(true);
        if (currentZone > 0)
        {
            zoneColliders[currentZone - 1].SetActive(false);
            nextZoneIndicators[currentZone - 1].enabled = false;
        }

        // Recalculate the bounds
        bounds = zoneColliders[currentZone].GetComponent<Collider2D>().bounds;

        // Ensure the player is within the new zone bounds
        Vector3 playerPosition = playerTransform.position;

        if (!bounds.Contains(playerPosition))
        {
            // Clamp the player's position to be within the bounds of the zone
            playerPosition.x = Mathf.Clamp(playerPosition.x, bounds.min.x + 0.5f, bounds.max.x - 0.5f);
            playerPosition.y = Mathf.Clamp(playerPosition.y, bounds.min.y + 0.5f, bounds.max.y - 0.5f);

            // Move the player to the clamped position
            playerTransform.position = playerPosition;
        }

        // Recalibrate the camera
        RecalibrateCamera();

        // Get the nodes for this new zone
        gridMap.InitializeNodes(bounds);

        // Start the enemy spawning coroutine again
        GameplayManager.instance.enemyManager.StartSpawnEnemyCoroutine();
        indicatingNextZone = false;
    }

    /// <summary>
    /// Recalibrate the camera for the new zone
    /// </summary>
    void RecalibrateCamera()
    {
        float vertical = bounds.size.y;
        float horizontal = bounds.size.x * mainCamera.pixelHeight / mainCamera.pixelWidth;

        mainCamera.rect = new Rect((1 - 0.8f) / 2, 0, 0.8f, 1);
        mainCamera.orthographicSize = Mathf.Max(horizontal, vertical) * 0.5f;
        mainCamera.transform.position = bounds.center + new Vector3(0, 0, -10);
    }
}