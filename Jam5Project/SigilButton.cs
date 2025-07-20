using UnityEngine;

namespace Jam5Project;

public class SigilButton : MonoBehaviour
{
    [SerializeField]
    private InteractReceiver _interactReceiver;
    [SerializeField]
    private Transform _buttonTransform;
    [SerializeField]
    private float _pressDist;
    [SerializeField]
    private OWEmissiveRenderer _emissiveRend;
    [SerializeField]
    private PlanetSigil _sigil;

    private Vector3 _initButtonPos;
    private PlanetSigilData _sigilData;
    private ShrinkDevice _shrinkDevice;
    private bool _activated;

    private void Awake()
    {
        _shrinkDevice = FindObjectOfType<ShrinkDevice>();
        _sigilData = new([_sigil]);
        _initButtonPos = _buttonTransform.localPosition;
    }

    private void Start()
    {
        _interactReceiver.ChangePrompt("Activate");
        _interactReceiver.OnPressInteract += OnPressInteract;
        _interactReceiver.OnReleaseInteract += OnReleaseInteract;

        _emissiveRend.SetEmissiveScale(0f);
    }

    private void OnPressInteract()
    {
        _buttonTransform.localPosition += new Vector3(0f, -_pressDist, 0f);
        
        if (!_activated)
        {
            _shrinkDevice.AddChildSigilData(_sigilData);
            _emissiveRend.SetEmissiveScale(1f);
            _activated = true;
        }
    }

    private void OnReleaseInteract()
    {
        _buttonTransform.localPosition = _initButtonPos;

        if (_activated)
        {
            _interactReceiver.DisableInteraction();
        }
    }

    private void OnDestroy()
    {
        _interactReceiver.OnPressInteract -= OnPressInteract;
        _interactReceiver.OnReleaseInteract -= OnReleaseInteract;
    }
}
