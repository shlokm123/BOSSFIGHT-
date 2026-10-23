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
    /// Supports hold grace window and Echo-only gating for Chapter I Difficulty Upgrade 01B.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class EchoSwitch : MonoBehaviour
    {
        [Header("Switch Configuration")]
        [SerializeField] protected bool latching = false;
        [SerializeField] protected bool allowPlayerActivation = true;
        [SerializeField] protected bool allowEchoActivation = true;
        [SerializeField] protected float holdGraceDuration = 0f;
        [SerializeField] protected bool isPowered = true;

        [Header("Visual Feedback")]
        [SerializeField] protected SpriteRenderer switchRenderer;
        [SerializeField] protected Light2D glowLight;
        [SerializeField] protected Color inactiveColor = new Color(0.12f, 0.28f, 0.38f, 1f);
        [SerializeField] protected Color activeColor = new Color(0.84f, 0f, 1f, 1f); // Holographic Magenta #D600FF
        [SerializeField] protected Transform depressVisual;
        [SerializeField] protected float depressDistance = 0.08f;
        [SerializeField] protected UnityEngine.UI.Text consoleStatusText;

        [Header("State")]
        [SerializeField] protected bool isActivated = false;
        protected BoxCollider2D triggerCol;
        protected Vector3 initialVisualPos;
        private float currentGraceTimer = 0f;

        public bool IsActivated => isActivated;
        public bool IsPowered => isPowered;
        public bool AllowPlayerActivation { get => allowPlayerActivation; set => allowPlayerActivation = value; }
        public bool AllowEchoActivation { get => allowEchoActivation; set => allowEchoActivation = value; }
        public float HoldGraceDuration { get => holdGraceDuration; set => holdGraceDuration = value; }
        public event Action<bool> OnSwitchStateChanged;

        protected virtual void Awake()
        {
            triggerCol = GetComponent<BoxCollider2D>();
            triggerCol.isTrigger = true;

            if (depressVisual != null)
            {
                initialVisualPos = depressVisual.localPosition;
            }

            UpdateVisuals();
        }

        public virtual void SetSwitchPowered(bool powered)
        {
            isPowered = powered;
            if (!isPowered && isActivated)
            {
                SetActivated(false);
            }
            UpdateVisuals();
        }

        public virtual void ResetSwitch()
        {
            currentGraceTimer = 0f;
            isActivated = false;
            if (depressVisual != null)
            {
                depressVisual.localPosition = initialVisualPos;
            }
            UpdateVisuals();
        }

        private void FixedUpdate()
        {
            if (triggerCol == null || !isPowered) return;

            // Continuous overlap check for both Live Player and Echo Ghost
            var hits = Physics2D.OverlapBoxAll(triggerCol.bounds.center, triggerCol.bounds.size, 0f);
            bool occupied = false;
            bool livePlayerContact = false;

            foreach (var hit in hits)
            {
                if (hit == null || hit.gameObject == gameObject) continue;

                if (hit.GetComponent<EchoGhostMarker>() != null)
                {
                    if (allowEchoActivation)
                    {
                        occupied = true;
                        break;
                    }
                }

                if (hit.GetComponent<PlayerController2D>() != null || hit.CompareTag("Player"))
                {
                    livePlayerContact = true;
                    if (allowPlayerActivation)
                    {
                        occupied = true;
                        break;
                    }
                }
            }

            // Visual feedback if live player steps on an Echo-only switch
            if (livePlayerContact && !allowPlayerActivation && consoleStatusText != null && !isActivated)
            {
                consoleStatusText.text = "<color=#FF1E40>[REJECTED: LIVE BIOMETRICS]</color>\n<size=11><color=#FFAA00>ECHO GHOST RECORDING REQUIRED</color></size>";
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
                if (occupied)
                {
                    currentGraceTimer = 0f;
                    if (!isActivated)
                    {
                        SetActivated(true);
                    }
                }
                else if (isActivated)
                {
                    if (holdGraceDuration > 0f)
                    {
                        currentGraceTimer += Time.fixedDeltaTime;
                        if (currentGraceTimer >= holdGraceDuration)
                        {
                            SetActivated(false);
                        }
                    }
                    else
                    {
                        SetActivated(false);
                    }
                }
            }
        }

        protected virtual void SetActivated(bool active)
        {
            isActivated = active;
            UpdateVisuals();
            OnSwitchStateChanged?.Invoke(isActivated);
            Debug.Log($"[EchoSwitch] {gameObject.name} state changed. Activated: {isActivated}");
        }

        protected virtual void UpdateVisuals()
        {
            Color curColor = isActivated ? activeColor : (isPowered ? inactiveColor : new Color(0.2f, 0.22f, 0.28f, 0.6f));

            if (switchRenderer != null)
            {
                switchRenderer.color = curColor;
            }

            if (glowLight != null)
            {
                glowLight.color = curColor;
                glowLight.intensity = isActivated ? 1.6f : (isPowered ? 0.4f : 0.1f);
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

            if (consoleStatusText != null)
            {
                if (isActivated)
                {
                    consoleStatusText.text = "<color=#00F0FF>[AUTHORIZING: ECHO GHOST]</color>\n<size=11><color=#00FF88>SECURITY INTERLOCK OFFLINE</color></size>";
                }
                else if (!isPowered)
                {
                    consoleStatusText.text = "<color=#666666>[STANDBY]</color>\n<size=11><color=#444444>AWAITING INTERLOCK ACTIVATION</color></size>";
                }
                else if (!allowPlayerActivation)
                {
                    consoleStatusText.text = "<color=#D600FF>[TEMPORAL SENSOR: ACTIVE]</color>\n<size=11><color=#FFFFFF>ECHO GHOST REQUIRED TO OPEN BARRIER</color></size>";
                }
                else
                {
                    consoleStatusText.text = "<color=#FFAA00>[READY]</color>\n<size=11><color=#FFFFFF>STEP TO ENGAGE</color></size>";
                }
            }
        }
    }
}
