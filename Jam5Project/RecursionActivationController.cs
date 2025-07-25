﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jam5Project;

public class RecursionActivationController : MonoBehaviour
{
    [SerializeField]
    private RecursionLever[] _levers;
    [SerializeField]
    private ShrinkablePlanetItem _targetWorld;

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
            StartCoroutine(InitiateRecursion());
        }
    }

    private IEnumerator InitiateRecursion()
    {
        yield return new WaitForSeconds(1.5f);
        _targetWorld.ActivatePlanet();
    }

    private void OnDestroy()
    {
        foreach (var lever in _levers)
        {
            lever.OnLeverPulled -= OnLeverPulled;
        }
    }
}
