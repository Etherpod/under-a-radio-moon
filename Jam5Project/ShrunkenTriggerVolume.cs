using UnityEngine;

namespace Jam5Project;

[RequireComponent(typeof(OWTriggerVolume))]
public class ShrunkenTriggerVolume : MonoBehaviour
{
    private OWTriggerVolume _triggerVolume;
    private bool _volumeActive;
    private bool _volumeEnabled;
    private bool _initialized = false;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _triggerVolume = gameObject.GetRequiredComponent<OWTriggerVolume>();
        _volumeActive = _triggerVolume._active;
        _volumeEnabled = _volumeActive;
        _initialized = true;
    }

    public void SetVolumeEnabled(bool enabled)
    {
        if (!_initialized)
        {
            Initialize();
        }

        if (enabled == _volumeEnabled || !_triggerVolume.gameObject.activeInHierarchy) return;

        _volumeEnabled = enabled;

        if (!enabled)
        {
            _volumeActive = _triggerVolume._active;
            _triggerVolume.SetTriggerActivation(false);
        }
        else
        {
            _triggerVolume.SetTriggerActivation(_volumeActive);
        }
    }
}
