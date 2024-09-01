using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    // The max and current hp of the entity
    [SerializeField] float originalHp = 1;
    [SerializeField] float maxHp;
    [SerializeField] float currHp;
    [SerializeField] Dictionary<float, float> hpEffects = new Dictionary<float, float>();

    // The original and current attack of the entity
    [SerializeField] float originalAtk = 1;
    [SerializeField] float currAtk;
    [SerializeField] Dictionary<float, float> atkEffects = new Dictionary<float, float>();

    // The cooldown of the entity
    [SerializeField] float atkCd = 1;
    // The time of the entity's previous attack
    [SerializeField] float lastAtkTime;
    [SerializeField] Dictionary<float, float> atkCdEffects = new Dictionary<float, float>();

    // The distance the entity is from its target before it starts attacking the player
    [SerializeField] float atkRange = 0f;
    [SerializeField] Dictionary<float, float> atkRangeEffects = new Dictionary<float, float>();

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
    public Dictionary<float, float> HpEffects { get { return hpEffects; } }
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
    public Dictionary<float, float> AtkEffects { get { return atkEffects; } }
    public float AtkCd
    {
        get { return atkCd; }
        set { atkCd = value; }
    }
    public Dictionary<float, float> AtkCdEffects { get { return atkCdEffects; } }
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
    public Dictionary<float, float> AtkRangeEffects { get { return atkRangeEffects; } }
    #endregion

    void Awake()
    {
        currHp = originalHp;
        currAtk = originalAtk;
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
    /// Attacks the other entity
    /// </summary>
    /// <param name="otherEntity">The entity to attack</param>
    public void Attack(Combat otherEntity)
    {
        lastAtkTime = Time.time; // Set the last attack time to the current time
        otherEntity.ChangeHP(currAtk);
    }

    /// <summary>
    /// Applies a change to the entity's health
    /// </summary>
    /// <param name="amount">The amount changed</param>
    public void ChangeHP(float amount)
    {
        currHp -= amount;
        if (currHp > maxHp) currHp = maxHp;
        else if (currHp <= 0) Die(); // The entity dies if it's hp goes to 0
    }

    /// <summary>
    /// Applies a change to the entity's max health
    /// </summary>
    /// <param name="amount">The amount changed</param>
    public void ChangeMaxHP(float amount)
    {
        maxHp -= amount;
        if (currHp > maxHp) currHp = maxHp;
        else if (currHp < maxHp) ChangeHP(amount);
    }

    /// <summary>
    /// Destroys the entity if they are killed
    /// </summary>
    public void Die()
    {
        // Execute game over if the entity is the player
        if (gameObject == GameplayManager.instance.Player.gameObject)
        {
            GameplayManager.instance.GameOver();
            return;
        }
        // Handle enemy despawning
        GameplayManager.instance.enemyManager.DespawnEnemy(gameObject);
        Destroy(gameObject);
    }
}