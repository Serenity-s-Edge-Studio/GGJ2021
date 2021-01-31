using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;

public class SlowMo : MonoBehaviour
{
    public UnityEvent OnStartSlowdown;
    public UnityEvent OnEndSlowdown;
    [SerializeField]
    private Image TimerUI;
    [SerializeField]
    private float _SlowMotionLength;
    [SerializeField]
    private float _RemainingEnergy;
    [SerializeField]
    private AnimationCurve timeScaleCurve;
    [SerializeField]
    private Animator CinemachineAnim;
    [SerializeField]
    private PlayerGunController gunController;
    [SerializeField]
    private LayerMask enemyMask;
    [SerializeField]
    private int maxTargets;
    [SerializeField]
    private CinemachineVirtualCamera bulletCam;

    private PlayerActions.MovementActions input;
    private bool Slowed;
    // Start is called before the first frame update
    void Start()
    {
        input = new PlayerActions().Movement;
        input.Enable();
        input.DetectiveMode.performed += DetectiveMode_performed;
    }

    private void DetectiveMode_performed(InputAction.CallbackContext obj)
    {
        if (Slowed && !obj.ReadValueAsButton())
        {
            state = GinVisionState.Shooting;
            OnEndSlowdown.Invoke();
            Slowed = false;
        }
        else if (!Slowed && _RemainingEnergy / _SlowMotionLength > .8f)
        {
            OnStartSlowdown.Invoke();
            Slowed = true;
            state = GinVisionState.Starting;
        }
    }
    [SerializeField]
    private GinVisionState state;
    private Queue<Enemy> targetedEnemies = new Queue<Enemy>();
    private Bullet bullet;
    private void Update()
    {
        switch (state)
        {
            case GinVisionState.Idle:
                _RemainingEnergy = Mathf.Min(_RemainingEnergy + (Time.unscaledDeltaTime * 2), _SlowMotionLength);
                break;
            case GinVisionState.Starting:
                CinemachineAnim.SetBool("Gin Vision", true);
                gunController.canShoot = false;
                foreach (Enemy enemy in EnemyManager.instance.enemies)
                {
                    enemy.GinVisionUI.gameObject.SetActive(true);
                    enemy.GinVisionUI.fillAmount = 0;
                }
                state = GinVisionState.Selecting;
                break;
            case GinVisionState.Selecting:
                _RemainingEnergy -= Time.unscaledDeltaTime;
                if (_RemainingEnergy <= 0 || !Slowed)
                {
                    Slowed = false;
                    state = GinVisionState.Shooting;
                    break;
                }
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    RaycastHit2D hit = Physics2D.Raycast(
                                        Camera.main.ScreenToWorldPoint(
                                            Mouse.current.position.ReadValue()), 
                                        Vector2.zero, float.MaxValue, enemyMask);
                    if (hit.collider == null) break;
                    Enemy hitEnemy = hit.collider.GetComponentInParent<Enemy>();
                    if (hitEnemy != null)
                    {
                        targetedEnemies.Enqueue(hitEnemy);
                        foreach (Enemy enemy in targetedEnemies.ToArray())
                        {
                            enemy.GinVisionUI.fillAmount = (float) targetedEnemies.Count / maxTargets;
                        }
                        if (targetedEnemies.Count == maxTargets)
                        {
                            state = GinVisionState.Shooting;
                            break;
                        }
                    }
                }
                break;
            case GinVisionState.Shooting:
                CinemachineAnim.SetBool("Gin Vision", false);
                CinemachineAnim.SetBool("Bullet Cam", true);
                if (targetedEnemies.Count == 0 && bullet == null)
                {
                    state = GinVisionState.Ending;
                    break;
                }
                if (bullet == null)
                {
                    bullet = gunController.ShootGun();
                    Vector3 dir = targetedEnemies.Dequeue().transform.position - transform.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    bullet.rigidbody.velocity = Vector2.zero;
                    bullet.rigidbody.AddForce(bullet.transform.right * gunController.bulletForce);
                    bulletCam.Follow = bullet.transform;
                }
                break;
            case GinVisionState.Ending:
                CinemachineAnim.SetBool("Gin Vision", false);
                CinemachineAnim.SetBool("Bullet Cam", false);
                gunController.canShoot = true;
                foreach (Enemy enemy in EnemyManager.instance.enemies)
                {
                    enemy.GinVisionUI.gameObject.SetActive(false);
                    enemy.GinVisionUI.fillAmount = 0;
                }
                state = GinVisionState.Idle;
                break;
        }
        Time.timeScale = timeScaleCurve.Evaluate(_RemainingEnergy / _SlowMotionLength);
        TimerUI.fillAmount = _RemainingEnergy / _SlowMotionLength;
    }
    private enum GinVisionState
    {
        Idle,
        Starting,
        Selecting,
        Shooting,
        Ending,
    }
}
