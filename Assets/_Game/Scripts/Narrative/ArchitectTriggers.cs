using System;
using UnityEngine;
using UnityEngine.InputSystem;
using GlitchVerse.Core;
using GlitchVerse.Echo;
using GlitchVerse.Interactables;
using GlitchVerse.Mechanics;
using GlitchVerse.Player;

namespace GlitchVerse.Narrative
{
    /// <summary>
    /// Event connector that hooks into gameplay events (level lifecycle, powers,
    /// echo results, pressure plate state changes) and triggers context-sensitive
    /// dialogue from The Architect.
    /// Follows Phase 07 directives.
    /// </summary>
    public class ArchitectTriggers : MonoBehaviour
    {
        [Header("Scene Component Dependencies")]
        [SerializeField] private LevelDirector levelDirector;
        [SerializeField] private AbilityController abilityController;
        [SerializeField] private EchoPlayback echoPlayback;
        [SerializeField] private EchoSwitch echoSwitch;
        [SerializeField] private PressurePlate pressurePlate;

        private void Start()
        {
            if (levelDirector == null) levelDirector = LevelDirector.Current;
            if (abilityController == null) abilityController = FindAnyObjectByType<AbilityController>(FindObjectsInactive.Exclude);
            if (echoPlayback == null) echoPlayback = FindAnyObjectByType<EchoPlayback>(FindObjectsInactive.Exclude);
            if (echoSwitch == null) echoSwitch = FindAnyObjectByType<EchoSwitch>(FindObjectsInactive.Exclude);
            if (pressurePlate == null) pressurePlate = FindAnyObjectByType<PressurePlate>(FindObjectsInactive.Exclude);

            // Subscribe to Level events
            if (levelDirector != null)
            {
                levelDirector.OnLevelComplete += HandleLevelComplete;
                levelDirector.OnPlayerDied += HandlePlayerDied;
            }

            // Subscribe to Ability events
            if (abilityController != null)
            {
                abilityController.OnGravityToggled += HandleGravityToggled;
                abilityController.OnTimeFreezeToggled += HandleTimeFreezeToggled;
                abilityController.OnDimensionToggled += HandleDimensionToggled;
            }

            // Subscribe to Echo events
            if (echoPlayback != null)
            {
                echoPlayback.OnPlaybackStarted += HandlePlaybackStarted;
                echoPlayback.OnPlaybackFinished += HandlePlaybackFinished;
            }

            // Subscribe to Plate events
            if (pressurePlate != null)
            {
                pressurePlate.OnPlateStateChanged += HandlePlateStateChanged;
            }

            // Trigger Entry Commentary when chapter intro concludes (or after brief delay if already concluded)
            if (ChapterIntroController.Current != null && ChapterIntroController.Current.IsIntroPlaying)
            {
                ChapterIntroController.OnIntroConcluded += HandleIntroConcluded;
            }
            else
            {
                Invoke(nameof(TriggerEntryCommentary), 0.5f);
            }
        }

        private void HandleIntroConcluded()
        {
            ChapterIntroController.OnIntroConcluded -= HandleIntroConcluded;
            Invoke(nameof(TriggerEntryCommentary), 0.2f);
        }

        private void OnDestroy()
        {
            ChapterIntroController.OnIntroConcluded -= HandleIntroConcluded;

            if (levelDirector != null)
            {
                levelDirector.OnLevelComplete -= HandleLevelComplete;
                levelDirector.OnPlayerDied -= HandlePlayerDied;
            }

            if (abilityController != null)
            {
                abilityController.OnGravityToggled -= HandleGravityToggled;
                abilityController.OnTimeFreezeToggled -= HandleTimeFreezeToggled;
                abilityController.OnDimensionToggled -= HandleDimensionToggled;
            }

            if (echoPlayback != null)
            {
                echoPlayback.OnPlaybackStarted -= HandlePlaybackStarted;
                echoPlayback.OnPlaybackFinished -= HandlePlaybackFinished;
            }

            if (pressurePlate != null)
            {
                pressurePlate.OnPlateStateChanged -= HandlePlateStateChanged;
            }
        }

        private static bool hasCommentedOnPassingFloor = false;
        private static bool hasCommentedOnPassingQuestion = false;
        private PlayerController2D cachedPlayer;

        private void Update()
        {
            if (cachedPlayer == null)
            {
                cachedPlayer = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);
            }

