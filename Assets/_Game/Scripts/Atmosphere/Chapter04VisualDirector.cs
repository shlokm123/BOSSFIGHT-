using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using GlitchVerse.Mechanics;

namespace GlitchVerse.Atmosphere
{
    /// <summary>
    /// Cinematic director for Chapter IV — REVELATION.
    /// Drives visual dimension feedback on surveillance wall monitors and chamber atmospheric lighting
    /// when the player shifts between Dimension Alpha (Emergency Crimson) and Dimension Beta (Desaturated Toxic Haze/Slate).
    /// Zero collision impact, zero gameplay disruption.
    /// </summary>
    public class Chapter04VisualDirector : MonoBehaviour
    {
        [Header("Surveillance Display Wall")]
        [SerializeField] private SpriteRenderer surveillanceWallRenderer;
        [SerializeField] private Sprite alphaScreenSprite;
        [SerializeField] private Sprite betaScreenSprite;

        [Header("Global Ambient Atmosphere")]
        [SerializeField] private Light2D globalLight;
        [SerializeField] private Color alphaAmbientColor = new Color(0.92f, 0.32f, 0.36f, 1f);
        [SerializeField] private Color betaAmbientColor = new Color(0.30f, 0.44f, 0.52f, 1f);

        [Header("Emergency Beacons & Local Lights")]
        [SerializeField] private Light2D[] emergencyLights;
        [SerializeField] private Color alphaBeaconColor = new Color(1.0f, 0.22f, 0.18f, 1f);
        [SerializeField] private Color betaBeaconColor = new Color(0.28f, 0.65f, 0.78f, 1f);

        [Header("Beta Structural Decors")]
        [SerializeField] private GameObject[] betaEnvironmentalElements;

        private AbilityController abilityController;
        private Coroutine transitionRoutine;

        public SpriteRenderer SurveillanceWallRenderer
        {
            get => surveillanceWallRenderer;
            set => surveillanceWallRenderer = value;
        }

        public Sprite AlphaScreenSprite
        {
            get => alphaScreenSprite;
            set => alphaScreenSprite = value;
        }

        public Sprite BetaScreenSprite
        {
            get => betaScreenSprite;
            set => betaScreenSprite = value;
        }

        public Light2D GlobalLight
        {
            get => globalLight;
            set => globalLight = value;
        }

        public Light2D[] EmergencyLights
        {
            get => emergencyLights;
            set => emergencyLights = value;
        }

        public GameObject[] BetaEnvironmentalElements
        {
            get => betaEnvironmentalElements;
            set => betaEnvironmentalElements = value;
        }

        private bool isSubscribed = false;

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

        private void Awake()
        {
            Subscribe();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Start()
        {
            Subscribe();
            if (abilityController != null)
            {
                ApplyDimensionVisuals(abilityController.CurrentDimension, instant: true);
            }
            else
            {
                ApplyDimensionVisuals(DimensionType.Alpha, instant: true);
            }
        }

        private void Update()
        {
            if (!isSubscribed)
            {
                Subscribe();
            }
        }

        private void HandleDimensionToggled(DimensionType newDimension)
        {
            ApplyDimensionVisuals(newDimension, instant: false);
        }

        public void ApplyDimensionVisuals(DimensionType dim, bool instant)
        {
            bool isAlpha = (dim == DimensionType.Alpha);

            // 1. Swap surveillance monitor feed texture
            if (surveillanceWallRenderer != null)
            {
                surveillanceWallRenderer.sprite = isAlpha ? alphaScreenSprite : betaScreenSprite;
            }

            // 2. Toggle Beta environmental reveals
            if (betaEnvironmentalElements != null)
            {
                foreach (var elem in betaEnvironmentalElements)
                {
                    if (elem != null) elem.SetActive(!isAlpha);
                }
            }

            // 3. Modulate atmospheric lighting
            Color targetGlobal = isAlpha ? alphaAmbientColor : betaAmbientColor;
            Color targetBeacon = isAlpha ? alphaBeaconColor : betaBeaconColor;

            if (instant)
            {
                if (globalLight != null) globalLight.color = targetGlobal;
                if (emergencyLights != null)
                {
                    foreach (var l in emergencyLights)
                    {
                        if (l != null) l.color = targetBeacon;
                    }
                }
            }
            else
            {
                if (transitionRoutine != null) StopCoroutine(transitionRoutine);
                transitionRoutine = StartCoroutine(TransitionLighting(targetGlobal, targetBeacon, 0.25f));
            }
        }

        private IEnumerator TransitionLighting(Color targetGlobal, Color targetBeacon, float duration)
        {
            Color startGlobal = (globalLight != null) ? globalLight.color : targetGlobal;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float smoothT = t * t * (3f - 2f * t);

                if (globalLight != null)
                {
                    globalLight.color = Color.Lerp(startGlobal, targetGlobal, smoothT);
                }

                if (emergencyLights != null)
                {
                    foreach (var l in emergencyLights)
                    {
                        if (l != null)
                        {
                            l.color = Color.Lerp(l.color, targetBeacon, smoothT);
                        }
                    }
                }

                yield return null;
            }

            if (globalLight != null) globalLight.color = targetGlobal;
            if (emergencyLights != null)
            {
                foreach (var l in emergencyLights)
                {
                    if (l != null) l.color = targetBeacon;
                }
            }
        }

        public void ForceSetDimension(DimensionType dim)
        {
            ApplyDimensionVisuals(dim, instant: true);
        }
    }
}
