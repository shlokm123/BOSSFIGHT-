using System.Collections;
using UnityEngine;
using GlitchVerse.Core;
using GlitchVerse.Interactables;
using GlitchVerse.Player;

namespace GlitchVerse.Narrative
{
    /// <summary>
    /// Implements the scripted one-time Fake Exit prank for Level 01.
    /// When the player first enters the unlocked exit, it flashes ACCESS GRANTED,
    /// triggers a glitch effect, and relocates the real exit door to Observation Platform A.
    /// Follows Phase 07 Checkpoint 2 fairness requirements.
    /// </summary>
    public class FakeExitPrank : MonoBehaviour
    {
        [Header("Door References")]
        [SerializeField] private ExitDoor decoyDoor;
        [SerializeField] private ExitDoor realDoor;

        [Header("Platform Relocation Target")]
        [SerializeField] private Vector3 relocatedPosition = new Vector3(-3.0f, -0.6f, 0f);

        [Header("Security Interlock")]
        [SerializeField] private SecurityBarrier securityBarrier;
        [SerializeField] private SecurityOverrideSwitch secondarySwitch;

        private bool prankTriggered = false;
        public bool IsPrankTriggered => prankTriggered;
        public SecurityBarrier Barrier => securityBarrier;
        public SecurityOverrideSwitch SecondarySwitch => secondarySwitch;

        private void Start()
        {
            if (decoyDoor == null) decoyDoor = GetComponent<ExitDoor>();

            // Ensure real door is ready and disabled initially if it exists separately
            if (realDoor != null && realDoor != decoyDoor)
            {
                realDoor.gameObject.SetActive(false);
            }

            if (securityBarrier != null)
            {
                securityBarrier.gameObject.SetActive(false);
            }

            if (secondarySwitch != null)
            {
                secondarySwitch.SetSwitchPowered(false);
            }
        }

        /// <summary>
        /// Intercepts player entering the door. Returns true if the prank intercepted it.
        /// </summary>
        public bool TryInterceptExit(Collider2D playerCol)
        {
            if (prankTriggered) return false;

            prankTriggered = true;
            StartCoroutine(ExecutePrankRoutine());
            return true;
        }

        private IEnumerator ExecutePrankRoutine()
        {
            // 1. Brief flash and glitch
            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.FlashScreenRed(0.4f, 0.25f);
                ArchitectDialogue.Current.Speak(
                    "I said you could leave. I didn't say where.",
                    "PORTAL RELOCATED",
                    3.2f,
                    "fake_exit_prank",
                    0f,
                    priority: true
                );
            }

            // 2. Change decoy door visuals to a deactivated/relocated look
            if (decoyDoor != null)
            {
                decoyDoor.SetUnlocked(false);
                var rend = decoyDoor.GetComponent<SpriteRenderer>();
                if (rend != null)
                {
                    rend.color = new Color(0.4f, 0.4f, 0.45f, 0.6f);
                }
            }

            yield return new WaitForSeconds(0.15f);

            // 3. Reveal and activate the real exit on Observation Platform A
            if (realDoor != null)
            {
                realDoor.gameObject.SetActive(true);
                realDoor.transform.position = relocatedPosition;
                realDoor.SetUnlocked(true);

                // Add visual beacon/glow on the new door
                var light = realDoor.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
                if (light != null)
                {
                    light.intensity = 2.0f;
                    light.color = new Color(0f, 0.95f, 1f, 1f);
                }
            }

            // 4. Activate security barrier and power up the secondary override breaker on Platform B
            if (securityBarrier != null)
            {
                securityBarrier.gameObject.SetActive(true);
                securityBarrier.SetBarrierLocked(true);
            }

            if (secondarySwitch != null)
            {
                secondarySwitch.SetSwitchPowered(true);
            }

            Debug.Log("[FakeExitPrank] Exit relocated to Observation Platform A. Security barrier locked. Override switch energized.");
        }

        public void ResetPrank()
        {
            prankTriggered = false;
            if (realDoor != null && realDoor != decoyDoor)
            {
                realDoor.gameObject.SetActive(false);
            }

            if (securityBarrier != null)
            {
                securityBarrier.ResetBarrier();
                securityBarrier.gameObject.SetActive(false);
            }

            if (secondarySwitch != null)
            {
                secondarySwitch.ResetSwitch();
            }
        }
    }
}
