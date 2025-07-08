using UnityEngine;

namespace Jam5Project;

public class ButtonTest : MonoBehaviour
{
    [SerializeField]
    private InteractReceiver _interactReceiver;
    [SerializeField]
    private int _planetIndex;

    private void Start()
    {
        _interactReceiver.OnPressInteract += OnPressInteract;
        _interactReceiver.ChangePrompt("Toggle " + _planetIndex);
    }

    private void OnPressInteract()
    {
        GameObject switcher = Jam5Project.NHAPI.GetPlanet("PlanetSwitcherBase");
        GameObject planet = switcher.transform.Find("Sector/PlanetStates/Planet_" + _planetIndex).gameObject;
        planet.SetActive(!planet.activeSelf);
    }

    private void OnDestroy()
    {
        _interactReceiver.OnPressInteract -= OnPressInteract;
    }
}