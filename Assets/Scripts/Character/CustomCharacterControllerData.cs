using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Custom Character Controller Data", menuName = "ScriptableObjects/Custom Character Controller Data")]
public class CustomCharacterControllerData : ScriptableObject
{
    [field: SerializeField] public MovementData MovementData { get; private set; }
    [field: SerializeField] public RotationData RotationData { get; private set; }
    [field: SerializeField] public DragData DragData { get; private set; }
    [field: SerializeField] public GravityData GravityData { get; private set; }
    [field: SerializeField] public JumpingData JumpingData { get; private set; }
    [field: SerializeField] public ChargingData ChargingData { get; private set; }
    [field: SerializeField] public NoClipData NoClipData { get; private set; }
    [field: SerializeField] public CrouchingData CrouchingData { get; private set; }
}

[Serializable]
public class MovementData
{
    public bool WorldSpaceInput = false;
    public StableData Stable;
    public UnstableData Unstable;

    [Serializable]
    public class StableData
    {
        public bool Enable = true;
        public float MaxSpeed = 5f;
        public float Sharpness = 15f;
    }

    [Serializable]
    public class UnstableData
    {
        [Header("In the air and on slopes")]
        public bool Enable = true;
        public float MaxSpeed = 3f;
        public float Acceleration = 5f;
    }
}

[Serializable]
public class RotationData
{
    public OrientTowardsInputData OrientTowardsInput;
    public OrientTowardsMovementData OrientTowardsMovement;
    public OrientTowardsGroundData OrientTowardsGround;

    [Serializable]
    public class OrientTowardsInputData
    {
        public bool Enable = false;
    }

    [Serializable]
    public class OrientTowardsMovementData
    {
        public bool Enable = true;
        public float Sharpness = 10;
    }

    [Serializable]
    public class OrientTowardsGroundData
    {
        public bool Enable = true;
        public float MaxDegreeChange = 60f;
        public float VerticalOffsetForOverlapPosition = 0.4f;
        public float VerticalOffsetForDepenetration = -0.07f;
        public int MaxRotationIterations = 8;
    }
}

[Serializable]
public class DragData
{
    public float Drag = 0.1f;
}

[Serializable]
public class GravityData
{
    public bool Enable = true;
    public bool OrientTowardsGravity = false;
    public Vector3 Gravity = new Vector3(0, -30f, 0);
}

[Serializable]
public class JumpingData
{
    public bool Enable = true;
    public bool EnableSlopeJumping = false;
    public float JumpHeight = 3f;

    public DoubleJumpData DoubleJump;
    public WallJumpData WallJump;

    [Serializable]
    public class DoubleJumpData
    {
        public bool Enable = true;
    }

    [Serializable]
    public class WallJumpData
    {
        public bool Enable = true;
        [Range(0f, 0.99f)]
        public float UpwardContribution = 0.66f;
        public float InflateRadius = 0.1f;
        public AnimationCurve MovementResistance;
    }
}

[Serializable]
public class ChargingData
{
    public bool Enable = true;
    public float Speed = 15f;
    public float MaxDuration = 1.5f;
    public float StoppedTime = 1f;
}

[Serializable]
public class NoClipData
{
    public bool Enable = true;
    public float Speed = 10f;
    public float SpeedSharpness = 15;
}

[Serializable]
public class CrouchingData
{
    public bool Enable = true;
    public Vector3 CrouchCapsuleSize = new Vector3(1f, 0.5f, 1f);
    [Header("radius, height, yOffset")]
    public Vector3 CrouchMeshSize = new Vector3(0.5f, 1f, 0.5f);
}