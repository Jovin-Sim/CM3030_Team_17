using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    int id = 0;
    Vector3 startingPosition;

    Rigidbody2D rb2d;

    [SerializeField] float currentMoveSpeed;
    [SerializeField] float originalMoveSpeed = 0f;
    [SerializeField] float rotationSpeed = 0f;
    [SerializeField] bool canMove = true;

    [SerializeField] Collider2D target = null;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        currentMoveSpeed = originalMoveSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateTowardsTarget();
        SetVelocity();
    }

    void RotateTowardsTarget()
    {
        if (!canMove || !target) return;

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, target.transform.position);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        rb2d.SetRotation(rotation);
    }

    void SetVelocity()
    {
        if (!canMove) return;
        
        rb2d.velocity = transform.up * currentMoveSpeed;
    }
}
