using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerUpgrades/MaxHealthPowerup")]
public class MaxHealthPowerup : BasePowerup
{
    [SerializeField] float amount;
    [SerializeField] int percentage;

    protected override void ApplyEffect(PlayerController player)
    {
        if (player.TryGetComponent<Combat>(out Combat combat))
        {
            amount = combat.MaxHp * percentage / 100f;
            combat.ChangeMaxHP(-amount);
        }
    }

    protected override void RemoveEffect(PlayerController player)
    {
        if (player.TryGetComponent<Combat>(out Combat combat)) combat.ChangeMaxHP(amount);
    }
}