            // Context-aware passing lines
            if (cachedPlayer != null && levelDirector != null)
            {
                if (levelDirector.ChamberNumber == 2 && CollapsingFloor.HasDiedToFloor && !hasCommentedOnPassingFloor)
                {
                    if (cachedPlayer.transform.position.x > 2.5f)
                    {
                        hasCommentedOnPassingFloor = true;
                        if (ArchitectDialogue.Current != null)
                        {
                            ArchitectDialogue.Current.Speak(
                                "Remarkable. You managed to step over a hole.",
                                "TRAP EVADED",
                                2.6f,
                                "passed_floor_roast",
                                10f
                            );
                        }
                    }
                }
                else if (levelDirector.ChamberNumber == 3 && ImpossibleQuestionPrank.HasExperiencedQuestion && !hasCommentedOnPassingQuestion)
                {
                    if (cachedPlayer.transform.position.x > -2.0f)
                    {
                        hasCommentedOnPassingQuestion = true;
                        if (ArchitectDialogue.Current != null)
                        {
                            ArchitectDialogue.Current.Speak(
                                "Silence suits you. Proceed with the chamber.",
                                "SUBJECT PASSING",
                                2.6f,
                                "passed_question_roast",
                                10f
                            );
                        }
                    }
                }
            }

            // Listen for Echo clear [C] key
            var kb = Keyboard.current;
            if (kb != null && kb.cKey.wasPressedThisFrame)
            {
                if (cachedPlayer != null && cachedPlayer.InputLocked) return;

                if (ArchitectDialogue.Current != null)
                {
                    ArchitectDialogue.Current.Speak(
                        "Your past self has filed a complaint.",
                        "TIMELINE CLEARED",
                        2.5f,
                        "echo_clear_roast",
                        20f
                    );
                }
            }
        }

        private void TriggerEntryCommentary()
        {
            if (ArchitectDialogue.Current == null) return;

            if (levelDirector == null) levelDirector = LevelDirector.Current;
            int chamber = 1;
            if (levelDirector != null)
            {
                chamber = levelDirector.ChamberNumber;
            }
            else
            {
                string sName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                if (sName.StartsWith("Level") && sName.Length >= 7 && int.TryParse(sName.Substring(5, 2), out int parsed))
                {
                    chamber = parsed;
                }
            }

            // Checkpoint 5: Context-aware retry lines after traps
            if (chamber == 2 && CollapsingFloor.HasDiedToFloor)
            {
                ArchitectDialogue.Current.Speak(
                    "Floor integrity remains unverified. Step lightly.",
                    "ADVISORY: WATCH STEP",
                    2.8f,
                    "retry_floor_trap",
                    5f,
                    priority: true
                );
                return;
            }
            if (chamber == 3 && ImpossibleQuestionPrank.HasExperiencedQuestion)
            {
                ArchitectDialogue.Current.Speak(
                    "Relax. You've already failed this interview.",
                    "INTERVIEW FAILED",
                    2.8f,
                    "retry_question_trap",
                    5f,
                    priority: true
                );
                return;
            }

            int restarts = GameSession.Instance != null ? GameSession.Instance.ResetCount : 0;

            if (restarts >= 5)
            {
                ArchitectDialogue.Current.Speak(
                    "The definition of insanity, simulated in real-time.",
                    "EVALUATION: INSANE",
                    3.0f,
                    "restart_high",
                    10f,
                    priority: true
                );
                return;
            }
            else if (restarts >= 2)
            {
                ArchitectDialogue.Current.Speak(
                    "Statistically, you should be improving.",
                    "RETRIAL MONITORED",
                    2.8f,
                    "restart_medium",
                    10f,
                    priority: true
                );
                return;
            }

            // Level entry lines
            switch (chamber)
            {
                case 1:
                    ArchitectDialogue.Current.Speak(
                        "SUBJECT 075. AGAIN.",
                        "SUBJECT RESTORED",
                        2.8f,
                        "entry_lvl1",
                        30f
                    );
                    break;
                case 2:
                    ArchitectDialogue.Current.Speak(
                        "Physics was never a guarantee.",
                        "INVERSION READY",
                        2.8f,
                        "entry_lvl2",
                        30f
                    );
                    break;
                case 3:
                    ArchitectDialogue.Current.Speak(
                        "Time is an illusion. Your reaction time is a joke.",
                        "TEMPORAL LOCK ACTIVE",
                        3.0f,
                        "entry_lvl3",
                        30f
                    );
                    break;
                case 4:
                    ArchitectDialogue.Current.Speak(
                        "YOU WERE NEVER SUPPOSED TO GET THIS FAR.",
                        "CRITICAL ANOMALY",
                        3.2f,
                        "entry_lvl4",
                        30f,
                        priority: true
                    );
                    break;
                case 5:
                    ArchitectDialogue.Current.Speak(
                        "You still think choosing is the same as being free?",
                        "CHOICE: ILLUSION",
                        3.2f,
                        "entry_lvl5",
                        30f
                    );
                    break;
                case 6:
                    ArchitectDialogue.Current.Speak(
                        "You've spent six chambers proving you can escape.",
                        "QUARANTINE BREACH",
                        3.4f,
                        "entry_lvl6",
                        30f,
                        priority: true
                    );
                    break;
            }
        }

