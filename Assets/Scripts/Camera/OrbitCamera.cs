using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField]
    private OrbitCameraData _data;

    private Transform _target;
    private Camera _camera;
    private Vector2 _orbitAngles = new Vector2(45f, 0f);
    private Vector3 _focusPoint;
    private float _targetDistance;
    private float _lastManualRotationTime;
    private Vector3 _previousFocusPoint;
    private CharacterRenderer _characterRenderer;

    public Quaternion Rotation => transform.rotation;
    private bool IsFirstPersonView => _targetDistance == 0;
    private float MinVerticalAngle => IsFirstPersonView ? _data.FirstPerson.MinVerticalAngle : _data.ThirdPerson.MinVerticalAngle;
    private float MaxVerticalAngle => IsFirstPersonView ? _data.FirstPerson.MaxVerticalAngle : _data.ThirdPerson.MaxVerticalAngle;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        transform.localRotation = Quaternion.Euler(_orbitAngles);
    }

    public void FollowTo(Transform target, [CanBeNull] CharacterRenderer characterRenderer)
    {
        _focusPoint = target.position;
        _target = target;
        _characterRenderer = characterRenderer;
        _targetDistance = _data.DefaultZoomDistance;
    }

    public void SwitchPointOfView()
    {
        _targetDistance = _targetDistance == 0f ? _data.Distance : 0f;
    }

    public void UpdateWithInput(Vector2 lookInputVector, float zoomInput)
    {
        UpdateFocusPoint();
        UpdateTargetDistance(zoomInput);
        UpdateCharacterVisibility();
        Quaternion lookRotation = GetLookRotation(lookInputVector, ref _orbitAngles);
        Vector3 lookDirection = lookRotation * Vector3.forward;

        if (IsFirstPersonView)
            RotateInFirstPerson(lookRotation);
        else
            RotateInThirdPerson(lookDirection, lookRotation);
    }

    private void UpdateFocusPoint()
    {
        _previousFocusPoint = _focusPoint;
        Vector3 targetPoint = _target.position;
        if (_data.FocusRadius > 0f)
        {
            float distance = Vector3.Distance(targetPoint, _focusPoint);
            float t = 1f;
            if (distance > 0.01f && _data.FocusCentering > 0f)
            {
                t = Mathf.Pow(1f - _data.FocusCentering, Time.unscaledDeltaTime);
            }

            if (distance > _data.FocusRadius)
            {
                t = Mathf.Min(t, _data.FocusRadius / distance);
            }
            _focusPoint = Vector3.Lerp(targetPoint, _focusPoint, t);
        }
        else
        {
            _focusPoint = targetPoint;
        }
    }

    private void UpdateTargetDistance(float zoomInput)
    {
        _targetDistance += zoomInput * _data.ZoomSpeed * Time.unscaledDeltaTime;
        _targetDistance = Mathf.Max(_targetDistance, 0f);
        _targetDistance = Mathf.Min(_targetDistance, _data.MaxZoomDistance);
    }

    private void RotateInFirstPerson(Quaternion lookRotation)
    {
        transform.SetPositionAndRotation(_target.position, lookRotation);
    }

    private void RotateInThirdPerson(Vector3 lookDirection, Quaternion lookRotation)
    {
        Vector3 lookPosition = _focusPoint - lookDirection * _targetDistance;

        Vector3 rectOffset = lookDirection * _camera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;

        Vector3 castFrom = _target.position;
        Vector3 castTo = rectPosition - castFrom;
        float castDistance = castTo.magnitude;
        Vector3 castDirection = castTo / castDistance;

        if (Physics.BoxCast(
            castFrom, MathfUtil.CameraHalfExtents(_camera), castDirection, out RaycastHit hit,
            lookRotation, castDistance, _data.ObstructionMask))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    private void UpdateCharacterVisibility()
    {
        Debug.Assert(_characterRenderer);
        if (_characterRenderer == null)
            return;

        if (_targetDistance < _data.CharacterVisibilityThreshold)
            _characterRenderer.SetCastingModeToShadowsOnly();
        else
            _characterRenderer.SetCastingModeToDefault();
    }

    private Quaternion GetLookRotation(Vector2 lookInputVector, ref Vector2 orbitAngles)
    {
        if (TryAddManualRotation(lookInputVector, ref orbitAngles) ||
            TryAddAutomaticRotation(ref orbitAngles))
        {
            ConstrainAngles(ref orbitAngles);
            return Quaternion.Euler(orbitAngles);
        }

        return transform.localRotation;
    }

    private bool TryAddManualRotation(Vector2 lookInputVector, ref Vector2 orbitAngles)
    {
        const float e = 0.001f;

        if (Mathf.Abs(lookInputVector.x) < e || Mathf.Abs(lookInputVector.y) < e)
            return false;

        orbitAngles += lookInputVector * (_data.RotationSpeed * Time.unscaledDeltaTime);
        _lastManualRotationTime = Time.unscaledTime;
        return true;
    }

    private bool TryAddAutomaticRotation(ref Vector2 orbitAngles)
    {
        if (IsFirstPersonView)
            return false;

        if (Time.unscaledTime - _lastManualRotationTime < _data.AlignDelay)
            return false;

        Vector2 movement = new Vector2(
            _focusPoint.x - _previousFocusPoint.x,
            _focusPoint.z - _previousFocusPoint.z);

        float movementDeltaSqr = movement.sqrMagnitude;
        if (movementDeltaSqr < 0.0001f)
            return false;

        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
        float rotationChange = _data.RotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
        if (deltaAbs < _data.AlignSmoothRange)
        {
            rotationChange *= deltaAbs / _data.AlignSmoothRange;
        }
        else if (180f - deltaAbs < _data.AlignSmoothRange)
        {
            rotationChange *= (180f - deltaAbs) / _data.AlignSmoothRange;
        }
        orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);

        return true;
    }

    private void ConstrainAngles(ref Vector2 orbitAngles)
    {
        orbitAngles.x = Mathf.Clamp(orbitAngles.x, MinVerticalAngle, MaxVerticalAngle);

        if (orbitAngles.y < 0f)
        {
            orbitAngles.y += 360f;
        }
        else if (orbitAngles.y >= 360f)
        {
            orbitAngles.y -= 360f;
        }
    }

    private static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }
}