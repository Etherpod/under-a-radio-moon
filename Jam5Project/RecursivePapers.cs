using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jam5Project;

public class RecursivePapers : MonoBehaviour
{
    [SerializeField]
    private Animator _animator = null;
    [SerializeField]
    private InteractReceiver _interactReceiver = null;
    [SerializeField]
    private ErnestonianText _ernestonianText = null;
    [SerializeField]
    private NomaiTextLine _hiddenPage = null;

    private OWCollider _textCollider;

    private void Start()
    {
        if (_animator == null) _animator = gameObject.GetRequiredComponent<Animator>();
        if (_interactReceiver == null) _interactReceiver = gameObject.GetRequiredComponent<InteractReceiver>();
        _textCollider = _ernestonianText.GetComponent<OWCollider>();
        
        _hiddenPage.SetUnreadState();
        _textCollider.SetActivation(false);
        
        _interactReceiver.ChangePrompt("Spread pages");
        _interactReceiver.OnPressInteract += OnPressInteract;
    }

    private void OnDestroy()
    {
        _interactReceiver.OnPressInteract -= OnPressInteract;
    }

    private void FixedUpdate()
    {
        if (_textCollider.IsActive()) _textCollider.SetActivation(false);
    }

    private void OnPressInteract()
    {
        enabled = false;
        _interactReceiver.DisableInteraction();
        _ernestonianText.ShowImmediate();
        _textCollider.SetActivation(true);
        _animator.SetBool("Fan", true);
    }

}
