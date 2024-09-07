using System;
using System.Collections;
using UnityEngine;

public class Particles : MonoBehaviour
{
    [Tooltip("The damage the bullet deals")]
    [SerializeField] float damage;
    private float accumulatedDamage = 0f;
    private bool damageScheduled = false;

    /// <summary>
    /// An initialization function that is called when the bullet is first created
    /// </summary>
    /// <param name="damage">The bullet's damage</param>
    public void Init(float damage)
    {
        this.damage = damage;
    }

    private void OnParticleCollision(GameObject other)
    {
        // Check if the other entity has the combat class and damage it if it does
        if (other.TryGetComponent<Combat>(out Combat entity))
        {
            accumulatedDamage += damage;
            if (!damageScheduled)
            {
                StartCoroutine(ApplyDamageAfterDelay(entity));
            }
        }
    }
    private IEnumerator ApplyDamageAfterDelay(Combat entity)
    {
        damageScheduled = true;
        yield return new WaitForSeconds(0.1f); // Delay for accumulating damage

        if (entity != null)
        {
            entity.ChangeHP(accumulatedDamage);
        }

        accumulatedDamage = 0f;
        damageScheduled = false;
    }
}