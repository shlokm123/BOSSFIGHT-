using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GlitchVerse.Player
{
    /// <summary>
    /// Lightweight cosmetic animation controller for Subject 075.
    /// Provides idle breathing, running lean/stride bob, and jump/land squash & stretch.
    /// CRITICAL: Operates strictly on a child visual transform or sprite renderer,
    /// leaving root BoxCollider2D, Rigidbody2D, physics velocities, and Echo recording completely untouched.
    /// </summary>
    public class PlayerVisualAnimator : MonoBehaviour
    {
        [Header("Target Visuals")]
        [SerializeField] private Transform visualTransform;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Light2D visorLight;

        [Header("Squash & Stretch Settings")]
        [SerializeField] private float jumpStretchY = 1.18f;
        [SerializeField] private float jumpSquashX = 0.85f;
        [SerializeField] private float landSquashY = 0.82f;
        [SerializeField] private float landStretchX = 1.18f;
        [SerializeField] private float recoverySpeed = 12.0f;

        [Header("Locomotion Visuals")]
        [SerializeField] private float runLeanAngle = 3.5f;
        [SerializeField] private float runBobFrequency = 14.0f;
        [SerializeField] private float runBobAmplitude = 0.035f;
        [SerializeField] private float idleBreatheSpeed = 2.4f;
        [SerializeField] private float idleBreatheAmount = 0.02f;

        private PlayerController2D controller;
        private Vector3 targetScale = Vector3.one;
        private Vector3 currentScale = Vector3.one;
        private float stepTimer = 0f;
        private float initialVisorIntensity = 0.45f;

        public Transform VisualTransform { get => visualTransform; set => visualTransform = value; }
        public SpriteRenderer SpriteRend { get => spriteRenderer; set => spriteRenderer = value; }
        public Light2D VisorLight { get => visorLight; set => visorLight = value; }

        private void Awake()
        {
            controller = GetComponent<PlayerController2D>();
            if (visualTransform == null)
            {
                // If no dedicated visual transform assigned, use child transform or self
                if (transform.childCount > 0 && transform.Find("Player_Visual") != null)
                {
                    visualTransform = transform.Find("Player_Visual");
                }
                else
                {
                    visualTransform = transform;
                }
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (visorLight == null)
            {
                visorLight = GetComponentInChildren<Light2D>();
            }

            if (visorLight != null)
            {
                initialVisorIntensity = visorLight.intensity;
            }
        }

        private void OnEnable()
        {
            if (controller != null)
            {
                controller.OnJumped += HandleJumped;
                controller.OnLanded += HandleLanded;
            }
        }

        private void OnDisable()
        {
            if (controller != null)
            {
                controller.OnJumped -= HandleJumped;
                controller.OnLanded -= HandleLanded;
            }
        }

        private void HandleJumped()
        {
            // Vertical stretch, horizontal squash
            currentScale = new Vector3(jumpSquashX, jumpStretchY, 1f);
        }

        private void HandleLanded()
        {
            // Vertical squash, horizontal stretch
            currentScale = new Vector3(landStretchX, landSquashY, 1f);
        }

        private void Update()
        {
            if (visualTransform == null) return;

            float dt = Time.deltaTime;
            bool isGrounded = controller != null && controller.IsGrounded;
            float horizontalInput = controller != null ? controller.HorizontalInput : 0f;
            bool isMoving = isGrounded && Mathf.Abs(horizontalInput) > 0.1f;

            // 1. Recover squash/stretch toward base scale
            currentScale = Vector3.Lerp(currentScale, targetScale, dt * recoverySpeed);

            // 2. Idle Breathing vs Running Bob
            float breatheScaleY = 1.0f;
            float localYOffset = 0f;
            float leanZ = 0f;

            if (isMoving)
            {
                stepTimer += dt * runBobFrequency;
                localYOffset = Mathf.Abs(Mathf.Sin(stepTimer)) * runBobAmplitude;
                // Running lean forward in move direction
                float leanDir = horizontalInput > 0 ? -1f : 1f;
                leanZ = leanDir * runLeanAngle;
            }
            else if (isGrounded)
            {
                stepTimer += dt * idleBreatheSpeed;
                breatheScaleY = 1.0f + Mathf.Sin(stepTimer) * idleBreatheAmount;
                localYOffset = 0f;
                leanZ = 0f;
            }
            else
            {
                // In air: subtle tilt based on vertical movement
                localYOffset = 0f;
                leanZ = 0f;
            }

            // Apply scale & rotation to visualTransform
            // When gravity is inverted, flip lean and offset
            bool inverted = controller != null && controller.IsGravityInverted;
            float gravitySign = inverted ? -1f : 1f;

            Vector3 finalScale = new Vector3(currentScale.x, currentScale.y * breatheScaleY, 1f);
            
            // If visualTransform is not the root GameObject, modify local scale and position safely
            if (visualTransform != transform)
            {
                visualTransform.localScale = finalScale;
                visualTransform.localPosition = new Vector3(0f, localYOffset * gravitySign, 0f);
                visualTransform.localRotation = Quaternion.Euler(0f, 0f, leanZ * gravitySign);
            }

            // 3. Subtle visor pulse
            if (visorLight != null)
            {
                float pulse = 1f + Mathf.Sin(Time.time * 3f) * 0.12f;
                visorLight.intensity = initialVisorIntensity * pulse;
            }
        }
    }
}
