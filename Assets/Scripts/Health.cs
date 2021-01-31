using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public UnityEvent onDeath;
    public int currentHealth;
    public void Damage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }
    public void Die()
    {
        onDeath.Invoke();
    }
}
