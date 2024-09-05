using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerUpgrades/AtkPowerup")]
public class AtkPowerup : BasePowerup
{
    [SerializeField] float amount;
    [SerializeField] int percentage;

    protected override void ApplyEffect(PlayerController player)
    {
        if (player.TryGetComponent<Combat>(out Combat combat))
        {
            amount = combat.CurrAtk * percentage / 100f;
            combat.CurrAtk += amount;
        }
    }

    protected override void RemoveEffect(PlayerController player)
    {
        if (player.TryGetComponent<Combat>(out Combat combat)) combat.CurrAtk -= amount;
    }
}