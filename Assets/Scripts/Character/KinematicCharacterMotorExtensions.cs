using KinematicCharacterController;
using UnityEngine;

public static class KinematicCharacterMotorExtensions
{
    public static int CharacterCollisionWithHitNormal(this KinematicCharacterMotor motor, out Vector3 averageHitNormal, Collider[] overlappedColliders, float inflateRadius = 0f, float inflateHeight = 0f)
    {
        float lastRadius = motor.Capsule.radius;
        float lastHeight = motor.Capsule.height;
        float lastYOffset = motor.Capsule.center.y;
        motor.SetCapsuleDimensions(lastRadius + inflateRadius, lastHeight + inflateHeight, lastYOffset);

        int hits = motor.CharacterCollisionsOverlap(motor.TransientPosition, motor.TransientRotation, overlappedColliders);

        averageHitNormal = Vector3.zero;
        if (hits != 0)
        {
            for (int i = 0; i < hits; i++)
            {
                Collider collider = overlappedColliders[i];
                Transform t = collider.transform;

                if (Physics.ComputePenetration(
                    motor.Capsule, motor.TransientPosition, motor.TransientRotation,
                    collider, t.position, t.rotation,
                    out Vector3 direction, out float distance))
                {
                    averageHitNormal += direction * distance;
                }
            }

            averageHitNormal = averageHitNormal.normalized;
        }

        motor.SetCapsuleDimensions(lastRadius, lastHeight, lastYOffset);
        return hits;
    }
}