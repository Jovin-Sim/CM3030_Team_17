using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Lure : MonoBehaviour
{
    Collider2D lureCollider;
    HashSet<BaseEnemy> luredEnemies;
    Collider2D[] colliderBuffer;

    [SerializeField] float range;
    [SerializeField] float duration;
    [SerializeField] float cooldown;
    [SerializeField] float lastActiveTime;
    [SerializeField] float lastInactiveTime;
    [SerializeField] bool active;

    void Awake()
    {
        lureCollider = GetComponent<Collider2D>();
        luredEnemies = new HashSet<BaseEnemy>();
        colliderBuffer = new Collider2D[10];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = active ? Color.red : Color.green;

        Gizmos.DrawWireSphere(transform.position, range);
    }

    IEnumerator Activate()
    {
        active = true;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            Attract();
            yield return new WaitForSeconds(0.1f);
        }

        Deactivate();
    }

    void Deactivate()
    {
        lastInactiveTime = Time.time;

        foreach (BaseEnemy enemy in luredEnemies)
        {
            enemy.Target = GameplayManager.instance.Player.GetComponent<Collider2D>();
        }

        luredEnemies.Clear();

        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        float startTime = Time.time;
        while (Time.time < startTime + cooldown)
        {
            yield return new WaitForSeconds(0.1f);
        }

        active = false;
    }

    void Attract()
    {
        int numColliders = Physics2D.OverlapCircleNonAlloc(transform.position, range, colliderBuffer);
        for (int i = 0; i < numColliders; ++i)
        {
            if (colliderBuffer[i].TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
            {
                if (luredEnemies.Contains(enemy)) continue;
                enemy.Target = lureCollider;
                luredEnemies.Add(enemy);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null || (12 & (1 << collision.gameObject.layer)) != 0) return;
        if (active) return;

        StartCoroutine(Activate());
    }
}
