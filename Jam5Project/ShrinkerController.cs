using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jam5Project;

public class ShrinkerController : MonoBehaviour
{
    /*[SerializeField]
    private float _minScale = 0.001f;*/
    [SerializeField]
    private GameObject _basePlanet;

    private PlayerCameraController _cameraController;
    private bool _updateShrink;
    private bool _shrinkAfterDelay;
    private bool _hasResetFOV;
    private float _startShrinkTime;
    private float _startDistance;
    private List<ShrunkenPlanet> _shrunkenPlanets = [];
    private List<Transform> _startPositions = [];
    private float _startScaleTime;
    private float _endScaleTime;
    private bool _updateScale;
    private bool _scaleUp;

    private void Start()
    {
        _cameraController = Locator.GetPlayerCamera().GetComponent<PlayerCameraController>();
        //transform.localScale = Vector3.one * _minScale;
    }

    private void Update()
    {
        if (Keyboard.current.numpadDivideKey.wasPressedThisFrame
            && !_updateShrink && _shrunkenPlanets.Count > 0 && !_shrunkenPlanets[0].IsShrunk)
        {
            SetShrunken(true, _shrunkenPlanets[0]);
        }

        if (_shrinkAfterDelay && Time.time > _startShrinkTime)
        {
            StartShrink();
        }

        if (_updateScale)
        {
            if (_shrunkenPlanets.Count == 0 || Time.time > _endScaleTime)
            {
                _updateScale = false;
                return;
            }

            float scaleLerp = Mathf.InverseLerp(_startScaleTime, _endScaleTime, Time.time);
            if (!_scaleUp)
            {
                scaleLerp = 1 - scaleLerp;
            }
            _shrunkenPlanets[0].SetScaleLerp(scaleLerp);
        }
    }

    private void FixedUpdate()
    {
        if (_updateShrink)
        {
            bool makeBig = _shrunkenPlanets[0].IsShrunk;

            Vector3 toTarget;
            if (makeBig)
            {
                toTarget = _shrunkenPlanets[0].GetArrivalPoint().position - Locator.GetPlayerTransform().position;
            }
            else
            {
                toTarget = _startPositions[0].position - Locator.GetPlayerTransform().position;
            }

            Vector3 targetPos = toTarget.normalized * 20f;
            Vector3 velocity = Locator.GetPlayerBody().GetVelocity();
            if (toTarget.magnitude < 7f)
            {
                targetPos = Vector3.zero;
                if (!_hasResetFOV)
                {
                    _cameraController.SnapToInitFieldOfView(1.5f, true);
                    _hasResetFOV = true;
                }
                if (velocity.magnitude < 1f)
                {
                    _hasResetFOV = false;
                    _updateShrink = false;
                    Locator.GetPlayerController().SetColliderActivation(true);
                    Locator.GetPlayerController().UnlockMovement();
                    Locator.GetPlayerDetector().GetComponent<ForceApplier>().SetApplyForces(true);
                    Locator.GetPlayerTransform().GetComponent<PlayerLockOnTargeting>().BreakLock();
                    OWInput.ChangeInputMode(InputMode.Character);
                    _shrunkenPlanets[0].IsShrunk = !_shrunkenPlanets[0].IsShrunk;

                    if (!makeBig)
                    {
                        var posObj = _startPositions[0];
                        _startPositions.RemoveAt(0);
                        Destroy(posObj.gameObject);

                        _shrunkenPlanets[0].SetTempParent(false);
                        _shrunkenPlanets.RemoveAt(0);

                        if (_shrunkenPlanets.Count == 0)
                        {
                            _basePlanet.SetActive(true);
                        }
                        else
                        {
                            _shrunkenPlanets[0].gameObject.SetActive(true);
                        }
                    }

                    return;
                }
            }
            Vector3 nextPos = Vector3.MoveTowards(velocity, targetPos, 50f * Time.deltaTime);
            Locator.GetPlayerBody().AddVelocityChange(nextPos - velocity);
        }
    }

    private void StartShrink()
    {
        /*if (_shrunkenPlanets.Count == 0 || _startPositions.Count == 0)
        {
            Locator.GetPlayerTransform().GetComponent<PlayerLockOnTargeting>().BreakLock();
            return;
        }*/

        Locator.GetPlayerController().SetColliderActivation(false);
        Locator.GetPlayerController().LockMovement(false);
        Locator.GetPlayerDetector().GetComponent<ForceApplier>().SetApplyForces(false);
        _cameraController.SetTargetFieldOfView(120f, 0.5f, true);
        if (_shrunkenPlanets[0].IsShrunk)
        {
            _startDistance = (_shrunkenPlanets[0].GetArrivalPoint().position - Locator.GetPlayerTransform().position).magnitude;
            if (_shrunkenPlanets.Count > 1)
            {
                _shrunkenPlanets[0].SetTempParent(true);
                _shrunkenPlanets[1].gameObject.SetActive(false);
            }
            else
            {
                _basePlanet.SetActive(false);
            }
        }
        else
        {
            _startDistance = (_startPositions[0].position - Locator.GetPlayerTransform().position).magnitude;
        }
        _updateShrink = true;
        _shrinkAfterDelay = false;
        _updateScale = true;
        _scaleUp = _shrunkenPlanets[0].IsShrunk;
        _startScaleTime = Time.time;
        _endScaleTime = Time.time + 5f;
    }

    public void SetShrunken(bool setShrunk, ShrunkenPlanet planet)
    {
        if (setShrunk == planet.IsShrunk || _updateShrink) return;

        if (!setShrunk)
        {
            var startTransform = new GameObject("ShrinkStartPos").transform;
            startTransform.parent = _shrunkenPlanets.Count > 0 ? planet.transform : _basePlanet.transform;
            startTransform.position = Locator.GetPlayerTransform().position;
            _startPositions.Insert(0, startTransform);
            _shrunkenPlanets.Insert(0, planet);

            Locator.GetPlayerTransform().GetComponent<PlayerLockOnTargeting>().LockOn(planet.GetLookTarget(), 5f, false, 1f);
        }
        else
        {
            Locator.GetPlayerTransform().GetComponent<PlayerLockOnTargeting>().LockOn(_startPositions[0], 5f, false, 1f);
        }

        OWInput.ChangeInputMode(InputMode.None);
        _shrinkAfterDelay = true;
        _startShrinkTime = Time.time + 0.4f;
    }
}
