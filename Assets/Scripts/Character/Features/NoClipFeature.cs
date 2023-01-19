using UnityEngine;

namespace KinematicCharacterController.Character.Features
{
    public class NoClipFeature
    {
        private readonly NoClipData _data;
        private readonly KinematicCharacterMotor _motor;

        private bool _flyUp;
        private bool _flyDown;
        private Vector3 _moveInputDirection;

        public NoClipFeature(NoClipData data, KinematicCharacterMotor motor)
        {
            _data = data;
            _motor = motor;
        }

        public void OnStateEnter()
        {
            _motor.SetCapsuleCollisionsActivation(false);
            _motor.SetMovementCollisionsSolvingActivation(false);
            _motor.SetGroundSolvingActivation(false);
        }

        public void OnStateExit()
        {
            _motor.SetCapsuleCollisionsActivation(true);
            _motor.SetMovementCollisionsSolvingActivation(true);
            _motor.SetGroundSolvingActivation(true);
        }

        public void SetInputs(in PlayerCharacterInputs inputs)
        {
            _flyUp = inputs.NoClipFlyUp;
            _flyDown = inputs.NoClipFlyDown;

            _moveInputDirection = inputs.CameraRotation * inputs.MoveInputVector;
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (!_data.Enable) return;

            float verticalInput = (_flyUp ? 1f : 0f) + (_flyDown ? -1f : 0f);
            Vector3 verticalVelocity = _motor.CharacterUp * verticalInput;

            Vector3 targetVelocity = (_moveInputDirection + verticalVelocity).normalized * _data.Speed;
            currentVelocity = MathfUtil.Smooth(currentVelocity, targetVelocity, _data.SpeedSharpness * deltaTime);
        }
    }
}