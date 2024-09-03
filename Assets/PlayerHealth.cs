using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int maxArmour = 100;
    public int currentArmour = 0;
    public HealthBar healthBar;
    public HealthBar armourBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        armourBar.SetMaxArmour(maxArmour);
        armourBar.SetHealth(0);
    }

    public void TakeDamage(int damage, bool pierce)
    {
        if (pierce)
        {
            currentHealth -= damage;
        }
        else
        {
            if (currentArmour > 0)
            {
                int remainingDamage = damage - currentArmour;

                if (remainingDamage > 0)
                {
                    currentArmour = 0;
                    currentHealth -= remainingDamage;
                }
                else
                {
                    currentArmour -= damage;
                }
            }
            else
            {
                currentHealth -= damage;
            }
        }

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        healthBar.SetHealth(currentHealth);
        armourBar.SetHealth(currentArmour);
        if (currentArmour == 0)
        {

        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.SetHealth(currentHealth);
    }
    public void Equip(int amount)
    {
        currentArmour += amount;
        if (currentArmour > maxHealth)
        {
            currentArmour = maxHealth;
        }
        armourBar.SetHealth(currentArmour);
    }
}
