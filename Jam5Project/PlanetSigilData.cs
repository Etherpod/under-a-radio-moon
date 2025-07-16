using System;
using System.Collections.Generic;
using System.Linq;

namespace Jam5Project;

public class PlanetSigilData
{
    private HashSet<PlanetSigil> _planetSigils = new();
    private List<PlanetSigilData> _childData = new();

    public PlanetSigilData(PlanetSigil[] sigils)
    {
        foreach (var sigil in sigils)
        {
            _planetSigils.Add(sigil);
        }
    }

    public HashSet<PlanetSigil> GetPlanetSigils()
    {
        HashSet<PlanetSigil> allSigils = new();

        foreach (var orig in _planetSigils)
        {
            allSigils.Add(orig);
        }

        foreach (var data in _childData)
        {
            var list = data.GetPlanetSigils();
            foreach (var sigil in list)
            {
                allSigils.Add(sigil);
            }
        }

        return allSigils;
    }

    public void AddChildData(PlanetSigilData data)
    {
        Jam5Project.WriteDebugMessage("Add sigil " + data.GetPlanetSigils().ToArray()[0]);
        _childData.Add(data);
    }

    public void RemoveChildData(PlanetSigilData data)
    {
        Jam5Project.WriteDebugMessage("Remove sigil " + data.GetPlanetSigils().ToArray()[0]);
        _childData.Remove(data);
    }
}

public enum PlanetSigil
{
    Red,
    Blue,
    Green
}