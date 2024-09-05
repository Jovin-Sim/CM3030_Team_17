using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidLogic : WaterLogic
{
    [Tooltip("The damage the acid does every second")]
    [SerializeField] float damagePerTurn;
    [Tooltip("The cooldown between each hit from the acid")]
    [SerializeField] float damageCooldown;

    Coroutine damageAffectedEntities;

    /// <summary>
    /// Applies damage to all enemies within the acid
    /// </summary>
    /// <returns>Returns the cooldown</returns>
    IEnumerator DamageAffectedEntities()
    {
        while (true)
        {
            // Loops through all affected entities
            List<GameObject> entities = new List<GameObject>(affectedEntities.Keys);

            foreach (GameObject entity in entities)
            {
                // Remove the entity if it's no longer valid
                if (entity == null || !affectedEntities.ContainsKey(entity))
                    affectedEntities.Remove(entity);
                // Else does damage to it
                else if (entity.TryGetComponent<Combat>(out Combat combat))
                    combat.ChangeHP(damagePerTurn);
            }

            // Wait for the cooldown before damaging again
            yield return new WaitForSeconds(damageCooldown);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision); // Call base method to apply slowdown

        // Starts the coroutine if there are affected entities and the coroutine is null
        if (affectedEntities.Count > 0 && damageAffectedEntities == null)
            damageAffectedEntities = StartCoroutine(DamageAffectedEntities());
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision); // Call base method to remove slowdown

        // Stops the coroutine if there are no affected entities
        if (affectedEntities.Count <= 0 && damageAffectedEntities != null)
        {
            StopCoroutine(damageAffectedEntities);
            damageAffectedEntities = null;
        }
    }
}