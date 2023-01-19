using KinematicCharacterController;
using UnityEngine;

namespace Character.Features
{
    public class DragFeature
    {
        private readonly DragData _data;
        private readonly KinematicCharacterMotor _motor;

        public DragFeature(DragData data, KinematicCharacterMotor motor)
        {
            _data = data;
            _motor = motor;
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (!_motor.GroundingStatus.IsStableOnGround)
            {
                // It differs from Unity implementation (velocity *= 1 - drag * deltaTime)
                currentVelocity *= 1f / (1f + _data.Drag * deltaTime);
            }
        }
    }
}