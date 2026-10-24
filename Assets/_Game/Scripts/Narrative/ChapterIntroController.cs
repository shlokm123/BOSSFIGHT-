using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using GlitchVerse.Core;
using GlitchVerse.Player;

namespace GlitchVerse.Narrative
{
    /// <summary>
    /// Horror-inspired fullscreen chapter introduction screen that plays on first
    /// chamber entry. Freezes gameplay and input, then fades into the chamber.
    /// Does not replay on deaths or restarts of the same chapter.
    /// Follows Phase 07.5 Checkpoint 1 requirements.
    /// </summary>
    public class ChapterIntroController : MonoBehaviour
    {
        public static ChapterIntroController Current { get; private set; }

        // Tracks seen chapters across scenes in this play session
        private static readonly HashSet<int> SeenChapters = new HashSet<int>();
        public static event Action OnIntroConcluded;

        [Header("UI Canvas Elements")]
        [SerializeField] private GameObject introPanel;
        [SerializeField] private Image backgroundOverlay;
        [SerializeField] private Text chapterTitleText;
        [SerializeField] private Text chapterSubtitleText;
        [SerializeField] private Text skipHintText;

        [Header("Dependencies")]
        [SerializeField] private LevelDirector levelDirector;
        [SerializeField] private PlayerController2D player;

        [Header("Timing")]
        [SerializeField] private float displayDuration = 2.4f;
        [SerializeField] private float fadeDuration = 0.4f;

        private bool isIntroPlaying = false;
        private bool canSkip = false;
        private Coroutine introRoutine;

        public bool IsIntroPlaying => isIntroPlaying;

        private static readonly char[] GlitchChars = new char[] { '░', '█', '▒', '▓', '†', '‡', '§', '#', 'Ø', 'X' };

        private void Awake()
        {
            Current = this;
            if (levelDirector == null) levelDirector = LevelDirector.Current;
            if (player == null) player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);

            int chamberNum = GetChamberNumber();
            if (SeenChapters.Contains(chamberNum))
            {
                if (introPanel != null) introPanel.SetActive(false);
                if (player != null) player.InputLocked = false;
            }
        }

        private void Start()
        {
            if (levelDirector == null) levelDirector = LevelDirector.Current;
            if (player == null) player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);

            int chamberNum = GetChamberNumber();

            // Rule: Do NOT replay on every death or restart of the same chapter
            if (SeenChapters.Contains(chamberNum))
            {
                if (introPanel != null) introPanel.SetActive(false);
                if (player != null) player.InputLocked = false;
                OnIntroConcluded?.Invoke();
                return;
            }

