using UnityEngine;
using UnityEngine.Rendering.Universal;
using GlitchVerse.Core;
using GlitchVerse.Echo;
using GlitchVerse.Player;

namespace GlitchVerse.Mechanics
{
    /// <summary>
    /// Moving red laser hazard that sweeps along a specified path.
    /// Kills the live player on contact, but ignores the Echo ghost.
    /// Inactive initially, energized when the Fake Exit Prank triggers in Chapter I.
    /// Follows Chapter I Difficulty Upgrade 01B requirements.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class MovingLaserHazard : MonoBehaviour
    {
        public enum MotionAxis
        {
            Horizontal,
            Vertical
        }

        [Header("Movement Configuration")]
        [SerializeField] private MotionAxis axis = MotionAxis.Horizontal;
        [SerializeField] private Vector3 pointA;
        [SerializeField] private Vector3 pointB;
        [SerializeField] private float moveSpeed = 1.6f;
        [SerializeField] [Range(0f, 1f)] private float startPhase = 0f;

        [Header("Hazard State")]
        [SerializeField] private bool isActive = false;

        [Header("Visual References")]
        [SerializeField] private SpriteRenderer laserRenderer;
        [SerializeField] private Light2D laserLight;
        [SerializeField] private Color activeColor = new Color(1.0f, 0.12f, 0.22f, 1f); // Emergency Red #FF1E38
        [SerializeField] private Color inactiveColor = new Color(0.3f, 0.1f, 0.12f, 0f);

        private BoxCollider2D col;
        private bool movingTowardsB = true;
        private Vector3 initialPosition;

        public bool IsActive => isActive;
        public MotionAxis Axis => axis;
        public Vector3 PointA => pointA;
        public Vector3 PointB => pointB;
        public float MoveSpeed => moveSpeed;

        private void Awake()
        {
            col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;

            if (laserRenderer == null)
            {
                laserRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (laserLight == null)
            {
                laserLight = GetComponentInChildren<Light2D>();
            }

            initialPosition = transform.position;
            UpdateVisuals();
        }

        private void Start()
        {
            ResetHazard();
        }

        public void ConfigurePath(MotionAxis motionAxis, Vector3 start, Vector3 end, float speed, float phase = 0f)
        {
            axis = motionAxis;
            pointA = start;
            pointB = end;
            moveSpeed = speed;
            startPhase = Mathf.Clamp01(phase);
            ResetHazard();
        }

        public void SetHazardActive(bool active)
        {
            isActive = active;
            if (col != null)
            {
                col.enabled = active;
            }

            UpdateVisuals();
            Debug.Log($"[MovingLaserHazard] {gameObject.name} active state: {isActive}");
        }

        public void ResetHazard()
        {
            // Position at start phase between pointA and pointB
            if (pointA != Vector3.zero || pointB != Vector3.zero)
            {
                transform.position = Vector3.Lerp(pointA, pointB, startPhase);
            }
            movingTowardsB = startPhase < 0.5f;

            if (col != null)
            {
                col.enabled = isActive;
            }

            UpdateVisuals();
        }

        private void Update()
        {
            if (!isActive) return;

            Vector3 target = movingTowardsB ? pointB : pointA;
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.02f)
            {
                movingTowardsB = !movingTowardsB;
            }

            // Subtle pulsing of laser light
            if (laserLight != null && isActive)
            {
                float pulse = 0.85f + 0.15f * Mathf.Sin(Time.time * 8f);
                laserLight.intensity = 1.4f * pulse;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CheckHazardCollision(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            CheckHazardCollision(other);
        }

        private void CheckHazardCollision(Collider2D other)
        {
            if (!isActive) return;

            // Ghost is intangible and immune to laser hazards
            if (other.GetComponent<EchoGhostMarker>() != null)
            {
                return;
            }

            // Live player takes lethal damage
            if (other.GetComponent<PlayerController2D>() != null || other.CompareTag("Player"))
            {
                Debug.Log($"[MovingLaserHazard] Player contacted active hazard {gameObject.name} at {transform.position}!");
                var dir = LevelDirector.Current;
                if (dir != null)
                {
                    dir.KillPlayer();
                }
            }
        }

        private void UpdateVisuals()
        {
            if (laserRenderer != null)
            {
                laserRenderer.enabled = isActive;
                laserRenderer.color = isActive ? activeColor : inactiveColor;
            }

            if (laserLight != null)
            {
                laserLight.enabled = isActive;
                laserLight.color = activeColor;
                laserLight.intensity = isActive ? 1.4f : 0f;
            }
        }
    }
}
