using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using GlitchVerse.Mechanics;

namespace GlitchVerse.Atmosphere
{
    /// <summary>
    /// Cinematic visual director for Chapter V — PARANOIA.
    /// Controls surveillance monitor flicker/scanlines, security camera sensor indicators,
    /// tracking light sweeps, and dimension reactivity.
    /// Zero collision impact, zero gameplay disruption.
    /// </summary>
    public class Chapter05VisualDirector : MonoBehaviour
    {
        [Header("Surveillance Monitor Banks")]
        [SerializeField] private SpriteRenderer[] monitorRenderers;
        [SerializeField] private Sprite alphaMonitorSprite;
        [SerializeField] private Sprite betaMonitorSprite;

        [Header("Security Tracking Spotlights")]
        [SerializeField] private Light2D[] trackingLights;
        [SerializeField] private Transform[] cameraHeads;

        [Header("Atmospheric Global & Ambient")]
        [SerializeField] private Light2D globalLight;
        [SerializeField] private Color alphaAmbient = new Color(0.14f, 0.05f, 0.22f, 1f);
        [SerializeField] private Color betaAmbient = new Color(0.18f, 0.08f, 0.16f, 1f);

        [Header("Conduit & Beacon Accent Lights")]
        [SerializeField] private Light2D[] cyanConduitLights;
        [SerializeField] private Light2D[] redSensorLights;

        private AbilityController abilityController;
        private Coroutine monitorFlickerRoutine;
        private Coroutine cameraSweepRoutine;
        private bool isSubscribed = false;

        public SpriteRenderer[] MonitorRenderers { get => monitorRenderers; set => monitorRenderers = value; }
        public Sprite AlphaMonitorSprite { get => alphaMonitorSprite; set => alphaMonitorSprite = value; }
        public Sprite BetaMonitorSprite { get => betaMonitorSprite; set => betaMonitorSprite = value; }
        public Light2D GlobalLight { get => globalLight; set => globalLight = value; }
        public Light2D[] TrackingLights { get => trackingLights; set => trackingLights = value; }
        public Transform[] CameraHeads { get => cameraHeads; set => cameraHeads = value; }
        public Light2D[] CyanConduitLights { get => cyanConduitLights; set => cyanConduitLights = value; }
        public Light2D[] RedSensorLights { get => redSensorLights; set => redSensorLights = value; }

        private void Awake()
        {
            Subscribe();
        }

        private void OnEnable()
        {
            Subscribe();
            if (cameraSweepRoutine == null) cameraSweepRoutine = StartCoroutine(CameraTrackingSweep());
            if (monitorFlickerRoutine == null) monitorFlickerRoutine = StartCoroutine(MonitorStaticPulse());
        }

        private void OnDisable()
        {
            Unsubscribe();
            if (cameraSweepRoutine != null) { StopCoroutine(cameraSweepRoutine); cameraSweepRoutine = null; }
            if (monitorFlickerRoutine != null) { StopCoroutine(monitorFlickerRoutine); monitorFlickerRoutine = null; }
        }

        private void Start()
        {
            Subscribe();
            if (abilityController != null)
            {
                ApplyDimensionVisuals(abilityController.CurrentDimension);
            }
            else
            {
                ApplyDimensionVisuals(DimensionType.Alpha);
            }
        }

        private void Update()
        {
            if (!isSubscribed)
            {
                Subscribe();
            }
        }

        private void Subscribe()
        {
            if (isSubscribed) return;
            if (abilityController == null)
            {
                abilityController = FindFirstObjectByType<AbilityController>();
            }
            if (abilityController != null)
            {
                abilityController.OnDimensionToggled += HandleDimensionToggled;
                isSubscribed = true;
            }
        }

        private void Unsubscribe()
        {
            if (!isSubscribed) return;
            if (abilityController != null)
            {
                abilityController.OnDimensionToggled -= HandleDimensionToggled;
            }
            isSubscribed = false;
        }

        private void HandleDimensionToggled(DimensionType newDimension)
        {
            ApplyDimensionVisuals(newDimension);
        }

        public void ApplyDimensionVisuals(DimensionType dim)
        {
            bool isAlpha = (dim == DimensionType.Alpha);

            if (monitorRenderers != null && monitorRenderers.Length > 0)
            {
                Sprite targetSprite = isAlpha ? alphaMonitorSprite : betaMonitorSprite;
                if (targetSprite != null)
                {
                    for (int i = 0; i < monitorRenderers.Length; i++)
                    {
                        if (monitorRenderers[i] != null)
                        {
                            monitorRenderers[i].sprite = targetSprite;
                        }
                    }
                }
            }

            if (globalLight != null)
            {
                globalLight.color = isAlpha ? alphaAmbient : betaAmbient;
            }
        }

        private IEnumerator CameraTrackingSweep()
        {
            float timer = 0f;
            while (true)
            {
                timer += Time.deltaTime * 0.8f;
                float angle = Mathf.Sin(timer) * 12.0f; // Subtle +-12 deg sweep

                if (cameraHeads != null)
                {
                    for (int i = 0; i < cameraHeads.Length; i++)
                    {
                        if (cameraHeads[i] != null)
                        {
                            float offset = i * 1.57f;
                            float currentAngle = Mathf.Sin(timer + offset) * 10f;
                            cameraHeads[i].localRotation = Quaternion.Euler(0f, 0f, currentAngle);
                        }
                    }
                }

                if (trackingLights != null)
                {
                    for (int i = 0; i < trackingLights.Length; i++)
                    {
                        if (trackingLights[i] != null)
                        {
                            float pulse = 0.85f + 0.15f * Mathf.Sin(timer * 2.5f + i);
                            trackingLights[i].intensity = pulse;
                        }
                    }
                }

                yield return null;
            }
        }

        private IEnumerator MonitorStaticPulse()
        {
            var wait = new WaitForSeconds(0.12f);
            while (true)
            {
                if (Random.value < 0.08f && monitorRenderers != null && monitorRenderers.Length > 0)
                {
                    int targetIdx = Random.Range(0, monitorRenderers.Length);
                    if (monitorRenderers[targetIdx] != null)
                    {
                        Color orig = monitorRenderers[targetIdx].color;
                        monitorRenderers[targetIdx].color = new Color(orig.r * 1.3f, orig.g * 1.3f, orig.b * 1.3f, orig.a);
                        yield return new WaitForSeconds(0.06f);
                        if (monitorRenderers[targetIdx] != null)
                        {
                            monitorRenderers[targetIdx].color = orig;
                        }
                    }
                }
                yield return wait;
            }
        }
    }
}
