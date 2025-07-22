using System.Collections;
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
    private GearRotator _invertedGear;
    [SerializeField]
    private GearInvertLever _invertLever;
    [SerializeField]
    private GearDoorController _gearDoor;

    private bool _inverted = false;

    private void Awake()
    {
        _topGearSwitch.OnSwitchGear += OnSwitchGear;
        _bottomGearSwitch.OnSwitchGear += OnSwitchGear;
        _invertLever.OnLeverPulled += OnLeverPulled;
    }

    private void Start()
    {
        UpdateGears();
    }

    private void OnSwitchGear(GearSwitch gearSwitch)
    {
        UpdateGears();
    }

    private void OnLeverPulled(GearInvertLever lever)
    {
        StartCoroutine(InvertSequence());
    }

    private IEnumerator InvertSequence()
    {
        foreach (var main in _mainGears)
        {
            main.InvertDirection(true);
            main.SetGearEnabled(true);
        }
        _inverted = true;

        yield return new WaitForSeconds(2f);

        _gearDoor.OpenDoor();
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

        if (!_inverted && _topGearSwitch.IsSwitchEnabled() && _bottomGearSwitch.IsSwitchEnabled())
        {
            foreach (var gear in _mainGears)
            {
                gear.SetGearEnabled(false);
            }
            foreach (var gear in _auxiliaryGears)
            {
                gear.SetGearEnabled(false);
            }
            _invertedGear.SetGearEnabled(false);
            return;
        }
        else
        {
            foreach (var gear in _mainGears)
            {
                gear.SetGearEnabled(true);
            }
            _invertedGear.SetGearEnabled(true);
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
        _invertLever.OnLeverPulled -= OnLeverPulled;
    }
}
