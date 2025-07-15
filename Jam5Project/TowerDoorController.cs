using System.Linq;
using UnityEngine;

namespace Jam5Project;

public class TowerDoorController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private OWAudioSource _loopSource;
    [SerializeField]
    private OWAudioSource _oneShotSource;
    [SerializeField]
    private OWEmissiveRenderer[] _sigilRenderers;
    [SerializeField]
    private InteractReceiver _interactReceiver;

    private bool _opened = false;
    private int[] _activatedSigils = new int[3];

    private void Start()
    {
        foreach (var rend in _sigilRenderers)
        {
            rend.SetEmissiveScale(0f);
        }

        _interactReceiver.ChangePrompt("Open Door");
        _interactReceiver.OnPressInteract += OnPressInteract;
        _interactReceiver.DisableInteraction();
    }

    private void Update()
    {
        if (OWInput.IsNewlyPressed(InputLibrary.autopilot))
        {
            for (int i = 0; i <= 2; i++)
            {
                SetSigilActive(i, true);
            }
        }
    }

    private void OnPressInteract()
    {
        OpenDoor();
    }

    public void OpenDoor()
    {
        if (_opened || _activatedSigils.Any(num => num == 0)) return;

        _animator.SetTrigger("OpenDoor");
        _interactReceiver.DisableInteraction();
        _opened = true;
    }

    public void StartDoorAudio()
    {
        _loopSource.FadeIn(1f);
        _oneShotSource.PlayOneShot(AudioType.NomaiDoorStart);
    }

    public void StopDoorAudio()
    {
        _loopSource.FadeOut(0.1f);
        _oneShotSource.PlayOneShot(AudioType.NomaiDoorStop);
    }

    // 0: Red
    // 1: Blue
    // 2: Green
    public void SetSigilActive(int index, bool active)
    {
        _activatedSigils[index] = active ? 1 : 0;
        _sigilRenderers[index].SetEmissiveScale(active ? 1 : 0);

        if (!_opened && _activatedSigils.All(num => num == 1))
        {
            _interactReceiver.EnableInteraction();
        }
    }

    private void OnDestroy()
    {
        _interactReceiver.OnPressInteract -= OnPressInteract;
    }
}
