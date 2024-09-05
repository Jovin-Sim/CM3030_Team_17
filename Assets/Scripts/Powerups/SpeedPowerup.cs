using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerUpgrades/SpeedPowerup")]
public class SpeedPowerup : BasePowerup
{
    [Tooltip("The percentage to increase by")]
    [SerializeField] int percentage;
    // The amount to increase by
    float amount;

    protected override void ApplyEffect(PlayerController player)
    {
        // Calculate the amount that their speed should be increased by
        amount = player.CurrMoveSpeed * percentage / 100f;
        // Increase their speed by the amount
        player.CurrMoveSpeed += amount;
    }

    protected override void RemoveEffect(PlayerController player)
    {
        // Lower their speed back to the previous amount
        player.CurrMoveSpeed -= amount;
    }
}