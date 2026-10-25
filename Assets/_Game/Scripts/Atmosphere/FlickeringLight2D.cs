using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GlitchVerse.Atmosphere
{
    /// <summary>
    /// Provides lightweight, atmospheric lighting modulation for URP 2D Light2D fixtures.
    /// Supports chapter-specific modes (subtle fluorescent buzz, emergency alarms, malfunctioning sparks).
    /// Respects Time.timeScale so effects react to Level 03's Time Freeze power.
    /// </summary>
    [RequireComponent(typeof(Light2D))]
    public class FlickeringLight2D : MonoBehaviour
    {
        public enum FlickerMode
        {
            SubtleFluorescent,  // Chapter I: Abandoned facility fluorescent fixture hum
            MalfunctioningArc,  // Chapter II: Malfunctioning gravity chamber conduit spark
            SlowBreathing,      // Chapter III: Cold cryogenic stasis / containment core pulse
            EmergencyStrobe,    // Chapter IV: Quarantine emergency warning siren
            SurveillanceGlow,   // Chapter V: Paranoia surveillance complex pulse
            QuarantineAlarm,    // Chapter VI: Quarantine Core pulsating alarm
            BossCorePulse       // Chapter VII: Corrupted AI core heartbeat surge
        }

        [Header("Flicker Configuration")]
        [SerializeField] private FlickerMode mode = FlickerMode.SubtleFluorescent;
        [SerializeField] private float baseIntensity = 0.85f;
        [SerializeField] private float flickerAmount = 0.25f;
        [SerializeField] private float speed = 4.0f;
        [SerializeField] private bool syncWithTimeScale = true;

        private Light2D targetLight;
        private float seed;
        private float timer;

        public FlickerMode Mode { get => mode; set => mode = value; }
        public float BaseIntensity { get => baseIntensity; set => baseIntensity = value; }
        public float FlickerAmount { get => flickerAmount; set => flickerAmount = value; }

        private void Awake()
        {
            targetLight = GetComponent<Light2D>();
            seed = Random.Range(0f, 1000f);
            if (baseIntensity <= 0f && targetLight != null)
            {
                baseIntensity = targetLight.intensity;
            }
        }

        private void Update()
        {
            if (targetLight == null) return;

            float dt = syncWithTimeScale ? Time.deltaTime : Time.unscaledDeltaTime;
            timer += dt * speed;

            float intensityModifier = 1f;

            switch (mode)
            {
                case FlickerMode.SubtleFluorescent:
                    // High-frequency Perlin noise with occasional slight voltage dip
                    float noise = Mathf.PerlinNoise(seed, timer * 2.5f);
                    float microBuzz = (Mathf.Sin(timer * 22f) * 0.05f);
                    intensityModifier = Mathf.Lerp(1f - flickerAmount, 1f + (flickerAmount * 0.4f), noise) + microBuzz;
                    break;

                case FlickerMode.MalfunctioningArc:
                    // Sharp chaotic drops and sudden recovery
                    float arcNoise = Mathf.PerlinNoise(seed + 42f, timer * 5f);
                    if (arcNoise < 0.25f)
                    {
                        intensityModifier = 0.2f + Random.Range(0f, 0.3f);
                    }
                    else if (arcNoise > 0.85f)
                    {
                        intensityModifier = 1.3f;
                    }
                    else
                    {
                        intensityModifier = 0.9f + (Mathf.Sin(timer * 15f) * 0.1f);
                    }
                    break;

                case FlickerMode.SlowBreathing:
                    // Smooth sinusoidal pulse for cryogenic containment
                    float breath = (Mathf.Sin(timer * 1.5f) + 1f) * 0.5f;
                    intensityModifier = Mathf.Lerp(1f - flickerAmount, 1f + flickerAmount, breath);
                    break;

                case FlickerMode.EmergencyStrobe:
                    // Harsh emergency alarm cycle
                    float strobeCycle = (timer * 2.0f) % 2.0f;
                    if (strobeCycle < 0.35f)
                    {
                        intensityModifier = 1.5f;
                    }
                    else if (strobeCycle < 0.7f)
                    {
                        intensityModifier = 0.3f;
                    }
                    else
                    {
                        intensityModifier = 0.15f;
                    }
                    break;

                case FlickerMode.SurveillanceGlow:
                    // Subtle nervous twitch / surveillance sweep
                    float sWave = Mathf.Sin(timer * 3.5f) * 0.2f;
                    float sNoise = (Mathf.PerlinNoise(seed + 99f, timer * 8f) - 0.5f) * 0.15f;
                    intensityModifier = 1.0f + sWave + sNoise;
                    break;

                case FlickerMode.QuarantineAlarm:
                    // Heavy deep pulsing dread alarm
                    float qPulse = (Mathf.Sin(timer * 2.5f) + 1f) * 0.5f;
                    intensityModifier = Mathf.Lerp(0.35f, 1.4f, qPulse);
                    break;

                case FlickerMode.BossCorePulse:
                    // Erratic, aggressive double-beat AI heartbeat with micro-glitches
                    float bBeat = Mathf.Sin(timer * 5.0f);
                    float bGlitch = (Mathf.PerlinNoise(seed + 137f, timer * 16f) > 0.65f) ? 0.35f : 0f;
                    intensityModifier = Mathf.Lerp(0.5f, 1.5f, Mathf.Clamp01(bBeat * bBeat + bGlitch));
                    break;
            }

            targetLight.intensity = Mathf.Max(0.05f, baseIntensity * intensityModifier);
        }
    }
}
