using UnityEngine;

[RequireComponent(typeof(OWLight2))]
public class ShrunkenLightData : MonoBehaviour
{
    private OWLight2 _light;
    private float _initialRange;

    private void Awake()
    {
        _light = gameObject.GetRequiredComponent<OWLight2>();
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
