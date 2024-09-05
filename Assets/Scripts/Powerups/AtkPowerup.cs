using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerUpgrades/AtkPowerup")]
public class AtkPowerup : BasePowerup
{
    [Tooltip("The percentage to increase by")]
    [SerializeField] int percentage;
    // The amount to increase by
    float amount;

    protected override void ApplyEffect(PlayerController player)
    {
        // Check if the player has a combat component
        if (player.TryGetComponent<Combat>(out Combat combat))
        {
            // Calculate the amount that their attack should be increased by
            amount = combat.CurrAtk * percentage / 100f;
            // Increase their attack by the amount
            combat.CurrAtk += amount;
        }
    }

    protected override void RemoveEffect(PlayerController player)
    {
        // Check if the player has a combat component
        // And lower their attack back to the previous amount
        if (player.TryGetComponent<Combat>(out Combat combat)) combat.CurrAtk -= amount;
    }
}