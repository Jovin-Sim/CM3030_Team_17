using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject hitEffect;
    [SerializeField] int damage;
    [SerializeField] float lifespan = 5;
    [SerializeField] bool pierce;

    Coroutine aliveCoroutine;

    public void Init(int damage, bool pierce = false)
    {
        this.damage = damage;
        this.pierce = pierce;
    }
    
    void Start()
    {
        aliveCoroutine = StartCoroutine(Alive());
    }

    IEnumerator Alive()
    {
        float startTime = Time.time;
        while (Time.time < startTime + lifespan)
        {
            yield return null;
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
   {
        if (aliveCoroutine != null) StopCoroutine(aliveCoroutine);
        if (collision.TryGetComponent<Combat>(out Combat entity)) entity.TakeDamage(damage);
        if (pierce) return;
        Destroy(gameObject);
   }
}