using UnityEngine;

namespace Jam5Project;

public class GearSwitch : MonoBehaviour
{
    public delegate void SwitchGearEvent(GearSwitch gearSwitch);
    public event SwitchGearEvent OnSwitchGear;

    [SerializeField]
    private GearRotator _targetGear;
    [SerializeField]
    private Transform _disabledGearPos;
    [SerializeField]
    private GearSwitchInterface _gearInterface;
    [SerializeField]
    private bool _startEnabled = false;

    private bool _gearEnabled;
    private Vector3 _initGearPos;
    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _startMoveTime;
    private float _moveLength = 2.5f;

    private void Awake()
    {
        _initGearPos = _targetGear.transform.localPosition;
        _gearEnabled = _startEnabled;

        if (!_gearEnabled)
        {
            _targetGear.transform.localPosition = _disabledGearPos.localPosition;
        }

        _gearInterface.OnInteractWithGear += OnInteractWithGear;
        enabled = false;
    }

    private void Update()
    {
        float lerp = Mathf.InverseLerp(_startMoveTime, _startMoveTime + _moveLength, Time.time);
        float step = Mathf.SmoothStep(0f, 1f, lerp);
        Vector3 pos = Vector3.Lerp(_startPos, _targetPos, step);
        _targetGear.transform.localPosition = pos;

        if (lerp >= 1f)
        {
            OnSwitchGear?.Invoke(this);
            enabled = false;
        }
    }

    private void OnInteractWithGear()
    {
        if (enabled) return;

        _gearEnabled = !_gearEnabled;

        if (!_gearEnabled)
        {
            _targetPos = _disabledGearPos.localPosition;
        }
        else
        {
            _targetPos = _initGearPos;
        }

        _startPos = _targetGear.transform.localPosition;
        _startMoveTime = Time.time;
        enabled = true;
    }

    public bool IsSwitchEnabled()
    {
        return _gearEnabled;
    }

    private void OnDestroy()
    {
        _gearInterface.OnInteractWithGear -= OnInteractWithGear;
    }
}
