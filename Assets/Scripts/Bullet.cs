using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public float decayTime;
    public GameObject bloodParticles;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && collision.TryGetComponent(out Health health))
        {
            health.Damage(damage);
            Destroy(Instantiate(bloodParticles, transform.position, Quaternion.identity), 5);
        }
    }
    private void Update()
    {
        decayTime -= Time.deltaTime;
        if (decayTime <= 0)
            Destroy(gameObject);
    }
}
