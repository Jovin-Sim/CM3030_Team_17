using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerUpgrades/HealthPowerup")]
public class HealthPowerup : BasePowerup
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
            // Calculate the amount that their health should be increased by
            amount = combat.MaxHp * percentage / 100f;
            // Increase their health by the amount
            combat.ChangeHP(-amount);
        }
    }

    protected override void RemoveEffect(PlayerController player)
    {}
}