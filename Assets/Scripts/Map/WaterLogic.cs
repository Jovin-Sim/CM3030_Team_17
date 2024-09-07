using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLogic : MonoBehaviour
{
    [Tooltip("The speed drop that entities within the object will experience")]
    [SerializeField] float slowdownPercentage;

    // A dictionary of the affected entities and the amount of speed that was dropped
    protected Dictionary<GameObject, float> affectedEntities = new Dictionary<GameObject, float>();

    /// <summary>
    /// An initializer for the object
    /// </summary>
    /// <param name="lifespan">The lifespan of the object</param>
    public void Init(float lifespan = -1f)
    {
        // Do nothing if the object is not limited in its lifespan
        if (lifespan <= 0) return;

        // Get all the sprite renderers in the game object
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // Start the fade out effect for every sprite renderer in the object
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            StartCoroutine(FadeOut(spriteRenderer, lifespan));
        }
        // Destroy the object after its lifespan has been reached
        Destroy(gameObject, lifespan);
    }

    /// <summary>
    /// A coroutine that slowly fades out the object's sprite
    /// </summary>
    /// <param name="spriteRenderer">The sprite renderer of the object to fade</param>
    /// <param name="lifespan">The lifespan of the object</param>
    /// <returns></returns>
    IEnumerator FadeOut(SpriteRenderer spriteRenderer, float lifespan)
    {
        // Do nothing if there is no sprite renderer
        if (spriteRenderer == null) yield break;

        // Get the original color of the sprite renderer
        Color originalColor = spriteRenderer.color;
        // Store the elapsed time
        float timeElapsed = 0f;

        // Continue execution of the coroutine while the lifespan has not been reached
        while (timeElapsed < lifespan)
        {
            // Increase the time elapsed by delta time
            timeElapsed += Time.deltaTime;

            // Change the alpha for the sprite
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

    private void OnDestroy()
    {
        affectedEntities.Clear();
        affectedEntities = null;
    }
}