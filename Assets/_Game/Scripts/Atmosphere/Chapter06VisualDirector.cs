using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using GlitchVerse.Interactables;

namespace GlitchVerse.Atmosphere
{
    /// <summary>
    /// Cinematic visual director for Chapter VI — TRUTH.
    /// Manages the massive containment reactor core breathing pulse,
    /// sterile white floodlight flickering, and vault seal status indicators.
    /// Zero collision impact, zero gameplay disruption.
    /// </summary>
    public class Chapter06VisualDirector : MonoBehaviour
    {
        [Header("Reactor Core Lighting")]
        [SerializeField] private Light2D reactorCoreLight;
        [SerializeField] private Light2D reactorGazeLight;
        [SerializeField] private float pulseSpeed = 1.2f;

        [Header("Sterile White Floodlights")]
        [SerializeField] private Light2D[] sterileFloodlights;

        [Header("Console & Status Displays")]
        [SerializeField] private SpriteRenderer[] loreConsoleRenderers;
        [SerializeField] private Light2D[] consoleLights;

        [Header("Quarantine Gate Indicators")]
        [SerializeField] private SecurityBarrier stage1Gate;
        [SerializeField] private SecurityBarrier stage2Gate;
        [SerializeField] private SecurityBarrier stage3Barrier;

        private Coroutine reactorPulseRoutine;
        private Coroutine floodlightFlickerRoutine;

        public Light2D ReactorCoreLight { get => reactorCoreLight; set => reactorCoreLight = value; }
        public Light2D ReactorGazeLight { get => reactorGazeLight; set => reactorGazeLight = value; }
        public Light2D[] SterileFloodlights { get => sterileFloodlights; set => sterileFloodlights = value; }
        public SpriteRenderer[] LoreConsoleRenderers { get => loreConsoleRenderers; set => loreConsoleRenderers = value; }
        public Light2D[] ConsoleLights { get => consoleLights; set => consoleLights = value; }
        public SecurityBarrier Stage1Gate { get => stage1Gate; set => stage1Gate = value; }
        public SecurityBarrier Stage2Gate { get => stage2Gate; set => stage2Gate = value; }
        public SecurityBarrier Stage3Barrier { get => stage3Barrier; set => stage3Barrier = value; }

        private void OnEnable()
        {
            if (reactorPulseRoutine == null) reactorPulseRoutine = StartCoroutine(ReactorBreathingPulse());
            if (floodlightFlickerRoutine == null) floodlightFlickerRoutine = StartCoroutine(SterileFloodlightFlicker());
        }

        private void OnDisable()
        {
            if (reactorPulseRoutine != null) { StopCoroutine(reactorPulseRoutine); reactorPulseRoutine = null; }
            if (floodlightFlickerRoutine != null) { StopCoroutine(floodlightFlickerRoutine); floodlightFlickerRoutine = null; }
        }

        private IEnumerator ReactorBreathingPulse()
        {
            float timer = 0f;
            while (true)
            {
                timer += Time.deltaTime * pulseSpeed;
                // Deep ominous slow breathing cycle
                float wave = (Mathf.Sin(timer) + 1f) * 0.5f; // 0..1
                float intensity = Mathf.Lerp(1.1f, 1.8f, wave);

                if (reactorCoreLight != null)
                {
                    reactorCoreLight.intensity = intensity;
                }
                if (reactorGazeLight != null)
                {
                    reactorGazeLight.intensity = Mathf.Lerp(0.6f, 1.1f, wave);
                }

                yield return null;
            }
        }

        private IEnumerator SterileFloodlightFlicker()
        {
            var wait = new WaitForSeconds(0.15f);
            while (true)
            {
                if (Random.value < 0.06f && sterileFloodlights != null && sterileFloodlights.Length > 0)
                {
                    int idx = Random.Range(0, sterileFloodlights.Length);
                    if (sterileFloodlights[idx] != null)
                    {
                        float orig = sterileFloodlights[idx].intensity;
                        sterileFloodlights[idx].intensity = orig * 0.4f;
                        yield return new WaitForSeconds(0.05f);
                        if (sterileFloodlights[idx] != null)
                        {
                            sterileFloodlights[idx].intensity = orig;
                        }
                    }
                }
                yield return wait;
            }
        }
    }
}
