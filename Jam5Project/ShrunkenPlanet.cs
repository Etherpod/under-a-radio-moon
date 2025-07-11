using UnityEngine;

namespace Jam5Project;

public class ShrunkenPlanet : MonoBehaviour
{
    [SerializeField]
    private InteractReceiver _interactReceiver;
    [SerializeField]
    private Transform _arrivalPoint;
    [SerializeField]
    private Transform _lookTarget;
    [SerializeField]
    private Transform _scaleRoot;
    [SerializeField]
    private Transform _expandParent;
    [SerializeField]
    private float _minScale = 0.001f;
    [SerializeField]
    private ShrunkenLightData[] _lightsToScale;

    private ShrinkerController _shrinkerController;
    private bool _shrunken;
    private Transform _defaultParent;

    public bool IsShrunk
    {
        get
        {
            return _shrunken;
        }
        set
        {
            _shrunken = value;
        }
    }

    private void Awake()
    {
        _shrinkerController = FindObjectOfType<ShrinkerController>();
        _defaultParent = transform.parent;
    }

    private void Start()
    {
        SetScaleLerp(_minScale);
        _shrunken = true;

        _interactReceiver.ChangePrompt("Enter World");
        _interactReceiver.OnPressInteract += OnPressInteract;
    }

    private void OnPressInteract()
    {
        _shrinkerController.SetShrunken(false, this);
    }

    public Transform GetArrivalPoint()
    {
        return _arrivalPoint;
    }

    public Transform GetLookTarget()
    {
        return _lookTarget;
    }

    public void SetScaleLerp(float lerp)
    {
        var scale = Mathf.SmoothStep(_minScale, 1f, lerp);
        _scaleRoot.transform.localScale = Vector3.one * scale;
        foreach (var light in _lightsToScale)
        {
            light.SetRangeScale(scale);
        }
    }

    public void SetTempParent(bool tempParent)
    {
        if (tempParent)
        {
            transform.parent = _expandParent;
        }
        else
        {
            transform.parent = _defaultParent;
        }
    }

    private void OnDestroy()
    {
        _interactReceiver.OnPressInteract -= OnPressInteract;
    }
}
