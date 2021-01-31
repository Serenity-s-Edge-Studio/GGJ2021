using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health), typeof(Animator), typeof(CapsuleCollider2D))]
public class Enemy : MonoBehaviour
{
    public Image GinVisionUI;
    public Health health;
    public GameObject target;
    public float bulletForce;

    private Rigidbody2D[] rigidbodies;
    private Collider2D mainCollider;
    private Animator animator;
    [SerializeField]
    private Transform GunIKSolver;
    [SerializeField]
    private Bullet Bullet;
    [SerializeField]
    private Transform BulletSpawnPosition;
    [SerializeField]
    private AudioClip Gunshot;
    [SerializeField]
    private LayerMask playerMask;
    [SerializeField]
    private float gunTimer;
    [SerializeField]
    private float minGunTimer;

    private float timer;

    private AudioSource source;

    private void Start()
    {
        health = GetComponent<Health>();
        health.onDeath.AddListener(() => animator.SetTrigger("Die"));
        health.onDeath.AddListener(() => Destroy(gameObject, 5));
        health.onDeath.AddListener(() => this.enabled = false);

        rigidbodies = GetComponentsInChildren<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        timer = gunTimer;
        EnemyManager.instance.enemies.Add(this);
    }
    private void Update()
    {
        if (target != null) GunIKSolver.position = target.transform.position;
        if (timer <= 0 && target != null && isAimedAtTarget())
        {
            ShootGun();
            timer = gunTimer;
        }
        timer -= Time.deltaTime;
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
    public Bullet ShootGun()
    {
        Bullet bullet = Instantiate(Bullet, BulletSpawnPosition.position, BulletSpawnPosition.rotation);
        Vector2 force = BulletSpawnPosition.right * bulletForce;
        bullet.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        bullet.targetTag = "Player";
        source.PlayOneShot(Gunshot);
        return bullet;
    }
    public bool isAimedAtTarget()
    {
        RaycastHit2D hit = Physics2D.Raycast(BulletSpawnPosition.position, BulletSpawnPosition.right, 50f, playerMask);
        if (hit.collider)
        {
            Debug.Log(hit.collider.name + " " + target.name);
        }
        return hit.collider != null && hit.collider.gameObject.name.Equals(target.gameObject.name);
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