        private void HandleGravityToggled(bool inverted)
        {
            if (ArchitectDialogue.Current == null) return;
            ArchitectDialogue.Current.Speak(
                "Gravity is merely a recommendation.",
                inverted ? "VECT: INVERTED" : "VECT: NORMAL",
                2.4f,
                "gravity_roast",
                25f
            );
        }

        private void HandleTimeFreezeToggled(bool frozen)
        {
            if (ArchitectDialogue.Current == null) return;
            if (frozen)
            {
                ArchitectDialogue.Current.Speak(
                    "Freezing time won't fix your reaction speed.",
                    "CHRONO FIELD: FROZEN",
                    2.4f,
                    "freeze_roast",
                    25f
                );
            }
        }

        private void HandleDimensionToggled(DimensionType dim)
        {
            if (ArchitectDialogue.Current == null) return;
            ArchitectDialogue.Current.Speak(
                "Flipping realities won't change the outcome.",
                $"PHASE: {dim.ToString().ToUpper()}",
                2.4f,
                "dimension_roast",
                25f
            );
        }

        private void HandlePlaybackStarted()
        {
            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.SetMonitorStatus("TIMELINE REPLAY ACTIVE");
            }
        }

        private void HandlePlaybackFinished()
        {
            if (ArchitectDialogue.Current == null) return;

            // Checkpoint 3: Echo Betrayal / Failure reaction
            // If the echo playback finished and the EchoSwitch in the level was not satisfied
            if (echoSwitch != null && !echoSwitch.IsActivated)
            {
                string activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                if (activeScene == "Level03")
                {
                    ArchitectDialogue.Current.FlashScreenRed(0.45f, 0.3f);
                    ArchitectDialogue.Current.Speak(
                        "You made that mistake before. I merely remembered it.",
                        "TEMPORAL CONSEQUENCE",
                        3.2f,
                        "ch3_echo_betrayal",
                        0f,
                        priority: true
                    );
                }
                else
                {
                    ArchitectDialogue.Current.Speak(
                        "Even your previous self couldn't help you.",
                        "ECHO SYNC FAILED",
                        2.8f,
                        "echo_betrayal_roast",
                        15f
                    );
                }
            }
            else
            {
                ArchitectDialogue.Current.SetMonitorStatus("ECHO DISSIPATED");
            }
        }

        private void HandlePlateStateChanged(bool activated)
        {
            if (ArchitectDialogue.Current == null) return;

            if (!activated && levelDirector != null && !levelDirector.IsLevelCompleted)
            {
                ArchitectDialogue.Current.Speak(
                    "Bold strategy. Terrible execution.",
                    "CIRCUIT INTERRUPTED",
                    2.5f,
                    "plate_release_roast",
                    20f
                );
            }
        }

        private void HandlePlayerDied()
        {
            if (ArchitectDialogue.Current == null) return;
            if (ArchitectDialogue.Current.IsSpeaking) return; // Preserve active trap roast dialogue

            ArchitectDialogue.Current.FlashScreenRed(0.45f, 0.35f);
            ArchitectDialogue.Current.Speak(
                "Another biological iteration terminated.",
                "SUBJECT PURGED",
                2.5f,
                "death_roast",
                5f,
                priority: true
            );
        }

        private void HandleLevelComplete()
        {
            if (ArchitectDialogue.Current == null) return;

            bool isFinal = levelDirector != null && levelDirector.IsFinalLevel;
            if (!isFinal)
            {
                ArchitectDialogue.Current.Speak(
                    "I expected less. Somehow, you delivered.",
                    "CHAMBER CLEARED",
                    2.8f,
                    "level_complete_roast",
                    10f,
                    priority: true
                );
            }
        }
    }
}
