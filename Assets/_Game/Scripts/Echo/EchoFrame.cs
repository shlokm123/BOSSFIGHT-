using System;
using UnityEngine;

namespace GlitchVerse.Echo
{
    /// <summary>
    /// Lightweight telemetry snapshot captured by EchoRecorder.
    /// Stores timestamp, 2D coordinates, and facing orientation.
    /// </summary>
    [Serializable]
    public struct EchoFrame
    {
        public float timestamp;
        public Vector2 position;
        public bool facingRight;

        public EchoFrame(float timestamp, Vector2 position, bool facingRight)
        {
            this.timestamp = timestamp;
            this.position = position;
            this.facingRight = facingRight;
        }
    }
}
