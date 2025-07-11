using UnityEngine;

namespace Jam5Project;

[RequireComponent(typeof(Light))]
public class ShrunkenLightData : MonoBehaviour
{
    private Light _light;
    private float _initialRange;

    private void Awake()
    {
        _light = gameObject.GetRequiredComponent<Light>();
        _initialRange = _light.range;
    }

    public void SetRangeScale(float scale)
    {
        _light.range = _initialRange * scale;
    }

    public float GetInitialRange()
    {
        return _initialRange;
    }
}
