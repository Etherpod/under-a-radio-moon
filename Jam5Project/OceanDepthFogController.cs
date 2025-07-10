using UnityEngine;

namespace Jam5Project;

public class OceanDepthFogController : MonoBehaviour
{
    [SerializeField]
    private Sector _sector;
    [SerializeField]
    private MeshRenderer _fogRenderer;
    [SerializeField]
    private float _lowerDepth;
    [SerializeField]
    private float _upperDepth;
    [SerializeField]
    private Color _maxDepthColor;

    private Material _fogMat;
    private int _fogColorPropID;
    private Color _startColor;

    private void Awake()
    {
        _fogMat = _fogRenderer.material;
        _fogColorPropID = Shader.PropertyToID("_Color");
        _startColor = _fogMat.GetColor(_fogColorPropID);
    }

    private void Update()
    {
        if (_sector.ContainsOccupant(DynamicOccupant.Player))
        {
            float distSqr = (Locator.GetPlayerTransform().position - transform.position).sqrMagnitude;
            float lerp = Mathf.InverseLerp(_upperDepth * _upperDepth, _lowerDepth * _lowerDepth, distSqr);
            _fogMat.SetColor(_fogColorPropID, Color.Lerp(_startColor, _maxDepthColor, lerp));
        }
    }
}
