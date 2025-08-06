using UnityEngine;

namespace Jam5Project;

public class CrushTriggerVolume : MonoBehaviour
{
    private OWTriggerVolume _triggerVolume;

    private void Awake()
    {
        _triggerVolume = GetComponent<OWTriggerVolume>();
        _triggerVolume.OnEntry += OnEntry;
    }

    private void OnEntry(GameObject hitObj)
    {
        if (hitObj.CompareTag("PlayerDetector"))
        {
            Locator.GetDeathManager().KillPlayer(DeathType.CrushedByElevator);
            RumbleManager.PlayerCrushedByElevator();
        }
    }

    private void OnDestroy()
    {
        _triggerVolume.OnEntry -= OnEntry;
    }
}
