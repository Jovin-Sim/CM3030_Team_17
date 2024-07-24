using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    int id = 0;
    Vector3 startingPosition;
    Rigidbody2D rb2d;

    Pathfinding pathfinding;
    Vector3[] path;
    int targetIndex;

    [SerializeField] float currentAccel;
    [SerializeField] float originalAccel = 1f;
    [SerializeField] float rotationSpeed = 30f;
    [SerializeField] float tolerance = 0f;

    [SerializeField] Collider2D target = null;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        tolerance = GetComponent<SpriteRenderer>().bounds.extents.x;
        currentAccel = originalAccel;
    }

    void Start()
    {
        pathfinding = FindObjectOfType<Pathfinding>();
    }

    void Update()
    {
        if (target == null) return;

        if (path == null) UpdatePath();

        FollowPath();
    }

    void UpdatePath()
    {
        if (pathfinding == null || target == null) return;

        path = pathfinding.AStarPathfinding(transform.position, target.transform.position, tolerance * 2);
        targetIndex = 0;
    }

    void FollowPath()
    {
        if (path == null || path.Length == 0) return;

        if (Vector2.Distance(transform.position, target.transform.position) <= tolerance * 2) return;

        if (targetIndex >= path.Length) return;

        Vector3 currentWaypoint = path[targetIndex];

        if (Vector2.Distance(transform.position, currentWaypoint) <= tolerance)
        {
            targetIndex++;
            if (targetIndex >= path.Length)
            {
                rb2d.velocity = Vector2.Lerp(rb2d.velocity, Vector3.zero, Time.deltaTime * currentAccel);
                return;
            }
            currentWaypoint = path[targetIndex];
        }

        Vector2 direction = (currentWaypoint - transform.position).normalized;
        rb2d.velocity = Vector2.Lerp(rb2d.velocity, direction, Time.deltaTime * currentAccel);
        RotateTowards(currentWaypoint);
    }

    void RotateTowards(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        float angleDifference = Mathf.DeltaAngle(rb2d.rotation, angle);

        float rotationThreshold = 12.0f; // Define a threshold for micro-rotations

        if (Mathf.Abs(angleDifference) > rotationThreshold)
        {
            float targetAngle = Mathf.MoveTowardsAngle(rb2d.rotation, angle, rotationSpeed * Time.deltaTime);
            rb2d.rotation = targetAngle;
        }
    }

    void OnDrawGizmos()
    {
        if (path == null) return;

        for (int i = targetIndex; i < path.Length; ++i)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(path[i], Vector3.one * 0.1f);

            if (i == targetIndex) Gizmos.DrawLine(transform.position, path[i]);
            else Gizmos.DrawLine(path[i - 1], path[i]);
        }
    }
}
