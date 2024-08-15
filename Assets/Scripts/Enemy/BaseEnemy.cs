using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the enemy's gameplay logic.
/// </summary>
public class BaseEnemy : MonoBehaviour
{
    // Composition classes  
    Combat combat;
    PathBasedMovement movement;
    // The target the entity is chasing
    [SerializeField] Collider2D target = null;

    #region Getters & Setters
    public Combat Combat { get { return combat; } }
    public PathBasedMovement Movement { get { return movement; } }
    public Collider2D Target 
    { 
        get { return target; } 
        set { 
            target = value;
            if (movement != null) movement.Target = value; // Set movement's target too
        } 
    }
    #endregion

    private void Awake()
    {
        combat = GetComponent<Combat>();
        movement = GetComponent<PathBasedMovement>();

        if (target != null) return;
        // Set the target as the player
        Target = GameplayManager.instance.Player.GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (target != null) return;
        // Set the target as the player if the enemy has no target
        target = GameplayManager.instance.Player.GetComponent<Collider2D>();
        movement.Target = target;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Collisions between enemies are disabled,
        // therefore any collisions will be between 2 different types of entities
        if (collision.collider == null) return;
        if (collision.transform.TryGetComponent<Combat>(out Combat otherEntity)) combat.TryAttack(otherEntity);
    }
}