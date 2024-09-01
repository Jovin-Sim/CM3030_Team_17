using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosives : MonoBehaviour
{
    public float delay = 3f;
    public float radius;
    public float force;

    public LayerMask LayerToHit;

    //public GameObject explosionEffect;

    float countdown;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Bullet>())
        {
            Explode();
            Destroy(collision.gameObject);
        }
    }
    void Explode()
    {
        Debug.Log("BOOM!");

        // Show effect
        //Instantiate(explosionEffect, transform.position, transform.rotation);

        // Get nearby objects
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, LayerToHit);

        foreach (Collider2D nearbyObject in colliders)
        {
            Debug.Log(nearbyObject.gameObject.name);
            // Add force
            Vector2 direction = nearbyObject.transform.position - transform.position;

            nearbyObject.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
            // Damage
        }

        // Remove Barrel
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}