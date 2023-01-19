using KinematicCharacterController;
using UnityEngine;

namespace Character.Features
{
    public class JumpingFeature
    {
        private readonly JumpingData _data;
        private readonly KinematicCharacterMotor _motor;
        private readonly CustomCharacterController _controller;

        private bool _jumpRequested;
        private bool _jumpConsumed;
        private bool _doubleJumpConsumed;
        private bool _jumpedThisFrame;
        private bool _canWallJump;
        private Vector3 _wallJumpNormal;
        private float _wallJumpTime;

        private bool OnJumpingGround => _data.EnableSlopeJumping ? _motor.GroundingStatus.FoundAnyGround : _motor.GroundingStatus.IsStableOnGround;

        public float MovementResistance { get; private set; }

        public JumpingFeature(JumpingData data, KinematicCharacterMotor motor, CustomCharacterController controller)
        {
            _data = data;
            _motor = motor;
            _controller = controller;
        }

        public void SetInputs(in PlayerCharacterInputs inputs)
        {
            if (inputs.JumpDown)
            {
                _jumpRequested = true;
            }
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (!_data.Enable) return;

            _jumpedThisFrame = false;

            if (_data.WallJump.Enable)
            {
                if (!_motor.GroundingStatus.FoundAnyGround)
                {
                    int hits = _motor.CharacterCollisionWithHitNormal(out Vector3 averageHitNormal, _controller.FreeColliders, _data.WallJump.InflateRadius);
                    if (hits != 0)
                    {
                        _wallJumpNormal = Vector3.Lerp(averageHitNormal, _motor.CharacterUp, _data.WallJump.UpwardContribution);
                        _canWallJump = true;
                    }
                }
            }

            if (_jumpRequested)
            {
                if (_data.DoubleJump.Enable)
                {
                    if (!_canWallJump && _jumpConsumed && !_doubleJumpConsumed && !OnJumpingGround)
                    {
                        _motor.ForceUnground();
                        AddJumpVelocity(ref currentVelocity, _motor.CharacterUp);

                        _jumpRequested = false;
                        _doubleJumpConsumed = true;
                        _jumpedThisFrame = true;
                        _wallJumpTime = Time.time;
                    }
                }

                if (_canWallJump || (!_jumpConsumed && OnJumpingGround))
                {
                    Vector3 jumpDirection = _motor.CharacterUp;
                    if (_canWallJump)
                    {
                        _wallJumpTime = Time.time; //todo rename
                        jumpDirection = _wallJumpNormal;
                    }
                    else if (_motor.GroundingStatus.FoundAnyGround && !_motor.GroundingStatus.IsStableOnGround)
                    {
                        jumpDirection = _motor.GroundingStatus.GroundNormal;
                    }

                    _motor.ForceUnground();
                    AddJumpVelocity(ref currentVelocity, jumpDirection);

                    _jumpRequested = false;
                    _jumpConsumed = true;
                    _jumpedThisFrame = true;
                }
            }

            _canWallJump = false;
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            if (!_data.Enable) return;

            if (OnJumpingGround)
            {
                if (!_jumpedThisFrame)
                {
                    _doubleJumpConsumed = false;
                    _jumpConsumed = false;
                }
            }

            if (_motor.GroundingStatus.IsStableOnGround)
            {
                MovementResistance = 0f;
            }
            else
            {
                float elapsedTime = Time.time - _wallJumpTime;
                if (elapsedTime <= 1f)
                {
                    MovementResistance = _data.WallJump.MovementResistance.Evaluate(elapsedTime);
                }
            }
        }

        private void AddJumpVelocity(ref Vector3 currentVelocity, Vector3 jumpDirection)
        {
            currentVelocity += jumpDirection * _data.JumpHeight - Vector3.Project(currentVelocity, jumpDirection);
        }
    }
}