using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerUpgrades/FireRatePowerup")]
public class FireRatePowerup : BasePowerup
{
    [SerializeField] float amount;
    [SerializeField] int percentage;

    protected override void ApplyEffect(PlayerController player)
    {
        float amount = player.FireRate * percentage / 100f;
        player.FireRate += amount;
    }

    protected override void RemoveEffect(PlayerController player)
    {
        player.FireRate -= amount;
    }
}