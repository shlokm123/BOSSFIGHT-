using System;
using System.Collections.Generic;
using UnityEngine;
using GlitchVerse.Player;

namespace GlitchVerse.Echo
{
    /// <summary>
    /// Bounded recording system that samples player position and orientation
    /// at fixed intervals. Produces immutable snapshots for EchoPlayback.
    /// Follows the architecture defined in docs/INTERFACES.md.
    /// </summary>
    public class EchoRecorder : MonoBehaviour
    {
        [Header("Sampling Configuration")]
        [SerializeField] private float sampleInterval = 0.05f; // 20 Hz
        [SerializeField] private float maxDuration = 15.0f;     // 15 seconds max (300 frames)
        [SerializeField] private PlayerController2D targetPlayer;

        private readonly List<EchoFrame> recordedFrames = new List<EchoFrame>();
        private float sampleTimer = 0f;
        private float recordingStartTime;
        private bool isRecording = true;
        private int maxFrames;

        public bool IsRecording => isRecording;
        public int FrameCount => recordedFrames.Count;
        public event Action<EchoFrame[]> OnSnapshotReady;

        private void Awake()
        {
            maxFrames = Mathf.CeilToInt(maxDuration / sampleInterval);
            if (targetPlayer == null)
            {
                targetPlayer = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);
            }
        }

        private void Start()
        {
            recordingStartTime = Time.fixedTime;
        }

        private void FixedUpdate()
        {
            if (!isRecording || targetPlayer == null) return;

            sampleTimer += Time.fixedDeltaTime;
            if (sampleTimer >= sampleInterval)
            {
                sampleTimer -= sampleInterval;
                RecordFrame();
            }
        }

        private void RecordFrame()
        {
            float relativeTime = Time.fixedTime - recordingStartTime;
            EchoFrame frame = new EchoFrame(
                relativeTime,
                targetPlayer.Position,
                targetPlayer.FacingRight
            );

            // Bounded memory enforcement: remove oldest if exceeding capacity
            if (recordedFrames.Count >= maxFrames)
            {
                recordedFrames.RemoveAt(0);
            }

            recordedFrames.Add(frame);
        }

        /// <summary>
        /// Returns an immutable copy of the captured telemetry frames.
        /// </summary>
        public EchoFrame[] GetSnapshot()
        {
            if (recordedFrames.Count == 0) return Array.Empty<EchoFrame>();
            var snapshot = recordedFrames.ToArray();
            OnSnapshotReady?.Invoke(snapshot);
            return snapshot;
        }

        /// <summary>
        /// Resets the recording buffer and restarts recording from current player position.
        /// </summary>
        public void ResetRecording()
        {
            recordedFrames.Clear();
            recordingStartTime = Time.time;
            sampleTimer = 0f;
            Debug.Log("[EchoRecorder] Recording buffer cleared and restarted.");
        }
    }
}
