using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GlitchVerse.Core;
using GlitchVerse.Boss;
using GlitchVerse.Narrative;
using GlitchVerse.Player;

namespace GlitchVerse.UI
{
    /// <summary>
    /// Master controller for the final plot-twist ending sequence and restart loop.
    /// Triggers exclusively in Chapter VII (Level07) upon genuine defeat of the Architect Core.
    /// Manages the 5-beat cinematic sequence:
    /// - Beat 1: False Victory (Cyan/Green core disabled + escape granted)
    /// - Beat 2: Silence (Empty black screen hold)
    /// - Beat 3: The Truth ("I WASN'T KEEPING YOU IN. I WAS KEEPING THEM OUT." in crimson)
    /// - Beat 4: The Consequence (Quarantine disabled, external perimeter breached)
    /// - Beat 5: Final End Screen + RESTART SIMULATION button returning to MainMenu.
    /// </summary>
    public class FinalEndingController : MonoBehaviour
    {
        public static FinalEndingController Instance { get; private set; }

        [Header("UI Hierarchy")]
        [SerializeField] private CanvasGroup endingCanvasGroup;
        [SerializeField] private Image fullScreenBackdrop;
        [SerializeField] private Text primaryStatusText;
        [SerializeField] private Text secondaryStatusText;
        [SerializeField] private Text truthClimaxText;
        [SerializeField] private Text consequenceText;
        [SerializeField] private GameObject finalEndScreenPanel;
        [SerializeField] private Button restartSimulationButton;

        private bool hasTriggered = false;
        private bool isRestarting = false;
        private Coroutine endingCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (restartSimulationButton != null)
            {
                restartSimulationButton.onClick.RemoveAllListeners();
                restartSimulationButton.onClick.AddListener(OnRestartSimulationClicked);
            }

            // Ensure ending overlay starts completely hidden and non-blocking
            if (endingCanvasGroup != null)
            {
                endingCanvasGroup.alpha = 0f;
                endingCanvasGroup.blocksRaycasts = false;
                endingCanvasGroup.interactable = false;
            }

            if (finalEndScreenPanel != null)
            {
                finalEndScreenPanel.SetActive(false);
            }
        }

        private void OnEnable()
        {
            var director = LevelDirector.Current ?? FindAnyObjectByType<LevelDirector>();
            if (director != null)
            {
                director.OnLevelComplete += HandleLevelComplete;
            }
        }

        private void OnDisable()
        {
            var director = LevelDirector.Current ?? FindAnyObjectByType<LevelDirector>();
            if (director != null)
            {
                director.OnLevelComplete -= HandleLevelComplete;
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            // Editor-only test shortcut (F10) for rapid verification of the ending UI & restart flow
            var kb = Keyboard.current;
            if (kb != null && kb.f10Key.wasPressedThisFrame && !hasTriggered)
            {
                Debug.Log("[FinalEndingController] Editor test trigger (F10) engaged!");
                BeginEndingSequence();
            }
#endif
        }

        private void HandleLevelComplete()
        {
            var director = LevelDirector.Current ?? FindAnyObjectByType<LevelDirector>();
            if (director != null && director.IsFinalLevel && !hasTriggered)
            {
                // Verify boss is genuinely defeated
                var boss = BossDirector.Instance ?? FindAnyObjectByType<BossDirector>();
                if (boss != null && boss.CurrentPhase != BossDirector.BossPhase.Defeated)
                {
                    Debug.LogWarning("[FinalEndingController] Ignoring completion: Boss not in Defeated phase.");
                    return;
                }

                BeginEndingSequence();
            }
        }

        public void BeginEndingSequence()
        {
            if (hasTriggered) return;
            hasTriggered = true;

            Debug.Log("[FinalEndingController] BEGINNING CINEMATIC FINAL PLOT-TWIST SEQUENCE!");

            // Lock player controls safely
            var player = FindAnyObjectByType<PlayerController2D>();
            if (player != null)
            {
                player.InputLocked = true;
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }

            // Suppress standard in-game subtitle banners to keep ending text clean
            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.StopAllCoroutines();
                var banner = GameObject.Find("ArchitectSubtitleBanner");
                if (banner != null) banner.SetActive(false);
            }

            var hud = FindAnyObjectByType<LevelHUD>();
            if (hud != null && hud.CompletionPanel != null)
            {
                hud.CompletionPanel.SetActive(false);
            }

            // Ensure cursor is freed for final button interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (endingCoroutine != null) StopCoroutine(endingCoroutine);
            endingCoroutine = StartCoroutine(ExecuteEndingSequenceRoutine());
        }

