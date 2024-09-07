using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RangedEnemy : BaseEnemy
{
    [Tooltip("The prefab of the ranged particles")]
    [SerializeField] ParticleSystem rangedParticles;
    LayerMask obstacleLayer; // The obstacle layer

    private void Start()
    {
        // Initialize the particles
        rangedParticles = GetComponentInChildren<ParticleSystem>();
        rangedParticles.GetComponent<Particles>().Init(Combat.CurrAtk);

        // Set the obstacle layer
        obstacleLayer = GameplayManager.instance.gridMap.ObstacleLayer;
    }

    private void FixedUpdate()
    {
        // Get the distance between the entity and its target
        float targetDistance = Vector2.Distance(transform.position, Target.transform.position);

        // Stop attacking if the target is outside of the entity's attack range
        if (targetDistance > Combat.AtkRange)
        {
            HoldFire();
            return; 
        }

        // Get the direction of the target
        Vector2 targetDirection = (Target.transform.position - transform.position).normalized;

        // Check if the entity can see the target
        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection, targetDistance, obstacleLayer);

        // Stop attacking if it cannot see the target
        if (hit.collider != null)
        {
            HoldFire();
            return; 
        }

        // Shoots the target
        Fire(targetDirection);
    }

    /// <summary>
    /// Fires particles
    /// </summary>
    /// <param name="direction">The direction to aim the particles towards</param>
    void Fire(Vector2 direction)
    {
        // Log an error if the bullet prefab is missing
        if (rangedParticles == null)
        {
            Debug.LogError("Particle System not found!");
            return;
        }

        // Fire the particles at the direction of the target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rangedParticles.transform.rotation = Quaternion.Euler(new Vector3(-angle, 90f, 0));
        // Fire a bullet
        rangedParticles.Play(); 
    }

    /// <summary>
    /// Stops firing particles
    /// </summary>
    void HoldFire()
    {
        // Log an error if the particles prefab is missing
        if (rangedParticles == null)
        {
            Debug.LogError("Particle System not found!");
            return;
        }
        // Stops the particle system
        if (rangedParticles.isPlaying) rangedParticles.Stop();
    }

    void OnParticleCollision(GameObject other)
    {
        // When the particle collides with another entity,
        // Damages them if it is possible to do so
        if (other.transform.TryGetComponent<Combat>(out Combat otherEntity)) Combat.Attack(otherEntity);
    }
}
