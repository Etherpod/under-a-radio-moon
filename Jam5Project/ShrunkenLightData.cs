using UnityEngine;

namespace Jam5Project;

[RequireComponent(typeof(Light))]
public class ShrunkenLightData : MonoBehaviour
{
    private Light _light;
    private float _initialRange;
    private bool _initialized = false;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _light = gameObject.GetRequiredComponent<Light>();
        _initialRange = _light.range;
        _initialized = true;
    }

    public void SetRangeScale(float scale)
    {
        if (!_initialized)
        {
            Initialize();
        }

        _light.range = _initialRange * scale;
    }

    public float GetInitialRange()
    {
        return _initialRange;
    }
}
