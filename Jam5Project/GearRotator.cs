using UnityEngine;

namespace Jam5Project;

[RequireComponent(typeof(RotateTransform))]
public class GearRotator : MonoBehaviour
{
    private static readonly float _baseGearSpeed = 7f;

    [SerializeField]
    private float _speedFactor = 1f;
    [SerializeField]
    private bool _flip = false;

    private RotateTransform _rotator;
    private bool _inverse = false;

    private void Awake()
    {
        _rotator = gameObject.GetRequiredComponent<RotateTransform>();
        _rotator._degreesPerSecond = _baseGearSpeed * _speedFactor * (_flip ? -1 : 1);
    }

    public void SetGearEnabled(bool enabled)
    {
        _rotator.enabled = enabled;
    }

    public void InvertDirection(bool invert)
    {
        _inverse = invert;
        _rotator._degreesPerSecond = _baseGearSpeed * _speedFactor * (_flip ? -1 : 1) * (_inverse ? -1 : 1);
    }
}
