using System;
using UnityEngine;

namespace GlitchVerse.Player
{
    /// <summary>
    /// Compact fixed-step movement snapshot representing player physics state.
    /// Emitted and sampled per simulation tick for deterministic Echo recording.
    /// </summary>
    public struct PlayerMovementSnapshot
    {
        /// <summary>Simulation tick corresponding to this snapshot, provided by the simulation host.</summary>
        public uint SimulationTick;

        /// <summary>World-space position of the player's Rigidbody2D.</summary>
        public Vector2 WorldPosition;

        /// <summary>Current linear velocity of the player's Rigidbody2D.</summary>
        public Vector2 Velocity;

        /// <summary>Current facing direction (+1 for Right, -1 for Left).</summary>
        public sbyte FacingDirection;

        /// <summary>True if the player is currently detected as grounded.</summary>
        public bool IsGrounded;

        /// <summary>True if the player is actively executing an upward jump.</summary>
        public bool IsJumping;

        /// <summary>True if horizontal movement input is actively applied.</summary>
        public bool IsMoving;

        /// <summary>Horizontal movement input axis value (-1.0 to +1.0) applied during this tick.</summary>
        public float MovementInput;
    }

    /// <summary>
    /// Contract defining player movement capabilities, physics telemetry, and lifecycle resets.
    /// Implemented by PlayerController2D.
    /// </summary>
    public interface IPlayerController
    {
        /// <summary>Current world-space position of the player.</summary>
        Vector2 Position { get; }

        /// <summary>Current linear velocity of the player.</summary>
        Vector2 Velocity { get; }

        /// <summary>Facing direction (+1 for Right, -1 for Left).</summary>
        sbyte FacingDirection { get; }

        /// <summary>Whether the player is currently grounded on non-trigger ground geometry.</summary>
        bool IsGrounded { get; }

        /// <summary>When true, player movement input is locked (velocity X zeroed).</summary>
        bool IsMovementLocked { get; set; }

        /// <summary>
        /// Explicit unit vector defining player "up". Defaults to (0, 1).
        /// Allows external mechanics (e.g. gravity inversion) to set orientation explicitly
        /// without relying on inferring gravity direction from gravityScale alone.
        /// </summary>
        Vector2 UpDirection { get; set; }

        /// <summary>Unit vector pointing in the direction of gravitational pull (-UpDirection).</summary>
        Vector2 DownDirection { get; }

        /// <summary>Fired when grounded state transitions between true and false.</summary>
        event Action<bool> OnGroundedStateChanged;

        /// <summary>Fired immediately upon executing a valid jump impulse.</summary>
        event Action OnJumpExecuted;

        /// <summary>Fired when the player state is safely reset.</summary>
        event Action OnPlayerReset;

        /// <summary>Sets horizontal movement input axis clamped between -1 and +1.</summary>
        void SetMoveInput(float horizontalInput);

        /// <summary>Queues a jump request within the jump buffer window.</summary>
        void TriggerJump();

        /// <summary>Dampens upward jump velocity if the jump button is released early (variable jump height).</summary>
        void CutJump();

        /// <summary>Returns a zero-allocation movement snapshot for the specified simulation tick.</summary>
        PlayerMovementSnapshot GetCurrentSnapshot(uint simulationTick);

        /// <summary>
        /// Teleports player to spawnPosition, safely zeroes velocity and buffers, and resets internal bookkeeping.
        /// </summary>
        void ResetPlayerState(Vector2 spawnPosition, bool resetVelocity = true);
    }
}

