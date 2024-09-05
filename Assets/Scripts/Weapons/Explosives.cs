using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Explosives : MonoBehaviour
{
    // The explosion data
    [SerializeField] Explosion explosionData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if a bullet hit the explosive
        if (collision.GetComponent<Bullet>())
        {
            // Causes an explosion if the bullet hit the explosive
            explosionData.Explode(transform.position);

            // Destroy both game objects
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draws the range of the explosion
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionData.Radius);
    }
}