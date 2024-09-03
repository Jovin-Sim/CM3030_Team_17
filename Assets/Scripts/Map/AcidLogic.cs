using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidLogic : WaterLogic
{
    [SerializeField] float damagePerTurn;
    [SerializeField] float damageCooldown;

    Coroutine damageAffectedEntities;

    IEnumerator DamageAffectedEntities()
    {
        while (true)
        {
            List<GameObject> entities = new List<GameObject>(affectedEntities.Keys);

            foreach (GameObject entity in entities)
            {
                if (entity == null || !affectedEntities.ContainsKey(entity))
                {
                    // Remove the entity if it's no longer valid
                    affectedEntities.Remove(entity);
                }
                else if (entity.TryGetComponent<Combat>(out Combat combat))
                    combat.ChangeHP(damagePerTurn);
            }

            yield return new WaitForSeconds(damageCooldown);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision); // Call base method to apply slowdown

        if (affectedEntities.Count > 0 && damageAffectedEntities == null)
            damageAffectedEntities = StartCoroutine(DamageAffectedEntities());
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision); // Call base method to remove slowdown

        if (affectedEntities.Count <= 0 && damageAffectedEntities != null)
        {
            StopCoroutine(damageAffectedEntities);
            damageAffectedEntities = null;
        }
    }
}