using System;

namespace Jam5Project;

public class ShrinkablePlanetSocket : OWItemSocket
{
    public override void Awake()
    {
        base.Awake();
        _acceptableType = ShrinkablePlanetItem.ItemType;
    }
}
