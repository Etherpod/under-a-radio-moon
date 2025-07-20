using System.Collections.Generic;
using UnityEngine;

namespace Jam5Project;

public class RecursionActivationController : MonoBehaviour
{
    [SerializeField]
    private RecursionLever[] _levers;
    [SerializeField]
    private ShrunkenPlanet _targetWorld;

    private List<RecursionLever> _activatedLevers = [];

    private void Awake()
    {
        foreach (var lever in _levers)
        {
            lever.OnLeverPulled += OnLeverPulled;
        }
    }

    private void OnLeverPulled(RecursionLever lever)
    {
        if (_activatedLevers.Count == _levers.Length) return;

        if (!_activatedLevers.Contains(lever))
        {
            _activatedLevers.Add(lever);
        }

        if (_activatedLevers.Count == _levers.Length)
        {
            InitiateRecursion();
        }
    }

    private void InitiateRecursion()
    {
        _targetWorld.OnPressInteract();
    }

    private void OnDestroy()
    {
        foreach (var lever in _levers)
        {
            lever.OnLeverPulled -= OnLeverPulled;
        }
    }
}
