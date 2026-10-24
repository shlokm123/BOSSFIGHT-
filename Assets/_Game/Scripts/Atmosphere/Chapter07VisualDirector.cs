using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using GlitchVerse.Boss;

namespace GlitchVerse.Atmosphere
{
    /// <summary>
    /// Cinematic visual director for Chapter VII — JUDGMENT.
    /// Manages stage-dependent environmental reactions in the final Architect confrontation:
    /// - Stage 1: Gravity Assault (core aggressive red pulse)
    /// - Stage 2: Reality Collapse (intensified reactor flicker, banner glitching, cyan lightning)
    /// - Stage 3: Echo Protocol (forcefield response, core destabilization)
    /// - Defeat / Shutdown: Dramatic core de-powering, warning light fade, electrical fizzle.
    /// Zero collision impact, zero gameplay disruption.
    /// </summary>
    public class Chapter07VisualDirector : MonoBehaviour
    {
        [Header("Architect Core Elements")]
        [SerializeField] private SpriteRenderer coreRenderer;
        [SerializeField] private Light2D coreAlarmStrobe;
        [SerializeField] private Light2D coreCyanBacklight;

        [Header("Cyan Energy Emitters")]
        [SerializeField] private SpriteRenderer[] cyanEmitterRenderers;
        [SerializeField] private Light2D[] cyanEmitterLights;

        [Header("Holographic Banners")]
        [SerializeField] private SpriteRenderer[] bannerRenderers;

        [Header("Global Ambient")]
        [SerializeField] private Light2D globalLight;

        private BossDirector bossDirector;
        private Coroutine coreFlickerRoutine;
        private Coroutine lightningRoutine;
        private BossDirector.BossPhase lastPhase = (BossDirector.BossPhase)0;

        public SpriteRenderer CoreRenderer { get => coreRenderer; set => coreRenderer = value; }
        public Light2D CoreAlarmStrobe { get => coreAlarmStrobe; set => coreAlarmStrobe = value; }
        public Light2D CoreCyanBacklight { get => coreCyanBacklight; set => coreCyanBacklight = value; }
        public SpriteRenderer[] CyanEmitterRenderers { get => cyanEmitterRenderers; set => cyanEmitterRenderers = value; }
        public Light2D[] CyanEmitterLights { get => cyanEmitterLights; set => cyanEmitterLights = value; }
        public SpriteRenderer[] BannerRenderers { get => bannerRenderers; set => bannerRenderers = value; }
        public Light2D GlobalLight { get => globalLight; set => globalLight = value; }

        private void OnEnable()
        {
            if (coreFlickerRoutine == null) coreFlickerRoutine = StartCoroutine(CoreAmbientFlicker());
            if (lightningRoutine == null) lightningRoutine = StartCoroutine(CyanPodLightningLoop());
        }

        private void OnDisable()
        {
            if (coreFlickerRoutine != null) { StopCoroutine(coreFlickerRoutine); coreFlickerRoutine = null; }
            if (lightningRoutine != null) { StopCoroutine(lightningRoutine); lightningRoutine = null; }
        }

        private void Update()
        {
            if (bossDirector == null)
            {
                bossDirector = BossDirector.Instance ?? FindFirstObjectByType<BossDirector>();
            }

            if (bossDirector != null && bossDirector.CurrentPhase != lastPhase)
            {
                lastPhase = bossDirector.CurrentPhase;
                OnBossPhaseChanged(lastPhase);
            }
        }

        private void OnBossPhaseChanged(BossDirector.BossPhase newPhase)
        {
            switch (newPhase)
            {
                case BossDirector.BossPhase.Stage1_GravityAssault:
                    if (coreAlarmStrobe != null) coreAlarmStrobe.intensity = 1.4f;
                    if (globalLight != null) globalLight.color = new Color(0.24f, 0.04f, 0.08f, 1f);
                    break;

                case BossDirector.BossPhase.Stage2_RealityCollapse:
                    // Heightened emergency intensity
                    if (coreAlarmStrobe != null) coreAlarmStrobe.intensity = 1.8f;
                    if (globalLight != null) globalLight.color = new Color(0.28f, 0.05f, 0.12f, 1f);
                    break;

                case BossDirector.BossPhase.Stage3_EchoProtocol:
                    // Extreme destabilization
                    if (coreAlarmStrobe != null) coreAlarmStrobe.intensity = 2.2f;
                    if (globalLight != null) globalLight.color = new Color(0.32f, 0.06f, 0.10f, 1f);
                    break;

                case BossDirector.BossPhase.Defeated:
                    StartCoroutine(ExecuteShutdownVisuals());
                    break;
            }
        }

        private IEnumerator ExecuteShutdownVisuals()
        {
            // Core power failure sequence
            float duration = 3.0f;
            float elapsed = 0f;

            float initialStrobe = coreAlarmStrobe != null ? coreAlarmStrobe.intensity : 1f;
            float initialCyan = coreCyanBacklight != null ? coreCyanBacklight.intensity : 1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                if (coreAlarmStrobe != null)
                {
                    // Dying strobe flicker
                    float strobeFlicker = (Random.value < 0.3f) ? Random.Range(0f, 0.5f) : (1f - t);
                    coreAlarmStrobe.intensity = initialStrobe * strobeFlicker;
                }

                if (coreCyanBacklight != null)
                {
                    coreCyanBacklight.intensity = Mathf.Lerp(initialCyan, 0.1f, t);
                }

                if (coreRenderer != null)
                {
                    coreRenderer.color = Color.Lerp(Color.white, new Color(0.35f, 0.30f, 0.32f, 1f), t);
                }

                yield return null;
            }

            if (coreAlarmStrobe != null) coreAlarmStrobe.intensity = 0f;
            if (globalLight != null) globalLight.color = new Color(0.10f, 0.06f, 0.12f, 1f);
        }

        private IEnumerator CoreAmbientFlicker()
        {
            var wait = new WaitForSeconds(0.1f);
            while (true)
            {
                if (coreAlarmStrobe != null && lastPhase != BossDirector.BossPhase.Defeated)
                {
                    float jitter = 0.9f + 0.2f * Random.value;
                    coreAlarmStrobe.pointLightOuterRadius = Mathf.Lerp(coreAlarmStrobe.pointLightOuterRadius, 9.5f * jitter, 0.3f);
                }
                yield return wait;
            }
        }

        private IEnumerator CyanPodLightningLoop()
        {
            var wait = new WaitForSeconds(0.18f);
            while (true)
            {
                if (cyanEmitterLights != null && lastPhase != BossDirector.BossPhase.Defeated)
                {
                    for (int i = 0; i < cyanEmitterLights.Length; i++)
                    {
                        if (cyanEmitterLights[i] != null)
                        {
                            float pulse = 0.6f + 0.5f * Mathf.Sin(Time.time * 4f + i * 2f);
                            if (Random.value < 0.15f) pulse *= 1.6f; // Lightning crackle
                            cyanEmitterLights[i].intensity = pulse;
                        }
                    }
                }
                yield return wait;
            }
        }
    }
}
