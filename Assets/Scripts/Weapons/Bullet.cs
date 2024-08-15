using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // The effect that is created on bullet's destruction
    [SerializeField] GameObject hitEffect;
    // The damage the bulllet deals
    [SerializeField] int damage;
    // The lifespan of the bullet before it is automatically destroyed
    [SerializeField] float lifespan = 5;
    // A check for if the bullet can pierce through objects or enemies
    [SerializeField] bool pierce;

    /// <summary>
    /// An initialization function that is called when the bullet is first created
    /// </summary>
    /// <param name="damage">The bullet's damage</param>
    /// <param name="pierce">Whether the bullet can pierce through objects or enemies</param>
    public void Init(int damage, bool pierce = false)
    {
        this.damage = damage;
        this.pierce = pierce;
    }
    
    void Start()
    {
        // Start the coroutine counting down to the bullet's death
        StartCoroutine(Alive());
    }

    /// <summary>
    /// A coroutine that counts down to the bullet's death
    /// </summary>
    IEnumerator Alive()
    {
        // Set the start time
        float startTime = Time.time;
        // Yield null while the bullet has not exceeded its lifespan
        while (Time.time < startTime + lifespan)
        {
            yield return null;
        }
        // Destroy the bullet once its lifespan is gone
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
   {
        // Check if the other entity has the combat class and damage it if it does
        if (collision.TryGetComponent<Combat>(out Combat entity)) entity.TakeDamage(damage);
        if (pierce) return;

        // Create the hit effect before destroying the bullet
        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(effect, 0.25f);
        Destroy(gameObject);
    }
}