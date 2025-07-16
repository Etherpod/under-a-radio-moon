using System.Collections.Generic;
using UnityEngine;

namespace Jam5Project;

public class ShrinkablePlanetItem : OWItem
{
    public static ItemType ItemType = Jam5Project.ShrinkablePlanetType;

    [SerializeField]
    private string _displayName;
    [SerializeField]
    private ShrunkenPlanet _planetController;

    public override void Awake()
    {
        _type = ItemType;
        base.Awake();
        List<OWCollider> goodColliders = [];
        for (int i = 0; i < _colliders.Length; i++)
        {
            if (!_colliders[i].GetComponent<OWTriggerVolume>()
                && !_colliders[i].GetComponent<InteractReceiver>())
            {
                goodColliders.Add(_colliders[i]);
            }
        }
        _colliders = goodColliders.ToArray();
    }

    public override string GetDisplayName()
    {
        return _displayName;
    }

    public override void DropItem(Vector3 position, Vector3 normal, Transform parent, Sector sector, IItemDropTarget customDropTarget)
    {
        base.DropItem(position, normal, parent, sector, customDropTarget);
        var planet = GetComponentInParent<ShrunkenPlanet>();
        if (GetComponentInParent<ShrunkenPlanet>())
        {
            planet.GetSigilData().AddChildData(_planetController.GetSigilData());
        }
    }

    public override void PickUpItem(Transform holdTranform)
    {
        base.PickUpItem(holdTranform);
        var planet = GetComponentInParent<ShrunkenPlanet>();
        if (GetComponentInParent<ShrunkenPlanet>())
        {
            planet.GetSigilData().RemoveChildData(_planetController.GetSigilData());
        }
    }

    public ShrunkenPlanet GetPlanet()
    {
        return _planetController;
    }

    public void ActivatePlanet()
    {
        SetColliderActivation(true);
        _planetController.OnPressInteract();
    }
}
