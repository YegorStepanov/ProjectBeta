using KinematicCharacterController;
using UnityEngine;

namespace Character.Features
{
    public class CrouchingFeature
    {
        private readonly CrouchingData _data;
        private readonly KinematicCharacterMotor _motor;
        private readonly CustomCharacterController _controller;
        private readonly Vector3 _uncrouchCapsuleSize;
        private readonly Vector3 _uncrouchMeshSize;

        private bool _shouldBeCrouching;
        private bool _isCrouching;

        public CrouchingFeature(CrouchingData data, KinematicCharacterMotor motor, CustomCharacterController controller)
        {
            _data = data;
            _motor = motor;
            _controller = controller;

            _uncrouchCapsuleSize = GetCapsuleSize();
            _uncrouchMeshSize = GetMeshSize();
        }

        public void SetInputs(in PlayerCharacterInputs inputs)
        {
            if (inputs.CrouchDown)
            {
                _shouldBeCrouching = true;

                if (!_isCrouching)
                {
                    _isCrouching = true;
                    CrouchCapsule();
                    CrouchMesh();
                }
            }
            else if (inputs.CrouchUp)
            {
                _shouldBeCrouching = false;
            }
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            if (_isCrouching && !_shouldBeCrouching)
            {
                UncrouchCapsule();

                if (_motor.CharacterOverlap(
                    _motor.TransientPosition,
                    _motor.TransientRotation,
                    _controller.FreeColliders,
                    _motor.CollidableLayers,
                    QueryTriggerInteraction.Ignore) > 0)
                {
                    CrouchCapsule();
                }
                else
                {
                    UncrouchMesh();
                    _isCrouching = false;
                }
            }
        }

        private void CrouchCapsule() =>
            _motor.SetCapsuleDimensions(_data.CrouchCapsuleSize.x, _data.CrouchCapsuleSize.y, _data.CrouchCapsuleSize.z);

        private void UncrouchCapsule() =>
            _motor.SetCapsuleDimensions(_uncrouchCapsuleSize.x, _uncrouchCapsuleSize.y, _uncrouchCapsuleSize.z);

        private void CrouchMesh() =>
            _controller.MeshRoot.localScale = _data.CrouchMeshSize;

        private void UncrouchMesh() =>
            _controller.MeshRoot.localScale = _uncrouchMeshSize;

        private Vector3 GetMeshSize() =>
            _controller.MeshRoot.localScale;

        private Vector3 GetCapsuleSize()
        {
            CapsuleCollider capsuleCollider = _motor.Capsule;
            return new Vector3(capsuleCollider.radius, capsuleCollider.height, capsuleCollider.center.y);
        }
    }
}