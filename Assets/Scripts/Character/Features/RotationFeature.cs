using KinematicCharacterController;
using UnityEngine;

namespace Character.Features
{
    public class RotationFeature
    {
        private readonly RotationData _data;
        private readonly KinematicCharacterMotor _motor;
        private readonly CustomCharacterController _controller;

        private Vector3 _lookInputVector;
        private Vector3 _lastMoveDirection = Vector3.forward;

        public RotationFeature(RotationData data, KinematicCharacterMotor motor, CustomCharacterController controller)
        {
            _data = data;
            _motor = motor;
            _controller = controller;
        }

        public void SetInputs(in PlayerCharacterInputs inputs)
        {
            Vector3 cameraPlanarDirection = MathfUtil.ProjectCameraOnPlane(inputs.CameraRotation, _motor.CharacterUp);
            _lookInputVector = cameraPlanarDirection;

            Vector3 cameraDirection = MathfUtil.ProjectCameraOnPlane(inputs.CameraRotation, _motor.CharacterUp);
            Quaternion cameraRotation = MathfUtil.LookRotationUp(_motor.CharacterUp, cameraDirection);
            Vector3 moveInputVector = cameraRotation * inputs.MoveInputVector;

            if (moveInputVector != Vector3.zero)
            {
                _lastMoveDirection = moveInputVector.normalized;
            }
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            var oldForward = currentRotation * Vector3.forward;

            if (_data.OrientTowardsInput.Enable || _data.OrientTowardsMovement.Enable)
            {
                currentRotation = Quaternion.LookRotation(_lookInputVector, _motor.CharacterUp);
            }

            if (_data.OrientTowardsGround.Enable)
            {
                if (_motor.GroundingStatus is { FoundAnyGround: true, IsStableOnGround: true })
                {
                    if (_motor.BaseVelocity != Vector3.zero)
                    {
                        for (int i = 0; i < _data.OrientTowardsGround.MaxRotationIterations; i++)
                        {
                            Vector3 characterUp = currentRotation * Vector3.up;

                            Vector3 groundNormal = CalculateDepenetrationNormal(characterUp, currentRotation);
                            if (groundNormal == Vector3.zero)
                                break;

                            Quaternion rotationTowardsGround = Quaternion.FromToRotation(characterUp, groundNormal);

                            float maxSpeed = _data.OrientTowardsGround.MaxDegreeChange / _data.OrientTowardsGround.MaxRotationIterations * deltaTime;
                            currentRotation = Quaternion.RotateTowards(currentRotation, rotationTowardsGround, maxSpeed);
                        }
                    }
                }
            }

            if (_data.OrientTowardsMovement.Enable)
            {
                Vector3 direction = Vector3.Slerp(oldForward, _lastMoveDirection, _data.OrientTowardsMovement.Sharpness * deltaTime);
                currentRotation = MathfUtil.LookRotationUp(currentRotation * Vector3.up, direction);
            }
        }

        private Vector3 CalculateDepenetrationNormal(Vector3 characterUp, Quaternion currentRotation)
        {
            Vector3 overlapPosition = _motor.TransientPosition + characterUp * _data.OrientTowardsGround.VerticalOffsetForOverlapPosition;
            int overlapCount = OverlapSphere(overlapPosition, _motor.Capsule.radius, _controller.FreeColliders, _motor.Capsule);

            Vector3 depenetrationNormal = Vector3.zero;
            for (int i = 0; i < overlapCount; i++)
            {
                Collider collider = _controller.FreeColliders[i];
                Transform colliderTransform = collider.transform;

                Vector3 depenetrationPosition = _motor.TransientPosition + characterUp * _data.OrientTowardsGround.VerticalOffsetForDepenetration;

                if (Physics.ComputePenetration(
                    _motor.Capsule, depenetrationPosition, currentRotation,
                    collider, colliderTransform.position, colliderTransform.rotation,
                    out Vector3 direction, out float distance))
                {
                    if (Vector3.Angle(characterUp, direction) < _motor.MaxStableSlopeAngle)
                    {
                        depenetrationNormal += direction * distance;
                    }
                }
            }

            return depenetrationNormal.normalized;
        }

        private static int OverlapSphere(Vector3 position, float radius, Collider[] overlappedColliders, Collider exclude)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(position, radius, overlappedColliders);

            for (int i = hitCount - 1; i >= 0; i--)
            {
                if (overlappedColliders[i] == exclude)
                {
                    overlappedColliders[i] = default;
                    int lastIndex = hitCount - 1;
                    ObjectUtil.Swap(ref overlappedColliders[i], ref overlappedColliders[lastIndex]);
                    hitCount--;
                    break;
                }
            }

            return hitCount;
        }
    }
}