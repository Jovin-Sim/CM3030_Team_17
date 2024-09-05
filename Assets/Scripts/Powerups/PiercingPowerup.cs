using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerUpgrades/PiercingPowerup")]
public class PiercingPowerup : BasePowerup
{
    protected override void ApplyEffect(PlayerController player)
    {
        // Enable bullet piercing
        player.BulletPierce = true;
    }

    protected override void RemoveEffect(PlayerController player)
    {
        // Disable bullet piercing
        player.BulletPierce = false;
    }
}