using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // The effect that is created on bullet's destruction
    [SerializeField] GameObject hitEffect;
    [SerializeField] Explosion explosionEffect;
    // The damage the bullet deals
    [SerializeField] float damage;
    // The lifespan of the bullet before it is automatically destroyed
    [SerializeField] float lifespan = 1f;
    // A check for if the bullet can pierce through objects or enemies
    [SerializeField] bool pierce;
    // A check for if the bullet explodes upon impact
    [SerializeField] bool explodeOnImpact;

    /// <summary>
    /// An initialization function that is called when the bullet is first created
    /// </summary>
    /// <param name="damage">The bullet's damage</param>
    /// <param name="pierce">Whether the bullet can pierce through objects or enemies</param>
    public void Init(float damage, bool pierce = false, bool explodeOnImpact = false)
    {
        this.damage = damage;
        this.pierce = pierce;
        this.explodeOnImpact = explodeOnImpact;
    }
    
    void Start()
    {
        // Destroy the object after its lifespan has been reached
        Destroy(gameObject, lifespan);
    }

    void OnTriggerEnter2D(Collider2D collision)
   {
        // Check if the collision object is water and ignore it if it is
        if (collision.GetComponent<WaterLogic>() != null) return;

        // Check if the other entity has the combat class and damage it if it does
        if (collision.TryGetComponent<Combat>(out Combat entity)) entity.ChangeHP(damage);

        // Create the hit effect
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(effect, 0.25f);

        // Destroy the bullet if pierce is not active
        if (!pierce) Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!explodeOnImpact) return;

        explosionEffect.Explode(transform.position);
    }
}