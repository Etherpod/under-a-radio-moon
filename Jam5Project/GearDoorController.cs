using UnityEngine;

namespace Jam5Project;

public class GearDoorController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private OWAudioSource _oneShotAudio;
    [SerializeField]
    private OWAudioSource _loopAudio;

    public void OpenDoor()
    {
        _animator.SetTrigger("OpenDoor");
    }

    public void StartDoorAudio()
    {
        _loopAudio.FadeIn(1f);
        _oneShotAudio.PlayOneShot(AudioType.NomaiDoorStart);
    }

    public void StopDoorAudio()
    {
        _loopAudio.FadeOut(0.1f);
        _oneShotAudio.PlayOneShot(AudioType.NomaiDoorStop);
    }
}
