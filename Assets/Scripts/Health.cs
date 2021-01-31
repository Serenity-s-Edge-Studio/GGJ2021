using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public UnityEvent onDeath;
    public UnityEvent onDamage;
    public int currentHealth;
    [SerializeField]
    private AudioClip[] deathClips;
    [SerializeField]
    private AudioSource source;

    private bool isDead = false;
    public void Damage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
        else
            onDamage.Invoke();
    }
    public void Die()
    {
        if (!isDead)
        {
            isDead = true;
            onDeath.Invoke();
            source.PlayOneShot(deathClips[Random.Range(0, deathClips.Length - 1)]);
        }
    }
}
