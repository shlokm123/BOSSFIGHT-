using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GlitchVerse.Narrative
{
    /// <summary>
    /// Lightweight narrative coordinator that displays The Architect's psychological
    /// roasts, alerts, and mind games as nonblocking, queued screen subtitles with
    /// glitch entrance animations and monitor synchronization.
    /// Follows Phase 07 directives.
    /// </summary>
    public class ArchitectDialogue : MonoBehaviour
    {
        public static ArchitectDialogue Current { get; private set; }

        [Header("UI Subtitle Elements")]
        [SerializeField] private Text subtitleText;
        [SerializeField] private Image flashOverlay;
        [SerializeField] private TextMesh monitorTextMesh;

        [Header("Dialogue Timing Settings")]
        [SerializeField] private float defaultDisplayDuration = 2.8f;
        [SerializeField] private float glitchEntranceDuration = 0.12f;

        private struct DialogueEntry
        {
            public string text;
            public string monitorNote;
            public float duration;
            public string cooldownKey;
            public Color textColor;
        }

        private readonly Queue<DialogueEntry> messageQueue = new Queue<DialogueEntry>();
        private readonly Dictionary<string, float> cooldownTimers = new Dictionary<string, float>();
        private Coroutine activeDialogueRoutine;
        private Coroutine flashRoutine;

        private static readonly char[] GlitchChars = new char[] { '░', '█', '▒', '▓', '#', '$', '%', '&', '/', '<', '>', '?' };

        private void Awake()
        {
            Current = this;

            if (subtitleText != null)
            {
                subtitleText.text = "";
                subtitleText.gameObject.SetActive(false);
            }

            if (flashOverlay != null)
            {
                flashOverlay.color = new Color(1f, 0f, 0.15f, 0f);
                flashOverlay.raycastTarget = false;
            }

            if (monitorTextMesh == null)
            {
                var monitorObj = GameObject.Find("Monitor_Text");
                if (monitorObj != null)
                {
                    monitorTextMesh = monitorObj.GetComponent<TextMesh>();
                }
            }
        }

        private void OnDestroy()
        {
            if (Current == this)
            {
                Current = null;
            }
        }

        /// <summary>
        /// Requests an Architect subtitle message. Enforces cooldown limits per key.
        /// Nonblocking and queues cleanly.
        /// </summary>
        public void Speak(string message, string monitorNote = null, float duration = 2.8f, 
                          string cooldownKey = null, float cooldownSeconds = 15f, bool priority = false)
        {
            if (!string.IsNullOrEmpty(cooldownKey))
            {
                if (cooldownTimers.TryGetValue(cooldownKey, out float lastTime))
                {
                    if (Time.time - lastTime < cooldownSeconds)
                    {
                        // Cooldown active, skip spam
                        return;
                    }
                }
                cooldownTimers[cooldownKey] = Time.time;
            }

            var entry = new DialogueEntry
            {
                text = message,
                monitorNote = monitorNote,
                duration = duration > 0f ? duration : defaultDisplayDuration,
                cooldownKey = cooldownKey,
                textColor = Color.white
            };

            if (priority)
            {
                // If high priority, stop current and speak immediately
                if (activeDialogueRoutine != null)
                {
                    StopCoroutine(activeDialogueRoutine);
                    activeDialogueRoutine = null;
                }
                messageQueue.Clear();
                activeDialogueRoutine = StartCoroutine(DisplayDialogueRoutine(entry));
            }
            else
            {
                messageQueue.Enqueue(entry);
                if (activeDialogueRoutine == null)
                {
                    activeDialogueRoutine = StartCoroutine(ProcessQueueRoutine());
                }
            }
        }

        private IEnumerator ProcessQueueRoutine()
        {
            while (messageQueue.Count > 0)
            {
                var entry = messageQueue.Dequeue();
                yield return StartCoroutine(DisplayDialogueRoutine(entry));
                yield return new WaitForSeconds(0.2f); // Brief buffer between lines
            }
            activeDialogueRoutine = null;
        }

        private IEnumerator DisplayDialogueRoutine(DialogueEntry entry)
        {
            if (subtitleText == null) yield break;

            subtitleText.gameObject.SetActive(true);

            // Update monitor if provided
            if (monitorTextMesh != null && !string.IsNullOrEmpty(entry.monitorNote))
            {
                monitorTextMesh.text = $"[ARCHITECT]\n{entry.monitorNote.ToUpper()}";
            }

            // 1. Subtle digital glitch entrance animation
            string rawPrefix = "<color=#FF1E40>[ARCHITECT]</color> ";
            string targetMessage = entry.text;
            float elapsed = 0f;

            while (elapsed < glitchEntranceDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                char[] scrambled = targetMessage.ToCharArray();
                for (int i = 0; i < scrambled.Length; i++)
                {
                    if (scrambled[i] != ' ' && UnityEngine.Random.value > 0.45f)
                    {
                        scrambled[i] = GlitchChars[UnityEngine.Random.Range(0, GlitchChars.Length)];
                    }
                }
                subtitleText.text = rawPrefix + "<color=#00F0FF>" + new string(scrambled) + "</color>";
                yield return new WaitForSecondsRealtime(0.03f);
            }

            // 2. Clear legible display
            subtitleText.text = rawPrefix + "<color=#FFFFFF>" + targetMessage + "</color>";

            // 3. Readable hold duration
            yield return new WaitForSeconds(entry.duration);

            // 4. Subtle exit
            subtitleText.text = "";
            subtitleText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Flashes the screen edge with a momentary red emergency pulse.
        /// </summary>
        public void FlashScreenRed(float peakAlpha = 0.35f, float duration = 0.22f)
        {
            if (flashOverlay == null) return;

            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }
            flashRoutine = StartCoroutine(FlashScreenRoutine(peakAlpha, duration));
        }

        private IEnumerator FlashScreenRoutine(float peakAlpha, float duration)
        {
            float elapsed = 0f;
            Color baseColor = new Color(1.0f, 0.1f, 0.2f, peakAlpha);
            Color clearColor = new Color(1.0f, 0.1f, 0.2f, 0f);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                flashOverlay.color = Color.Lerp(baseColor, clearColor, t);
                yield return null;
            }

            flashOverlay.color = clearColor;
            flashRoutine = null;
        }

        /// <summary>
        /// Explicit helper to set the monitor text directly.
        /// </summary>
        public void SetMonitorStatus(string status)
        {
            if (monitorTextMesh != null)
            {
                monitorTextMesh.text = $"[ARCHITECT]\n{status.ToUpper()}";
            }
        }
    }
}
