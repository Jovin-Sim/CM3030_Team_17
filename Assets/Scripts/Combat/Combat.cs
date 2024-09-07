using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [Header("Health Properties")]
    [Tooltip("The entity's health bar")]
    [SerializeField] HealthBar healthBar;
    [Tooltip("The hp the entity starts at")]
    [SerializeField] float originalHp = 1;
    [Tooltip("The maximum hp of the entity (Read Only)")]
    [SerializeField] float maxHp;
    [Tooltip("The current hp of the entity (Read Only)")]
    [SerializeField] float currHp;

    [Header("Attack Properties")]
    [Tooltip("The attack the entity starts at")]
    [SerializeField] float originalAtk = 1;
    [Tooltip("The current attack of the entity (Read Only)")]
    [SerializeField] float currAtk;

    [Tooltip("The attack cooldown of the entity")]
    [SerializeField] float atkCd = 1;
    [Tooltip("The time of the entity's previous attack")]
    [SerializeField] float lastAtkTime;

    [Tooltip("The attacking range of the entity")]
    [SerializeField] float atkRange = 0f;

    #region Getters & Setters
    public float OriginalHp
    {
        get { return originalHp; }
        set { originalHp = value; }
    }
    public float MaxHp
    {
        get { return originalHp; }
        set { originalHp = value; }
    }
    public float CurrHp
    {
        get { return currHp; }
        set { currHp = value; }
    }
    public float OriginalAtk
    {
        get { return originalAtk; }
        set { originalAtk = value; }
    }
    public float CurrAtk
    {
        get { return currAtk; }
        set { currAtk = value; }
    }
    public float AtkCd
    {
        get { return atkCd; }
        set { atkCd = value; }
    }
    public float LastAtkTime
    {
        get { return lastAtkTime; }
        set { lastAtkTime = value; }
    }
    public float AtkRange
    {
        get { return atkRange; }
        set { atkRange = value; }
    }
    #endregion

    void Awake()
    {
        // Set the hp and atk to the original hp and atk
        currHp = originalHp;
        maxHp = originalHp;
        currAtk = originalAtk;

        // Get the health bar of the entity and set their hp
        if (healthBar == null) healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHp);
            healthBar.SetHealth(currHp);
        }
    }

    /// <summary>
    /// Check if the entity is able to attack
    /// </summary>
    public bool TryAttack()
    {
        // Check if the attack cooldown is over
        if (Time.time <= lastAtkTime + AtkCd) return false;
        return true;

    }

    /// <summary>
    /// Attack another entity
    /// </summary>
    /// <param name="otherEntity">The entity to attack</param>
    public void Attack(Combat otherEntity)
    {
        lastAtkTime = Time.time; // Set the last attack time to the current time
        otherEntity.ChangeHP(currAtk); // Change the entity's hp by currAtk
        GameManager.instance.audioManager.PlaySFX(GameManager.instance.audioManager.enemyAttack);
    }

    /// <summary>
    /// Applies a change to the entity's health
    /// </summary>
    /// <param name="amount">The amount changed, where positive amounts lowers the hp</param>
    public void ChangeHP(float amount)
    {
        // Lower the hp by amount
        currHp -= amount;

        // Ensure that the current hp is between 0 and max hp
        currHp = Mathf.Clamp(currHp, 0, maxHp);

        // The entity dies if it's hp goes to 0
        if (currHp <= 0) Die();
        
        // Change the health bar's health value
        if (healthBar != null) healthBar.SetHealth(currHp);
    }

    /// <summary>
    /// Applies a change to the entity's max health
    /// </summary>
    /// <param name="amount">The amount changed, where positive amounts lowers the hp</param>
    public void ChangeMaxHP(float amount)
    {
        // Lower the max hp by amount
        maxHp -= amount;

        // Change the health bar's health value
        if (healthBar != null) healthBar.SetMaxHealth(maxHp);

        // If the resulting max hp is lesser than the current hp
        // Lower current hp to max hp's value
        if (currHp > maxHp) currHp = maxHp;
        // If the max hp was increased, increase current hp by the same amount
        else if (amount < 0) ChangeHP(amount);
    }

    /// <summary>
    /// Destroys the entity if they are killed
    /// </summary>
    public void Die()
    {
        // Execute game over if the entity is the player
        if (gameObject == GameplayManager.instance.Player.gameObject)
        {
            GameManager.instance.audioManager.PlaySFX(GameManager.instance.audioManager.playerDeath);
            GameplayManager.instance.EndGame(false);
            return;
        }
        // Handle enemy despawning
        GameManager.instance.audioManager.PlaySFX(GameManager.instance.audioManager.enemyDeath);
        GameplayManager.instance.enemyManager.DespawnEnemy(gameObject);
        Destroy(gameObject);
    }
}