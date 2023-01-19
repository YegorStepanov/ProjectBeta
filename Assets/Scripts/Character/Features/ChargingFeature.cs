using KinematicCharacterController;
using UnityEngine;

namespace Character.Features
{
    public class ChargingFeature
    {
        private readonly ChargingData _data;
        private readonly KinematicCharacterMotor _motor;
        private readonly CustomCharacterController _controller;

        private Vector3 _currentChargeVelocity;
        private bool _isStopped;
        private bool _mustStopVelocity;
        private float _currentDuration;
        private float _timeSinceStopped;

        public ChargingFeature(ChargingData data, KinematicCharacterMotor motor, CustomCharacterController controller)
        {
            _data = data;
            _motor = motor;
            _controller = controller;
        }

        public void OnStateEnter()
        {
            if (_data.Enable)
            {
                SetDefaultState();
                return;
            }

            _currentChargeVelocity = _motor.CharacterForward * _data.Speed;
            _isStopped = false;
            _currentDuration = 0f;
            _timeSinceStopped = 0f;
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            _currentDuration += deltaTime;
            if (_isStopped)
            {
                _timeSinceStopped += deltaTime;
            }
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (!_data.Enable)
            {
                SetDefaultState();
                return;
            }

            if (_mustStopVelocity)
            {
                currentVelocity = Vector3.zero;
                _mustStopVelocity = false;
            }

            if (_isStopped)
            {
                // When stopped, add gravity only
                currentVelocity += _controller.Gravity * deltaTime;
            }
            else
            {
                // When charging, velocity is always constant
                currentVelocity.x = _currentChargeVelocity.x;
                currentVelocity.z = _currentChargeVelocity.z;
                currentVelocity += _controller.Gravity * deltaTime;
            }
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            if (!_isStopped && _currentDuration > _data.MaxDuration)
            {
                _mustStopVelocity = true;
                _isStopped = true;
            }

            if (_timeSinceStopped > _data.StoppedTime)
            {
                SetDefaultState();
            }
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            // Detect being stopped by obstructions
            bool isAngleLess30 = Vector3.Dot(-hitNormal, _currentChargeVelocity.normalized) > 0.5f;
            if (!_isStopped && !hitStabilityReport.IsStable && isAngleLess30)
            {
                _mustStopVelocity = true;
                _isStopped = true;
            }
        }

        private void SetDefaultState()
        {
            _controller.SetState(CharacterState.Default);
        }
    }
}