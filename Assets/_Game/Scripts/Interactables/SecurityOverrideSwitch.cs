using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using GlitchVerse.Echo;
using GlitchVerse.Narrative;
using GlitchVerse.Player;

namespace GlitchVerse.Interactables
{
    /// <summary>
    /// Interactive temporal Echo-only security switch mounted on Platform B in Chapter I.
    /// Powers up when the Fake Exit Prank triggers.
    /// Strictly rejects live player activation; can only be engaged and held by a temporal Echo ghost.
    /// Disables the security barrier at Observation Platform A while held.
    /// Reactivates the barrier when the Echo releases, with a fair grace buffer.
    /// Follows Chapter I Difficulty Upgrade 01B specifications.
    /// </summary>
    public class SecurityOverrideSwitch : EchoSwitch
    {
        [Header("Target Interlock")]
        [SerializeField] private SecurityBarrier targetBarrier;

        [Header("State Colors")]
        [SerializeField] private Color unpoweredColor = new Color(0.25f, 0.28f, 0.35f, 0.7f);
        [SerializeField] private Color poweredColor = new Color(0.85f, 0f, 1f, 1f);     // Holographic Magenta #D600FF
        [SerializeField] private Color activatedColor = new Color(0.0f, 0.95f, 1.0f, 1f);  // Cyan Cleared #00F0FF

        private bool hasSpokenPraise = false;

        public SecurityBarrier TargetBarrier
        {
            get => targetBarrier;
            set => targetBarrier = value;
        }

        public event Action OnSwitchActivated;

        protected override void Awake()
        {
            // Configure strictly for Echo-only temporal gating
            allowPlayerActivation = false;
            allowEchoActivation = true;
            latching = false;
            holdGraceDuration = 2.0f; // 2-second grace hold window for fair traversal
            isPowered = false;

            inactiveColor = poweredColor;
            activeColor = activatedColor;

            base.Awake();

            OnSwitchStateChanged += HandleSwitchStateChanged;
        }

        private void OnDestroy()
        {
            OnSwitchStateChanged -= HandleSwitchStateChanged;
        }

        private void HandleSwitchStateChanged(bool active)
        {
            if (targetBarrier != null)
            {
                // Barrier locked when switch is not activated, unlocked when switch is activated
                targetBarrier.SetBarrierLocked(!active);
            }

            if (active)
            {
                OnSwitchActivated?.Invoke();

                if (!hasSpokenPraise)
                {
                    hasSpokenPraise = true;
                    if (ArchitectDialogue.Current != null)
                    {
                        ArchitectDialogue.Current.Speak(
                            "Congratulations. Your past self is finally useful.",
                            "SECURITY OVERRIDE",
                            3.2f,
                            "ch1_echo_useful",
                            0f,
                            priority: true
                        );
                    }
                }
            }
        }

        public override void ResetSwitch()
        {
            hasSpokenPraise = false;
            base.ResetSwitch();
            if (targetBarrier != null)
            {
                targetBarrier.SetBarrierLocked(true);
            }
        }

        public void ActivateSwitch()
        {
            if (isActivated) return;
            SetActivated(true);
        }

        protected override void UpdateVisuals()
        {
            base.UpdateVisuals();

            if (consoleStatusText != null)
            {
                if (isActivated)
                {
                    consoleStatusText.text = "<color=#00F0FF>[ECHO OVERRIDE ACTIVE]</color>\n<size=11><color=#00FF88>EXIT BARRIER UNLOCKED</color></size>";
                }
                else if (isPowered)
                {
                    consoleStatusText.text = "<color=#D600FF>[TEMPORAL SENSOR: ONLINE]</color>\n<size=11><color=#FFFFFF>ECHO GHOST [H] REQUIRED TO ENGAGE</color></size>";
                }
                else
                {
                    consoleStatusText.text = "<color=#666666>[STANDBY]</color>\n<size=11><color=#444444>AWAITING INTERLOCK ACTIVATION</color></size>";
                }
            }
        }
    }
}
