// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerController : MonoBehaviour
// {
//     Vector2 movement;

//     SpriteRenderer spriteRenderer;

//     void Start() {
//         spriteRenderer = GetComponent<SpriteRenderer>();
//      }

//     void FixedUpdate()
//     {
//          if (movement == Vector2.zero)
//     {
//         // animator.SetBool("isMoving", false);
//     } else {
//         // Flip the sprite if we're moving left
//         spriteRenderer.flipX = movement.x < 0;
//     }
//         transform.Translate(movement * Time.deltaTime);
//     }

//     void OnMove(InputValue movementValue)
//     {
//         movement = movementValue.Get<Vector2>() * 2;
//     }
// }

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerController : MonoBehaviour
// {
//     Vector2 movement;
//     SpriteRenderer spriteRenderer;

//     void Start() {
//         spriteRenderer = GetComponent<SpriteRenderer>();
//     }

//     void FixedUpdate()
//     {
//         if (movement == Vector2.zero)
//         {
//             // animator.SetBool("isMoving", false);
//         }
//         else
//         {
//             // Flip the sprite if we're moving left
//             spriteRenderer.flipX = movement.x < 0;
//         }

//         // Move the player
//         transform.Translate(movement * Time.deltaTime);

//         // Rotate the player to face the mouse
//         RotateToMouse();
//     }

//     void OnMove(InputValue movementValue)
//     {
//         movement = movementValue.Get<Vector2>() * 2;
//     }

//     void RotateToMouse()
//     {
//         // Get the mouse position in world space
//         Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
//         mousePosition.z = 0f; // Ensure the Z value is zero for 2D

//         // Calculate the direction from the player to the mouse
//         Vector3 direction = mousePosition - transform.position;

//         // Calculate the angle in degrees
//         float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

//         // Set the player's rotation
//         transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Camera cam;
    Vector2 movement;
    Vector2 mousePos;
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate(){
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

}




