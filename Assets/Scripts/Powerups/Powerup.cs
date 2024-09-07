using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public abstract class BasePowerup : ScriptableObject
{
    [Tooltip("The sprite for the powerup")]
    [SerializeField] Sprite sprite;
    [Tooltip("The name of the powerup")]
    [SerializeField] string powerupName;
    [Tooltip("The duration of the powerup")]
    [SerializeField] float duration = -1;
    [Tooltip("A boolean for if the powerup can only be used once")]
    [SerializeField] bool limitedUse = false;

    #region Getters & Setters
    public Sprite Sprite { get { return sprite; } }
    public string PowerupName { get { return powerupName; } }
    public float Duration { get { return duration; } set { duration = value; } }
    public bool LimitedUse { get { return limitedUse; } }
    #endregion

    /// <summary>
    /// Apply the powerup on the player
    /// </summary>
    /// <param name="player">The player</param>
    public virtual void ApplyPowerup(PlayerController player)
    {
        // Apply the powerup
        ApplyEffect(player);
    }
    /// <summary>
    /// Remove the powerup from the player
    /// </summary>
    /// <param name="player">The player</param>
    public virtual void RemovePowerup(PlayerController player)
    {
        // Remove the powerup
        RemoveEffect(player);
    }

    /// <summary>
    /// Apply the effect on the player
    /// </summary>
    /// <param name="player">The player</param>
    protected abstract void ApplyEffect(PlayerController player);
    /// <summary>
    /// Remove the effect from the player
    /// </summary>
    /// <param name="player">The player</param>
    protected abstract void RemoveEffect(PlayerController player);
}