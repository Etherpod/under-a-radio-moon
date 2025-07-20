using System.Collections.Generic;
using UnityEngine;

namespace Jam5Project;

public class ShrinkDevice : MonoBehaviour
{
    public delegate void SigilUpdateEvent(HashSet<PlanetSigil> newSigils);
    public event SigilUpdateEvent OnSigilsUpdated;

    [SerializeField]
    private ShrinkablePlanetSocket _mainSlot;
    [SerializeField]
    private ShrinkablePlanetSocket _secondarySlot;
    [SerializeField]
    private InteractReceiver _activateButton;

    private PlanetSigilData _activeSigilData;

    private void Start()
    {
        _mainSlot.OnSocketablePlaced += OnPlacedInMain;
        _mainSlot.OnSocketableRemoved += OnRemovedFromMain;
        _secondarySlot.OnSocketablePlaced += OnPlacedInSecondary;
        _secondarySlot.OnSocketableRemoved += OnRemovedFromSecondary;

        _activateButton.ChangePrompt("Activate device");
        _activateButton.OnPressInteract += OnPressInteract;

        _activeSigilData = new([]);
    }

    private void OnPlacedInMain(OWItem item)
    {
        // idk do something
    }

    private void OnRemovedFromMain(OWItem item)
    {
        // do something else
    }

    private void OnPlacedInSecondary(OWItem item)
    {
        //_activeSigilData = (item as ShrinkablePlanetItem).GetPlanet().GetSigilData();
        //OnSigilsUpdated?.Invoke(_activeSigilData.GetPlanetSigils());
    }

    private void OnRemovedFromSecondary(OWItem item)
    {
        //_activeSigilData = null;
        //OnSigilsUpdated?.Invoke([]);
    }

    private void OnPressInteract()
    {
        if (_mainSlot.GetSocketedItem() == null) return;

        (_mainSlot.GetSocketedItem() as ShrinkablePlanetItem).ActivatePlanet();
    }

    public void AddChildSigilData(PlanetSigilData data)
    {
        _activeSigilData.AddChildData(data);
        OnSigilsUpdated?.Invoke(_activeSigilData.GetPlanetSigils());
    }

    public void RemoveChildSigilData(PlanetSigilData data)
    {
        _activeSigilData.RemoveChildData(data);
        OnSigilsUpdated?.Invoke(_activeSigilData.GetPlanetSigils());
    }

    public HashSet<PlanetSigil> GetActiveSigils()
    {
        return _activeSigilData.GetPlanetSigils();
    }

    private void OnDestroy()
    {
        _mainSlot.OnSocketablePlaced -= OnPlacedInMain;
        _mainSlot.OnSocketableRemoved -= OnRemovedFromMain;
        _secondarySlot.OnSocketablePlaced -= OnPlacedInSecondary;
        _secondarySlot.OnSocketableRemoved -= OnRemovedFromSecondary;

        _activateButton.OnPressInteract -= OnPressInteract;
    }
}
