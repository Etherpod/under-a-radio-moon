using UnityEngine;
using UnityEngine.InputSystem;

public class ShrinkerController : MonoBehaviour
{
    [SerializeField]
    private float _minScale = 0.001f;
    [SerializeField]
    private Transform _targetTransform;
    [SerializeField]
    private Transform _startTransform;
    [SerializeField]
    private GameObject _basePlanet;

    private PlayerCameraController _cameraController;
    private bool _updateShrink;
    private bool _shrinkAfterDelay;
    private bool _hasResetFOV;
    private float _startShrinkTime;
    private float _startDistance;
    private bool _shrinked = false;

    private void Start()
    {
        _cameraController = Locator.GetPlayerCamera().GetComponent<PlayerCameraController>();
        transform.localScale = Vector3.one * _minScale;
    }

    private void Update()
    {
        if (!_updateShrink && Keyboard.current.numpadDivideKey.wasPressedThisFrame)
        {
            if (_shrinked)
            {
                Locator.GetPlayerTransform().GetComponent<PlayerLockOnTargeting>().LockOn(_startTransform, 5f, false, 1f);
            }
            else
            {
                Locator.GetPlayerTransform().GetComponent<PlayerLockOnTargeting>().LockOn(transform, 5f, false, 1f);
            }
            OWInput.ChangeInputMode(InputMode.None);
            _shrinkAfterDelay = true;
            _startShrinkTime = Time.time + 0.4f;
        }

        if (_shrinkAfterDelay && Time.time > _startShrinkTime)
        {
            StartShrink();
        }
    }

    private void FixedUpdate()
    {
        if (_updateShrink)
        {
            Vector3 toTarget;
            if (!_shrinked)
            {
                toTarget = _targetTransform.position - Locator.GetPlayerTransform().position;
            }
            else
            {
                toTarget = _startTransform.position - Locator.GetPlayerTransform().position;
            }

            Vector3 targetPos = toTarget.normalized * 20f;
            Vector3 velocity = Locator.GetPlayerBody().GetVelocity();
            float num = 50f;
            if (toTarget.magnitude < 7f)
            {
                targetPos = Vector3.zero;
                num = 50f;
                if (!_hasResetFOV)
                {
                    _cameraController.SnapToInitFieldOfView(0.5f, true);
                    _hasResetFOV = true;
                }
                if (velocity.magnitude < 1f)
                {
                    if (_shrinked)
                    {
                        _basePlanet.SetActive(true);
                    }

                    _hasResetFOV = false;
                    _updateShrink = false;
                    Locator.GetPlayerController().SetColliderActivation(true);
                    Locator.GetPlayerController().UnlockMovement();
                    Locator.GetPlayerDetector().GetComponent<ForceApplier>().SetApplyForces(true);
                    Locator.GetPlayerTransform().GetComponent<PlayerLockOnTargeting>().BreakLock();
                    OWInput.ChangeInputMode(InputMode.Character);
                    _shrinked = !_shrinked;

                    return;
                }
            }
            Vector3 vector3 = Vector3.MoveTowards(velocity, targetPos, num * Time.deltaTime);
            Locator.GetPlayerBody().AddVelocityChange(vector3 - velocity);
            float scaleLerp = Mathf.InverseLerp(_startDistance, 7f, toTarget.magnitude);
            float multStart = _shrinked ? 1f : _minScale;
            float multEnd = _shrinked ? _minScale : 1f;
            float smoothStep = Mathf.Lerp(multStart, multEnd, scaleLerp);
            transform.localScale = Vector3.one * smoothStep;
        }
    }

    private void StartShrink()
    {
        Locator.GetPlayerController().SetColliderActivation(false);
        Locator.GetPlayerController().LockMovement(false);
        Locator.GetPlayerDetector().GetComponent<ForceApplier>().SetApplyForces(false);
        _cameraController.SetTargetFieldOfView(120f, 2f, true);
        if (!_shrinked)
        {
            _startDistance = (_targetTransform.position - Locator.GetPlayerTransform().position).magnitude;
            _startTransform.position = Locator.GetPlayerTransform().position;
            _basePlanet.SetActive(false);
        }
        else
        {
            _startDistance = (_startTransform.position - Locator.GetPlayerTransform().position).magnitude;
        }
        _updateShrink = true;
        _shrinkAfterDelay = false;
    }
}
