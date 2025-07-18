using UnityEngine;

namespace Jam5Project;

[RequireComponent(typeof(OWLight2))]
public class BlinkingLight : MonoBehaviour
{
    [SerializeField]
    private float _blinkLength = 1f;
    [SerializeField]
    [Range(0f, 1f)]
    private float _ratio = 0.3f;

    private OWLight2 _light;

    private void Awake()
    {
        _light = gameObject.GetRequiredComponent<OWLight2>();
    }

    private void Update()
    {
        _light.SetIntensityScale(Time.time % _blinkLength > _ratio ? 0f : 1f);
    }
}
