using UnityEngine;

namespace Character.Features
{
    public class ExternalForceFeature
    {
        private Vector3 _externalVelocity;

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (_externalVelocity.sqrMagnitude > 0f)
            {
                currentVelocity += _externalVelocity;
                _externalVelocity = Vector3.zero;
            }
        }

        public void AddVelocity(Vector3 velocity)
        {
            _externalVelocity += velocity;
        }
    }
}