using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlitchVerse.Player
{
    /// <summary>
    /// Rigidbody2D-based 2D player controller with responsive movement,
    /// reliable ground checking, and jump dynamics.
    /// Exposes movement telemetry for the Echo recording system.
    /// Uses Unity's New Input System for compatibility with URP 2D template.
    /// Follows docs/INTERFACES.md specifications.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController2D : MonoBehaviour
    {
        [Header("Movement Tuning")]
        [SerializeField] private float moveSpeed = 6.5f;
        [SerializeField] private float jumpForce = 11.0f;
        [SerializeField] private float groundCheckRadius = 0.22f;
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private LayerMask groundLayers;

        [Header("Visual Representation")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Runtime state
        private Rigidbody2D rb;
        private Collider2D col;
        private float horizontalInput;
        private bool isGrounded;
        private bool facingRight = true;
        private bool isGravityInverted = false;
        private bool jumpRequested = false;

        public Vector2 Position => rb != null ? rb.position : (Vector2)transform.position;
        public Vector2 Velocity => rb != null ? rb.linearVelocity : Vector2.zero;
        public bool IsGrounded => isGrounded;
        public bool FacingRight => facingRight;
        public bool IsGravityInverted => isGravityInverted;
        public float HorizontalInput => horizontalInput;
        public bool InputLocked { get; set; } = false;

        public event Action OnJumped;
        public event Action OnLanded;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            // Ensure Rigidbody2D is configured for crisp platforming
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.freezeRotation = true;
        }

        public void SetGravityInverted(bool inverted)
        {
            isGravityInverted = inverted;
            if (rb != null)
            {
                rb.gravityScale = isGravityInverted ? -1f : 1f;
            }
            if (spriteRenderer != null)
            {
                spriteRenderer.flipY = isGravityInverted;
            }
        }

        private void Update()
        {
            if (InputLocked)
            {
                horizontalInput = 0f;
                return;
            }

            // Input gathering using Unity Input System
            float input = 0f;
            var kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input += 1f;

                if (kb.spaceKey.wasPressedThisFrame && isGrounded)
                {
                    jumpRequested = true;
                }
            }
            horizontalInput = input;

            // Handle visual sprite orientation
            if (horizontalInput > 0.05f && !facingRight)
            {
                Flip();
            }
            else if (horizontalInput < -0.05f && facingRight)
            {
                Flip();
            }
        }

        private void FixedUpdate()
        {
            CheckGround();

            // Horizontal movement
            Vector2 currentVelocity = rb.linearVelocity;
            currentVelocity.x = horizontalInput * moveSpeed;

            // Jump execution
            if (jumpRequested)
            {
                currentVelocity.y = isGravityInverted ? -jumpForce : jumpForce;
                jumpRequested = false;
                OnJumped?.Invoke();
            }

            rb.linearVelocity = currentVelocity;
        }

        private void CheckGround()
        {
            bool wasGrounded = isGrounded;

            Vector2 checkDir = isGravityInverted ? Vector2.up : Vector2.down;
            Vector2 checkPos = groundCheckPoint != null 
                ? (Vector2)transform.position + checkDir * 0.45f
                : (Vector2)transform.position + checkDir * (col.bounds.extents.y);

            // If groundLayers mask is zero (layers not configured yet), check anything except player itself
            Collider2D[] hits = Physics2D.OverlapCircleAll(checkPos, groundCheckRadius);
            isGrounded = false;

            foreach (var hit in hits)
            {
                if (hit != null && hit.gameObject != gameObject && !hit.isTrigger)
                {
                    isGrounded = true;
                    break;
                }
            }

            if (!wasGrounded && isGrounded)
            {
                OnLanded?.Invoke();
            }
        }

        private void Flip()
        {
            facingRight = !facingRight;
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !facingRight;
                spriteRenderer.flipY = isGravityInverted;
            }
            else
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (facingRight ? 1f : -1f);
                transform.localScale = scale;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 checkPos = groundCheckPoint != null 
                ? (Vector2)groundCheckPoint.position 
                : (Vector2)transform.position + Vector2.down * 0.8f;

            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
        }
    }
}