        private IEnumerator ExecuteEndingSequenceRoutine()
        {
            // Activate ending canvas group
            if (endingCanvasGroup != null)
            {
                endingCanvasGroup.blocksRaycasts = true;
                endingCanvasGroup.interactable = true;
            }

            // =========================================================================
            // BEAT 1 — FALSE VICTORY
            // =========================================================================
            // Fade overlay to dark/black
            yield return StartCoroutine(FadeCanvasGroup(endingCanvasGroup, 0f, 1f, 1.2f));

            if (primaryStatusText != null)
            {
                primaryStatusText.text = "<color=#00FF88>ARCHITECT CORE DISABLED</color>";
                primaryStatusText.gameObject.SetActive(true);
            }
            yield return new WaitForSecondsRealtime(2.0f);

            if (secondaryStatusText != null)
            {
                secondaryStatusText.text = "<color=#00F0FF>EXTERNAL CONTAINMENT ACCESS GRANTED</color>";
                secondaryStatusText.gameObject.SetActive(true);
            }
            yield return new WaitForSecondsRealtime(2.2f);

            if (truthClimaxText != null)
            {
                truthClimaxText.text = "<color=#D8F4FF>Congratulations, Subject 075.\nYou finally escaped.</color>";
                truthClimaxText.gameObject.SetActive(true);
            }
            yield return new WaitForSecondsRealtime(3.2f);

            // =========================================================================
            // BEAT 2 — SILENCE
            // =========================================================================
            // Fade dialogue out, hold pitch-black screen
            if (primaryStatusText != null) primaryStatusText.gameObject.SetActive(false);
            if (secondaryStatusText != null) secondaryStatusText.gameObject.SetActive(false);
            if (truthClimaxText != null) truthClimaxText.gameObject.SetActive(false);
            if (consequenceText != null) consequenceText.gameObject.SetActive(false);

            yield return new WaitForSecondsRealtime(2.0f);

            // =========================================================================
            // BEAT 3 — THE TRUTH
            // =========================================================================
            if (truthClimaxText != null)
            {
                truthClimaxText.gameObject.SetActive(true);

                truthClimaxText.text = "<color=#EEEEEE>SUBJECT 075...</color>";
                yield return new WaitForSecondsRealtime(2.0f);

                truthClimaxText.text = "<color=#EEEEEE>SUBJECT 075...\n\nYOU NEVER UNDERSTOOD.</color>";
                yield return new WaitForSecondsRealtime(2.4f);

                truthClimaxText.text = "<color=#EEEEEE>SUBJECT 075...\n\nYOU NEVER UNDERSTOOD.\n\nI WASN'T KEEPING YOU IN.</color>";
                yield return new WaitForSecondsRealtime(3.0f);

                // Climactic revelation line in deep crimson
                truthClimaxText.text = "<color=#EEEEEE>SUBJECT 075...\n\nYOU NEVER UNDERSTOOD.\n\nI WASN'T KEEPING YOU IN.\n\n</color><color=#FF1E40><b>I WAS KEEPING THEM OUT.</b></color>";
                yield return new WaitForSecondsRealtime(4.0f);
            }

            // =========================================================================
            // BEAT 4 — THE CONSEQUENCE
            // =========================================================================
            if (truthClimaxText != null) truthClimaxText.gameObject.SetActive(false);

            if (consequenceText != null)
            {
                consequenceText.gameObject.SetActive(true);

                consequenceText.text = "<color=#FF2233>GLOBAL QUARANTINE: DISABLED</color>";
                yield return new WaitForSecondsRealtime(1.8f);

                consequenceText.text = "<color=#FF2233>GLOBAL QUARANTINE: DISABLED\n\nEXTERNAL PERIMETER BREACHED</color>";
                yield return new WaitForSecondsRealtime(1.8f);

                consequenceText.text = "<color=#FF2233>GLOBAL QUARANTINE: DISABLED\n\nEXTERNAL PERIMETER BREACHED\n\n</color><color=#FF8899>They know the door is open.</color>";
                yield return new WaitForSecondsRealtime(3.2f);
            }

            // =========================================================================
            // BEAT 5 — FINAL END SCREEN
            // =========================================================================
            if (consequenceText != null) consequenceText.gameObject.SetActive(false);

            if (finalEndScreenPanel != null)
            {
                finalEndScreenPanel.SetActive(true);
            }

            if (restartSimulationButton != null)
            {
                restartSimulationButton.gameObject.SetActive(true);
                restartSimulationButton.interactable = true;
            }
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
        {
            if (cg == null) yield break;
            float elapsed = 0f;
            cg.alpha = start;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
                yield return null;
            }
            cg.alpha = end;
        }

        public void OnRestartSimulationClicked()
        {
            if (isRestarting) return;
            isRestarting = true;

            Debug.Log("[FinalEndingController] Restarting Simulation -> Returning to MainMenu!");

            if (endingCoroutine != null)
            {
                StopCoroutine(endingCoroutine);
                endingCoroutine = null;
            }

            Time.timeScale = 1.0f;

            // Release player lock
            var player = FindAnyObjectByType<PlayerController2D>();
            if (player != null)
            {
                player.InputLocked = false;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Fully reset campaign and session state
            BossDirector.ResetSessionCheckpoint();
            CollapsingFloor.ResetSessionFlag();
            ImpossibleQuestionPrank.ResetSessionFlag();
            ChapterIntroController.ResetSeenChapters();
            ParanoiaTrapPrank.ResetSessionFlag();

            if (GameSession.Instance != null)
            {
                GameSession.Instance.ResetSession();
            }

            // Load MainMenu by name
            SceneManager.LoadScene("MainMenu");
        }
    }
}
