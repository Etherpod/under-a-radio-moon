using System.Collections;
using System.Collections.Generic;
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
    private TowerDoorSigilLock[] _sigilLocks;
    [SerializeField]
    private InteractReceiver _interactReceiver;
    [SerializeField]
    private AudioVolume _towerAmbience;
    [SerializeField]
    private AudioVolume _planetAmbience;
    [SerializeField]
    private AudioClip _openDoorSound;

    private ShrinkDevice _shrinkDevice;
    private bool _opened = false;
    private Dictionary<TowerDoorSigilLock, PlanetSigil> _lockToSigil = [];

    private void Awake()
    {
        _shrinkDevice = FindObjectOfType<ShrinkDevice>();
        _shrinkDevice.OnSigilsUpdated += OnSigilsUpdated;

        foreach (var sigilLock in _sigilLocks)
        {
            _lockToSigil.Add(sigilLock, sigilLock.GetSigil());
        }
    }

    private void Start()
    {
        _interactReceiver.ChangePrompt("Open Door");
        _interactReceiver.OnPressInteract += OnPressInteract;
        _interactReceiver.DisableInteraction();
        _towerAmbience.SetVolumeActivation(false);
    }

    private void OnSigilsUpdated(HashSet<PlanetSigil> newSigils)
    {
        var hasAll = true;

        if (newSigils.Count == 0)
        {
            foreach (var sigilLock in _sigilLocks)
            {
                sigilLock.SetSigilActive(false);
            }
            return;
        }

        foreach (var pair in _lockToSigil)
        {
            if (newSigils.Contains(pair.Value))
            {
                pair.Key.SetSigilActive(true);
            }
            else
            {
                hasAll = false;
                pair.Key.SetSigilActive(false);
            }
        }

        if (!_opened && hasAll)
        {
            _interactReceiver.EnableInteraction();
            _planetAmbience.SetVolumeActivation(false);
        }
    }

    private void OnPressInteract()
    {
        OpenDoor();
    }

    public void OpenDoor()
    {
        if (_opened || !HasRequiredSigils())
        {
            _interactReceiver.DisableInteraction();
            return;
        }

        _animator.SetTrigger("OpenDoor");
        _interactReceiver.DisableInteraction();
        _opened = true;

        StartCoroutine(DoorMusicSequence());
    }

    private IEnumerator DoorMusicSequence()
    {
        yield return new WaitForSeconds(0.5f);
        _towerAmbience.GetComponent<OWAudioSource>().PlayOneShot(_openDoorSound);
        yield return new WaitForSeconds(8f);
        _towerAmbience.SetVolumeActivation(true);
    }

    private bool HasRequiredSigils()
    {
        var list = _shrinkDevice.GetActiveSigils();
        foreach (var sigil in _lockToSigil.Values)
        {
            if (!list.Contains(sigil))
            {
                return false;
            }
        }
        return true;
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

    public void SetSigilActive(PlanetSigil sigil, bool active)
    {
        foreach (var sigilLock in _sigilLocks)
        {
            if (sigilLock.GetSigil() == sigil)
            {
                sigilLock.SetSigilActive(true);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        _interactReceiver.OnPressInteract -= OnPressInteract;
    }
}
