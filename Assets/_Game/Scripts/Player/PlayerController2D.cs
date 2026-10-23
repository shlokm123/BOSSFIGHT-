using System;
using UnityEngine;

namespace GlitchVerse.Player
{
    /// <summary>
    /// Physics-based 2D player controller utilizing Rigidbody2D.
    /// Handles horizontal movement, grounded jumping, coyote time, jump buffering,
    /// variable jump height, zero-allocation ground detection, and safe state resets.
    /// Ready for explicit gravity direction modification without relying on gravityScale alone.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController2D : MonoBehaviour, IPlayerController
    {
        [Header("Configuration")]
        [Tooltip("Serialized movement configuration asset containing physics and timing parameters.")]
        [SerializeField] private PlayerMovementConfig config;

        [Header("Ground Detection")]
        [Tooltip("Optional child transform marking the ground check center. If null, automatically calculated via groundCheckOffset along DownDirection.")]
        [SerializeField] private Transform groundCheckPoint;

        [Tooltip("LayerMask identifying walkable surfaces. Must exclude the Player layer.")]
        [SerializeField] private LayerMask groundLayerMask;

        [Header("Physics References")]
        [Tooltip("Rigidbody2D component driving the player. If unassigned, automatically acquired via GetComponent.")]
        [SerializeField] private Rigidbody2D rb2d;

        [Tooltip("Primary player collider to exclude from ground detection. If unassigned, automatically acquired via GetComponent.")]
        [SerializeField] private Collider2D playerCollider;

        // Gravity Orientation
        private Vector2 _upDirection = Vector2.up;

        // Input & State Tracking
        private float _horizontalInput;
        private sbyte _facingDirection = 1;
        private bool _isGrounded;
        private bool _wasGroundedLastFrame;
        private bool _isJumping;
        private bool _isMovementLocked;

        // Jump Timing Timers
        private float _coyoteTimer;
        private float _jumpBufferTimer;

        // Preallocated buffer & contact filter for zero-allocation Physics2D queries
        private readonly Collider2D[] _groundHitBuffer = new Collider2D[8];
        private ContactFilter2D _groundFilter;

        #region IPlayerController Properties

        /// <inheritdoc />
        public Vector2 Position => rb2d != null ? rb2d.position : (Vector2)transform.position;

        /// <inheritdoc />
        public Vector2 Velocity => rb2d != null ? rb2d.linearVelocity : Vector2.zero;

        /// <inheritdoc />
        public sbyte FacingDirection => _facingDirection;

        /// <inheritdoc />
        public bool IsGrounded => _isGrounded;

        /// <inheritdoc />
        public bool IsMovementLocked
        {
            get => _isMovementLocked;
            set
            {
                _isMovementLocked = value;
                if (_isMovementLocked && rb2d != null)
                {
                    rb2d.linearVelocity = new Vector2(0f, rb2d.linearVelocity.y);
                    _horizontalInput = 0f;
                }
            }
        }

        /// <inheritdoc />
        public Vector2 UpDirection
        {
            get => _upDirection;
            set
            {
                if (value.sqrMagnitude > 0.0001f)
                {
                    _upDirection = value.normalized;
                }
            }
        }

        /// <inheritdoc />
        public Vector2 DownDirection => -_upDirection;

        #endregion

        #region Events

        /// <inheritdoc />
        public event Action<bool> OnGroundedStateChanged;

        /// <inheritdoc />
        public event Action OnJumpExecuted;

        /// <inheritdoc />
        public event Action OnPlayerReset;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Validate movement configuration
            if (config == null)
            {
                Debug.LogError($"[{nameof(PlayerController2D)}] CRITICAL: Missing '{nameof(PlayerMovementConfig)}' on '{gameObject.name}'. " +
                               "Please create a config asset via Assets > Create > GlitchVerse > Player > Movement Config and assign it in the Inspector.", this);
                enabled = false;
                return;
            }

            // Acquire and validate Rigidbody2D
            if (rb2d == null && !TryGetComponent(out rb2d))
            {
                Debug.LogError($"[{nameof(PlayerController2D)}] CRITICAL: Missing Rigidbody2D component on '{gameObject.name}'.", this);
                enabled = false;
                return;
            }

            // Acquire collider for self-exclusion
            if (playerCollider == null)
            {
                playerCollider = GetComponent<Collider2D>();
            }

            // Initialize non-allocating ContactFilter2D
            _groundFilter = new ContactFilter2D();
            _groundFilter.SetLayerMask(groundLayerMask);
            _groundFilter.useLayerMask = true;
            _groundFilter.useTriggers = false; // Never treat trigger volumes as ground
        }

