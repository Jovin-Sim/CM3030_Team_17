using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    // The max and current hp of the entity
    [SerializeField] private int maxHp = 1;
    [SerializeField] private int currHp;

    // The original and current attack of the entity
    [SerializeField] private int originalAtk = 1;
    [SerializeField] private int currAtk;

    // The cooldown of the entity
    [SerializeField] private float atkCd = 1;
    // The time of the entity's previous attack
    [SerializeField] private float lastAtkTime;

    // Initialize AudioManager
    AudioManager audioManager;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    #region Getters & Setters
    public int MaxHp
    {
        get { return maxHp; }
        set { maxHp = value; }
    }
    public int CurrHp
    {
        get { return currHp; }
        set { currHp = value; }
    }
    public int OriginalAtk
    {
        get { return originalAtk; }
        set { originalAtk = value; }
    }
    public int CurrAtk
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
    #endregion

    void Start()
    {
        currHp = maxHp;
        currAtk = originalAtk;
    }

    public void TryAttack(Combat otherEntity)
    {
        if (Time.time <= lastAtkTime + AtkCd) return;
        Attack(otherEntity);
    }    

    /// <summary>
    /// Attacks the target
    /// </summary>
    public void Attack(Combat otherEntity)
    {
        lastAtkTime = Time.time;
        audioManager.PlaySFX(audioManager.enemyAttack);
        otherEntity.TakeDamage(currAtk);
    }

    /// <summary>
    /// Applies damage to the entity
    /// </summary>
    /// <param name="damage">The damage taken by the entity</param>
    public void TakeDamage(int damage)
    {
        currHp -= damage;
        if (currHp <= 0) Die();
    }

    /// <summary>
    /// Destroys the entity if they are killed
    /// </summary>
    public void Die()
    {
        if (gameObject == GameplayManager.instance.Player.gameObject)
        {
            GameplayManager.instance.GameOver();
            return;
        }
        GameplayManager.instance.enemyManager.DespawnEnemy(gameObject);
        Destroy(gameObject);
    }
}