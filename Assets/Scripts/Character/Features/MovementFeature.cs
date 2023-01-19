using KinematicCharacterController;
using UnityEngine;

namespace Character.Features
{
    public class MovementFeature
    {
        private readonly MovementData _data;
        private readonly KinematicCharacterMotor _motor;
        private readonly CustomCharacterController _controller;

        private Vector3 _moveInputVector;

        public MovementFeature(MovementData data, KinematicCharacterMotor motor, CustomCharacterController controller)
        {
            _data = data;
            _motor = motor;
            _controller = controller;
        }

        public void SetInputs(in PlayerCharacterInputs inputs)
        {
            Vector3 cameraDirection = _data.WorldSpaceInput
                ? Vector3.forward
                : MathfUtil.ProjectCameraOnPlane(inputs.CameraRotation, _motor.CharacterUp);
            Quaternion cameraRotation = Quaternion.LookRotation(cameraDirection, _motor.CharacterUp);

            _moveInputVector = cameraRotation * inputs.MoveInputVector;
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (_motor.GroundingStatus.IsStableOnGround)
            {
                HandleStableMovement(ref currentVelocity, deltaTime);
            }
            else
            {
                HandleUnstableMovement(ref currentVelocity, deltaTime);
            }
        }

        private void HandleStableMovement(ref Vector3 currentVelocity, float deltaTime)
        {
            if (!_data.Stable.Enable) return;

            currentVelocity = _motor.GetDirectionTangentToSurface(currentVelocity, _motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

            Vector3 targetVelocity = _motor.GetDirectionTangentToSurface(_moveInputVector, _motor.GroundingStatus.GroundNormal) * _data.Stable.MaxSpeed;

            currentVelocity = MathfUtil.Smooth(currentVelocity, targetVelocity, _data.Stable.Sharpness * deltaTime);
        }

        private void HandleUnstableMovement(ref Vector3 currentVelocity, float deltaTime)
        {
            if (!_data.Unstable.Enable) return;

            if (_moveInputVector.sqrMagnitude > 0f)
            {
                Vector3 targetVelocity = _moveInputVector * _data.Unstable.MaxSpeed;

                // Prevent climbing on unstable slopes with air movement
                if (_motor.GroundingStatus.FoundAnyGround)
                {
                    Vector3 perpendicularObstructionNormal = _motor.GetDirectionTangentToSurface(_motor.GroundingStatus.GroundNormal, _motor.CharacterUp);
                    targetVelocity = Vector3.ProjectOnPlane(targetVelocity, perpendicularObstructionNormal);
                }

                Vector3 velocityChange = Vector3.ProjectOnPlane(targetVelocity - currentVelocity, _controller.Gravity);
                velocityChange += velocityChange * (_data.Unstable.Acceleration * deltaTime);
                velocityChange *= 1f - _controller.MovementResistance;
                currentVelocity += velocityChange;
            }
        }
    }
}