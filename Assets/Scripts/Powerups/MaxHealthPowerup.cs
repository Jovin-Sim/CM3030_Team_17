using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerUpgrades/MaxHealthPowerup")]
public class MaxHealthPowerup : BasePowerup
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
            // Calculate the amount that their max health should be increased by
            amount = combat.MaxHp * percentage / 100f;
            // Increase their max health by the amount
            combat.ChangeMaxHP(-amount);
        }
    }

    protected override void RemoveEffect(PlayerController player)
    {
        // Check if the player has a combat component
        // And lower their max health back to the previous amount
        if (player.TryGetComponent<Combat>(out Combat combat)) combat.ChangeMaxHP(amount);
    }
}