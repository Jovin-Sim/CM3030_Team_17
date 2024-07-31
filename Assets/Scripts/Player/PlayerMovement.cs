using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    int id = 0;
    Vector3 startingPosition;

    Rigidbody2D rb2d;

    [SerializeField] float currMoveSpeed;
    [SerializeField] float originalMoveSpeed = 0f;
    [SerializeField] float rotationSpeed = 0f;
    [SerializeField] bool canMove = true;
    Vector2 movement;

    [SerializeField] Collider2D target = null;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        currMoveSpeed = originalMoveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        rb2d.MovePosition(rb2d.position + movement * currMoveSpeed * Time.deltaTime);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - rb2d.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb2d.rotation = angle;
    }
}
