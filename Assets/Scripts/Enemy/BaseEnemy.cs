using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the enemy's gameplay logic.
/// </summary>
public class BaseEnemy : MonoBehaviour
{
    // Unique identifier for the entity
    int id = 0;

    [SerializeField] Combat combat;
    [SerializeField] PathBasedMovement movement;

    // The target the entity is chasing
    [SerializeField] Collider2D target = null;
    public Combat Combat { get { return combat; } }
    public PathBasedMovement Movement { get { return movement; } }
    public Collider2D Target 
    { 
        get { return target; } 
        set { 
            target = value; 
            if (movement != null) movement.Target = value;
        } 
    }

    private void Awake()
    {
        combat = GetComponent<Combat>();
        movement = GetComponent<PathBasedMovement>();

        if (target != null) return;
        target = GameplayManager.instance.Player.GetComponent<Collider2D>();
        movement.Target = target;
    }

    private void FixedUpdate()
    {
        if (target != null) return;
        target = GameplayManager.instance.Player.GetComponent<Collider2D>();
        movement.Target = target;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider == null) return;
        if (collision.transform.TryGetComponent<Combat>(out Combat otherEntity)) combat.TryAttack(otherEntity);
    }
}