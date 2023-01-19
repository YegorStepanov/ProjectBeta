using System;
using System.Collections.Generic;
using KinematicCharacterController;
using KinematicCharacterController.Character.Features;
using Character.Features;
using JetBrains.Annotations;
using UnityEngine;

public enum CharacterState
{
    Default,
    Charging,
    NoClip,
}

public sealed class CustomCharacterController : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor _motor;
    [SerializeField] private Transform _meshRoot;
    [SerializeField] private CustomCharacterControllerData _data;

    private MovementFeature _movementFeature;
    private RotationFeature _rotationFeature;
    private DragFeature _dragFeature;
    private GravityFeature _gravityFeature;
    private JumpingFeature _jumpingFeature;
    private ChargingFeature _chargingFeature;
    private CrouchingFeature _crouchingFeature;
    private ExternalForceFeature _externalForceFeature;
    private LandingEventsFeature _landingEventsFeature;
    private NoClipFeature _noClipFeature;

    public CharacterState CurrentState { get; private set; }
    public Vector3 Gravity => _gravityFeature.Gravity;
    public Transform MeshRoot => _meshRoot;

    public float MovementResistance => _jumpingFeature.MovementResistance;

    [NonSerialized] public Collider[] FreeColliders = new Collider[16];
    private List<Collider> _ignoredColliders = new List<Collider>(16);

    private void Awake()
    {
        FreeColliders = new Collider[16];

        _movementFeature = new MovementFeature(_data.MovementData, _motor, this);
        _rotationFeature = new RotationFeature(_data.RotationData, _motor, this);
        _dragFeature = new DragFeature(_data.DragData, _motor);
        _gravityFeature = new GravityFeature(_data.GravityData, _motor);
        _jumpingFeature = new JumpingFeature(_data.JumpingData, _motor, this);
        _chargingFeature = new ChargingFeature(_data.ChargingData, _motor, this);
        _crouchingFeature = new CrouchingFeature(_data.CrouchingData, _motor, this);
        _externalForceFeature = new ExternalForceFeature();
        _landingEventsFeature = new LandingEventsFeature(_motor);
        _noClipFeature = new NoClipFeature(_data.NoClipData, _motor);

        _ignoredColliders.AddRange(GetComponentsInChildren<Collider>());
    }

    private void Start()
    {
        _motor.CharacterController = this;
        SetState(CharacterState.Default);
    }

    public void SetState(CharacterState newState)
    {
        CharacterState lastState = CurrentState;
        OnStateExit(CurrentState);
        CurrentState = newState;
        OnStateEnter(CurrentState);
    }

    private void OnStateEnter(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.Default:
                break;
            case CharacterState.Charging:
                _chargingFeature.OnStateEnter();
                break;
            case CharacterState.NoClip:
                _noClipFeature.OnStateEnter();
                break;
        }
    }

    private void OnStateExit(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.Default:
                break;
            case CharacterState.NoClip:
                _noClipFeature.OnStateExit();
                break;
            case CharacterState.Charging:
                break;
        }
    }

    public void SetInputs(in PlayerCharacterInputs inputs)
    {
        if (inputs.NoClipDown)
        {
            switch (CurrentState)
            {
                case CharacterState.Default:
                    SetState(CharacterState.NoClip);
                    break;
                case CharacterState.NoClip:
                    SetState(CharacterState.Default);
                    break;
                case CharacterState.Charging:
                default:
                    break;
            }
        }
        if (inputs.ChargingDown)
        {
            SetState(CharacterState.Charging);
        }

        _rotationFeature.SetInputs(in inputs);

        switch (CurrentState)
        {
            case CharacterState.Default:
            {
                _movementFeature.SetInputs(in inputs);
                _jumpingFeature.SetInputs(in inputs);
                _crouchingFeature.SetInputs(in inputs);
                break;
            }
            case CharacterState.NoClip:
                _noClipFeature.SetInputs(in inputs);
                break;
            case CharacterState.Charging:
            default:
                break;
        }
    }

    #region ICharacterController
    public void BeforeCharacterUpdate(float deltaTime)
    {
        switch (CurrentState)
        {
            case CharacterState.Charging:
                _chargingFeature.BeforeCharacterUpdate(deltaTime);
                break;
            case CharacterState.Default:
            case CharacterState.NoClip:
            default:
                break;
        }
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        switch (CurrentState)
        {
            case CharacterState.Default:
            case CharacterState.NoClip:
                _rotationFeature.UpdateRotation(ref currentRotation, deltaTime);
                _gravityFeature.UpdateRotation(ref currentRotation, deltaTime);
                break;
            case CharacterState.Charging:
            default:
                break;
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        switch (CurrentState)
        {
            case CharacterState.Default:
                _movementFeature.UpdateVelocity(ref currentVelocity, deltaTime);
                _jumpingFeature.UpdateVelocity(ref currentVelocity, deltaTime);
                _gravityFeature.UpdateVelocity(ref currentVelocity, deltaTime);
                _dragFeature.UpdateVelocity(ref currentVelocity, deltaTime);
                _externalForceFeature.UpdateVelocity(ref currentVelocity, deltaTime);
                break;
            case CharacterState.Charging:
                _chargingFeature.UpdateVelocity(ref currentVelocity, deltaTime);
                break;
            case CharacterState.NoClip:
                _noClipFeature.UpdateVelocity(ref currentVelocity, deltaTime);
                break;
            default:
                break;
        }
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        switch (CurrentState)
        {
            case CharacterState.Default:
                _jumpingFeature.AfterCharacterUpdate(deltaTime);
                _crouchingFeature.AfterCharacterUpdate(deltaTime);
                break;
            case CharacterState.Charging:
                _chargingFeature.AfterCharacterUpdate(deltaTime);
                break;
            case CharacterState.NoClip:
            default:
                break;
        }
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return !_ignoredColliders.Contains(coll);
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        switch (CurrentState)
        {
            case CharacterState.Charging:
                _chargingFeature.OnMovementHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
                break;
            case CharacterState.Default:
            case CharacterState.NoClip:
            default:
                break;
        }
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }

    public void PostGroundingUpdate(float deltaTime)
    {
        _landingEventsFeature.PostGroundingUpdate(deltaTime);
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider) { }
    #endregion

    [PublicAPI]
    public void AddVelocity(Vector3 velocity)
    {
        if (CurrentState == CharacterState.Default)
        {
            _externalForceFeature.AddVelocity(velocity);
        }
    }
}