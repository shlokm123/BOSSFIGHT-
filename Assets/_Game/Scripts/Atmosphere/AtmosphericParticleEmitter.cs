using UnityEngine;

namespace GlitchVerse.Atmosphere
{
    /// <summary>
    /// Lightweight 2D atmospheric particle emitter for GlitchVerse chambers.
    /// Creates restrained dust, gravity motes, cryo mist, or quarantine embers.
    /// Ensures 0 gameplay collision, low particle count (15-30), and zero performance impact.
    /// </summary>
    public class AtmosphericParticleEmitter : MonoBehaviour
    {
        public enum AtmosphereType
        {
            ContainmentDust,     // Chapter I: Amber dust motes drifting gently in abandoned air
            GravityAnomalies,    // Chapter II: Anti-gravity motes drifting upwards
            CryoStasisMist,      // Chapter III: Cold cyan temporal crystal mist
            EmergencyEmbers      // Chapter IV: Quarantine alarm embers & steam particles
        }

        [SerializeField] private AtmosphereType atmosphereType = AtmosphereType.ContainmentDust;
        [SerializeField] private int maxParticles = 28;
        [SerializeField] private Vector2 chamberBounds = new Vector2(22f, 9f);

        private ParticleSystem ps;

        public AtmosphereType CurrentAtmosphere
        {
            get => atmosphereType;
            set
            {
                atmosphereType = value;
                ConfigureParticleSystem();
            }
        }

        private void Awake()
        {
            ConfigureParticleSystem();
        }

        public void ConfigureParticleSystem()
        {
            ps = GetComponent<ParticleSystem>();
            if (ps == null)
            {
                ps = gameObject.AddComponent<ParticleSystem>();
            }

            var main = ps.main;
            main.maxParticles = maxParticles;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.playOnAwake = true;
            main.loop = true;

            var emission = ps.emission;
            emission.rateOverTime = 4f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(chamberBounds.x, chamberBounds.y, 1f);

            var col = ps.colorOverLifetime;
            col.enabled = true;

            var sizeOverLife = ps.sizeOverLifetime;
            sizeOverLife.enabled = true;

            // Ensure ParticleSystemRenderer is on background sorting layer
            var psRend = GetComponent<ParticleSystemRenderer>();
            if (psRend != null)
            {
                psRend.sortingOrder = -5; // Deep in background, never obscuring gameplay objects (order 0-5)
            }

            // Chapter specific tuning
            switch (atmosphereType)
            {
                case AtmosphereType.ContainmentDust:
                    main.startLifetime = 8f;
                    main.startSpeed = 0.12f;
                    main.startSize = 0.08f;
                    main.startColor = new Color(1.0f, 0.75f, 0.35f, 0.22f); // Faint amber
                    main.gravityModifier = 0.015f; // Gentle drift downward
                    break;

                case AtmosphereType.GravityAnomalies:
                    main.startLifetime = 6f;
                    main.startSpeed = 0.35f;
                    main.startSize = 0.10f;
                    main.startColor = new Color(0.2f, 0.85f, 0.95f, 0.30f); // Cool slate teal
                    main.gravityModifier = -0.06f; // Floats UP against gravity
                    break;

                case AtmosphereType.CryoStasisMist:
                    main.startLifetime = 7f;
                    main.startSpeed = 0.18f;
                    main.startSize = 0.12f;
                    main.startColor = new Color(0.1f, 0.95f, 1.0f, 0.25f); // Crisp cyan
                    main.gravityModifier = 0f;
                    break;

                case AtmosphereType.EmergencyEmbers:
                    main.startLifetime = 5f;
                    main.startSpeed = 0.25f;
                    main.startSize = 0.09f;
                    main.startColor = new Color(1.0f, 0.25f, 0.15f, 0.35f); // Emergency crimson/amber
                    main.gravityModifier = -0.03f; // Rises like faint embers
                    break;
            }
        }
    }
}
