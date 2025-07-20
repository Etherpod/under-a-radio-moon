using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jam5Project;

public class ShrunkenPlanet : MonoBehaviour
{
    private static readonly int MusicTimeProperty = Shader.PropertyToID("_MusicTime");

    [SerializeField]
    private Transform _arrivalPoint = null;
    [SerializeField]
    private Transform _lookTarget = null;
    [SerializeField]
    private Transform _scaleRoot = null;
    [SerializeField]
    private Transform _expandParent = null;
    [SerializeField]
    private float _minScale = 0.002f;
    [SerializeField]
    private PlanetSigil[] _planetSigils = [];
    [SerializeField]
    private OWAudioSource _musicSource = null;
    [SerializeField]
    private MeshRenderer _skyorbRenderer = null;

    private ShrinkerController _shrinkerController;
    private bool _shrunken;
    private Transform _defaultParent;
    private List<ShrunkenTriggerVolume> _triggerVolumes = [];
    private List<ShrunkenLightData> _lights = [];
    private PlanetSigilData _sigilData;

    public bool IsShrunk
    {
        get => _shrunken;
        set => _shrunken = value;
    }

    private void Awake()
    {
        _shrinkerController = FindObjectOfType<ShrinkerController>();
        _defaultParent = transform.parent;
        _sigilData = new(_planetSigils);
    }

    private void Start()
    {
        foreach (var light in GetComponentsUnderPlanet<Light>(transform))
        {
            if (light.TryGetComponent(out ShrunkenLightData lightData))
            {
                _lights.Add(lightData);
            }
            else
            {
                _lights.Add(light.gameObject.AddComponent<ShrunkenLightData>());
            }
        }

        foreach (var triggerVolume in GetComponentsUnderPlanet<OWTriggerVolume>(transform))
        {
            if (triggerVolume.TryGetComponent(out ShrunkenTriggerVolume vol))
            {
                _triggerVolumes.Add(vol);
            }
            else
            {
                _triggerVolumes.Add(triggerVolume.gameObject.AddComponent<ShrunkenTriggerVolume>());
            }
        }

        SetScaleLerp(0f);
        OnChangeSize(false);
        _shrunken = true;
    }

    private void Update()
    {
        if (!_shrunken)
        {
            Shader.SetGlobalFloat(MusicTimeProperty, _musicSource.time);
        }
    }

    private T[] GetComponentsUnderPlanet<T>(Transform t)
    {
        if (t.childCount == 0 
            || (t.GetComponent<ShrunkenPlanet>() && t != transform))
        {
            return [];
        }

        var list = new List<T>();
        foreach (Transform child in t)
        {
            list.AddRange(child.GetComponents<T>());
            list.AddRange(GetComponentsUnderPlanet<T>(child));
        }

        return list.ToArray();
    }

    public void OnPressInteract()
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

    public PlanetSigilData GetSigilData()
    {
        return _sigilData;
    }

    public void SetScaleLerp(float lerp)
    {
        var scale = Mathf.SmoothStep(_minScale, 1f, lerp);
        _scaleRoot.transform.localScale = Vector3.one * scale;
        foreach (var light in _lights)
        {
            light.SetRangeScale(scale);
        }
        _skyorbRenderer.material.SetFloat("_Alpha", lerp);
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

    public void OnChangeSize(bool makeBig)
    {
        foreach (var vol in _triggerVolumes)
        {
            vol.SetVolumeEnabled(makeBig);
        }
    }
}
