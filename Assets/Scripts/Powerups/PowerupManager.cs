using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    [Tooltip("The gameobject for the collectable")]
    [SerializeField] GameObject collectable;

    [Tooltip("The list of all powerups")]
    [SerializeField] List<BasePowerup> allPowerups = new List<BasePowerup>();
    [Tooltip("The list of all available powerups")]
    [SerializeField] List<BasePowerup> availablePowerups;
    [Tooltip("The list of all active powerups")]
    [SerializeField] List<BasePowerup> activePowerups = new List<BasePowerup>();

    public List<BasePowerup> AvailablePowerups { get { return availablePowerups; } }

    public void Init()
    {
        // Copy the contents of allPowerups into availablePowerups
        availablePowerups = new List<BasePowerup>(allPowerups);
        activePowerups = new List<BasePowerup>();
    }

    /// <summary>
    /// Get a random powerup from the list of available powerups
    /// </summary>
    /// <param name="duration">The duration of the powerup</param>
    /// <returns>The powerup</returns>
    public BasePowerup GetRandomPowerup(float duration)
    {
        // Do nothing if there are no available powerups
        if (availablePowerups.Count == 0) return null;

        // Get a random powerup from the list
        BasePowerup originalPowerup = availablePowerups[Random.Range(0, availablePowerups.Count)];

        // Create a copy of the powerup
        BasePowerup powerup = Instantiate(originalPowerup);

        // Set its duration
        powerup.Duration = duration;

        // Return the powerup
        return powerup;
    }

    /// <summary>
    /// Try to spawn a collectable
    /// </summary>
    /// <param name="position">The position to spawn the collectable</param>
    public void TrySpawnCollectable(Vector2 position)
    {
        // Do nothing if collectable is null
        if (collectable == null) return;

        // Get the spawn chance and do nothing if the spawn chance is not reached
        int spawnChance = Random.Range(0, 100);
        if (spawnChance < 55) return;

        // Create a new copy of the collectable
        GameObject newCollectable = Instantiate(collectable, position, Quaternion.identity);

        // Initialize it with a random powerup
        if (newCollectable.TryGetComponent<Collectable>(out Collectable item))
            item.Init(GetRandomPowerup(Random.Range(10, 20)));
    }

    /// <summary>
    /// Apply a powerup to the player
    /// </summary>
    /// <param name="powerup">The powerup to apply</param>
    public void ApplyPowerup(BasePowerup powerup)
    {
        // Add the powerup to the active list and apply the effect
        activePowerups.Add(powerup);
        powerup.ApplyPowerup(GameplayManager.instance.Player);

        // If the powerup has a duration, manage its removal after the duration ends
        if (powerup.Duration > 0) 
            StartCoroutine(HandleDuration(powerup));
        // Check if the powerup can only be used once
        else if (powerup.LimitedUse)
        {
            // Remove the powerup from the list of available powerups
            BasePowerup originalPowerup = availablePowerups.Find(p => p.PowerupName == powerup.PowerupName);
            if (originalPowerup != null) availablePowerups.Remove(originalPowerup);
        }
    }

    /// <summary>
    /// Handle the powerup's duration
    /// </summary>
    /// <param name="powerup">The powerup to handle</param>
    IEnumerator HandleDuration(BasePowerup powerup)
    {
        // Remove the powerup effect once the duration is over
        yield return new WaitForSeconds(powerup.Duration);

        // Remove the powerup from the player and the list of active powerups
        powerup.RemovePowerup(GameplayManager.instance.Player);
        activePowerups.Remove(powerup);
    }

    /// <summary>
    /// Remove all powerups
    /// </summary>
    public void RemoveAllPowerups()
    {
        // Loop through all active powerups and remove their effects
        foreach (var powerup in activePowerups)
        {
            powerup.RemovePowerup(GameplayManager.instance.Player);
        }

        // Clear the list of active powerups
        activePowerups.Clear();
    }
}
