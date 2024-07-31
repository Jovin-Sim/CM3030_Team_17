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

    #region Combat Variables
    // The max and current hp of the entity
    [SerializeField] int maxHp;
    [SerializeField] int currHp;

    // The original and current attack of the entity
    [SerializeField] int originalAtk;
    [SerializeField] int currAtk;

    // The cooldown of the entity
    [SerializeField] float atkCd;
    // The time of the entity's previous attack
    [SerializeField] float lastAtkTime;
    #endregion

    #region Movement Variables
    // The Rigidbody2D of the entity
    Rigidbody2D rb2d;
    // Array of nodes leading the entity to its target
    Vector3[] path;
    // The index of the current node it is moving towards
    int targetIndex;

    // The original and current acceleration of the entity
    [SerializeField] float currAccel;
    [SerializeField] float originalAccel = 1f;

    // The rotation speed of the entity
    [SerializeField] float rotationSpeed = 30f;
    // The tolerance for proximity checks between the entity and the nodes or the target
    [SerializeField] float tolerance = 0f;

    // The target the entity is chasing
    [SerializeField] Collider2D target = null;
    // The last node the target was at
    Node targetNode;
    #endregion

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        tolerance = GetComponent<SpriteRenderer>().bounds.extents.x;
        currAccel = originalAccel;

        if (target == null) target = GameplayManager.instance.Player.GetComponent<Collider2D>();
    }

    void Update()
    {
        if (target == null) target = GameplayManager.instance.Player.GetComponent<Collider2D>();

        // Update the path if there is a target but no path to it
        if (path == null && target != null) UpdatePath();

        if (targetNode != GameplayManager.instance.gridNodes.GetClosestNode(target.transform.position)) UpdatePath();

        // Follow the path
        FollowPath();
    }

    public float Tolerance {  get { return tolerance; } }

    #region Combat Functions
    /// <summary>
    /// Check if the entity is able to attack the target
    /// </summary>
    void TryAttack()
    {
        if (target == null ||
            Time.time >= lastAtkTime + atkCd ||
            Vector2.Distance(transform.position, target.transform.position) <= tolerance)
            return;

        Attack();
    }

    /// <summary>
    /// Attacks the target
    /// </summary>
    void Attack()
    {
        // Attack
    }

    /// <summary>
    /// Applies damage to the entity
    /// </summary>
    /// <param name="damage">The damage taken by the entity</param>
    void TakeDamage(int damage)
    {
        currHp -= damage;
        if (currHp < 0) Die();
    }

    /// <summary>
    /// Destroys the entity if they are killed
    /// </summary>
    void Die()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Movement Functions
    /// <summary>
    /// Updates the path via the Pathfinding class's pathfinding function
    /// </summary>
    void UpdatePath()
    {
        // Do nothing if no such class was found or if there is no target
        if (GameplayManager.instance == null || GameplayManager.instance.pathfinding == null || target == null) return;

        // Compute the path
        path = GameplayManager.instance.pathfinding.AStarPathfinding(transform.position, target.transform.position, tolerance * 2);
        // Set the starting index to 0
        targetIndex = 0;
    }

    /// <summary>
    /// Follow the path by moving through each node
    /// </summary>
    void FollowPath()
    {
        // Do nothing if there is no path or the entity has already reached its target
        if (path == null || path.Length == 0 || targetIndex >= path.Length) return;

        // Do nothing if the entity has already reached its target
        if (Vector2.Distance(transform.position, target.transform.position) <= tolerance * 2) return;

        // Get the position of the current waypoint
        Vector3 currentWaypoint = path[targetIndex];

        // Check if the entity has reached it
        if (Vector2.Distance(transform.position, currentWaypoint) <= tolerance)
        {
            // Set the next waypoint
            targetIndex++;
            // Stop the entity's movement if it has arrived at the target
            if (targetIndex >= path.Length)
            {
                rb2d.velocity = Vector2.Lerp(rb2d.velocity, Vector3.zero, Time.deltaTime * currAccel);
                return;
            }
            // Update the current waypoint
            currentWaypoint = path[targetIndex];
        }

        // Find the direction of the current waypoint
        Vector2 direction = (currentWaypoint - transform.position).normalized;
        // Move the entity towards it
        rb2d.velocity = Vector2.Lerp(rb2d.velocity, direction, Time.deltaTime * currAccel);
        // Rotate the entity to face the correct direction
        RotateTowards(currentWaypoint);
    }

    /// <summary>
    /// Rotate the entity to face the direction it is heading towards
    /// </summary>
    /// <param name="targetPosition">The position that the entity is currently heading towards</param>
    void RotateTowards(Vector3 targetPosition)
    {
        // Calculate the direction of the target position
        Vector2 direction = (targetPosition - transform.position).normalized;

        // Compute the angle of difference between the current direction it is facing and the desired direction it should face
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        float angleDifference = Mathf.DeltaAngle(rb2d.rotation, angle);

        // Set a minimum threshold that should be reached before the entity starts to rotate
        // This prevents the entity from vibrating due to miniscule rotations
        float rotationThreshold = 12.0f;

        // Rotate the entity if the angle is greater than the threshold
        if (Mathf.Abs(angleDifference) > rotationThreshold)
        {
            float targetAngle = Mathf.MoveTowardsAngle(rb2d.rotation, angle, rotationSpeed * Time.deltaTime);
            rb2d.rotation = targetAngle;
        }
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider) TryAttack();
    }

    void OnDrawGizmos()
    {
        // Do nothing if there is no path
        if (path == null) return;

        // Loop through each node and draw them, connecting each node with a line
        for (int i = targetIndex; i < path.Length; ++i)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(path[i], Vector3.one * 0.1f);

            if (i == targetIndex) Gizmos.DrawLine(transform.position, path[i]);
            else Gizmos.DrawLine(path[i - 1], path[i]);
        }
    }
}