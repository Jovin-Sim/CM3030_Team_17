using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerUpgrades/ExplosivesPowerup")]
public class ExplosivesPowerup : BasePowerup
{
    protected override void ApplyEffect(PlayerController player)
    {
        player.BulletExplode = true;
    }

    protected override void RemoveEffect(PlayerController player)
    {
        player.BulletExplode = false;
    }
}