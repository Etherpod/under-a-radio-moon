using UnityEngine;

namespace Jam5Project;

public class ReturnWarpButton : MonoBehaviour
{
    [SerializeField]
    private InteractZone _interactReceiver;

    private void Start()
    {
        _interactReceiver.ChangePrompt("Return");
        _interactReceiver.OnPressInteract += OnPressInteract;
    }

    private void OnPressInteract()
    {
        FindObjectOfType<ShrinkerController>().ShrinkCurrentPlanet();
    }

    private void OnDestroy()
    {
        _interactReceiver.OnPressInteract -= OnPressInteract;
    }
}
