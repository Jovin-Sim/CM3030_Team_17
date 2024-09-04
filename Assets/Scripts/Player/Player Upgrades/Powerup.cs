using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public abstract class BasePowerup : ScriptableObject
{
    [SerializeField] string powerupName;
    [SerializeField] string description;
    [SerializeField] public float duration = -1;

    public string PowerupName { get { return powerupName; } }

    public virtual void ApplyPowerup(PlayerController player)
    {
        // Apply the powerup
        ApplyEffect(player);

        // Start the countdown until the powerup is lost if there is a duration
        if (duration > 0) player.StartCoroutine(HandleDuration(player));
    }

    protected abstract void ApplyEffect(PlayerController player);

    private IEnumerator HandleDuration(PlayerController player)
    {
        // Remove the powerup effect once the duration is over
        yield return new WaitForSeconds(duration);
        RemoveEffect(player);
    }

    protected abstract void RemoveEffect(PlayerController player);
}