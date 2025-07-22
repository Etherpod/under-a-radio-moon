using UnityEngine;

namespace Jam5Project;

public class GearInvertLever : MonoBehaviour
{
    public delegate void LeverPullEvent(GearInvertLever lever);
    public LeverPullEvent OnLeverPulled;

    [SerializeField]
    private InteractReceiver _interactReceiver;
    [SerializeField]
    private float _startAngle = -31.578f;
    [SerializeField]
    private float _endAngle = -104.6f;

    private bool _activated = false;
    private float _moveLength = 0.8f;
    private float _moveStartTime;

    private void Start()
    {
        _interactReceiver.ChangePrompt("Pull Lever");
        _interactReceiver.OnPressInteract += OnPressInteract;
        enabled = false;
    }

    private void Update()
    {
        var lerp = Mathf.InverseLerp(_moveStartTime, _moveStartTime + _moveLength, Time.time);
        var xAngle = Mathf.SmoothStep(_startAngle, _endAngle, lerp);
        transform.localRotation = Quaternion.Euler(xAngle, 0, 0);

        if (lerp >= 1f)
        {
            OnLeverPulled?.Invoke(this);
            enabled = false;
        }
    }

    private void OnPressInteract()
    {
        if (_activated)
        {
            _interactReceiver.DisableInteraction();
            return;
        }

        _moveStartTime = Time.time;
        enabled = true;
        _interactReceiver.DisableInteraction();
        _activated = true;
    }

    private void OnDestroy()
    {
        _interactReceiver.OnPressInteract -= OnPressInteract;
    }
}
