using System.Collections;
using UnityEngine;
using GlitchVerse.Core;
using GlitchVerse.Player;

namespace GlitchVerse.Narrative
{
    /// <summary>
    /// Deceptive floor tile that appears mostly normal with subtle warning cracks.
    /// When stepped on, waits 0.3 seconds before collapsing, causing the player
    /// to fall into a death zone with the roast: "YOU TRUSTED THE FLOOR?"
    /// Follows Phase 07.5 Checkpoint 2 requirements.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class CollapsingFloor : MonoBehaviour
    {
        public static bool HasDiedToFloor { get; private set; } = false;

        [Header("Tuning")]
        [SerializeField] private float collapseDelay = 0.32f;
        [SerializeField] private float fallSpeed = 14.0f;
        [SerializeField] private Color warningTint = new Color(0.95f, 0.45f, 0.45f, 1f);

        [Header("Components")]
        [SerializeField] private SpriteRenderer floorRenderer;
        [SerializeField] private BoxCollider2D solidCollider;

        private Vector3 initialPosition;
        private Color initialColor;
        private bool hasTriggered = false;
        private bool isCollapsing = false;

        private void Awake()
        {
            initialPosition = transform.position;
            solidCollider = GetComponent<BoxCollider2D>();

            if (floorRenderer == null)
            {
                floorRenderer = GetComponent<SpriteRenderer>();
            }

            if (floorRenderer != null)
            {
                initialColor = floorRenderer.color;
            }
        }

        private void Update()
        {
            if (isCollapsing)
            {
                transform.position += Vector3.down * fallSpeed * Time.deltaTime;
                if (floorRenderer != null)
                {
                    Color c = floorRenderer.color;
                    c.a = Mathf.Max(0f, c.a - Time.deltaTime * 2.5f);
                    floorRenderer.color = c;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            CheckAndTriggerCollapse(collision.collider);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            CheckAndTriggerCollapse(collision.collider);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CheckAndTriggerCollapse(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            CheckAndTriggerCollapse(other);
        }

        private void CheckAndTriggerCollapse(Collider2D col)
        {
            if (hasTriggered) return;

            var p = col.GetComponent<PlayerController2D>();
            if (p != null || col.CompareTag("Player"))
            {
                hasTriggered = true;
                if (p == null) p = FindAnyObjectByType<PlayerController2D>();
                StartCoroutine(CollapseSequenceRoutine(p));
            }
        }

        private IEnumerator CollapseSequenceRoutine(PlayerController2D player)
        {
            // 1. Subtle tremor & crack warning (0.3s)
            float elapsed = 0f;
            while (elapsed < collapseDelay)
            {
                elapsed += Time.deltaTime;
                Vector3 jitter = new Vector3(Random.Range(-0.04f, 0.04f), Random.Range(-0.04f, 0.04f), 0f);
                transform.position = initialPosition + jitter;

                if (floorRenderer != null)
                {
                    floorRenderer.color = Color.Lerp(initialColor, warningTint, elapsed / collapseDelay);
                }
                yield return null;
            }

            // 2. Collapse! Disable solid collision
            if (solidCollider != null)
            {
                solidCollider.enabled = false;
            }
            isCollapsing = true;

            // 3. Fall beat
            yield return new WaitForSeconds(0.45f);

            // 4. If player fell down, trigger death and roast
            if (player == null) player = FindAnyObjectByType<PlayerController2D>();
            if (player != null && player.transform.position.y < initialPosition.y - 0.4f)
            {
                HasDiedToFloor = true;
                if (ArchitectDialogue.Current != null)
                {
                    ArchitectDialogue.Current.FlashScreenRed(0.45f, 0.3f);
                    ArchitectDialogue.Current.Speak(
                        "YOU TRUSTED THE FLOOR?",
                        "STRUCTURAL INTEGRITY: 0%",
                        2.8f,
                        "floor_roast",
                        0f,
                        priority: true
                    );
                }

                yield return new WaitForSeconds(0.4f);

                if (LevelDirector.Current != null)
                {
                    LevelDirector.Current.KillPlayer();
                }
            }
        }

        public void ResetTrap()
        {
            hasTriggered = false;
            isCollapsing = false;
            transform.position = initialPosition;
            if (solidCollider != null) solidCollider.enabled = true;
            if (floorRenderer != null) floorRenderer.color = initialColor;
        }

        public static void ResetSessionFlag()
        {
            HasDiedToFloor = false;
        }
    }
}
