using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GlitchVerse.Core;
using GlitchVerse.Interactables;
using GlitchVerse.Mechanics;
using GlitchVerse.Player;
using GlitchVerse.UI;

namespace GlitchVerse.Narrative
{
    /// <summary>
    /// Implements the scripted one-time Fake Exit prank for Level 01.
    /// When the player first enters the unlocked exit, it flashes ACCESS GRANTED,
    /// triggers a glitch effect, relocates the real exit door to Observation Platform A,
    /// locks it behind a security barrier, activates two moving red laser hazards,
    /// and powers up the temporal Echo switch on Platform B.
    /// Follows Chapter I Difficulty Upgrade 01B requirements.
    /// </summary>
    public class FakeExitPrank : MonoBehaviour
    {
        [Header("Door References")]
        [SerializeField] private ExitDoor decoyDoor;
        [SerializeField] private ExitDoor realDoor;

        [Header("Platform Relocation Target")]
        [SerializeField] private Vector3 relocatedPosition = new Vector3(-3.0f, 0.4f, 0f);

        [Header("Security Interlock")]
        [SerializeField] private SecurityBarrier securityBarrier;
        [SerializeField] private SecurityOverrideSwitch secondarySwitch;

        [Header("Moving Laser Hazards")]
        [SerializeField] private MovingLaserHazard laserA;
        [SerializeField] private MovingLaserHazard laserB;

        private bool prankTriggered = false;
        public bool IsPrankTriggered => prankTriggered;
        public SecurityBarrier Barrier => securityBarrier;
        public SecurityOverrideSwitch SecondarySwitch => secondarySwitch;
        public MovingLaserHazard LaserA => laserA;
        public MovingLaserHazard LaserB => laserB;

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

            if (laserA != null)
            {
                laserA.SetHazardActive(laserA.ActiveOnStart);
            }

            if (laserB != null)
            {
                laserB.SetHazardActive(false);
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
            // 1. Brief flash and betrayal dialogue
            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.FlashScreenRed(0.4f, 0.25f);
                ArchitectDialogue.Current.Speak(
                    "I said you could leave. I didn't say where.",
                    "PORTAL RELOCATED",
                    3.0f,
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

                var light = realDoor.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
                if (light != null)
                {
                    light.intensity = 2.0f;
                    light.color = new Color(0f, 0.95f, 1f, 1f);
                }
            }

            // 4. Activate security barrier and power up the Echo switch on Platform B
            if (securityBarrier != null)
            {
                securityBarrier.gameObject.SetActive(true);
                securityBarrier.SetBarrierLocked(true);
            }

            if (secondarySwitch != null)
            {
                secondarySwitch.SetSwitchPowered(true);
            }

            // 5. Activate Laser B (Laser A was already active from beginning)
            if (laserA != null)
            {
                laserA.SetHazardActive(true);
            }

            if (laserB != null)
            {
                laserB.SetHazardActive(true);
            }

            // 6. Update HUD instruction and Echo hint
            var hud = FindAnyObjectByType<LevelHUD>(FindObjectsInactive.Exclude);
            if (hud != null)
            {
                var echoHintField = typeof(LevelHUD).GetField("echoHintText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (echoHintField != null)
                {
                    var echoHintText = echoHintField.GetValue(hud) as Text;
                    if (echoHintText != null)
                    {
                        echoHintText.text = "[H] REPLAY ECHO / [C] CLEAR";
                    }
                }
            }

            // 7. Dialogue 2: "Funny how quickly a safe path becomes a bad decision."
            yield return new WaitForSeconds(3.1f);
            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.Speak(
                    "Funny how quickly a safe path becomes a bad decision.",
                    "HAZARD ESCALATION",
                    3.2f,
                    "ch1_path_betrayal",
                    0f,
                    priority: true
                );
            }

            // 8. Contextual temporal instruction after hazard escalation
            yield return new WaitForSeconds(3.3f);

            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.Speak(
                    "SOME DOORS REQUIRE SOMEONE YOU USED TO BE.",
                    "TEMPORAL ANOMALY",
                    3.5f,
                    "ch1_echo_hint",
                    0f,
                    priority: true
                );
            }

            Debug.Log("[FakeExitPrank] Exit relocated to Observation Platform A. Dual moving lasers online. Echo switch energized.");
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
                secondarySwitch.SetSwitchPowered(false);
            }

            if (laserA != null)
            {
                laserA.ResetHazard();
                laserA.SetHazardActive(false);
            }

            if (laserB != null)
            {
                laserB.ResetHazard();
                laserB.SetHazardActive(false);
            }
        }
    }
}
