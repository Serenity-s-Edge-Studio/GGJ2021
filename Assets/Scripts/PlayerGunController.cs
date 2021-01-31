using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGunController : MonoBehaviour
{
    [SerializeField]
    private new Camera camera;
    [SerializeField]
    private Vector2 gunPosition;
    [SerializeField]
    private Transform Gun;
    [SerializeField]
    private GameObject Bullet;
    [SerializeField]
    private Transform BulletSpawnPosition;
    [SerializeField]
    private AudioClip Gunshot;
    [SerializeField]
    private float bulletForce;
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
        GameObject bullet = Instantiate(Bullet, BulletSpawnPosition.position, BulletSpawnPosition.rotation);
        Vector2 force = BulletSpawnPosition.right * bulletForce;
        bullet.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        source.PlayOneShot(Gunshot);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePos);
        Vector2 dir = (worldPos - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(worldPos, (Vector2)transform.position);
        gunPosition = (Vector2)transform.position + (dir * (distance < 3 ? distance : 3));
        Gun.position = Vector2.Lerp(Gun.position, gunPosition, Time.unscaledDeltaTime);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Gun.position, .1f);
    }
}
