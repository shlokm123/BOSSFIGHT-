using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GlitchVerse.Interactables
{
    /// <summary>
    /// Pressure plate trigger that detects the MovableCube,
    /// depresses visually, alters its emissive state, and broadcasts state changes.
    /// </summary>
    public class PressurePlate : MonoBehaviour
    {
        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer plateTopRenderer;
        [SerializeField] private Light2D plateGlowLight;
        [SerializeField] private Color inactiveColor = new Color(0.15f, 0.35f, 0.45f, 1f);
        [SerializeField] private Color activeColor = new Color(0f, 0.95f, 1f, 1f); // #00F0FF
        [SerializeField] private Transform depressTarget;
        [SerializeField] private float depressDistance = 0.08f;

        [Header("State")]
        [SerializeField] private bool isActivated = false;
        private int objectCount = 0;
        private Vector3 initialTopPosition;
        private BoxCollider2D plateCollider;

        public bool IsActivated => isActivated;
        public event Action<bool> OnPlateStateChanged;

        private void Awake()
        {
            plateCollider = GetComponent<BoxCollider2D>();
            if (depressTarget != null)
            {
                initialTopPosition = depressTarget.localPosition;
            }
            UpdateVisuals();
        }

        private void FixedUpdate()
        {
            // Continuous physical overlap evaluation for foolproof trigger detection
            if (plateCollider != null)
            {
                var hits = Physics2D.OverlapBoxAll(plateCollider.bounds.center, plateCollider.bounds.size, 0f);
                int count = 0;
                foreach (var hit in hits)
                {
                    if (hit != null && hit.gameObject != gameObject && 
                        (hit.GetComponent<MovableCube>() != null || hit.CompareTag("Interactive")))
                    {
                        count++;
                    }
                }

                bool shouldBeActive = count > 0;
                if (shouldBeActive != isActivated)
                {
                    isActivated = shouldBeActive;
                    UpdateVisuals();
                    OnPlateStateChanged?.Invoke(isActivated);
                    Debug.Log($"[PressurePlate] State changed via OverlapCheck. Activated: {isActivated}");
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Specifically look for the puzzle cube (or player weight)
            if (other.GetComponent<MovableCube>() != null || other.CompareTag("Interactive"))
            {
                objectCount++;
                EvaluateState();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.GetComponent<MovableCube>() != null || other.CompareTag("Interactive"))
            {
                objectCount = Mathf.Max(0, objectCount - 1);
                EvaluateState();
            }
        }

        private void EvaluateState()
        {
            bool shouldBeActive = objectCount > 0;
            if (shouldBeActive != isActivated)
            {
                isActivated = shouldBeActive;
                UpdateVisuals();
                OnPlateStateChanged?.Invoke(isActivated);
                Debug.Log($"[PressurePlate] State changed. Activated: {isActivated}");
            }
        }

        private void UpdateVisuals()
        {
            if (plateTopRenderer != null)
            {
                plateTopRenderer.color = isActivated ? activeColor : inactiveColor;
            }

            if (plateGlowLight != null)
            {
                plateGlowLight.color = isActivated ? activeColor : inactiveColor;
                plateGlowLight.intensity = isActivated ? 1.2f : 0.25f;
            }

            if (depressTarget != null)
            {
                Vector3 targetPos = initialTopPosition;
                if (isActivated)
                {
                    targetPos.y -= depressDistance;
                }
                depressTarget.localPosition = targetPos;
            }
        }
    }
}
