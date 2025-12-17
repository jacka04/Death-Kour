using UnityEngine;

namespace PlayerSystem
{
    [CreateAssetMenu]
    public class ScriptableStats : ScriptableObject
    {
        [Header("LAYERS")]
        public LayerMask PlayerLayer;

        [Header("INPUT")]
        public bool SnapInput = true;
        public float VerticalDeadZoneThreshold = 0.3f;
        public float HorizontalDeadZoneThreshold = 0.1f;

        [Header("MOVEMENT")]
        public float MaxSpeed = 14;
        public float Acceleration = 120;
        public float GroundDeceleration = 60;
        public float AirDeceleration = 30;
        public float GroundingForce = -1.5f;
        public float GrounderDistance = 0.05f;

        [Header("JUMP")]
        public float JumpPower = 36;
        public float MaxFallSpeed = 40;
        public float FallAcceleration = 110;
        public float JumpEndEarlyGravityModifier = 3;
        public float CoyoteTime = .15f;
        public float JumpBuffer = .2f;

        [Header("DASH")]
        public float DashSpeed = 65f;
        public float DashDuration = 0.12f;
        public int MaxAirDashes = 1;
        public bool DashCancelsGravity = true;
        public bool DashAllowSteer = false;
    }
}