using System.Collections;
using UnityEngine;
using GlitchVerse.Player;

namespace GlitchVerse.Narrative
{
    /// <summary>
    /// Lightweight fake-checkpoint dialogue joke in Level 04.
    /// Informs the player that progress was saved, followed by:
    /// "Just kidding. I don't believe in those."
    /// Dialogue/UI only. Follows Phase 07.5 Checkpoint 4 requirements.
    /// </summary>
    public class FakeCheckpointPrank : MonoBehaviour
    {
        [Header("Trigger Location")]
        [SerializeField] private float triggerX = 0.0f;

        private bool hasTriggered = false;
        private PlayerController2D player;

        private void Start()
        {
            player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);
        }

        private void Update()
        {
            if (hasTriggered) return;

            if (player == null)
            {
                player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);
            }

            if (player != null && player.transform.position.x >= triggerX)
            {
                hasTriggered = true;
                StartCoroutine(FakeCheckpointRoutine());
            }
        }

        private IEnumerator FakeCheckpointRoutine()
        {
            if (ArchitectDialogue.Current == null) yield break;

            // 1. Initial fake checkpoint notice
            ArchitectDialogue.Current.Speak(
                "<color=#00F0FF>[SYSTEM] CHECKPOINT SAVED.</color>",
                "STATE SAVED",
                1.4f,
                "fake_checkpoint_1",
                0f,
                priority: true
            );

            yield return new WaitForSeconds(1.5f);

            // 2. Punchline
            ArchitectDialogue.Current.Speak(
                "Just kidding. I don't believe in those.",
                "NO CHECKPOINTS",
                2.8f,
                "fake_checkpoint_2",
                0f,
                priority: true
            );

            Debug.Log("[FakeCheckpointPrank] Checkpoint joke delivered.");
        }

        public void ResetPrank()
        {
            hasTriggered = false;
        }
    }
}
