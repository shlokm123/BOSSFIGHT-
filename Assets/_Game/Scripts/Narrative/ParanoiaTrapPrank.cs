using System.Collections;
using UnityEngine;
using GlitchVerse.Core;
using GlitchVerse.Player;

namespace GlitchVerse.Narrative
{
    /// <summary>
    /// Chapter V (Paranoia) Ragebait Event:
    /// A tempting lower "shortcut" route lined with deceptive warning indicators.
    /// If the player ignores the environmental warning cues and enters the false path,
    /// a security trap triggers, mocking their choice before resetting the room.
    /// On subsequent retries, the trap remains flagged so the player learns and can
    /// reliably avoid it by taking the upper gravity/dimension route.
    /// </summary>
    public class ParanoiaTrapPrank : MonoBehaviour
    {
        public static bool HasFallenForTrap { get; private set; } = false;

        [Header("Trap Zone Configuration")]
        [SerializeField] private float triggerX = 1.5f;
        [SerializeField] private float maxY = -2.0f; // Only affects lower floor route
        [SerializeField] private GameObject trapVisualOverlay;
        [SerializeField] private SpriteRenderer warningSignRenderer;

        private PlayerController2D player;
        private bool isTriggering = false;

        private void Start()
        {
            player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);

            if (HasFallenForTrap)
            {
                // On retry, keep the trap visually highlighted as armed/lethal so player learns
                if (warningSignRenderer != null)
                {
                    warningSignRenderer.color = new Color(1.0f, 0.1f, 0.2f, 1f);
                }
            }
        }

        private void Update()
        {
            if (isTriggering) return;
            if (HasFallenForTrap) return; // Only kills once per session; afterwards obvious hazard

            if (player == null)
            {
                player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);
            }

            if (player != null && player.transform.position.x >= triggerX && player.transform.position.y <= maxY)
            {
                StartCoroutine(ExecuteTrapSequence());
            }
        }

        private IEnumerator ExecuteTrapSequence()
        {
            isTriggering = true;
            HasFallenForTrap = true;

            // Lock player movement
            if (player != null)
            {
                player.InputLocked = true;
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }

            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.FlashScreenRed(0.6f, 0.5f);
                ArchitectDialogue.Current.SetMonitorStatus("CHOICE: TRAPPED");
                ArchitectDialogue.Current.Speak(
                    "You still think choosing is the same as being free?",
                    "PARANOIA PROTOCOL",
                    3.0f,
                    "paranoia_trap_prank",
                    0f,
                    priority: true
                );
            }

            yield return new WaitForSeconds(0.65f);

            // Trigger standard level restart
            if (LevelDirector.Current != null)
            {
                LevelDirector.Current.KillPlayer();
            }
            else
            {
                var dir = FindAnyObjectByType<LevelDirector>();
                if (dir != null) dir.KillPlayer();
            }
        }

        public static void ResetSessionFlag()
        {
            HasFallenForTrap = false;
        }
    }
}
