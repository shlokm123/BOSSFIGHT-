using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using GlitchVerse.Narrative;
using GlitchVerse.Player;

namespace GlitchVerse.Interactables
{
    /// <summary>
    /// Interactive secondary breaker switch on Platform B in Chapter I.
    /// Powers up when the Fake Exit Prank triggers, and requires the player
    /// to jump up and step on it to drop the security barrier at the real exit.
    /// Follows Phase 07 / Difficulty Upgrade 01 specifications.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class SecurityOverrideSwitch : MonoBehaviour
    {
        [Header("Target Interlock")]
        [SerializeField] private SecurityBarrier targetBarrier;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer switchRenderer;
        [SerializeField] private Light2D switchLight;
        [SerializeField] private Transform depressVisual;
        [SerializeField] private Text consoleStatusText;

        [Header("State Colors")]
        [SerializeField] private Color unpoweredColor = new Color(0.28f, 0.32f, 0.38f, 0.7f);
        [SerializeField] private Color poweredColor = new Color(1.0f, 0.65f, 0.15f, 1f);   // Warning Amber
        [SerializeField] private Color activatedColor = new Color(0.0f, 0.95f, 1.0f, 1f);   // Cyan Cleared

        [Header("State")]
        [SerializeField] private bool isPowered = false;
        [SerializeField] private bool isActivated = false;

        private Vector3 initialVisualPos;
        public bool IsPowered => isPowered;
        public bool IsActivated => isActivated;

        public event Action OnSwitchActivated;

        private void Awake()
        {
            var col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;

            if (depressVisual != null)
            {
                initialVisualPos = depressVisual.localPosition;
            }

            UpdateVisuals();
        }

        public void SetSwitchPowered(bool powered)
        {
            isPowered = powered;
            UpdateVisuals();
            Debug.Log($"[SecurityOverrideSwitch] Powered: {powered}");
        }

        public void ResetSwitch()
        {
            isPowered = false;
            isActivated = false;
            if (depressVisual != null)
            {
                depressVisual.localPosition = initialVisualPos;
            }
            UpdateVisuals();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CheckActivation(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            CheckActivation(other);
        }

        private void CheckActivation(Collider2D other)
        {
            if (!isPowered || isActivated) return;

            // Activated when live player or movable cube contacts the switch
            bool isPlayer = other.GetComponent<PlayerController2D>() != null || other.CompareTag("Player");
            bool isCube = other.GetComponent<MovableCube>() != null;

            if (isPlayer || isCube)
            {
                Debug.Log($"[SecurityOverrideSwitch] Activated by {other.gameObject.name} (tag: {other.tag}) at position {other.transform.position}");
                ActivateSwitch();
            }
        }

        public void ActivateSwitch()
        {
            if (isActivated) return;

            isActivated = true;
            Debug.Log("[SecurityOverrideSwitch] Player engaged secondary override breaker!");

            if (depressVisual != null)
            {
                depressVisual.localPosition = initialVisualPos + new Vector3(0f, -0.08f, 0f);
            }

            UpdateVisuals();

            // Unlock target barrier
            if (targetBarrier != null)
            {
                targetBarrier.UnlockBarrier();
            }

            // Narrative feedback from Architect
            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.Speak(
                    "Did you really think a breaker switch makes you clever?",
                    "SECURITY OVERRIDE",
                    2.8f,
                    "ch1_switch_override",
                    0f,
                    priority: true
                );
            }

            OnSwitchActivated?.Invoke();
        }

        private void UpdateVisuals()
        {
            Color curColor = isActivated ? activatedColor : (isPowered ? poweredColor : unpoweredColor);

            if (switchRenderer != null)
            {
                switchRenderer.color = curColor;
            }

            if (switchLight != null)
            {
                switchLight.color = curColor;
                switchLight.intensity = isActivated ? 1.6f : (isPowered ? 1.2f : 0.25f);
            }

            if (consoleStatusText != null)
            {
                if (isActivated)
                {
                    consoleStatusText.text = "<color=#00F0FF>[OVERRIDE ENGAGED]</color>\n<size=12><color=#00FF88>BARRIER OFFLINE</color></size>";
                }
                else if (isPowered)
                {
                    consoleStatusText.text = "<color=#FFAA00>[BREAKER ONLINE]</color>\n<size=12><color=#FFFFFF>STEP TO RELEASE INTERLOCK</color></size>";
                }
                else
                {
                    consoleStatusText.text = "<color=#666666>[STANDBY]</color>\n<size=12><color=#444444>AWAITING POWER ROUTE</color></size>";
                }
            }
        }
    }
}
