using UnityEngine;

namespace Jam5Project;

public class GearSwitchInterface : MonoBehaviour
{
    public delegate void GearInteractEvent();
    public event GearInteractEvent OnInteractWithGear;

    [SerializeField]
    private InteractReceiver _interactReceiver;
    [SerializeField]
    private float _interactRotation;

    private float _rotateStartTime;
    private float _rotateLength = 0.5f;
    private float _startAngle;
    private float _targetAngle;

    private void Start()
    {
        _interactReceiver.ChangePrompt("Move Gear");
        _interactReceiver.OnPressInteract += OnPressInteract;
        enabled = false;
    }

    private void Update()
    {
        var lerp = Mathf.InverseLerp(_rotateStartTime, _rotateStartTime + _rotateLength, Time.time);
        var xAngle = Mathf.SmoothStep(_startAngle, _targetAngle, lerp);
        transform.localRotation = Quaternion.Euler(xAngle, 0, 0);

        if (lerp >= 1f)
        {
            OnInteractWithGear?.Invoke();
            _interactReceiver.EnableInteraction();
            enabled = false;
        }
    }

    private void OnPressInteract()
    {
        _rotateStartTime = Time.time;
        _startAngle = transform.localEulerAngles.x;
        _targetAngle = _startAngle + _interactRotation;
        _interactReceiver.DisableInteraction();
        enabled = true;
    }

    private void OnDestroy()
    {
        _interactReceiver.OnPressInteract -= OnPressInteract;
    }
}
