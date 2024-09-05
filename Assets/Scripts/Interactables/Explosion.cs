using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interactables/Explosion")]
public class Explosion : ScriptableObject
{
    [SerializeField] float damage; // The damage of the explosion
    [SerializeField] float radius; // The radius of the explosion
    [SerializeField] float force; // The force of the explosion

    [SerializeField] LayerMask LayerToHit; // Layers that should be affected by the explosion

    //public GameObject explosionEffect;
    [SerializeField] GameObject poolPrefab; // Prefab of the pool of liquid that may be left after the explosion

    public float Radius { get { return radius; } }

    /// <summary>
    /// Cause an explosion
    /// </summary>
    /// <param name="origin">The origin point of the explosion</param>
    public void Explode(Vector3 origin)
    {
        // Show effect
        //Instantiate(explosionEffect, transform.position, transform.rotation);

        GameManager.instance.audioManager.PlaySFX(GameManager.instance.audioManager.explosion);

        // Get nearby objects
        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, radius, LayerToHit);

        // Loop through all nearby objects
        foreach (Collider2D nearbyObject in colliders)
        {
            // Get the direction to launch them towards
            Vector2 direction = (nearbyObject.transform.position - origin).normalized;

            // Launch them away from the explosion
            nearbyObject.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);

            // Locks the player movement if the entity is the player
            if (nearbyObject.TryGetComponent<PlayerController>(out PlayerController player))
                player.StartCoroutine(player.LockMovement());

            // Damage the entity
            if (damage > 0 && nearbyObject.TryGetComponent<Combat>(out Combat combat))
                combat.ChangeHP(damage);
        }

        // Instantiate the pool if there is one
        if (poolPrefab != null)
        {
            GameObject waterPool = Instantiate(poolPrefab, origin, Quaternion.identity);
            // Set its lifespan to 3 seconds
            if (waterPool.TryGetComponent<WaterLogic>(out WaterLogic waterLogic))
                waterLogic.Init(3f);
        }
    }
}
