using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Jam5Project;

public class ShrinkerController : MonoBehaviour
{
    public delegate void PlanetArriveEvent(ShrunkenPlanet planet);
    public event PlanetArriveEvent OnArriveAtPlanet;
    public delegate void PlanetLeaveEvent(ShrunkenPlanet planet);
    public event PlanetLeaveEvent OnLeavePlanet;

    [SerializeField]
    private GameObject _basePlanet;
    [SerializeField]
    private AudioVolume _basePlanetAmbience;

    private PlayerCameraController _cameraController;
    private bool _updateShrink;
    private bool _shrinkAfterDelay;
    private bool _hasResetFOV;
    private bool _hasCalledEvent;
    private float _startShrinkTime;
    private float _startDistance;
    private List<ShrunkenPlanet> _shrunkenPlanets = [];
    private List<Transform> _startPositions = [];
    private float _startScaleTime;
    private float _endScaleTime;
    private bool _updateScale;
    private bool _scaleUp;
    private bool _zoomIn;
    private Quaternion _initialPlanetRotation;
    private Vector3 _initialArrivalPoint;

    private void Start()
    {
        _cameraController = Locator.GetPlayerCamera().GetComponent<PlayerCameraController>();
        //transform.localScale = Vector3.one * _minScale;
    }

    private void Update()
    {
        if (_shrinkAfterDelay && Time.time > _startShrinkTime)
        {
            StartShrink();
        }

        if (_updateScale)
        {
            if (_shrunkenPlanets.Count == 0 || Time.time > _endScaleTime)
            {
                _updateScale = false;
                if (!_scaleUp && _updateShrink)
                {
                    _zoomIn = true;
                }
                return;
            }

            float scaleLerp = Mathf.InverseLerp(_startScaleTime, _endScaleTime, Time.time);
            if (!_scaleUp)
            {
                scaleLerp = 1 - scaleLerp;
            }
            _shrunkenPlanets[0].SetScaleLerp(scaleLerp);
            float smoothStep = Mathf.SmoothStep(0f, 1f, scaleLerp);
            var initialPos = _initialArrivalPoint.normalized;
            var targetPos = (Locator.GetPlayerTransform().position - _shrunkenPlanets[0].transform.position).normalized;
            var slerp = Vector3.Slerp(initialPos, targetPos, smoothStep).normalized;
            var rot = Quaternion.FromToRotation(initialPos, slerp);
            _shrunkenPlanets[0].transform.rotation = rot * _initialPlanetRotation;
        }
    }

