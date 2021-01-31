using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGunController : MonoBehaviour
{
    public bool canShoot = true;
    public float bulletForce;
    public Enemy target;
    [SerializeField]
    private new Camera camera;
    [SerializeField]
    private Vector2 gunPosition;
    [SerializeField]
    private Transform GunIKSolver;
    [SerializeField]
    private Bullet Bullet;
    [SerializeField]
    private Transform BulletSpawnPosition;
    [SerializeField]
    private AudioClip Gunshot;
    [SerializeField]
    private LayerMask enemyMask;

    private AudioSource source;
    private PlayerActions.MovementActions input;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        input = new PlayerActions().Movement;
        input.Enable();
        input.Shoot.performed += Shoot_performed;
        source = GetComponent<AudioSource>();
    }

    private void Shoot_performed(InputAction.CallbackContext obj)
    {
        if (canShoot)
        {
            ShootGun();
        }
    }
    public Bullet ShootGun()
    {
        Bullet bullet = Instantiate(Bullet, BulletSpawnPosition.position, BulletSpawnPosition.rotation);
        Vector2 force = BulletSpawnPosition.right * bulletForce;
        bullet.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        source.PlayOneShot(Gunshot);
        return bullet;
    }
    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 worldPos = camera.ScreenToWorldPoint(mousePos);
            Vector2 dir = (worldPos - (Vector2)transform.position).normalized;
            float distance = Vector2.Distance(worldPos, (Vector2)transform.position);
            gunPosition = (Vector2)transform.position + (dir * (distance < 3 ? distance : 3));
            GunIKSolver.position = Vector2.Lerp(GunIKSolver.position, gunPosition, Time.unscaledDeltaTime);
        }
        else
        {
            GunIKSolver.position = target.transform.position;
        }
    }
    public void SetTarget(Enemy target)
    {
        this.target = target;
    }
    public bool isAimedAtTarget()
    {
        RaycastHit2D hit = Physics2D.Raycast(BulletSpawnPosition.position, BulletSpawnPosition.right, 50f, enemyMask);
        if (hit.collider)
        {
            Debug.Log(hit.collider.name + " " + target.name);
        }
        return hit.collider != null && hit.collider.gameObject.name.Equals(target.gameObject.name);
        //Vector3 dir = (BulletSpawnPosition.position - target.transform.position).normalized;
        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //Debug.Log(angle);
        //return angle < 5f;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GunIKSolver.position, .1f);
    }
}