            SeenChapters.Add(chamberNum);
            PlayChapterIntro(chamberNum);
        }

        private int GetChamberNumber()
        {
            if (levelDirector != null) return levelDirector.ChamberNumber;
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (sceneName.StartsWith("Level") && sceneName.Length >= 7 && int.TryParse(sceneName.Substring(5, 2), out int parsed))
            {
                return parsed;
            }
            return 1;
        }

        private void OnDestroy()
        {
            if (Current == this)
            {
                Current = null;
            }
        }

        private void Update()
        {
            if (!isIntroPlaying) return;

            // Space to skip after 0.5s
            if (canSkip)
            {
                var kb = Keyboard.current;
                if (kb != null && kb.spaceKey.wasPressedThisFrame)
                {
                    if (introRoutine != null)
                    {
                        StopCoroutine(introRoutine);
                        StartCoroutine(FadeOutAndRestoreRoutine());
                    }
                }
            }
        }

        public void PlayChapterIntro(int chamberNum)
        {
            if (introPanel == null)
            {
                OnIntroConcluded?.Invoke();
                return;
            }

            string chapterRom = "I";
            string titleName = "DECEPTION";
            string subtitle = "Nothing here is what it seems.";

            switch (chamberNum)
            {
                case 1:
                    chapterRom = "I";
                    titleName = "DECEPTION";
                    subtitle = "Nothing here is what it seems.";
                    break;
                case 2:
                    chapterRom = "II";
                    titleName = "INVERSION";
                    subtitle = "Gravity was never a rule.";
                    break;
                case 3:
                    chapterRom = "III";
                    titleName = "BETRAYAL";
                    subtitle = "Your past knows too much.";
                    break;
                case 4:
                    chapterRom = "IV";
                    titleName = "REVELATION";
                    subtitle = "Some things disappear when you stop looking. Others finally become visible.";
                    break;
                case 5:
                    chapterRom = "V";
                    titleName = "PARANOIA";
                    subtitle = "Trust nothing. Especially yourself.";
                    break;
                case 6:
                    chapterRom = "VI";
                    titleName = "TRUTH";
                    subtitle = "Some doors were locked for a reason.";
                    break;
                case 7:
                    chapterRom = "VII";
                    titleName = "JUDGMENT";
                    subtitle = "You were never supposed to reach this room.";
                    break;
            }

            introRoutine = StartCoroutine(IntroSequenceRoutine(chapterRom, titleName, subtitle));
        }

        private IEnumerator IntroSequenceRoutine(string roman, string title, string sub)
        {
            isIntroPlaying = true;
            canSkip = false;

            // Lock player movement during intro
            if (player != null)
            {
                player.InputLocked = true;
            }

            introPanel.SetActive(true);
            if (backgroundOverlay != null)
            {
                backgroundOverlay.color = Color.black;
            }

            if (skipHintText != null)
            {
                skipHintText.gameObject.SetActive(false);
            }

            // 1. Digital horror glitch entrance (0.15s)
            float glitchTime = 0f;
            string targetTitle = $"CHAPTER {roman} — {title}";
            while (glitchTime < 0.15f)
            {
                glitchTime += Time.unscaledDeltaTime;
                char[] scrambled = targetTitle.ToCharArray();
                for (int i = 0; i < scrambled.Length; i++)
                {
                    if (scrambled[i] != ' ' && UnityEngine.Random.value > 0.5f)
                    {
                        scrambled[i] = GlitchChars[UnityEngine.Random.Range(0, GlitchChars.Length)];
                    }
                }

                if (chapterTitleText != null)
                {
                    chapterTitleText.text = $"<color=#FF1E40>{new string(scrambled)}</color>";
                }
                yield return new WaitForSecondsRealtime(0.03f);
            }

            // 2. Crisp, horror-styled display
            if (chapterTitleText != null)
            {
                chapterTitleText.text = $"<color=#FF1E40>CHAPTER {roman}</color>\n<size=36><color=#F0F0F0><b>{title}</b></color></size>";
            }

            if (chapterSubtitleText != null)
            {
                chapterSubtitleText.text = $"<color=#9095A0>\"{sub}\"</color>";
            }

            // Allow skipping after 0.5s
            yield return new WaitForSecondsRealtime(0.5f);
            canSkip = true;
            if (skipHintText != null)
            {
                skipHintText.gameObject.SetActive(true);
                skipHintText.text = "[SPACE] CONTINUE";
            }

            // Hold for remaining duration
            yield return new WaitForSecondsRealtime(displayDuration - 0.5f);

            yield return StartCoroutine(FadeOutAndRestoreRoutine());
        }

        private IEnumerator FadeOutAndRestoreRoutine()
        {
            isIntroPlaying = false;
            canSkip = false;

            float elapsed = 0f;
            CanvasGroup group = introPanel.GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = introPanel.AddComponent<CanvasGroup>();
            }

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }

            introPanel.SetActive(false);
            group.alpha = 1f;

            // Restore gameplay physics and player input
            if (player == null)
            {
                player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);
            }

            if (player != null)
            {
                player.InputLocked = false;
            }

            Debug.Log("[ChapterIntroController] Intro concluded. Gameplay restored.");
            OnIntroConcluded?.Invoke();
        }

        /// <summary>
        /// Clears seen chapters (useful if a new game session starts).
        /// </summary>
        public static void ResetSeenChapters()
        {
            SeenChapters.Clear();
        }
    }
}
