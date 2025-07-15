using UnityEngine;

namespace Jam5Project;

[RequireComponent(typeof(OWTriggerVolume))]
public class ShrunkenTriggerVolume : MonoBehaviour
{
    private OWTriggerVolume _triggerVolume;
    private bool _volumeActive;
    private bool _volumeEnabled;

    private void Awake()
    {
        _triggerVolume = gameObject.GetRequiredComponent<OWTriggerVolume>();
        _volumeActive = _triggerVolume._active;
        _volumeEnabled = _volumeActive;
    }

    public void SetVolumeEnabled(bool enabled)
    {
        if (enabled == _volumeEnabled) return;

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
