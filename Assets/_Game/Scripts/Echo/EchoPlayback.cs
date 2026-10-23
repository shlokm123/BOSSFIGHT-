using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlitchVerse.Echo
{
    /// <summary>
    /// Replays captured EchoFrame telemetry using a dedicated translucent cyan ghost.
    /// Demonstrates deterministic timeline replay without physical interference.
    /// Triggered via 'H' key or direct method call.
    /// Follows docs/INTERFACES.md specifications.
    /// </summary>
    public class EchoPlayback : MonoBehaviour
    {
        [Header("Playback Dependencies")]
        [SerializeField] private EchoRecorder recorder;
        [SerializeField] private GameObject ghostPrefab;
        [SerializeField] private Sprite ghostSprite;

        [Header("Visual Styling")]
        [SerializeField] private Color ghostCyanColor = new Color(0f, 0.94f, 1f, 0.55f); // Translucent Cyan #00F0FF
        [SerializeField] private int ghostSortingOrder = 5;

        private GameObject ghostInstance;
        private SpriteRenderer ghostRenderer;
        private EchoFrame[] currentSnapshot;
        private bool isReplaying = false;
        private float playbackTimer = 0f;
        private int currentFrameIndex = 0;

        public bool IsReplaying => isReplaying;
        public event Action OnPlaybackStarted;
        public event Action OnPlaybackFinished;

        private void Awake()
        {
            if (recorder == null)
            {
                recorder = FindFirstObjectByType<EchoRecorder>(FindObjectsInactive.Exclude);
            }

            CreateGhostVisualInstance();
        }

        private void Update()
        {
            // Requirement: H key to replay the most recent completed recording
            var kb = Keyboard.current;
            if (kb != null && kb.hKey.wasPressedThisFrame)
            {
                TriggerReplay();
            }

            if (isReplaying)
            {
                UpdatePlayback();
            }
        }

        private void CreateGhostVisualInstance()
        {
            if (ghostInstance != null) return;

            ghostInstance = new GameObject("EchoGhost_Visual");
            ghostRenderer = ghostInstance.AddComponent<SpriteRenderer>();
            if (ghostSprite != null)
            {
                ghostRenderer.sprite = ghostSprite;
            }
            ghostRenderer.color = ghostCyanColor;
            ghostRenderer.sortingOrder = ghostSortingOrder;

            // Ensure no physics colliders exist on the visual ghost
            var colliders = ghostInstance.GetComponentsInChildren<Collider2D>();
            foreach (var col in colliders)
            {
                Destroy(col);
            }

            ghostInstance.SetActive(false);
        }

        /// <summary>
        /// Initiates replay of the latest recorder snapshot.
        /// </summary>
        public void TriggerReplay()
        {
            if (recorder == null) return;

            currentSnapshot = recorder.GetSnapshot();
            if (currentSnapshot == null || currentSnapshot.Length < 2)
            {
                Debug.LogWarning("[EchoPlayback] Not enough recorded frames to replay.");
                return;
            }

            isReplaying = true;
            playbackTimer = 0f;
            currentFrameIndex = 0;

            if (ghostInstance != null)
            {
                ghostInstance.SetActive(true);
                ghostInstance.transform.position = currentSnapshot[0].position;
            }

            OnPlaybackStarted?.Invoke();
            Debug.Log($"[EchoPlayback] Replaying {currentSnapshot.Length} frames...");
        }

        private void UpdatePlayback()
        {
            if (currentSnapshot == null || currentSnapshot.Length == 0)
            {
                StopPlayback();
                return;
            }

            playbackTimer += Time.deltaTime;

            // Find current frame in snapshot
            while (currentFrameIndex < currentSnapshot.Length - 1 &&
                   playbackTimer > (currentSnapshot[currentFrameIndex + 1].timestamp - currentSnapshot[0].timestamp))
            {
                currentFrameIndex++;
            }

            if (currentFrameIndex >= currentSnapshot.Length - 1)
            {
                // Reached end of trajectory
                ghostInstance.transform.position = currentSnapshot[currentSnapshot.Length - 1].position;
                StopPlayback();
                return;
            }

            // Interpolate smoothly between current and next frame
            EchoFrame frameA = currentSnapshot[currentFrameIndex];
            EchoFrame frameB = currentSnapshot[currentFrameIndex + 1];

            float tA = frameA.timestamp - currentSnapshot[0].timestamp;
            float tB = frameB.timestamp - currentSnapshot[0].timestamp;
            float duration = Mathf.Max(0.0001f, tB - tA);
            float factor = Mathf.Clamp01((playbackTimer - tA) / duration);

            Vector2 interpPos = Vector2.Lerp(frameA.position, frameB.position, factor);
            ghostInstance.transform.position = interpPos;

            // Update facing orientation
            if (ghostRenderer != null)
            {
                ghostRenderer.flipX = !frameA.facingRight;
            }
        }

        public void StopPlayback()
        {
            isReplaying = false;
            if (ghostInstance != null)
            {
                // Leave ghost at final rest point with diminished opacity
                ghostRenderer.color = new Color(ghostCyanColor.r, ghostCyanColor.g, ghostCyanColor.b, 0.25f);
            }
            OnPlaybackFinished?.Invoke();
            Debug.Log("[EchoPlayback] Replay completed.");
        }
    }
}
