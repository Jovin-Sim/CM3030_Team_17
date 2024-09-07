using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Tooltip("The powerup the collectable holds (Read Only)")]
    [SerializeField] BasePowerup powerup;

    /// <summary>
    /// Initialize the collectable
    /// </summary>
    /// <param name="powerup">The powerup the collectable should have</param>
    public void Init(BasePowerup powerup)
    {
        // Do nothing if the powerup or its sprite is null
        if (powerup == null || powerup.Sprite == null) return;

        // Assign the powerup to the collectable
        this.powerup = powerup;
        // Assign the sprite to the collectable
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer)) spriteRenderer.sprite = powerup.Sprite;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Apply the powerup to the player on contact
        GameplayManager.instance.powerupManager.ApplyPowerup(powerup);
        // Destroy the powerup
        Destroy(gameObject);
    }
}