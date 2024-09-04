using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerUpgrades/SpeedPowerup")]
public class SpeedPowerup : BasePowerup
{
    [SerializeField] float amount;
    [SerializeField] int percentage;

    protected override void ApplyEffect(PlayerController player)
    {
        float amount = player.CurrMoveSpeed * percentage / 100f;
        player.CurrMoveSpeed += amount;
    }

    protected override void RemoveEffect(PlayerController player)
    {
        player.CurrMoveSpeed -= amount;
    }
}