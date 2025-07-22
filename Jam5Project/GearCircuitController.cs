using UnityEngine;

namespace Jam5Project;

public class GearCircuitController : MonoBehaviour
{
    [SerializeField]
    private GearSwitch _topGearSwitch;
    [SerializeField]
    private GearSwitch _bottomGearSwitch;
    [SerializeField]
    private GearRotator[] _mainGears;
    [SerializeField]
    private GearRotator[] _auxiliaryGears;
    [SerializeField]
    private GearRotator _reverseGear;

    private void Awake()
    {
        _topGearSwitch.OnSwitchGear += OnSwitchGear;
        _bottomGearSwitch.OnSwitchGear += OnSwitchGear;
    }

    private void Start()
    {
        UpdateGears();
    }

    private void OnSwitchGear(GearSwitch gearSwitch)
    {
        UpdateGears();
    }

    private void UpdateGears()
    {
        if (!_topGearSwitch.IsSwitchEnabled())
        {
            foreach (var gear in _auxiliaryGears)
            {
                gear.SetConnected(_bottomGearSwitch.IsSwitchEnabled());
            }
        }
        else
        {
            foreach (var gear in _auxiliaryGears)
            {
                gear.SetConnected(true);
            }
        }

        if (_topGearSwitch.IsSwitchEnabled() && _bottomGearSwitch.IsSwitchEnabled())
        {
            foreach (var gear in _mainGears)
            {
                gear.SetGearEnabled(false);
            }
            foreach (var gear in _auxiliaryGears)
            {
                gear.SetGearEnabled(false);
            }
            return;
        }
        else
        {
            foreach (var gear in _mainGears)
            {
                gear.SetGearEnabled(true);
            }
        }

        foreach (var gear in _auxiliaryGears)
        {
            gear.SetGearEnabled(gear.IsConnected());
        }
    }

    private void OnDestroy()
    {
        _topGearSwitch.OnSwitchGear -= OnSwitchGear;
        _bottomGearSwitch.OnSwitchGear -= OnSwitchGear;
    }
}
