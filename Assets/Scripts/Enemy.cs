using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health), typeof(Animator), typeof(CapsuleCollider2D))]
public class Enemy : MonoBehaviour
{
    public Image GinVisionUI;
    private Health health;
    private Rigidbody2D[] rigidbodies;
    private Collider2D mainCollider;
    private Animator animator;

    private void Start()
    {
        health = GetComponent<Health>();
        health.onDeath.AddListener(() => animator.SetTrigger("Die"));
        health.onDeath.AddListener(() => Invoke("remove", 5));
        rigidbodies = GetComponentsInChildren<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        EnemyManager.instance.enemies.Add(this);
    }
    private void SetRagdollStatus(bool enableRagdoll)
    {
        enableRagdoll = !enableRagdoll;
        mainCollider.enabled = enableRagdoll;
        animator.enabled = enableRagdoll;
        foreach(Rigidbody2D rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = enableRagdoll;
        }
    }
    private void remove()
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        EnemyManager.instance.enemies.Remove(this);
    }
}
