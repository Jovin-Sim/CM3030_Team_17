using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerUpgrades/ExplosivesPowerup")]
public class ExplosivesPowerup : BasePowerup
{
    protected override void ApplyEffect(PlayerController player)
    {
        // Enable bullet explosion
        player.BulletExplode = true;
    }

    protected override void RemoveEffect(PlayerController player)
    {
        // Disable bullet explosion
        player.BulletExplode = false;
    }
}