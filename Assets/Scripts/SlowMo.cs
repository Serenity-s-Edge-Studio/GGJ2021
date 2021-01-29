using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    private PlayerActions.MovementActions input;
    private bool Slowed;
    // Start is called before the first frame update
    void Start()
    {
        input = new PlayerActions().Movement;
        input.Enable();
        input.DetectiveMode.performed += DetectiveMode_performed;
    }

    private void DetectiveMode_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (Slowed && !obj.ReadValueAsButton())
        {
            OnEndSlowdown.Invoke();
            Slowed = false;
        }
        if (Slowed && _RemainingEnergy / _SlowMotionLength > .8f)
            OnStartSlowdown.Invoke();
        Slowed = obj.ReadValueAsButton();
    }
    private void Update()
    {
        if (Slowed)
        {
            _RemainingEnergy -= Time.unscaledDeltaTime;
            if (_RemainingEnergy <= 0)
            {
                OnEndSlowdown.Invoke();
                Slowed = false;
            }
        }
        else
        {
            _RemainingEnergy = Mathf.Min(_RemainingEnergy + (Time.unscaledDeltaTime * 2), _SlowMotionLength);
        }
        Time.timeScale = timeScaleCurve.Evaluate(_RemainingEnergy / _SlowMotionLength);
        TimerUI.fillAmount = _RemainingEnergy / _SlowMotionLength;
    }
}
