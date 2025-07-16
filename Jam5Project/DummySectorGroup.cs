using UnityEngine;

namespace Jam5Project;

public class DummySectorGroup : MonoBehaviour, ISectorGroup
{
    [SerializeField]
    private Sector _sector;

    public Sector GetSector()
    {
        return _sector;
    }

    public void SetSector(Sector sector)
    {
        _sector = sector;
    }
}