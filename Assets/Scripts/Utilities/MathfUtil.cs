using UnityEngine;

public static class MathfUtil
{
    public static Vector3 ProjectCameraOnPlane(Quaternion rotation, Vector3 upwards)
    {
        Vector3 planarDirection = Vector3.ProjectOnPlane(rotation * Vector3.forward, upwards).normalized;
        if (planarDirection.sqrMagnitude == 0f)
        {
            planarDirection = Vector3.ProjectOnPlane(rotation * Vector3.up, upwards).normalized;
        }
        return planarDirection;
    }

    // Like the `EaseOutExpo` function but without the `10` modifier
    public static Vector3 Smooth(Vector3 a, Vector3 b, float sharpnessByDeltaTime)
    {
        float t = 1 - Mathf.Exp(-sharpnessByDeltaTime);
        return Vector3.Lerp(a, b, t);
    }

    public static Vector3 SphericalSmooth(Vector3 a, Vector3 b, float sharpness)
    {
        float t = 1 - Mathf.Exp(-sharpness);
        return Vector3.Slerp(a, b, t);
    }

    public static Quaternion LookRotationUp(Vector3 newUpwards, Vector3 forward)
    {
        return Quaternion.LookRotation(newUpwards, -forward) * Quaternion.AngleAxis(90f, Vector3.right);
    }

    public static Vector3 CameraHalfExtents(Camera camera)
    {
        Vector3 halfExtends;
        halfExtends.y = camera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView);
        halfExtends.x = halfExtends.y * camera.aspect;
        halfExtends.z = 0f;
        return halfExtends;
    }
}