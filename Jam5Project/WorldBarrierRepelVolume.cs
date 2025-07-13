using UnityEngine;
using System.Collections.Generic;

namespace Jam5Project;

[RequireComponent(typeof(SphereShape))]
[RequireComponent(typeof(OWTriggerVolume))]
public class WorldBarrierRepelVolume : MonoBehaviour
{
    [SerializeField]
    private float _innerRadius;

    private SphereShape _sphereShape;
    private OWTriggerVolume _trigger;
    private OWRigidbody _parentBody;
    private List<OWRigidbody> _trackedBodies;

    private void Awake()
    {
        _parentBody = gameObject.GetAttachedOWRigidbody(false);
        _sphereShape = gameObject.GetRequiredComponent<SphereShape>();
        _trigger = gameObject.GetRequiredComponent<OWTriggerVolume>();
        _trigger.OnEntry += OnEntry;
        _trigger.OnExit += OnExit;
        _trackedBodies = new List<OWRigidbody>();
        enabled = false;
    }

    private void OnDestroy()
    {
        _trigger.OnEntry -= OnEntry;
        _trigger.OnExit -= OnExit;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < this._trackedBodies.Count; i++)
        {
            if (!_trackedBodies[i].CompareTag("Player") || !PlayerState.IsInsideShip())
            {
                Vector3 bodyDir = transform.position - _trackedBodies[i].GetPosition();
                float distance = bodyDir.magnitude;
                Vector3 velocityDiff = _trackedBodies[i].GetVelocity() - _parentBody.GetVelocity();
                float distLerp = Mathf.InverseLerp(_innerRadius, _sphereShape.radius, distance);
                Vector3 vector3 = (Vector3.ProjectOnPlane(velocityDiff, bodyDir).normalized * velocityDiff.magnitude - velocityDiff) * distLerp;
                _trackedBodies[i].AddVelocityChange(vector3);
            }
        }
    }

    private void OnEntry(GameObject hitObj)
    {
        if ((hitObj.CompareTag("PlayerDetector") || hitObj.CompareTag("ShipDetector") || hitObj.CompareTag("ProbeDetector")) && Vector3.Distance(base.transform.position, hitObj.transform.position) > this._innerRadius)
        {
            _trackedBodies.Add(hitObj.GetAttachedOWRigidbody(false));
            enabled = true;
        }
    }

    private void OnExit(GameObject hitObj)
    {
        if (hitObj.CompareTag("PlayerDetector") || hitObj.CompareTag("ShipDetector") || hitObj.CompareTag("ProbeDetector"))
        {
            _trackedBodies.Remove(hitObj.GetAttachedOWRigidbody(false));
            if (_trackedBodies.Count == 0)
            {
                enabled = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _innerRadius);
    }
}
