using UnityEngine;

// todo: refactor it to a record struct when Unity supports it (C# 10 required)
public readonly struct PlayerCharacterInputs
{
    public readonly Vector3 MoveInputVector;
    public readonly Quaternion CameraRotation;
    public readonly bool JumpDown;
    public readonly bool JumpHeld;
    public readonly bool CrouchDown;
    public readonly bool CrouchUp;
    public readonly bool CrouchHeld;
    public readonly bool ChargingDown;
    public readonly bool NoClipDown;
    public readonly bool NoClipFlyUp;
    public readonly bool NoClipFlyDown;

    public PlayerCharacterInputs(
        Vector3 moveInputVector,
        Quaternion cameraRotation,
        bool jumpDown,
        bool jumpHeld,
        bool crouchDown,
        bool crouchUp,
        bool crouchHeld,
        bool chargingDown,
        bool noClipDown,
        bool noClipFlyUp,
        bool noClipFlyDown)
    {
        MoveInputVector = moveInputVector;
        CameraRotation = cameraRotation;
        JumpDown = jumpDown;
        JumpHeld = jumpHeld;
        CrouchDown = crouchDown;
        CrouchUp = crouchUp;
        CrouchHeld = crouchHeld;
        ChargingDown = chargingDown;
        NoClipDown = noClipDown;
        NoClipFlyUp = noClipFlyUp;
        NoClipFlyDown = noClipFlyDown;
    }
}