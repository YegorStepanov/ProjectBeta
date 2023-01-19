using KinematicCharacterController;
using UnityEngine;

namespace Character.Features
{
    public class GravityFeature
    {
        private readonly GravityData _data;
        private readonly KinematicCharacterMotor _motor;

        public Vector3 Gravity => _data.Gravity;

        public GravityFeature(GravityData data, KinematicCharacterMotor motor)
        {
            _data = data;
            _motor = motor;
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (!_data.Enable) return;

            if (_data.OrientTowardsGravity)
            {
                Vector3 characterUp = currentRotation * Vector3.up;
                currentRotation = Quaternion.FromToRotation(characterUp, -_data.Gravity) * currentRotation;
            }
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (!_data.Enable) return;

            if (!_motor.GroundingStatus.IsStableOnGround)
            {
                currentVelocity += _data.Gravity * deltaTime;
            }
        }
    }
}