using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using GlitchVerse.Player;
using GlitchVerse.Core;

namespace GlitchVerse.Boss
{
    /// <summary>
    /// Telegraphed energy hazard used during Stage 1 (Gravity Assault) of the Architect boss encounter.
    /// Cycles cleanly through:
    /// 1. Cooldown (idle/safe)
    /// 2. Telegraph (readable amber strobe warning for 1.2s)
    /// 3. Surge (lethal sweeping energy beam for 1.5s)
    /// Allows the player to anticipate and evade via [G] Gravity Inversion.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class BossHazardEmitter : MonoBehaviour
    {
        public enum HazardState
        {
            Idle,
            Telegraph,
            Surging
        }

        [Header("Cycle Timing")]
        [SerializeField] private float initialDelay = 0.5f;
        [SerializeField] private float idleDuration = 2.2f;
        [SerializeField] private float warningDuration = 1.2f;
        [SerializeField] private float surgeDuration = 1.4f;

        [Header("Visual References")]
        [SerializeField] private SpriteRenderer hazardRenderer;
        [SerializeField] private Light2D warningLight;
        [SerializeField] private Color idleColor = new Color(0.2f, 0.05f, 0.08f, 0.2f);
        [SerializeField] private Color warningColor = new Color(1.0f, 0.7f, 0.1f, 0.8f); // Warning Amber
        [SerializeField] private Color surgeColor = new Color(1.0f, 0.15f, 0.25f, 1f);   // Lethal Crimson

        [Header("Runtime State")]
        [SerializeField] private HazardState currentState = HazardState.Idle;
        [SerializeField] private bool isEmitterEnabled = true;

        private BoxCollider2D hazardCol;
        private Coroutine cycleRoutine;

        public HazardState CurrentState => currentState;

        private void Awake()
        {
            hazardCol = GetComponent<BoxCollider2D>();
            hazardCol.isTrigger = true;
            ApplyVisualState(HazardState.Idle);
        }

        private void OnEnable()
        {
            if (isEmitterEnabled)
            {
                cycleRoutine = StartCoroutine(HazardCycleRoutine());
            }
        }

        private void OnDisable()
        {
            if (cycleRoutine != null)
            {
                StopCoroutine(cycleRoutine);
                cycleRoutine = null;
            }
        }

        public void SetEmitterActive(bool active)
        {
            isEmitterEnabled = active;
            if (!active)
            {
                if (cycleRoutine != null)
                {
                    StopCoroutine(cycleRoutine);
                    cycleRoutine = null;
                }
                ApplyVisualState(HazardState.Idle);
                if (hazardCol != null) hazardCol.enabled = false;
            }
            else if (gameObject.activeInHierarchy && cycleRoutine == null)
            {
                if (hazardCol != null) hazardCol.enabled = true;
                cycleRoutine = StartCoroutine(HazardCycleRoutine());
            }
        }

        private IEnumerator HazardCycleRoutine()
        {
            if (initialDelay > 0f)
            {
                yield return new WaitForSeconds(initialDelay);
            }

            while (isEmitterEnabled)
            {
                // 1. Idle Phase
                ApplyVisualState(HazardState.Idle);
                yield return new WaitForSeconds(idleDuration);

                if (!isEmitterEnabled) yield break;

                // 2. Telegraph / Warning Phase
                ApplyVisualState(HazardState.Telegraph);
                float warnElapsed = 0f;
                while (warnElapsed < warningDuration)
                {
                    if (warningLight != null)
                    {
                        warningLight.intensity = (Mathf.Sin(warnElapsed * 24f) > 0f) ? 1.6f : 0.2f;
                    }
                    warnElapsed += Time.deltaTime;
                    yield return null;
                }

                if (!isEmitterEnabled) yield break;

                // 3. Lethal Surge Phase
                ApplyVisualState(HazardState.Surging);
                yield return new WaitForSeconds(surgeDuration);
            }
        }

        private void ApplyVisualState(HazardState state)
        {
            currentState = state;

            switch (state)
            {
                case HazardState.Idle:
                    if (hazardRenderer != null) hazardRenderer.color = idleColor;
                    if (warningLight != null)
                    {
                        warningLight.color = warningColor;
                        warningLight.intensity = 0f;
                    }
                    break;

                case HazardState.Telegraph:
                    if (hazardRenderer != null) hazardRenderer.color = warningColor;
                    if (warningLight != null)
                    {
                        warningLight.color = warningColor;
                        warningLight.intensity = 1.2f;
                    }
                    break;

                case HazardState.Surging:
                    if (hazardRenderer != null) hazardRenderer.color = surgeColor;
                    if (warningLight != null)
                    {
                        warningLight.color = surgeColor;
                        warningLight.intensity = 2.0f;
                    }
                    break;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!isEmitterEnabled || currentState != HazardState.Surging) return;

            if (other != null && other.GetComponent<PlayerController2D>() != null)
            {
                if (BossDirector.Instance != null)
                {
                    BossDirector.Instance.OnPlayerHitHazard();
                }
                else if (LevelDirector.Current != null)
                {
                    LevelDirector.Current.KillPlayer();
                }
            }
        }
    }
}
