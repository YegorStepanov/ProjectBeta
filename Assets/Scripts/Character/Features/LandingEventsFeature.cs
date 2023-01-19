using KinematicCharacterController;

namespace Character.Features
{
    public class LandingEventsFeature
    {
        private readonly KinematicCharacterMotor _motor;

        public LandingEventsFeature(KinematicCharacterMotor motor)
        {
            _motor = motor;
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            if (_motor.GroundingStatus.IsStableOnGround && !_motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!_motor.GroundingStatus.IsStableOnGround && _motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLeaveStableGround();
            }
        }

        private static void OnLanded()
        {
            // Debug.Log("Landed");
        }

        private static void OnLeaveStableGround()
        {
            // Debug.Log("Left ground");
        }
    }
}