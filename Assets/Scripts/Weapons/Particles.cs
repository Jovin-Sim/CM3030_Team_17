using System;
using System.Collections;
using UnityEngine;

public class Particles : MonoBehaviour
{
    // The damage the bullet deals
    [SerializeField] float damage;

    /// <summary>
    /// An initialization function that is called when the bullet is first created
    /// </summary>
    /// <param name="damage">The bullet's damage</param>
    /// <param name="pierce">Whether the bullet can pierce through objects or enemies</param>
    public void Init(float damage)
    {
        this.damage = damage;
    }

    private void OnParticleCollision(GameObject other)
    {
        // Check if the other entity has the combat class and damage it if it does
        if (other.TryGetComponent<Combat>(out Combat entity)) entity.ChangeHP(damage);
    }
}