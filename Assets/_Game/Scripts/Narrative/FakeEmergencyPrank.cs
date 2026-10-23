using System.Collections;
using UnityEngine;
using GlitchVerse.Player;

namespace GlitchVerse.Narrative
{
    /// <summary>
    /// Implements the dramatic but harmless Fake Emergency countdown prank in Level 02.
    /// Triggers an ominous red countdown alert: "CONTAINMENT FAILURE IN 5... 4... 3... 2... 1...",
    /// then halts and reveals: "Just checking your reaction."
    /// Zero damage, zero progress reset. Follows Phase 07 Checkpoint 4 requirements.
    /// </summary>
    public class FakeEmergencyPrank : MonoBehaviour
    {
        [Header("Trigger Conditions")]
        [SerializeField] private float triggerDelay = 5.0f;
        [SerializeField] private float triggerXThreshold = -1.0f;

        private bool hasTriggered = false;
        private float timer = 0f;
        private Transform playerTransform;

        private void Start()
        {
            var playerObj = GameObject.Find("Player_Subject075");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }

        private void Update()
        {
            if (hasTriggered) return;

            timer += Time.deltaTime;

            // Trigger when player reaches the mid-chamber or after 5s
            bool playerReached = playerTransform != null && playerTransform.position.x > triggerXThreshold;
            if (timer >= triggerDelay || playerReached)
            {
                hasTriggered = true;
                StartCoroutine(EmergencyCountdownRoutine());
            }
        }

        private IEnumerator EmergencyCountdownRoutine()
        {
            if (ArchitectDialogue.Current == null) yield break;

            for (int count = 5; count >= 1; count--)
            {
                ArchitectDialogue.Current.FlashScreenRed(0.35f, 0.25f);
                ArchitectDialogue.Current.Speak(
                    $"CONTAINMENT FAILURE IN {count}...",
                    $"CRITICAL ALERT: T-{count}",
                    0.85f,
                    null,
                    0f,
                    priority: true
                );
                yield return new WaitForSeconds(1.0f);
            }

            // Climax at zero: instant calm, punchline
            yield return new WaitForSeconds(0.4f);
            ArchitectDialogue.Current.Speak(
                "Just checking your reaction.",
                "PULSE RATE RECORDED",
                3.0f,
                "fake_emergency_punchline",
                0f,
                priority: true
            );

            Debug.Log("[FakeEmergencyPrank] Emergency prank finished harmlessly.");
        }

        public void ResetPrank()
        {
            hasTriggered = false;
            timer = 0f;
        }
    }
}
