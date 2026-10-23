using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using GlitchVerse.Player;

namespace GlitchVerse.Echo
{
    /// <summary>
    /// Interactive pressure switch sensitive to timeline echoes (EchoGhostMarker)
    /// as well as the live player. Used for cooperative single-player temporal puzzles.
    /// Follows the architecture defined in docs/INTERFACES.md and docs/PRODUCTION_PLAN.md.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class EchoSwitch : MonoBehaviour
    {
        [Header("Switch Configuration")]
        [SerializeField] private bool latching = false;
        [SerializeField] private bool allowPlayerActivation = true;
        [SerializeField] private bool allowEchoActivation = true;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer switchRenderer;
        [SerializeField] private Light2D glowLight;
        [SerializeField] private Color inactiveColor = new Color(0.12f, 0.28f, 0.38f, 1f);
        [SerializeField] private Color activeColor = new Color(0.84f, 0f, 1f, 1f); // Holographic Magenta #D600FF
        [SerializeField] private Transform depressVisual;
        [SerializeField] private float depressDistance = 0.08f;

        [Header("State")]
        [SerializeField] private bool isActivated = false;
        private BoxCollider2D triggerCol;
        private Vector3 initialVisualPos;

        public bool IsActivated => isActivated;
        public event Action<bool> OnSwitchStateChanged;

        private void Awake()
        {
            triggerCol = GetComponent<BoxCollider2D>();
            triggerCol.isTrigger = true;

            if (depressVisual != null)
            {
                initialVisualPos = depressVisual.localPosition;
            }

            UpdateVisuals();
        }

        private void FixedUpdate()
        {
            if (triggerCol == null) return;

            // Continuous overlap check for both Live Player and Echo Ghost
            var hits = Physics2D.OverlapBoxAll(triggerCol.bounds.center, triggerCol.bounds.size, 0f);
            bool occupied = false;

            foreach (var hit in hits)
            {
                if (hit == null || hit.gameObject == gameObject) continue;

                if (allowEchoActivation && hit.GetComponent<EchoGhostMarker>() != null)
                {
                    occupied = true;
                    break;
                }

                if (allowPlayerActivation && hit.GetComponent<PlayerController2D>() != null)
                {
                    occupied = true;
                    break;
                }
            }

            if (latching)
            {
                if (occupied && !isActivated)
                {
                    SetActivated(true);
                }
            }
            else
            {
                if (occupied != isActivated)
                {
                    SetActivated(occupied);
                }
            }
        }

        private void SetActivated(bool active)
        {
            isActivated = active;
            UpdateVisuals();
            OnSwitchStateChanged?.Invoke(isActivated);
            Debug.Log($"[EchoSwitch] {gameObject.name} state changed. Activated: {isActivated}");
        }

        private void UpdateVisuals()
        {
            if (switchRenderer != null)
            {
                switchRenderer.color = isActivated ? activeColor : inactiveColor;
            }

            if (glowLight != null)
            {
                glowLight.color = isActivated ? activeColor : inactiveColor;
                glowLight.intensity = isActivated ? 1.4f : 0.3f;
            }

            if (depressVisual != null)
            {
                Vector3 targetPos = initialVisualPos;
                if (isActivated)
                {
                    targetPos.y -= depressDistance;
                }
                depressVisual.localPosition = targetPos;
            }
        }
    }
}
