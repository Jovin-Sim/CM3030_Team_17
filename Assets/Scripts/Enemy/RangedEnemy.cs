using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RangedEnemy : BaseEnemy
{
    [SerializeField] ParticleSystem rangedParticles; // The prefab of the ranged particles
    LayerMask obstacleLayer;
    [SerializeField] Vector2 dir;

    private void Start()
    {
        rangedParticles = GetComponentInChildren<ParticleSystem>();
        rangedParticles.GetComponent<Particles>().Init(Combat.CurrAtk);
        obstacleLayer = GameplayManager.instance.gridMap.ObstacleLayer;
    }

    private void FixedUpdate()
    {
        float targetDistance = Vector2.Distance(transform.position, Target.transform.position);
        if (targetDistance > Combat.AtkRange)
        {
            HoldFire();
            return; 
        }

        Vector2 targetDirection = (Target.transform.position - transform.position).normalized;
        dir = targetDirection;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection, targetDistance, obstacleLayer);
        if (hit.collider != null)
        {
            HoldFire();
            return; 
        }

        Fire(targetDirection);
    }

    void Fire(Vector2 direction)
    {
        // Log an error if the bullet prefab is missing
        if (rangedParticles == null)
        {
            Debug.LogError("Particle System not found!");
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rangedParticles.transform.rotation = Quaternion.Euler(new Vector3(-angle, 90f, 0));
        // Fire a bullet
        rangedParticles.Play(); 
    }

    void HoldFire()
    {
        // Log an error if the bullet prefab is missing
        if (rangedParticles == null)
        {
            Debug.LogError("Particle System not found!");
            return;
        }
        if (rangedParticles.isPlaying) rangedParticles.Stop();
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.transform.TryGetComponent<Combat>(out Combat otherEntity)) Combat.Attack(otherEntity);
    }
}
