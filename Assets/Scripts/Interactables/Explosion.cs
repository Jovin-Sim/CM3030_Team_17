using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Interactables/Explosion")]
public class Explosion : ScriptableObject
{
    public float delay = 3f;
    public float radius;
    public float force;

    public LayerMask LayerToHit;

    //public GameObject explosionEffect;
    [SerializeField] GameObject poolPrefab;

    float countdown;

    public void Explode(GameObject origin)
    {
        Debug.Log("BOOM!");

        // Show effect
        //Instantiate(explosionEffect, transform.position, transform.rotation);

        // Get nearby objects
        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin.transform.position, radius, LayerToHit);

        foreach (Collider2D nearbyObject in colliders)
        {
            // Add force
            Vector2 direction = (nearbyObject.transform.position - origin.transform.position).normalized;

            nearbyObject.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
            if (nearbyObject.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.StartCoroutine(player.LockMovement());
            }
            // Damage
        }

        // Instantiate the pool if there is one
        if (poolPrefab != null)
        {
            GameObject waterPool = Instantiate(poolPrefab, origin.transform.position, Quaternion.identity);
            // Set its lifespan to 3 seconds
            if (waterPool.TryGetComponent<WaterLogic>(out WaterLogic waterLogic))
                waterLogic.Init(3f);
        }
    }
}
