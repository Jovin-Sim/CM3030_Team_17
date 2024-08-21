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

    // The rotation speed of the entity
    [SerializeField] float rotationSpeed = 180f;
    // The tolerance for proximity checks between the entity and the nodes or the target
    [SerializeField] float tolerance = 0f;

    // The target the entity is chasing
    [SerializeField] Collider2D target = null;
    // The last node the target was at
    Node targetNode;
    // Last position the target was in
    Vector3 prevTargetPos;

    public Vector3[] Path { get { return path; } }
    public int TargetIndex { get { return targetIndex; } }
    public float CurrAccel { get { return currAccel; } set { currAccel = value; } }
    public float Tolerance { get { return tolerance; } }
    public Collider2D Target 
    { 
        get { return target; } 
        set { 
            target = value;
        } 
    }

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        tolerance = GetComponent<SpriteRenderer>().bounds.extents.x;
        currAccel = originalAccel;
    }

    void Update()
    {
        // Update the path if there is a target but no path to it
        if (path == null && target != null) UpdatePath();
        if (prevTargetPos != target.transform.position && targetNode != GameplayManager.instance.gridMap.GetClosestNode(target.transform.position)) UpdatePath();
        // Follow the path
        FollowPath();

        prevTargetPos = target.transform.position;
    }

    /// <summary>
    /// Updates the path via the Pathfinding class's pathfinding function
    /// </summary>
    void UpdatePath()
    {
        // Do nothing if no such class was found or if there is no target
        if (GameplayManager.instance == null || GameplayManager.instance.pathfinding == null || target == null) return;

        Debug.Log("Updating path");

        // Compute the path
        path = GameplayManager.instance.pathfinding.AStarPathfinding(transform.position, target.transform.position);

        // No path was found, do nothing
        if (path == null) return;

        // Get the closest node to the target
        targetNode = GameplayManager.instance.gridMap.GetClosestNode(path[path.Length - 1]);

        // Set the starting index to 0
        targetIndex = 0;
    }

    /// <summary>
    /// Follow the path by moving through each node
    /// </summary>
    void FollowPath()
    {
        // Do nothing if there is no path or the entity has already reached its target
        if (path == null || path.Length == 0 || targetIndex >= path.Length)
        {
            // Rotate the entity to face the correct direction
            MoveTowards(target.transform.position);
            return; 
        }

        // Do nothing if the entity has already reached its target
        if (Vector2.Distance(transform.position, target.transform.position) <= tolerance * 2)
        {
            // Rotate the entity to face the correct direction
            MoveTowards(target.transform.position);
            return;
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

        // Rotate the entity to face the correct direction
        MoveTowards(currentWaypoint);
    }

    /// <summary>
    /// Moves and rotates the entity towards the target position
    /// </summary>
    /// <param name="targetPosition">The position that the entity is currently heading towards</param>
    void MoveTowards(Vector3 targetPosition)
    {
        // Find the direction of the current waypoint
        Vector2 direction = (targetPosition - transform.position).normalized;

        // Move the entity towards it
        rb2d.velocity = Vector2.Lerp(rb2d.velocity, direction, Time.deltaTime * currAccel);

        if (direction.x > 0) GetComponent<SpriteRenderer>().flipX = false;
        else GetComponent<SpriteRenderer>().flipX = true;
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