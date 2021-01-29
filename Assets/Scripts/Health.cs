using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int currentHealth;
    public void Damage(int amount)
    {
        currentHealth -= amount;
        Destroy(gameObject);
    }
}
