using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using GlitchVerse.Player;

namespace GlitchVerse.Boss
{
    /// <summary>
    /// Interactive override terminal node used in Chapter VII (The Architect Boss Encounter).
    /// Stepping onto or contacting this node permanently overrides one layer of the Architect's Core.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class BossControlNode : MonoBehaviour
    {
        [Header("Node Configuration")]
        [SerializeField] private int stageNumber = 1;
        [SerializeField] private string nodeName = "OVERRIDE NODE 01";

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer nodeRenderer;
        [SerializeField] private Light2D glowLight;
        [SerializeField] private Color dormantColor = new Color(1.0f, 0.25f, 0.25f, 1f); // Alert Red
        [SerializeField] private Color activeColor = new Color(0f, 0.95f, 1f, 1f);       // Cyan Override
        [SerializeField] private Transform pulseVisual;

        [Header("State")]
        [SerializeField] private bool isActivated = false;

        public int StageNumber => stageNumber;
        public bool IsActivated => isActivated;
        public event Action<int> OnNodeTriggered;

        private BoxCollider2D triggerCol;

        private void Awake()
        {
            triggerCol = GetComponent<BoxCollider2D>();
            triggerCol.isTrigger = true;
            UpdateVisuals();
        }

        private void FixedUpdate()
        {
            if (isActivated || triggerCol == null) return;

            // Continuous overlap evaluation for foolproof player contact
            var hits = Physics2D.OverlapBoxAll(triggerCol.bounds.center, triggerCol.bounds.size, 0f);
            foreach (var hit in hits)
            {
                if (hit != null && hit.GetComponent<PlayerController2D>() != null)
                {
                    ActivateNode();
                    break;
                }
            }
        }

        public void ActivateNode()
        {
            if (isActivated) return;

            isActivated = true;
            UpdateVisuals();
            OnNodeTriggered?.Invoke(stageNumber);

            if (BossDirector.Instance != null)
            {
                BossDirector.Instance.OnNodeActivated(stageNumber);
            }

            Debug.Log($"[BossControlNode] Node {stageNumber} ({nodeName}) OVERRIDDEN by Subject 075!");
        }

        public void ResetNode()
        {
            isActivated = false;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            Color targetColor = isActivated ? activeColor : dormantColor;

            if (nodeRenderer != null)
            {
                nodeRenderer.color = targetColor;
            }

            if (glowLight != null)
            {
                glowLight.color = targetColor;
                glowLight.intensity = isActivated ? 1.4f : 0.6f;
            }

            if (pulseVisual != null)
            {
                pulseVisual.localScale = isActivated ? Vector3.one * 1.15f : Vector3.one;
            }
        }

        private void Update()
        {
            // Idle hover/pulsing visual animation
            if (pulseVisual != null && !isActivated)
            {
                float s = 1.0f + Mathf.Sin(Time.time * 4f) * 0.05f;
                pulseVisual.localScale = new Vector3(s, s, 1f);
            }
        }
    }
}
