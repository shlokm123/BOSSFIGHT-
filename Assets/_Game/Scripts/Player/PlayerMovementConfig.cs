using UnityEngine;

namespace GlitchVerse.Player
{
    /// <summary>
    /// Configuration asset containing movement speeds, jump curves, and timing windows for PlayerController2D.
    /// Create via Assets > Create > GlitchVerse > Player > Movement Config.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerMovementConfig", menuName = "GlitchVerse/Player/Movement Config", order = 0)]
    public class PlayerMovementConfig : ScriptableObject
    {
        [Header("Horizontal Movement")]
        [Tooltip("Maximum horizontal run speed in units per second.")]
        public float maxRunSpeed = 8.0f;

        [Tooltip("Rate of acceleration towards desired horizontal speed.")]
        public float acceleration = 50.0f;

        [Tooltip("Rate of deceleration when input drops to zero.")]
        public float deceleration = 60.0f;

        [Header("Jump Physics")]
        [Tooltip("Initial upward impulse velocity applied for a full jump.")]
        public float jumpVelocity = 14.0f;

        [Tooltip("Multiplier applied to upward velocity when jump input is released early (variable jump height, 0 to 1).")]
        [Range(0f, 1f)]
        public float jumpCutMultiplier = 0.5f;

        [Header("Timing Windows (Seconds)")]
        [Tooltip("Grace period allowing a jump after walking off a platform edge.")]
        public float coyoteTime = 0.12f;

        [Tooltip("Window to queue a jump press before making contact with the ground.")]
        public float jumpBufferTime = 0.15f;

        [Header("Ground Detection")]
        [Tooltip("Dimensions of the ground detection overlap box.")]
        public Vector2 groundCheckSize = new Vector2(0.6f, 0.1f);

        [Tooltip("Distance from player center to ground check origin along DownDirection (used when groundCheckPoint is unassigned).")]
        public float groundCheckOffset = 0.8f;
    }
}

