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

    private PlayerActions.MovementActions input;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        input = new PlayerActions().Movement;
        input.Enable();
        input.Shoot.performed += Shoot_performed;
    }

    private void Shoot_performed(InputAction.CallbackContext obj)
    {
        GameObject bullet = Instantiate(Bullet, Gun.position, Gun.rotation);
        Vector2 force = (Gun.position - transform.position).normalized * 60;
        bullet.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePos);
        Vector2 dir = (worldPos - (Vector2)transform.position).normalized;
        gunPosition = (Vector2)transform.position + dir;
        Gun.position = Vector2.Lerp(Gun.position, gunPosition, Time.unscaledDeltaTime);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Gun.position, .1f);
    }
}
