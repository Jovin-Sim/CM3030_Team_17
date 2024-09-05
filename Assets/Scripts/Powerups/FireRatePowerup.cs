using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerUpgrades/FireRatePowerup")]
public class FireRatePowerup : BasePowerup
{
    [Tooltip("The percentage to increase by")]
    [SerializeField] int percentage;
    // The amount to increase by
    float amount;

    protected override void ApplyEffect(PlayerController player)
    {
        // Calculate the amount that their fire rate should be increased by
        float amount = player.FireRate * percentage / 100f;
        // Increase their fire rate by the amount
        player.FireRate += amount;
    }

    protected override void RemoveEffect(PlayerController player)
    {
        // Lower their fire rate back to the previous amount
        player.FireRate -= amount;
    }
}