using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    public int maxHealth = 100;
    public int armour = 100;
    public int maxArmour = 100;

    public void TakeDamage(int damage, bool pierce)
    {
        if (pierce)
        {
            health -= damage;
        }
        else{
            if (armour > 0)
            {
                int remainingDamage = damage - armour;
                armour -= damage;

                if (armour < 0)
                {
                    armour = 0;
                }

                if (remainingDamage > 0)
                {
                    health -= remainingDamage;
                }
            }
            else
            {
                health -= damage;
            }
        }

        if (health < 0)
        { 
            health = 0; 
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public void Boost(int hp)
    {
        health += hp;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void Equip(int points)
    {
        armour += points;
        if (armour > maxArmour)
        {
            armour = maxArmour;
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