        private void Update()
        {
            UpdateTimers();
        }

        private void FixedUpdate()
        {
            CheckGroundedStatus();
            ProcessJumpQueue();
            ApplyHorizontalMovement();
        }

        #endregion

        #region Input Commands

        /// <inheritdoc />
        public void SetMoveInput(float horizontalInput)
        {
            if (_isMovementLocked)
            {
                _horizontalInput = 0f;
                return;
            }

            _horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);

            // Update facing direction on non-negligible input
            if (_horizontalInput > 0.05f)
            {
                _facingDirection = 1;
            }
            else if (_horizontalInput < -0.05f)
            {
                _facingDirection = -1;
            }
        }

        /// <inheritdoc />
        public void TriggerJump()
        {
            if (_isMovementLocked) return;

            // Register jump request within buffer window
            _jumpBufferTimer = config.jumpBufferTime;
        }

        /// <inheritdoc />
        public void CutJump()
        {
            if (_isMovementLocked || rb2d == null) return;

            // Dampen upward velocity if player is moving in the UpDirection during an active jump
            float upwardSpeed = Vector2.Dot(rb2d.linearVelocity, _upDirection);
            if (upwardSpeed > 0f && _isJumping)
            {
                Vector2 perpendicularVel = rb2d.linearVelocity - (upwardSpeed * _upDirection);
                float dampenedUpwardSpeed = upwardSpeed * config.jumpCutMultiplier;
                rb2d.linearVelocity = perpendicularVel + (_upDirection * dampenedUpwardSpeed);
                _isJumping = false;
            }
        }

        #endregion

        #region Internal Physics & Timing Logic

        /// <summary>
        /// Decrements timing buffers. Called in per-frame Update.
        /// </summary>
        private void UpdateTimers()
        {
            // Coyote timer: active while grounded; decrements upon becoming airborne without jump
            if (_isGrounded)
            {
                _coyoteTimer = config.coyoteTime;
            }
            else if (_coyoteTimer > 0f)
            {
                _coyoteTimer -= Time.deltaTime;
            }

            // Jump buffer timer countdown
            if (_jumpBufferTimer > 0f)
            {
                _jumpBufferTimer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Evaluates non-trigger ground contacts, excluding the player's own collider.
        /// Executed in FixedUpdate before jump processing.
        /// </summary>
        private void CheckGroundedStatus()
        {
            Vector2 checkOrigin = groundCheckPoint != null
                ? (Vector2)groundCheckPoint.position
                : (Vector2)transform.position + (DownDirection * config.groundCheckOffset);

            int hitCount = Physics2D.OverlapBox(checkOrigin, config.groundCheckSize, 0f, _groundFilter, _groundHitBuffer);

            bool validGroundFound = false;
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = _groundHitBuffer[i];
                if (hit != null && hit != playerCollider && hit.attachedRigidbody != rb2d && !hit.isTrigger)
                {
                    validGroundFound = true;
                    break;
                }
            }

            _isGrounded = validGroundFound;

            // Detect landing: when grounded and not traveling upwards along UpDirection
            float upwardSpeed = Vector2.Dot(rb2d.linearVelocity, _upDirection);
            if (_isGrounded && upwardSpeed <= 0.01f)
            {
                _isJumping = false;
            }

            // Fire event on state change
            if (_isGrounded != _wasGroundedLastFrame)
            {
                OnGroundedStateChanged?.Invoke(_isGrounded);
                _wasGroundedLastFrame = _isGrounded;
            }
        }

        /// <summary>
        /// Evaluates whether a buffered or immediate jump can be executed.
        /// Guarantees that one jump press produces at most one jump impulse.
        /// </summary>
        private void ProcessJumpQueue()
        {
            if (_isMovementLocked) return;

            // Execute jump if buffered request exists, not already jumping, AND (grounded OR within coyote window)
            if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
            {
                ExecuteJump();
            }
        }

        /// <summary>
        /// Applies jump velocity along UpDirection, consuming buffers and resetting grounded state.
        /// </summary>
        private void ExecuteJump()
        {
            _isJumping = true;
            _jumpBufferTimer = 0f; // Consume buffer immediately
            _coyoteTimer = 0f;     // Consume coyote window so air jumps are impossible
            _isGrounded = false;

            // Apply upward velocity along UpDirection, canceling any residual downward velocity
            Vector2 currentVelocity = rb2d.linearVelocity;
            float upwardSpeed = Vector2.Dot(currentVelocity, _upDirection);
            if (upwardSpeed < 0f)
            {
                currentVelocity -= upwardSpeed * _upDirection;
            }

            rb2d.linearVelocity = currentVelocity + (_upDirection * config.jumpVelocity);

            OnJumpExecuted?.Invoke();
        }

        /// <summary>
        /// Computes horizontal acceleration/deceleration along the X axis.
        /// Preserves vertical velocity components along the UpDirection.
        /// </summary>
        private void ApplyHorizontalMovement()
        {
            if (rb2d == null) return;

            float targetX = _horizontalInput * config.maxRunSpeed;
            float currentX = rb2d.linearVelocity.x;

            float rate = Mathf.Abs(targetX) > 0.01f ? config.acceleration : config.deceleration;
            float newX = Mathf.MoveTowards(currentX, targetX, rate * Time.fixedDeltaTime);

            rb2d.linearVelocity = new Vector2(newX, rb2d.linearVelocity.y);
        }

        #endregion

        #region Telemetry & Lifecycle Resets

        /// <inheritdoc />
        public PlayerMovementSnapshot GetCurrentSnapshot(uint simulationTick)
        {
            return new PlayerMovementSnapshot
            {
                SimulationTick = simulationTick,
                WorldPosition = rb2d != null ? rb2d.position : (Vector2)transform.position,
                Velocity = rb2d != null ? rb2d.linearVelocity : Vector2.zero,
                FacingDirection = _facingDirection,
                IsGrounded = _isGrounded,
                IsJumping = _isJumping,
                IsMoving = Mathf.Abs(_horizontalInput) > 0.01f,
                MovementInput = _horizontalInput
            };
        }

        /// <inheritdoc />
        public void ResetPlayerState(Vector2 spawnPosition, bool resetVelocity = true)
        {
            // 1. Safe Physics Teleport & Velocity Zeroing
            if (rb2d != null)
            {
                if (resetVelocity)
                {
                    rb2d.linearVelocity = Vector2.zero;
                    rb2d.angularVelocity = 0f;
                }

                rb2d.position = spawnPosition;
                rb2d.Sleep(); // Flush internal contacts and interpolation caches
                rb2d.WakeUp();
            }

            transform.position = spawnPosition;

            // 2. Clear All Inputs, Timers, and Bookkeeping
            _horizontalInput = 0f;
            _coyoteTimer = 0f;
            _jumpBufferTimer = 0f;
            _isJumping = false;
            _isGrounded = false;
            _wasGroundedLastFrame = false;
            _facingDirection = 1;
            _isMovementLocked = false;
            _upDirection = Vector2.up; // Reset to default upward orientation

            OnPlayerReset?.Invoke();
        }

        #endregion

        #region Editor Visualization

        private void OnDrawGizmosSelected()
        {
            if (config == null) return;

            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Vector2 checkOrigin = groundCheckPoint != null
                ? (Vector2)groundCheckPoint.position
                : (Vector2)transform.position + (DownDirection * config.groundCheckOffset);

            Gizmos.DrawWireCube(checkOrigin, config.groundCheckSize);
        }

        #endregion
    }
}

