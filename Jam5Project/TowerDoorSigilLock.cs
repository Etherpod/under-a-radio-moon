using UnityEngine;

namespace Jam5Project;

public class TowerDoorSigilLock : MonoBehaviour
{
    [SerializeField]
    private PlanetSigil _sigil;
    [SerializeField]
    private OWEmissiveRenderer _emissiveRenderer;

    private bool _active = true;

    private void Awake()
    {
        _emissiveRenderer.SetEmissiveScale(0f);
    }

    public PlanetSigil GetSigil()
    {
        return _sigil;
    }

    public bool IsActive()
    {
        return _active;
    }

    public void SetSigilActive(bool active)
    {
        _active = active;
        _emissiveRenderer.SetEmissiveScale(active ? 1f : 0f);
    }
}
