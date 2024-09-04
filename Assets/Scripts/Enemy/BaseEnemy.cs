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

        // Set the target as the player if the enemy has no target
        if (target != null) return;
        Target = GameplayManager.instance.Player.GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        // Set the target as the player if the enemy has no target
        if (target != null) return;
        Target = GameplayManager.instance.Player.GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Collisions between enemies are disabled,
        // Therefore any collisions will be between 2 different types of entities

        // Check if the enemy can attack
        if (collision.collider == null || !combat.TryAttack()) return;

        // Attack the other entity if they can
        if (collision.transform.TryGetComponent<Combat>(out Combat otherEntity)) combat.Attack(otherEntity);
    }
}