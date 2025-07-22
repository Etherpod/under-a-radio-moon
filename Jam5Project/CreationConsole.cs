using System;
using NewHorizons.Components;
using UnityEngine;

namespace Jam5Project;

public class CreationConsole : MonoBehaviour
{
    [SerializeField]
    private InteractReceiver interactReceiver = null;

    private void Start()
    {
        if (interactReceiver == null) interactReceiver = gameObject.GetRequiredComponentInChildren<InteractReceiver>();

        interactReceiver.ChangePrompt("Press big button");
        interactReceiver.OnPressInteract += OnButtonPressed;
    }

    private void OnDestroy()
    {
        interactReceiver.OnPressInteract -= OnButtonPressed;
    }

    private void OnButtonPressed()
    {
        DialogueConditionManager.SharedInstance.SetConditionState("URM_GAME_OVER", true);
        Jam5Project.StartGameOver();
    }
}