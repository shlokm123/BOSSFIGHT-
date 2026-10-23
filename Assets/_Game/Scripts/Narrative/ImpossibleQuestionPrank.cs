using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using GlitchVerse.Core;
using GlitchVerse.Player;

namespace GlitchVerse.Narrative
{
    /// <summary>
    /// Deliberately unfair, one-time question event for Level 03.
    /// Freezes the player to ask: "DO YOU TRUST ME? [A] YES  [D] NO".
    /// Answering either option triggers instant death: "WRONG. THE QUESTION WAS THE TRAP."
    /// Persists across scene reloads so on retry the player can walk through unhindered.
    /// Follows Phase 07.5 Checkpoint 3 requirements.
    /// </summary>
    public class ImpossibleQuestionPrank : MonoBehaviour
    {
        // One-time session persistence
        public static bool HasExperiencedQuestion { get; private set; } = false;

        [Header("Trigger Location")]
        [SerializeField] private float triggerX = -4.0f;

        [Header("UI Prompt Reference")]
        [SerializeField] private Text questionPromptText;

        private bool isAwaitingAnswer = false;
        private PlayerController2D player;

        private void Start()
        {
            player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);

            if (questionPromptText != null)
            {
                questionPromptText.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (HasExperiencedQuestion) return;

            // Trigger detection
            if (!isAwaitingAnswer && player != null && player.transform.position.x >= triggerX)
            {
                TriggerQuestion();
            }

            // Await [A] or [D] input
            if (isAwaitingAnswer)
            {
                var kb = Keyboard.current;
                if (kb != null)
                {
                    if (kb.aKey.wasPressedThisFrame || kb.dKey.wasPressedThisFrame)
                    {
                        isAwaitingAnswer = false;
                        StartCoroutine(ExecuteTrapDeathRoutine());
                    }
                }
            }
        }

        private void TriggerQuestion()
        {
            HasExperiencedQuestion = true;
            isAwaitingAnswer = true;

            if (player != null)
            {
                player.InputLocked = true;
            }

            if (questionPromptText != null)
            {
                questionPromptText.gameObject.SetActive(true);
                questionPromptText.text = "<color=#FF1E40><b>DO YOU TRUST ME?</b></color>\n<size=20><color=#00F0FF>[A] YES</color>         <color=#FFB000>[D] NO</color></size>";
            }

            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.SetMonitorStatus("QUERY: TRUST");
            }

            Debug.Log("[ImpossibleQuestionPrank] Question prompt presented.");
        }

        private IEnumerator ExecuteTrapDeathRoutine()
        {
            if (questionPromptText != null)
            {
                questionPromptText.gameObject.SetActive(false);
            }

            // 1. Red flash & betrayal roast
            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.FlashScreenRed(0.55f, 0.4f);
                ArchitectDialogue.Current.Speak(
                    "WRONG. THE QUESTION WAS THE TRAP.",
                    "FATAL ERROR",
                    3.0f,
                    "impossible_question_roast",
                    0f,
                    priority: true
                );
            }

            yield return new WaitForSeconds(0.7f);

            // 2. Kill player and restart level
            if (LevelDirector.Current != null)
            {
                LevelDirector.Current.KillPlayer();
            }
        }

        public static void ResetSessionFlag()
        {
            HasExperiencedQuestion = false;
        }
    }
}
