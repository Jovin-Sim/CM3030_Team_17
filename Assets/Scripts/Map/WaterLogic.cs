using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLogic : MonoBehaviour
{
    [SerializeField] protected float slowdownPercentage;
    protected Dictionary<GameObject, float> affectedEntities = new Dictionary<GameObject, float>();

    [SerializeField] float alpha;

    public void Init(float lifespan = -1f)
    {
        if (lifespan <= 0) return;

        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            StartCoroutine(FadeOut(spriteRenderer, lifespan));
        }
        // Destroy the object after its lifespan has been reached
        Destroy(gameObject, lifespan);
    }

    IEnumerator FadeOut(SpriteRenderer spriteRenderer, float lifespan)
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        float timeElapsed = 0f;

        while (timeElapsed < lifespan)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0f, timeElapsed / lifespan);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // Do nothing if no collider was found or if the entity has already been affected by the water
        if (collision == null || affectedEntities.ContainsKey(gameObject)) return;

        // Check if the object can be affected by the slowdown effect of the water
        // Add them to affectedEntities if they can be and slow down their speed
        if (collision.TryGetComponent<PlayerController>(out PlayerController player))
        {
            affectedEntities[player.gameObject] = player.CurrMoveSpeed * slowdownPercentage / 100f;
            player.CurrMoveSpeed -= affectedEntities[player.gameObject];
        }
        else if (collision.TryGetComponent<PathBasedMovement>(out PathBasedMovement entity))
        {
            affectedEntities[entity.gameObject] = entity.CurrAccel * slowdownPercentage / 100f;
            entity.CurrAccel -= affectedEntities[entity.gameObject];
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        // Do nothing if no collider was found or if the entity was not affected by the water
        if (collision == null || !affectedEntities.ContainsKey(collision.gameObject)) return;

        // Check if the object has been affected by the slowdown effect of the water
        // Remove their slowdown effect if they are
        if (collision.TryGetComponent<PlayerController>(out PlayerController player))
            player.CurrMoveSpeed += affectedEntities[player.gameObject];
        else if (collision.TryGetComponent<PathBasedMovement>(out PathBasedMovement entity))
            entity.CurrAccel += affectedEntities[entity.gameObject];

        // Remove them from affectedEntities
        affectedEntities.Remove(collision.gameObject);
    }
}