    private void FixedUpdate()
    {
        if (_updateShrink)
        {            
            Vector3 toTarget;
            if (_scaleUp)
            {
                if (_zoomIn)
                {
                    toTarget = _shrunkenPlanets[0].GetArrivalPoint().position - Locator.GetPlayerTransform().position;
                }
                else
                {
                    toTarget = _shrunkenPlanets[0].transform.position - Locator.GetPlayerTransform().position;
                }
            }
            else
            {
                toTarget = _startPositions[0].position - Locator.GetPlayerTransform().position;
            }

            // Max speed towards target
            Vector3 targetVelocity = toTarget.normalized * 20f;
            // Player velocity
            Vector3 velocity = Locator.GetPlayerBody().GetVelocity();
            bool slowDown = toTarget.magnitude < 10f;
            float speed = 20f;

            if (!_zoomIn)
            {
                targetVelocity = ((Locator.GetPlayerTransform().position * 2f) - _shrunkenPlanets[0].transform.position).normalized * 60f;
                slowDown = toTarget.magnitude > _startDistance + 60f;
                speed = 80f;
            }

            if (slowDown)
            {
                // Come to a stop
                targetVelocity = Vector3.zero;

                if (!_zoomIn && !_hasCalledEvent)
                {
                    _shrunkenPlanets[0].OnChangeSize(_scaleUp);
                    _hasCalledEvent = true;
                }
                // If moving slower than 1 m/s
                if (velocity.magnitude < 1f)
                {
                    if (_zoomIn)
                    {
                        // Drop player out of movement
                        _hasResetFOV = false;
                        _hasCalledEvent = false;
                        _zoomIn = false;
                        _updateShrink = false;
                        Locator.GetPlayerController().SetColliderActivation(true);
                        Locator.GetPlayerController().UnlockMovement();
                        Locator.GetPlayerDetector().GetComponent<ForceApplier>().SetApplyForces(true);
                        Locator.GetPlayerTransform().GetComponent<PlayerLockOnTargeting>().BreakLock();
                        Locator.GetPlayerBody().GetComponent<PlayerResources>()._invincible = false;
                        OWInput.ChangeInputMode(InputMode.Character);
                        _shrunkenPlanets[0].IsShrunk = !_shrunkenPlanets[0].IsShrunk;

                        if (!_scaleUp)
                        {
                            var posObj = _startPositions[0];
                            _startPositions.RemoveAt(0);
                            Destroy(posObj.gameObject);

                            _shrunkenPlanets[0].SetTempParent(false);
                            _shrunkenPlanets.RemoveAt(0);

                            if (_shrunkenPlanets.Count == 0)
                            {
                                _basePlanet.SetActive(true);
                                _basePlanetAmbience.SetVolumeActivation(true);

                                Locator.GetShipBody().gameObject.SetActive(true);
                                Locator.GetShipBody().Unsuspend();
                            }
                            else
                            {
                                _shrunkenPlanets[0].gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            Locator.GetShipLogManager().RevealFact("URM_MOON_SHIP_DEVICE_USE");
                            OnArriveAtPlanet?.Invoke(_shrunkenPlanets[0]);
                        }
                    }
                    else
                    {
                        // change to zoom in
                        _zoomIn = true;

                        // Reset FOV
                        if (!_hasResetFOV)
                        {
                            _cameraController.SnapToInitFieldOfView(5f, true);
                            _hasResetFOV = true;
                        }
                    }

                    return;
                }
            }

            float lerpSpeed = Time.deltaTime * speed;

            // Smooth lerp from current velocity to target velocity
            Vector3 nextVelocity = Vector3.MoveTowards(velocity, targetVelocity, lerpSpeed);
            Locator.GetPlayerBody().AddVelocityChange(nextVelocity - velocity);
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
        Locator.GetPlayerBody().GetComponent<PlayerResources>()._invincible = true;
        _cameraController.SetTargetFieldOfView(120f, 2f, true);
        if (_shrunkenPlanets[0].IsShrunk)
        {
            _startDistance = (_shrunkenPlanets[0].transform.position - Locator.GetPlayerTransform().position).magnitude;
            _initialPlanetRotation = _shrunkenPlanets[0].transform.rotation;
            _initialArrivalPoint = _shrunkenPlanets[0].GetArrivalPoint().transform.position - _shrunkenPlanets[0].transform.position;
            _shrunkenPlanets[0].SetTempParent(true);
            if (_shrunkenPlanets.Count > 1)
            {
                _shrunkenPlanets[1].gameObject.SetActive(false);
            }
            else
            {
                _basePlanet.SetActive(false);
                _basePlanetAmbience.SetVolumeActivation(false);
                Locator.GetShipBody().Suspend();
                Locator.GetShipBody().gameObject.SetActive(false);
            }
        }
        else
        {
            _startDistance = (_startPositions[0].position - Locator.GetPlayerTransform().position).magnitude;
            OnLeavePlanet?.Invoke(_shrunkenPlanets[0]);
        }
        _updateShrink = true;
        _shrinkAfterDelay = false;
        _updateScale = true;
        _scaleUp = _shrunkenPlanets[0].IsShrunk;
        _startScaleTime = Time.time;
        _endScaleTime = Time.time + 3f;
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

    public bool IsPlayerShrunken()
    {
        return _shrunkenPlanets.Count > 0;
    }

    public void ShrinkCurrentPlanet()
    {
        if (_shrunkenPlanets.Count > 0 && !_shrunkenPlanets[0].IsShrunk
            && !_updateShrink)
        {
            SetShrunken(true, _shrunkenPlanets[0]);
        }
    }
}
