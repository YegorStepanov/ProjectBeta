using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Orbit Camera Data", menuName = "ScriptableObjects/Orbit Camera Data")]
public class OrbitCameraData : ScriptableObject
{
    [field: Range(1.2f, 20f)]
    [field: SerializeField] public float Distance { get; private set; } = 3f;
    [field: Min(0f)]
    [field: SerializeField] public float FocusRadius { get; private set; } = 0.5f;
    [field: Range(0f, 1f)]
    [field: SerializeField] public float FocusCentering { get; private set; } = 0.75f;
    [field: Range(1f, 360f)]
    [field: SerializeField] public float RotationSpeed { get; private set; } = 90f;
    [field: Min(0f)]
    [field: SerializeField] public float AlignDelay { get; private set; } = 5f;
    [field: Range(0f, 90f)]
    [field: SerializeField] public float AlignSmoothRange { get; private set; } = 45f;
    [field: SerializeField] public LayerMask ObstructionMask { get; private set; } = -1;
    [field: Min(0f)]
    [field: SerializeField] public float ZoomSpeed { get; private set; } = 120f;
    [field: SerializeField] public float MaxZoomDistance { get; private set; } = 5f;
    [field: SerializeField] public float DefaultZoomDistance { get; private set; } = 3f;

    [field: SerializeField] public float CharacterVisibilityThreshold { get; private set; } = 1.2f;

    [field: SerializeField] public FirstPersonData FirstPerson = new FirstPersonData();
    [field: SerializeField] public ThirdPersonData ThirdPerson = new ThirdPersonData();

    [Serializable]
    public class FirstPersonData
    {
        [Range(-90f, 90f)]
        public float MinVerticalAngle = -90f;
        [Range(-90f, 90f)]
        public float MaxVerticalAngle = 90f;
    }

    [Serializable]
    public class ThirdPersonData
    {
        [Range(-90f, 90f)]
        public float MinVerticalAngle = -30f;
        [Range(-90f, 90f)]
        public float MaxVerticalAngle = 60f;
    }
}