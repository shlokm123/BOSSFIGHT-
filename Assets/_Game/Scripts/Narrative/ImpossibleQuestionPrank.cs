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
    /// Freezes the player to ask:
    /// THE ARCHITECT // INTERROGATION 001
    /// DO YOU TRUST ME?
    /// [Y] YES          [N] NO
    /// PRESS Y OR N TO ANSWER
    /// Answering Y or N delivers a deceptive fake success ("ANSWER ACCEPTED. YOU MAY PROCEED."),
    /// followed by the betrayal ("I LIED. The correct answer was never an option.") and instant death.
    /// Persists across scene reloads so on retry the player can walk through unhindered.
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
        private Coroutine activeTrapRoutine;

        private void Start()
        {
            player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);

            if (HasExperiencedQuestion)
            {
                if (questionPromptText != null)
                {
                    questionPromptText.gameObject.SetActive(false);
                }
                if (player != null)
                {
                    player.InputLocked = false;
                }
                return;
            }

            if (questionPromptText != null)
            {
                questionPromptText.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if (activeTrapRoutine != null)
            {
                StopCoroutine(activeTrapRoutine);
                activeTrapRoutine = null;
            }
        }

        private void Update()
        {
            if (HasExperiencedQuestion && !isAwaitingAnswer) return;

            if (player == null)
            {
                player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);
            }

            // Trigger detection
            if (!isAwaitingAnswer && player != null && player.transform.position.x >= triggerX)
            {
                TriggerQuestion();
            }

            // Await strictly [Y] or [N] input (A/D remain strictly for movement)
            if (isAwaitingAnswer)
            {
                var kb = Keyboard.current;
                if (kb != null)
                {
                    if (kb.yKey.wasPressedThisFrame || kb.yKey.isPressed ||
                        kb.nKey.wasPressedThisFrame || kb.nKey.isPressed)
                    {
                        isAwaitingAnswer = false;
                        activeTrapRoutine = StartCoroutine(ExecuteFakeSuccessRoutine());
                    }
                }
            }
        }

        private void TriggerQuestion()
        {
            HasExperiencedQuestion = true;
            isAwaitingAnswer = true;

            if (player == null)
            {
                player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);
            }

            if (player != null)
            {
                player.InputLocked = true;
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }
            }

            if (questionPromptText != null)
            {
                questionPromptText.gameObject.SetActive(true);
                questionPromptText.verticalOverflow = VerticalWrapMode.Overflow;
                questionPromptText.horizontalOverflow = HorizontalWrapMode.Overflow;
                questionPromptText.text = 
                    "<size=15><color=#00F0FF><b>THE ARCHITECT // INTERROGATION 001</b></color></size>\n\n" +
                    "<size=26><color=#FF1E40><b>DO YOU TRUST ME?</b></color></size>\n\n" +
                    "<size=22><color=#00F0FF><b>[Y] YES</b></color>          <color=#FFB000><b>[N] NO</b></color></size>\n\n" +
                    "<size=13><color=#9095A0>PRESS Y OR N TO ANSWER</color></size>";
            }

            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.SetMonitorStatus("QUERY: TRUST");
            }

            Debug.Log("[ImpossibleQuestionPrank] Interrogation prompt presented with Y/N controls.");
        }

        private IEnumerator ExecuteFakeSuccessRoutine()
        {
            // 1. Hide question & choices, display green: "ANSWER ACCEPTED."
            if (questionPromptText != null)
            {
                questionPromptText.text = 
                    "<size=15><color=#00F0FF><b>THE ARCHITECT // INTERROGATION 001</b></color></size>\n\n" +
                    "<size=26><color=#00FF66><b>ANSWER ACCEPTED.</b></color></size>";
            }

            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.SetMonitorStatus("RESPONSE LOGGED");
            }

            // 2. Wait approximately 0.5 seconds
            yield return new WaitForSeconds(0.5f);

            // 3. Display: "YOU MAY PROCEED."
            if (questionPromptText != null)
            {
                questionPromptText.text = 
                    "<size=15><color=#00F0FF><b>THE ARCHITECT // INTERROGATION 001</b></color></size>\n\n" +
                    "<size=26><color=#00FF66><b>ANSWER ACCEPTED.</b></color></size>\n\n" +
                    "<size=20><color=#00F0FF><b>YOU MAY PROCEED.</b></color></size>";
            }

            // 4. Wait approximately 0.7 seconds (player kept locked so they cannot run past trap)
            yield return new WaitForSeconds(0.7f);

            // 5. Hide the green prompt text
            if (questionPromptText != null)
            {
                questionPromptText.gameObject.SetActive(false);
            }

            // 6. Abrupt blood-red glitch flash and betrayal punchline: "I LIED."
            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.FlashScreenRed(0.6f, 0.4f);
                ArchitectDialogue.Current.Speak(
                    "<size=30><color=#FF1E40><b>I LIED.</b></color></size>\n<color=#FFFFFF>The correct answer was never an option.</color>",
                    "CRITICAL BETRAYAL",
                    3.0f,
                    "impossible_question_betrayal",
                    0f,
                    priority: true
                );
            }

            // 7. Brief dramatic beat (0.55s) before killing player
            yield return new WaitForSeconds(0.55f);

            // 8. Trigger death and restart level
            Time.timeScale = 1.0f;
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
            HasExperiencedQuestion = false;
        }
    }
}
