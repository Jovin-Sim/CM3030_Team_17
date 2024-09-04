using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    // The health bar that shows the entity's hp
    [SerializeField] HealthBar healthBar;

    // The hp of the entity
    [SerializeField] float originalHp = 1;
    [SerializeField] float maxHp;
    [SerializeField] float currHp;

    // The attack of the entity
    [SerializeField] float originalAtk = 1;
    [SerializeField] float currAtk;

    // The attack cooldown of the entity
    [SerializeField] float atkCd = 1;
    // The time of the entity's previous attack
    [SerializeField] float lastAtkTime;

    // The distance the entity is from its target before it starts attacking the player
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
    }

    /// <summary>
    /// Applies a change to the entity's health
    /// </summary>
    /// <param name="amount">The amount changed, where positive amounts lowers the hp</param>
    public void ChangeHP(float amount)
    {
        // Lower the hp by amount
        currHp -= amount;

        // If the resulting hp is greater than the max hp, change it to the max hp
        if (currHp > maxHp) currHp = maxHp;

        // The entity dies if it's hp goes to 0
        else if (currHp <= 0) Die();
        
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

        // If the resulting max hp is lesser than the current hp
        // Lower current hp to max hp's value
        if (currHp > maxHp) currHp = maxHp;
        // If the max hp was increased, increase current hp by the same amount
        else if (amount < 0) ChangeHP(amount);

        // Change the health bar's health value
        if (healthBar != null) healthBar.SetMaxHealth(maxHp);
    }

    /// <summary>
    /// Destroys the entity if they are killed
    /// </summary>
    public void Die()
    {
        // Execute game over if the entity is the player
        if (gameObject == GameplayManager.instance.Player.gameObject)
        {
            GameplayManager.instance.EndGame(false);
            return;
        }
        // Handle enemy despawning
        GameplayManager.instance.enemyManager.DespawnEnemy(gameObject);
        Destroy(gameObject);
    }
}