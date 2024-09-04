using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBasedMovement : MonoBehaviour
{
    // The Rigidbody2D of the entity
    Rigidbody2D rb2d;
    // Array of nodes leading the entity to its target
    Vector3[] path;
    // The index of the current node it is moving towards
    int targetIndex;

    // The original and current acceleration of the entity
    [SerializeField] float currAccel;
    [SerializeField] float originalAccel = 1f;

    // The distance the entity is from its target before it stops following the player
    [SerializeField] float stoppingRange = 0f;

    // The tolerance for proximity checks between the entity and the nodes or the target
    [SerializeField] float tolerance = 0.1f;

    // The target the entity is chasing
    [SerializeField] Collider2D target = null;
    // The last node the target was at
    Node targetNode;
    // Last position the target was in
    Vector3 prevTargetPos;
    // The obstacle layer
    LayerMask obstacleLayer;

    #region Getters & Setters
    public Vector3[] Path { get { return path; } }
    public int TargetIndex { get { return targetIndex; } }
    public float CurrAccel { get { return currAccel; } set { currAccel = value; } }
    public float Tolerance { get { return tolerance; } }
    public Collider2D Target { get { return target; } set {  target = value; } }
    #endregion

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        // Set the entity's tolerance in collision checks to be equal to its radius
        tolerance = GetComponent<CircleCollider2D>().radius * transform.localScale.x;
        // Set its stopping range to its circumference if it is 0
        if (stoppingRange == 0f) stoppingRange = tolerance * 2;

        //Randomize the speed of the entity
        originalAccel *= Random.Range(0.5f, 1.5f);
        originalAccel = Mathf.Round(originalAccel * 100f) / 100f;
        // Set the speed of the entity
        currAccel = originalAccel;

        // Set the obstacle layer
        obstacleLayer = GameplayManager.instance.gridMap.ObstacleLayer;
    }

    void FixedUpdate()
    {
        // Update the path if required
        if (IsPathUpdateRequired()) UpdatePath();
        // Follow the path
        FollowPath();

        // Set the previous target position as its current position
        prevTargetPos = target.transform.position;
    }
    
    /// <summary>
    /// Check if the path needs to be updated
    /// </summary>
    /// <returns>A bool to indicate whether an update is required</returns>
    bool IsPathUpdateRequired()
    {
        // Update the path if there is a target but no path to it
        if (path == null && target != null) return true;
        // Check if the target is within the stopping range
        if (Vector2.Distance(transform.position, target.transform.position) <= stoppingRange)
        {
            // Update the path if the target is obstructed from the entity's view
            if (GameplayManager.instance.gridMap.IsPathClear(transform.position, target.transform.position)) return true;
            return false;
        }
        // Update the path if the target's position has changed
        if (prevTargetPos != target.transform.position && targetNode != GameplayManager.instance.gridMap.GetClosestNode(target.transform.position, true)) return true;

        // No path is required
        return false;
    }

    /// <summary>
    /// Updates the path via pathfinding
    /// </summary>
    void UpdatePath()
    {
        // Do nothing if no pathfinding class was found or if there is no target
        if (GameplayManager.instance == null || GameplayManager.instance.pathfinding == null || target == null) return;

        // Compute the path
        path = GameplayManager.instance.pathfinding.AStarPathfinding(transform.position, target.transform.position);

        // No path was found, do nothing
        if (path == null) return;

        // Get the closest node to the target
        targetNode = GameplayManager.instance.gridMap.GetClosestNode(path[path.Length - 1], true);

        // Set the starting index to 0
        targetIndex = 0;
    }

    /// <summary>
    /// Follow the path by moving through each node
    /// </summary>
    void FollowPath()
    {
        // Do nothing if there is no path or the entity has already reached its target
        if (path == null || 
            path.Length == 0 || 
            targetIndex >= path.Length)
        {
            rb2d.velocity = Vector2.zero;
            // Rotate the entity to face its target
            RotateTowards(target.transform.position);
            return;
        }

        // Get the distance between the entity and its target
        float targetDistance = Vector2.Distance(transform.position, Target.transform.position);

        // Check if it is near enough to the target
        if (targetDistance <= stoppingRange)
        {
            // Get the direction of the target
            Vector2 targetDirection = (Target.transform.position - transform.position).normalized;

            // Check if the entity can see the target
            RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection, targetDistance, obstacleLayer);
            if (hit.collider == null)
            {
                rb2d.velocity = Vector2.zero;
                // Rotate the entity to face its target
                RotateTowards(target.transform.position);
                return;
            }
        }

        // Get the position of the current waypoint
        Vector3 currentWaypoint = path[targetIndex];

        // Check if the entity has reached it
        if (Vector2.Distance(transform.position, currentWaypoint) <= tolerance * 2)
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

        // Move the entity towards the waypoint
        MoveAndRotateTowards(currentWaypoint);
    }

    /// <summary>
    /// Flips the entity towards the target position
    /// </summary>
    /// <param name="targetPosition">The position that the entity is currently heading towards</param>
    void RotateTowards(Vector3 targetPosition)
    {
        // Find the direction of the current waypoint
        Vector2 direction = (targetPosition - transform.position).normalized;

        // Flip the entity to the direction of their target position
        // Also ensure that the health bar is not inverted
        if (direction.x > 0)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, 0, transform.localRotation.z);
            transform.GetComponentInChildren<Canvas>().transform.localRotation = Quaternion.Euler(transform.localRotation.x, 0, transform.localRotation.z);
        }
        else
        { 
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, 180, transform.localRotation.z);
            transform.GetComponentInChildren<Canvas>().transform.localRotation = Quaternion.Euler(transform.localRotation.x, 180, transform.localRotation.z);
        }
    }

    /// <summary>
    /// Moves and flips the entity towards the target position
    /// </summary>
    /// <param name="targetPosition">The position that the entity is currently heading towards</param>
    void MoveAndRotateTowards(Vector3 targetPosition)
    {
        // Find the direction of the current waypoint
        Vector2 direction = (targetPosition - transform.position).normalized;

        // Move the entity towards it
        rb2d.velocity = Vector2.Lerp(rb2d.velocity, direction, Time.deltaTime * currAccel);

        // Flip the entity to the direction of their target position
        // Also ensure that the health bar is not inverted
        if (direction.x > 0)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, 0, transform.localRotation.z);
            transform.GetComponentInChildren<Canvas>().transform.localRotation = Quaternion.Euler(transform.localRotation.x, 0, transform.localRotation.z);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, 180, transform.localRotation.z);
            transform.GetComponentInChildren<Canvas>().transform.localRotation = Quaternion.Euler(transform.localRotation.x, 180, transform.localRotation.z);
        }
    }

    void OnDrawGizmosSelected()
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