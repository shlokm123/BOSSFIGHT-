using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using GlitchVerse.Core;
using GlitchVerse.Player;
using GlitchVerse.Interactables;
using GlitchVerse.Echo;
using GlitchVerse.Mechanics;
using GlitchVerse.UI;
using GlitchVerse.Narrative;
using GlitchVerse.Atmosphere;
using GlitchVerse.Boss;

namespace GlitchVerse.Editor
{
    /// <summary>
    /// Autonomous Builder that procedurally creates stylized 2D sci-fi containment assets
    /// and constructs all four playable levels (Level01 to Level04) for GlitchVerse: Echo Protocol.
    /// PHASE 08: Atmospheric Environment Upgrade (The Last City) with chapter-distinct lighting,
    /// cosmetic squash/stretch visual animation, rich layered depth, and environmental storytelling.
    /// </summary>
    public static class ChamberBuilder
    {
        private const string ArtPath = "Assets/_Game/Art";
        private const string LevelsPath = "Assets/_Game/Scenes/Levels";

        public struct ChapterAtmosphereConfig
        {
            public Color ambientColor;
            public float ambientIntensity;
            public Color spawnLampColor;
            public float spawnLampIntensity;
            public FlickeringLight2D.FlickerMode spawnLampFlicker;
            public AtmosphericParticleEmitter.AtmosphereType particleType;
            public Color architectMonitorColor;
        }

        [MenuItem("GlitchVerse/Build All Seven Levels")]
        public static void BuildAll()
        {
            Debug.Log("[ChamberBuilder] Beginning autonomous build of all 7 levels (Phase 10 Boss Upgrade)...");
            GenerateProceduralSprites();
            
            ConstructLevel01Scene();
            ConstructLevel02Scene();
            ConstructLevel03Scene();
            ConstructLevel04Scene();
            ConstructLevel05Scene();
            ConstructLevel06Scene();
            ConstructLevel07Scene();

            RegisterScenesInBuildSettings();
            Debug.Log("[ChamberBuilder] All seven levels successfully built and registered!");
        }

        [MenuItem("GlitchVerse/Build Level 01 (Deception Upgrade)")]
        public static void BuildLevel01()
        {
            GenerateProceduralSprites();
            ConstructLevel01Scene();
        }

        [MenuItem("GlitchVerse/Build Level 02 (Inversion Upgrade)")]
        public static void BuildLevel02()
        {
            GenerateProceduralSprites();
            ConstructLevel02Scene();
        }

        [MenuItem("GlitchVerse/Build Level 03 (Betrayal Upgrade)")]
        public static void BuildLevel03()
        {
            GenerateProceduralSprites();
            ConstructLevel03Scene();
        }

        [MenuItem("GlitchVerse/Build Level 04 (Revelation Upgrade)")]
        public static void BuildLevel04()
        {
            GenerateProceduralSprites();
            ConstructLevel04Scene();
        }

        [MenuItem("GlitchVerse/Build Level 05 (Paranoia)")]
        public static void BuildLevel05()
        {
            GenerateProceduralSprites();
            ConstructLevel05Scene();
            RegisterScenesInBuildSettings();
        }

        [MenuItem("GlitchVerse/Build Level 06 (Truth)")]
        public static void BuildLevel06()
        {
            GenerateProceduralSprites();
            ConstructLevel06Scene();
            RegisterScenesInBuildSettings();
        }

        [MenuItem("GlitchVerse/Build Level 07 (Judgment Boss)")]
        public static void BuildLevel07()
        {
            GenerateProceduralSprites();
            ConstructLevel07Scene();
            RegisterScenesInBuildSettings();
        }

        #region Sprite Generation
        public static void GenerateProceduralSprites()
        {
            if (!Directory.Exists(ArtPath))
            {
                Directory.CreateDirectory(ArtPath);
            }

            // 1. Floor Tile (64x64) - Dark steel tread plate with glowing cyan top bevel
            CreateSpriteAsset("floor_tile.png", 64, 64, (x, y) =>
            {
                if (y >= 60) return new Color(0f, 0.94f, 1f, 1f); // Cyan top edge
                if (y == 59) return new Color(0f, 0.5f, 0.6f, 1f);
                if (x % 16 == 0 || y % 16 == 0) return new Color(0.10f, 0.12f, 0.16f, 1f);
                if ((x + y) % 8 == 0) return new Color(0.18f, 0.22f, 0.28f, 1f);
                return new Color(0.14f, 0.17f, 0.22f, 1f);
            });

            // 2. Wall Panel (64x64) - Monolithic brutalist steel bulkhead
            CreateSpriteAsset("wall_panel.png", 64, 64, (x, y) =>
            {
                if (x <= 3 || x >= 60 || y <= 3 || y >= 60) return new Color(0.08f, 0.10f, 0.13f, 1f);
                if ((x == 6 || x == 57) && (y == 6 || y == 57)) return new Color(0.4f, 0.45f, 0.5f, 1f);
                if (y >= 45 && y <= 47) return new Color(0.85f, 0.45f, 0.1f, 0.8f);
                if (y >= 41 && y <= 43) return new Color(0.10f, 0.10f, 0.12f, 1f);
                return new Color(0.16f, 0.19f, 0.24f, 1f);
            });

            // 3. Ceiling Beam (64x32) - Heavy structural I-beam
            CreateSpriteAsset("ceiling_beam.png", 64, 32, (x, y) =>
            {
                if (y <= 4 || y >= 27) return new Color(0.11f, 0.13f, 0.17f, 1f);
                if (x % 32 == 0) return new Color(0.07f, 0.08f, 0.11f, 1f);
                if (y >= 14 && y <= 17) return new Color(0.20f, 0.24f, 0.30f, 1f);
                return new Color(0.13f, 0.16f, 0.20f, 1f);
            });

            // 4. Pipe Conduit (64x16) - Industrial pipe
            CreateSpriteAsset("pipe_conduit.png", 64, 16, (x, y) =>
            {
                if (x % 32 <= 2) return new Color(0.35f, 0.40f, 0.45f, 1f);
                if (y == 14 || y == 1) return new Color(0.09f, 0.11f, 0.14f, 1f);
                if (y >= 8 && y <= 11) return new Color(0.30f, 0.35f, 0.42f, 1f);
                return new Color(0.18f, 0.22f, 0.27f, 1f);
            });

            // 5. Movable Cube (64x64) - High-tech containment crate with electric cyan core
            CreateSpriteAsset("cube_crate.png", 64, 64, (x, y) =>
            {
                if (x <= 3 || x >= 60 || y <= 3 || y >= 60) return new Color(0f, 0.94f, 1f, 1f);
                if ((x <= 8 && y <= 8) || (x >= 55 && y <= 8) || (x <= 8 && y >= 55) || (x >= 55 && y >= 55))
                    return new Color(0f, 0.7f, 0.85f, 1f);
                int distFromCenter = Mathf.Abs(x - 32) + Mathf.Abs(y - 32);
                if (distFromCenter <= 10) return new Color(0f, 0.94f, 1f, 1f);
                if (distFromCenter <= 14) return new Color(0f, 0.4f, 0.6f, 0.8f);
                return new Color(0.10f, 0.14f, 0.18f, 1f);
            });

            // 6. Pressure Plate (64x16) - Floor-embedded sensor pad
            CreateSpriteAsset("pressure_plate.png", 64, 16, (x, y) =>
            {
                if (y <= 2) return new Color(0.08f, 0.09f, 0.12f, 1f);
                if (x <= 4 || x >= 59) return new Color(0.20f, 0.24f, 0.30f, 1f);
                if (y >= 8 && y <= 12 && x >= 8 && x <= 55) return new Color(0f, 0.94f, 1f, 1f);
                return new Color(0.14f, 0.18f, 0.22f, 1f);
            });

            // 7. Echo Switch (64x16) - Holographic violet/magenta switch
            CreateSpriteAsset("echo_switch.png", 64, 16, (x, y) =>
            {
                if (y <= 2) return new Color(0.08f, 0.09f, 0.12f, 1f);
                if (x <= 4 || x >= 59) return new Color(0.35f, 0.15f, 0.40f, 1f);
                if (y >= 6 && y <= 14 && x >= 10 && x <= 53) return new Color(0.85f, 0f, 1f, 1f);
                return new Color(0.18f, 0.12f, 0.22f, 1f);
            });

            // 8. Exit Door (48x80) - Heavy sealed blast door with status light bar
            CreateSpriteAsset("exit_door.png", 48, 80, (x, y) =>
            {
                if (x <= 3 || x >= 44 || y <= 3 || y >= 76) return new Color(0.08f, 0.10f, 0.14f, 1f);
                if (y >= 68 && y <= 73 && x >= 8 && x <= 39) return new Color(1.0f, 0.15f, 0.25f, 1f);
                if (x == 24 || y == 25 || y == 50) return new Color(0.05f, 0.06f, 0.08f, 1f);
                if ((x + y) % 12 <= 2) return new Color(0.85f, 0.55f, 0.1f, 0.7f);
                return new Color(0.18f, 0.21f, 0.26f, 1f);
            });

            // 9. Freezable Hazard Laser (32x64) - Pulsing security laser
            CreateSpriteAsset("hazard_laser.png", 32, 64, (x, y) =>
            {
                if (y <= 4 || y >= 59) return new Color(0.2f, 0.2f, 0.25f, 1f);
                if (x >= 12 && x <= 19) return new Color(1.0f, 0.2f, 0.3f, 1f);
                if (x >= 8 && x <= 23) return new Color(1.0f, 0.1f, 0.2f, 0.5f);
                return Color.clear;
            });

            // 10. Dimension Alpha Platform (64x16) - Electric Cyan
            CreateSpriteAsset("dim_alpha_platform.png", 64, 16, (x, y) =>
            {
                if (y >= 13) return new Color(0f, 0.94f, 1f, 1f);
                if (x <= 2 || x >= 61 || y <= 2) return new Color(0f, 0.6f, 0.8f, 1f);
                return new Color(0.08f, 0.20f, 0.28f, 1f);
            });

            // 11. Dimension Beta Platform (64x16) - Vibrant Magenta
            CreateSpriteAsset("dim_beta_platform.png", 64, 16, (x, y) =>
            {
                if (y >= 13) return new Color(1.0f, 0.15f, 0.85f, 1f);
                if (x <= 2 || x >= 61 || y <= 2) return new Color(0.8f, 0.1f, 0.6f, 1f);
                return new Color(0.28f, 0.08f, 0.24f, 1f);
            });

            // 12. Player Sprite (32x56) - Test subject silhouette with glowing cyan visor
            CreateSpriteAsset("player_sprite.png", 32, 56, (x, y) =>
            {
                if (y >= 40 && y <= 52 && x >= 10 && x <= 22)
                {
                    if (y >= 44 && y <= 47 && x >= 15 && x <= 22) return new Color(0f, 0.94f, 1f, 1f);
                    return new Color(0.15f, 0.18f, 0.23f, 1f);
                }
                if (y >= 22 && y <= 39 && x >= 8 && x <= 24)
                {
                    if (y >= 29 && y <= 33 && x >= 14 && x <= 18) return new Color(0f, 0.94f, 1f, 0.9f);
                    return new Color(0.20f, 0.25f, 0.32f, 1f);
                }
                if (y >= 2 && y <= 21 && ((x >= 9 && x <= 14) || (x >= 18 && x <= 23)))
                {
                    return new Color(0.14f, 0.17f, 0.21f, 1f);
                }
                return Color.clear;
            });

            // 13. Echo Ghost Sprite (32x56) - Translucent cyan holographic wireframe
            CreateSpriteAsset("echo_ghost.png", 32, 56, (x, y) =>
            {
                if (y >= 40 && y <= 52 && x >= 10 && x <= 22)
                {
                    bool isEdge = (x == 10 || x == 22 || y == 40 || y == 52);
                    return isEdge ? new Color(0f, 0.94f, 1f, 0.8f) : new Color(0f, 0.7f, 0.9f, 0.35f);
                }
                if (y >= 22 && y <= 39 && x >= 8 && x <= 24)
                {
                    bool isEdge = (x == 8 || x == 24 || y == 22 || y == 39);
                    return isEdge ? new Color(0f, 0.94f, 1f, 0.8f) : new Color(0f, 0.7f, 0.9f, 0.35f);
                }
                if (y >= 2 && y <= 21 && ((x >= 9 && x <= 14) || (x >= 18 && x <= 23)))
                {
                    return new Color(0f, 0.94f, 1f, 0.45f);
                }
                return Color.clear;
            });

            // 14. Architect Monitor (128x64) - Overhead surveillance screen
            CreateSpriteAsset("architect_monitor.png", 128, 64, (x, y) =>
            {
                if (x <= 4 || x >= 123 || y <= 4 || y >= 59) return new Color(0.08f, 0.09f, 0.12f, 1f);
                if (x >= 8 && x <= 119 && y >= 8 && y <= 55)
                {
                    if (y % 4 == 0) return new Color(0.04f, 0.14f, 0.18f, 1f);
                    if (x == 8 || x == 119 || y == 8 || y == 55) return new Color(0f, 0.94f, 1f, 0.8f);
                    return new Color(0.06f, 0.18f, 0.22f, 1f);
                }
                return new Color(0.12f, 0.14f, 0.18f, 1f);
            });

            // 15. Background Column (48x128) - Receding structural pillar
            CreateSpriteAsset("bg_pillar.png", 48, 128, (x, y) =>
            {
                if (x <= 2 || x >= 45) return new Color(0.05f, 0.06f, 0.08f, 1f);
                if (y % 32 == 0) return new Color(0.06f, 0.07f, 0.09f, 1f);
                return new Color(0.09f, 0.11f, 0.14f, 1f);
            });

            // 16. Background Reactor Core (128x128) - Giant circular containment vessel
            CreateSpriteAsset("bg_reactor.png", 128, 128, (x, y) =>
            {
                float dx = x - 64;
                float dy = y - 64;
                float r = Mathf.Sqrt(dx * dx + dy * dy);
                if (r > 60) return Color.clear;
                if (r >= 56 && r <= 60) return new Color(0.08f, 0.10f, 0.13f, 1f);
                if (r >= 38 && r <= 42) return new Color(0f, 0.5f, 0.7f, 0.6f);
                if (r < 20) return new Color(0f, 0.6f, 0.8f, 0.4f);
                return new Color(0.07f, 0.08f, 0.11f, 0.9f);
            });

            // 17. Trap Floor (64x64) - Floor tile with visible amber stress fissure cracks
            CreateSpriteAsset("trap_floor.png", 64, 64, (x, y) =>
            {
                bool isCrack = (x >= 20 && x <= 23 && y >= 46 && y <= 59) ||
                               (x >= 23 && x <= 26 && y >= 36 && y <= 46) ||
                               (x >= 26 && x <= 29 && y >= 24 && y <= 36) ||
                               (x >= 29 && x <= 33 && y >= 12 && y <= 24) ||
                               (x >= 33 && x <= 36 && y >= 4 && y <= 12);

                if (isCrack) return new Color(1.0f, 0.55f, 0.12f, 1f); // Distinct glowing amber fissure
                if (y >= 60 && (x < 19 || x > 24)) return new Color(0f, 0.94f, 1f, 1f);
                if (y >= 60) return new Color(1.0f, 0.45f, 0.1f, 1f); // Faulty/fractured top edge at crack
                if (y == 59 && (x >= 19 && x <= 24)) return new Color(1.0f, 0.35f, 0.1f, 1f);
                if (y == 59) return new Color(0f, 0.5f, 0.6f, 1f);
                if (x % 16 == 0 || y % 16 == 0) return new Color(0.10f, 0.12f, 0.16f, 1f);
                if ((x + y) % 8 == 0) return new Color(0.18f, 0.22f, 0.28f, 1f);
                return new Color(0.14f, 0.17f, 0.22f, 1f);
            });

            // 18. Wall Grate (64x64) - Industrial ventilation grate with metallic louvers
            CreateSpriteAsset("wall_grate.png", 64, 64, (x, y) =>
            {
                if (x <= 3 || x >= 60 || y <= 3 || y >= 60) return new Color(0.10f, 0.12f, 0.15f, 1f);
                if ((x == 6 || x == 57) && (y == 6 || y == 57)) return new Color(0.45f, 0.50f, 0.58f, 1f);
                if (x <= 6 || x >= 57 || y <= 6 || y >= 57) return new Color(0.06f, 0.08f, 0.10f, 1f);
                int louverY = y % 8;
                if (louverY == 0 || louverY == 1) return new Color(0.04f, 0.05f, 0.07f, 1f);
                if (louverY == 2) return new Color(0.18f, 0.22f, 0.27f, 1f);
                if (louverY >= 3 && louverY <= 6) return new Color(0.12f, 0.15f, 0.19f, 1f);
                return new Color(0.08f, 0.10f, 0.13f, 1f);
            });

            // 19. Cable Bundle (64x32) - Sagging high-voltage industrial cables draped from ceiling
            CreateSpriteAsset("cable_bundle.png", 64, 32, (x, y) =>
            {
                float t = (x - 32f) / 32f;
                float sag1 = 28f - (t * t * 14f);
                if (Mathf.Abs(y - sag1) <= 1.2f) return new Color(0.20f, 0.24f, 0.30f, 1f);

                float sag2 = 24f - (t * t * 18f);
                if (Mathf.Abs(y - sag2) <= 1.5f)
                {
                    if ((x / 4) % 2 == 0) return new Color(0.85f, 0.55f, 0.10f, 1f);
                    return new Color(0.10f, 0.11f, 0.14f, 1f);
                }

                float sag3 = 20f - (t * t * 16f);
                if (Mathf.Abs(y - sag3) <= 1.2f) return new Color(0.14f, 0.16f, 0.20f, 1f);

                if ((x <= 6 || x >= 57) && y >= 22) return new Color(0.35f, 0.40f, 0.48f, 1f);
                return Color.clear;
            });

            // 20. Hazard Stripes (64x16) - 45-degree angled containment caution stripes
            CreateSpriteAsset("hazard_stripes.png", 64, 16, (x, y) =>
            {
                if (y == 0 || y == 15) return new Color(0.08f, 0.10f, 0.12f, 1f);
                if (y == 14) return new Color(0.30f, 0.35f, 0.42f, 1f);
                int diagonal = (x + y) % 16;
                if (diagonal < 8) return new Color(0.88f, 0.58f, 0.08f, 1f);
                return new Color(0.10f, 0.11f, 0.14f, 1f);
            });

            // 21. Subject 075 Wall Decal (64x32) - Stenciled containment identification
            CreateSpriteAsset("decal_subject075.png", 64, 32, (x, y) =>
            {
                bool isCorner = ((x <= 4 || x >= 59) && (y <= 4 || y >= 27));
                if (isCorner) return new Color(0.30f, 0.50f, 0.65f, 0.8f);

                if (y >= 14 && y <= 18 && x >= 8 && x <= 55)
                {
                    if (x % 5 == 0) return Color.clear;
                    return new Color(0.35f, 0.60f, 0.75f, 0.75f);
                }
                if (y >= 8 && y <= 10 && x >= 12 && x <= 51)
                {
                    if (x % 4 == 0) return Color.clear;
                    return new Color(0.25f, 0.40f, 0.52f, 0.60f);
                }
                return Color.clear;
            });

            // 22. Quarantine Warning Decal (64x32) - Stenciled hazard symbol & biohazard alert
            CreateSpriteAsset("decal_quarantine.png", 64, 32, (x, y) =>
            {
                if ((x == 4 || x == 59) && y >= 6 && y <= 25) return new Color(0.9f, 0.3f, 0.15f, 0.8f);
                int dx = x - 18;
                int dy = y - 16;
                if (dx * dx + dy * dy <= 48)
                {
                    if (dx * dx + dy * dy <= 8) return Color.clear;
                    return new Color(0.95f, 0.45f, 0.1f, 0.85f);
                }
                if (x >= 30 && x <= 58)
                {
                    if (y >= 18 && y <= 21 && (x % 4 != 0)) return new Color(0.9f, 0.35f, 0.15f, 0.8f);
                    if (y >= 10 && y <= 13 && (x % 4 != 0)) return new Color(0.7f, 0.25f, 0.15f, 0.6f);
                }
                return Color.clear;
            });

            // 23. Lab Server Terminal (32x64) - Damaged scientific monitoring equipment
            CreateSpriteAsset("lab_terminal.png", 32, 64, (x, y) =>
            {
                if (x <= 2 || x >= 29 || y <= 2 || y >= 61) return new Color(0.12f, 0.14f, 0.18f, 1f);
                if (y >= 40 && y <= 57 && x >= 5 && x <= 26)
                {
                    if (x == 5 || x == 26 || y == 40 || y == 57) return new Color(0f, 0.94f, 1f, 0.7f);
                    int waveY = 48 + (int)(Mathf.Sin(x * 0.6f) * 4f);
                    if (y == waveY) return new Color(0f, 0.94f, 1f, 1f);
                    return new Color(0.04f, 0.12f, 0.16f, 1f);
                }
                if (y >= 22 && y <= 36 && x >= 5 && x <= 26)
                {
                    if (y == 28 && (x == 8 || x == 14 || x == 20))
                    {
                        if (x == 8) return new Color(0f, 1f, 0.4f, 1f);
                        if (x == 14) return new Color(1f, 0.7f, 0f, 1f);
                        return new Color(0f, 0.8f, 1f, 1f);
                    }
                    if (y % 4 == 0) return new Color(0.08f, 0.10f, 0.12f, 1f);
                    return new Color(0.16f, 0.19f, 0.24f, 1f);
                }
                if (y >= 6 && y <= 18 && x >= 6 && x <= 25)
                {
                    if (x % 4 == 0) return new Color(0.10f, 0.12f, 0.15f, 1f);
                    return new Color(0.20f, 0.25f, 0.32f, 1f);
                }
                return new Color(0.10f, 0.12f, 0.15f, 1f);
            });

            // 24. Cryo Containment Pod (48x96) - Vertical specimen capsule
            CreateSpriteAsset("containment_pod.png", 48, 96, (x, y) =>
            {
                if (y <= 12 || y >= 83)
                {
                    if (x <= 3 || x >= 44) return new Color(0.10f, 0.12f, 0.15f, 1f);
                    if (y == 6 || y == 89) return new Color(0.35f, 0.40f, 0.48f, 1f);
                    return new Color(0.18f, 0.22f, 0.28f, 1f);
                }
                if (x <= 6 || x >= 41) return new Color(0.14f, 0.18f, 0.24f, 1f);
                if (x == 7 || x == 40) return new Color(0f, 0.8f, 0.95f, 0.6f);
                float cy = (y - 48f) / 35f;
                float alpha = 0.55f - (Mathf.Abs(cy) * 0.15f);
                if (x >= 20 && x <= 27 && y >= 32 && y <= 64)
                {
                    return new Color(0.04f, 0.18f, 0.24f, 0.85f);
                }
                return new Color(0.02f, 0.30f, 0.40f, alpha);
            });

            // 25. Floor Debris (32x16) - Scrap metal, severed conduits, broken clamps
            CreateSpriteAsset("floor_debris.png", 32, 16, (x, y) =>
            {
                if (x >= 4 && x <= 12 && y >= 2 && y <= 8 && (x + y <= 16))
                {
                    if (y == 7 || x == 11) return new Color(0.35f, 0.40f, 0.46f, 1f);
                    return new Color(0.16f, 0.19f, 0.23f, 1f);
                }
                if (x >= 15 && x <= 22 && y >= 1 && y <= 5)
                {
                    if (x == 18) return new Color(0.85f, 0.55f, 0.1f, 1f);
                    return new Color(0.24f, 0.28f, 0.34f, 1f);
                }
                if (x >= 24 && x <= 29 && y >= 2 && y <= 10 && ((x - 24) == (y - 2) || (x - 24) == (y - 3)))
                {
                    return new Color(0.28f, 0.32f, 0.38f, 1f);
                }
                return Color.clear;
            });

            // 26. Warning Beacon (24x24) - Wall-mounted alarm light with protective wire cage
            CreateSpriteAsset("warning_beacon.png", 24, 24, (x, y) =>
            {
                if (y <= 5) return new Color(0.12f, 0.14f, 0.18f, 1f);
                int dx = x - 12;
                int dy = y - 14;
                if (dx * dx + dy * dy <= 50)
                {
                    if (x == 8 || x == 12 || x == 16 || y == 14) return new Color(0.20f, 0.24f, 0.30f, 1f);
                    return new Color(1.0f, 0.25f, 0.15f, 0.9f);
                }
                return Color.clear;
            });

            // 27. Distant Skyline Slit View (128x64) - Reinforced observation slit showing The Last City
            CreateSpriteAsset("distant_skyline_vent.png", 128, 64, (x, y) =>
            {
                if (x <= 5 || x >= 122 || y <= 5 || y >= 58) return new Color(0.07f, 0.08f, 0.11f, 1f);
                if (x == 42 || x == 85) return new Color(0.12f, 0.14f, 0.18f, 1f);
                
                float skyT = (float)(y - 6) / 52f;
                Color skyColor = Color.Lerp(new Color(0.18f, 0.04f, 0.06f, 1f), new Color(0.04f, 0.02f, 0.04f, 1f), skyT);

                bool isTower = (x >= 12 && x <= 26 && y <= 35) ||
                               (x >= 32 && x <= 39 && y <= 45) ||
                               (x >= 50 && x <= 68 && y <= 40) ||
                               (x >= 75 && x <= 82 && y <= 30) ||
                               (x >= 92 && x <= 116 && y <= 48);

                if (isTower)
                {
                    if ((y == 35 && x == 19) || (y == 45 && x == 35) || (y == 40 && x == 59) || (y == 48 && x == 104))
                    {
                        return new Color(1.0f, 0.1f, 0.15f, 1f);
                    }
                    return new Color(0.05f, 0.03f, 0.05f, 1f);
                }

                return Color.Lerp(skyColor, new Color(0.05f, 0.10f, 0.14f, 1f), 0.25f);
            });

            // 29. Architect Boss Core (128x128) - Giant corrupted surveillance eye & AI reactor
            CreateSpriteAsset("architect_boss_core.png", 128, 128, (x, y) =>
            {
                float dx = x - 64;
                float dy = y - 64;
                float r = Mathf.Sqrt(dx * dx + dy * dy);
                if (r > 62) return Color.clear;

                // Heavy brutalist steel armored ring
                if (r >= 52 && r <= 62)
                {
                    if (x % 16 == 0 || y % 16 == 0) return new Color(0.12f, 0.15f, 0.20f, 1f);
                    return new Color(0.08f, 0.10f, 0.14f, 1f);
                }

                // Inner aperture iris
                if (r >= 38 && r < 52)
                {
                    if ((x + y) % 6 == 0) return new Color(0.9f, 0.15f, 0.25f, 0.8f);
                    return new Color(0.16f, 0.18f, 0.24f, 1f);
                }

                // Glowing central camera eye / core pupil
                if (r < 38)
                {
                    if (r <= 12) return new Color(1.0f, 0.95f, 0.95f, 1f); // Hot white core center
                    if (r <= 22) return new Color(1.0f, 0.15f, 0.25f, 1f); // Corrupted red pupil
                    if (y % 4 == 0) return new Color(0f, 0.94f, 1f, 0.85f); // Cyan digital scanlines
                    return new Color(0.7f, 0.08f, 0.20f, 1f);
                }

                return new Color(0.10f, 0.12f, 0.16f, 1f);
            });

            // 30. Boss Control Override Node (64x64) - Heavy industrial terminal with cyan conduits
            CreateSpriteAsset("boss_control_node.png", 64, 64, (x, y) =>
            {
                if (x <= 3 || x >= 60 || y <= 3 || y >= 60) return new Color(0.08f, 0.10f, 0.14f, 1f);
                if (x <= 6 || x >= 57 || y <= 6 || y >= 57) return new Color(0.25f, 0.30f, 0.38f, 1f);
                
                int dist = Mathf.Abs(x - 32) + Mathf.Abs(y - 32);
                if (dist <= 14)
                {
                    if (dist <= 8) return new Color(0f, 0.95f, 1f, 1f); // Glowing cyan active sensor
                    return new Color(0.10f, 0.40f, 0.55f, 1f);
                }

                if (x == 32 || y == 32) return new Color(0f, 0.8f, 0.95f, 0.8f); // Cyan power traces
                return new Color(0.14f, 0.17f, 0.22f, 1f);
            });

            // 31. Energy Surge Hazard Beam (128x32) - Telegraphed sweeping energy wave
            CreateSpriteAsset("energy_surge_hazard.png", 128, 32, (x, y) =>
            {
                if (y <= 2 || y >= 29) return new Color(0.2f, 0.05f, 0.08f, 1f);
                if (y >= 12 && y <= 19)
                {
                    if (y >= 14 && y <= 17) return new Color(1.0f, 0.95f, 0.90f, 1f); // Hot white core
                    return new Color(1.0f, 0.2f, 0.3f, 1f); // Electric crimson plasma
                }
                if (x % 16 <= 2 || (x + y) % 8 == 0) return new Color(1.0f, 0.4f, 0.1f, 0.7f); // Arcing discharge
                return new Color(0.8f, 0.1f, 0.2f, 0.4f);
            });

            // 32. Security Barrier Grid (48x80) - Laser fence interlock barrier
            CreateSpriteAsset("security_barrier_grid.png", 48, 80, (x, y) =>
            {
                if (y <= 5 || y >= 74) return new Color(0.12f, 0.14f, 0.18f, 1f);
                if (x <= 4 || x >= 43) return new Color(0.18f, 0.22f, 0.28f, 1f);

                if ((y <= 8 || y >= 71) && (x + y) % 6 <= 2) return new Color(1.0f, 0.55f, 0.10f, 0.9f);

                if (x == 12 || x == 18 || x == 24 || x == 30 || x == 36)
                {
                    return new Color(1.0f, 0.95f, 0.95f, 0.95f); // Hot white core
                }
                if (Mathf.Abs(x - 12) <= 2 || Mathf.Abs(x - 18) <= 2 || Mathf.Abs(x - 24) <= 2 || Mathf.Abs(x - 30) <= 2 || Mathf.Abs(x - 36) <= 2)
                {
                    return new Color(1.0f, 0.15f, 0.25f, 0.75f); // Crimson energy aura
                }

                return new Color(1.0f, 0.1f, 0.2f, 0.12f); // Faint laser field haze
            });

            // 33. Secondary Breaker Switch (64x32) - High-tech mechanical override switch
            CreateSpriteAsset("secondary_breaker_switch.png", 64, 32, (x, y) =>
            {
                if (y <= 3) return new Color(0.08f, 0.10f, 0.13f, 1f);
                if (x <= 4 || x >= 59 || y >= 28) return new Color(0.16f, 0.20f, 0.26f, 1f);

                if (y <= 8 && (x + y) % 8 <= 3) return new Color(0.95f, 0.60f, 0.10f, 1f);

                if (x >= 12 && x <= 51 && y >= 10 && y <= 24)
                {
                    if (x >= 20 && x <= 43 && y >= 14 && y <= 20)
                    {
                        return new Color(1.0f, 0.65f, 0.15f, 1f); // Illuminated amber pad
                    }
                    return new Color(0.24f, 0.28f, 0.35f, 1f);
                }

                return new Color(0.12f, 0.15f, 0.20f, 1f);
            });

            // 34A. Chapter 1: Lower Wall Wainscot Panel (128x128) - Heavy rusted steel/concrete base wainscoting
            CreateSpriteAsset("ch1_wall_lower_wainscot.png", 128, 128, (x, y) =>
            {
                if (x == 0 || x == 127) return new Color(0.10f, 0.09f, 0.08f, 1f);
                if (y == 55 || y == 56) return new Color(0.12f, 0.10f, 0.09f, 1f); // Wainscot dividing joint seam
                if (y <= 6) return new Color(0.08f, 0.07f, 0.06f, 1f); // Dark steel base kickboard

                float n = Mathf.Sin(x * 12.9898f + y * 78.233f) * 43758.5453f;
                float noise = n - Mathf.Floor(n);
                float n2 = Mathf.Sin(x * 26.345f + y * 45.123f) * 23421.631f;
                float noise2 = n2 - Mathf.Floor(n2);

                bool isRivet = ((x == 8 || x == 119) && (y == 16 || y == 46 || y == 70 || y == 118));
                bool isRivetHalo = ((Mathf.Abs(x - 8) <= 2 || Mathf.Abs(x - 119) <= 2) && 
                                    (Mathf.Abs(y - 16) <= 2 || Mathf.Abs(y - 46) <= 2 || Mathf.Abs(y - 70) <= 2 || Mathf.Abs(y - 118) <= 2));

                bool isRustDrip = false;
                if ((x == 8 || x == 9 || x == 118 || x == 119 || x == 42 || x == 43 || x == 85 || x == 86) && y < 56)
                {
                    float dripLen = 22f + ((x * 17) % 30);
                    if (56 - y < dripLen) isRustDrip = true;
                }

                if (isRivet) return new Color(0.24f, 0.20f, 0.16f, 1f);
                if (isRivetHalo) return new Color(0.40f, 0.22f, 0.14f, 1f);
                if (isRustDrip)
                {
                    float rustBlend = 0.5f + 0.3f * noise;
                    return Color.Lerp(new Color(0.45f, 0.24f, 0.14f, 1f), new Color(0.28f, 0.16f, 0.10f, 1f), rustBlend);
                }

                if (y < 56)
                {
                    // Stained wainscoting with warm rust, scratches, and grime
                    float baseVal = 0.38f + noise * 0.12f;
                    Color wainscotCol = new Color(baseVal * 1.15f, baseVal * 0.95f, baseVal * 0.82f, 1f);
                    if (noise2 > 0.65f) wainscotCol = Color.Lerp(wainscotCol, new Color(0.58f, 0.30f, 0.15f, 1f), 0.60f);
                    return wainscotCol;
                }
                else
                {
                    // Upper section transition to weathered off-white/beige bunker concrete
                    float baseVal = 0.76f + noise * 0.10f;
                    Color concreteCol = new Color(baseVal * 1.02f, baseVal * 0.98f, baseVal * 0.90f, 1f);
                    if (y < 68 && noise > 0.35f)
                    {
                        float rustFactor = (68f - y) / 12f * 0.6f;
                        concreteCol = Color.Lerp(concreteCol, new Color(0.62f, 0.34f, 0.18f, 1f), rustFactor);
                    }
                    return concreteCol;
                }
            });

            // 34B. Chapter 1: Upper Wall Weathered Concrete Panel (128x128) - Continuous weathered off-white/beige bunker wall
            System.Func<int, int, Color> upperConcreteFunc = (x, y) =>
            {
                if (x == 0 || x == 127) return new Color(0.20f, 0.18f, 0.16f, 1f);
                if (y == 0 || y == 127) return new Color(0.24f, 0.22f, 0.20f, 1f);

                float n = Mathf.Sin(x * 12.9898f + y * 78.233f) * 43758.5453f;
                float noise = n - Mathf.Floor(n);
                float n2 = Mathf.Sin(x * 26.345f + y * 45.123f) * 23421.631f;
                float noise2 = n2 - Mathf.Floor(n2);

                bool isRivet = ((x == 8 || x == 119) && (y == 16 || y == 64 || y == 112));
                bool isRivetHalo = ((Mathf.Abs(x - 8) <= 2 || Mathf.Abs(x - 119) <= 2) && 
                                    (Mathf.Abs(y - 16) <= 2 || Mathf.Abs(y - 64) <= 2 || Mathf.Abs(y - 112) <= 2));

                bool isRustDrip = false;
                if ((x == 8 || x == 9 || x == 118 || x == 119 || x == 48 || x == 49 || x == 80 || x == 81) && noise2 > 0.30f)
                {
                    isRustDrip = true;
                }

                if (isRivet) return new Color(0.35f, 0.30f, 0.25f, 1f);
                if (isRivetHalo) return new Color(0.58f, 0.32f, 0.18f, 1f);
                if (isRustDrip)
                {
                    float rustBlend = 0.5f + 0.3f * noise;
                    return Color.Lerp(new Color(0.62f, 0.34f, 0.18f, 1f), new Color(0.42f, 0.24f, 0.14f, 1f), rustBlend);
                }

                // Weathered off-white / beige industrial concrete
                float baseVal = 0.78f + noise * 0.10f + (noise2 > 0.8f ? -0.06f : 0.03f);
                Color concreteCol = new Color(baseVal * 1.02f, baseVal * 0.98f, baseVal * 0.90f, 1f);
                return concreteCol;
            };

            CreateSpriteAsset("ch1_wall_upper_concrete.png", 128, 128, upperConcreteFunc);
            CreateSpriteAsset("ch1_weathered_concrete.png", 128, 128, upperConcreteFunc);

            // 35. Chapter 1: Heavy Industrial Platform Deck (128x64) - Diamond plate tread with riveted girder rim
            CreateSpriteAsset("ch1_heavy_platform.png", 128, 64, (x, y) =>
            {
                if (y >= 44)
                {
                    if (y == 63) return new Color(0.45f, 0.48f, 0.52f, 1f);
                    if (y == 62) return new Color(0.35f, 0.38f, 0.42f, 1f);
                    int pattern = (x + y) % 8;
                    int pattern2 = (x - y + 64) % 8;
                    bool isTreadNub = (pattern == 0 && pattern2 <= 2) || (pattern2 == 0 && pattern <= 2);
                    if (isTreadNub) return new Color(0.38f, 0.42f, 0.46f, 1f);

                    float n = (Mathf.Sin(x * 19f + y * 31f) + 1f) * 0.5f;
                    if (n > 0.85f) return new Color(0.28f, 0.20f, 0.15f, 1f);
                    return new Color(0.18f, 0.20f, 0.24f, 1f);
                }

                if (y >= 16)
                {
                    if (y == 43) return new Color(0.10f, 0.11f, 0.13f, 1f);
                    if (y == 42) return new Color(0.14f, 0.15f, 0.18f, 1f);
                    if (y == 16) return new Color(0.28f, 0.30f, 0.35f, 1f);

                    int rivetX = x % 16;
                    bool isRivet = (rivetX == 8) && (y == 34 || y == 24);
                    bool isRivetEdge = (Mathf.Abs(rivetX - 8) <= 1) && (Mathf.Abs(y - 34) <= 1 || Mathf.Abs(y - 24) <= 1);
                    if (isRivet) return new Color(0.42f, 0.45f, 0.50f, 1f);
                    if (isRivetEdge) return new Color(0.12f, 0.14f, 0.17f, 1f);

                    if (x == 0 || x == 64 || x == 127) return new Color(0.08f, 0.09f, 0.11f, 1f);
                    if (x == 1 || x == 65) return new Color(0.25f, 0.28f, 0.32f, 1f);

                    float n = (Mathf.Sin(x * 37f + y * 73f) + 1f) * 0.5f;
                    float baseSteel = 0.17f + n * 0.04f;
                    Color steelCol = new Color(baseSteel, baseSteel * 1.05f, baseSteel * 1.15f, 1f);
                    if (rivetX == 8 && y < 24) steelCol = Color.Lerp(steelCol, new Color(0.10f, 0.08f, 0.06f, 1f), 0.6f);
                    return steelCol;
                }

                if (y < 16)
                {
                    int strut = (x / 2 + y) % 16;
                    if (strut <= 2 && y > 4) return new Color(0.12f, 0.14f, 0.17f, 1f);
                    if ((x == 22 || x == 78 || x == 104) && y < 10) return new Color(0.08f, 0.09f, 0.10f, 1f);
                    return new Color(0.05f, 0.06f, 0.08f, 0.95f);
                }

                return new Color(0.15f, 0.18f, 0.22f, 1f);
            });

            // 36. Chapter 1: Heavy Overhead Truss (128x64) - Steel roof structure with rivet plates
            CreateSpriteAsset("ch1_overhead_truss.png", 128, 64, (x, y) =>
            {
                if (y >= 58)
                {
                    if (y == 58) return new Color(0.12f, 0.13f, 0.16f, 1f);
                    if (y == 63) return new Color(0.35f, 0.38f, 0.44f, 1f);
                    return new Color(0.20f, 0.22f, 0.26f, 1f);
                }
                if (y <= 6)
                {
                    if (y == 6) return new Color(0.38f, 0.42f, 0.48f, 1f);
                    if (y == 0) return new Color(0.08f, 0.09f, 0.11f, 1f);
                    return new Color(0.22f, 0.24f, 0.28f, 1f);
                }

                int postX = x % 32;
                if (postX <= 3 || postX >= 29)
                {
                    if ((postX == 1 || postX == 30) && (y == 12 || y == 24 || y == 40 || y == 52))
                        return new Color(0.45f, 0.48f, 0.54f, 1f);
                    return new Color(0.18f, 0.20f, 0.24f, 1f);
                }

                int seg = x / 32;
                int localX = x % 32;
                bool isDiag = false;
                if (seg % 2 == 0)
                {
                    float targetY = 6f + (localX / 32f) * 52f;
                    if (Mathf.Abs(y - targetY) <= 3.0f) isDiag = true;
                }
                else
                {
                    float targetY = 58f - (localX / 32f) * 52f;
                    if (Mathf.Abs(y - targetY) <= 3.0f) isDiag = true;
                }

                if (isDiag) return new Color(0.24f, 0.27f, 0.32f, 1f);
                if (y >= 26 && y <= 30 && x % 4 != 0) return new Color(0.12f, 0.14f, 0.18f, 0.7f);
                return Color.clear;
            });

            // 37. Chapter 1: Catwalk Railing (64x32) - Weathered yellow industrial pipe railing
            CreateSpriteAsset("ch1_catwalk_railing.png", 64, 32, (x, y) =>
            {
                bool isStanchion = (Mathf.Abs(x - 4) <= 1 || Mathf.Abs(x - 32) <= 1 || Mathf.Abs(x - 60) <= 1) && y <= 30;
                bool isTopRail = (y >= 28 && y <= 31);
                bool isMidRail = (y >= 14 && y <= 17);
                bool isToeKick = (y <= 4);

                if (isTopRail)
                {
                    if (y == 31) return new Color(0.85f, 0.65f, 0.20f, 1f);
                    if (y == 30) return new Color(0.75f, 0.52f, 0.15f, 1f);
                    return new Color(0.45f, 0.32f, 0.10f, 1f);
                }
                if (isMidRail)
                {
                    if (y == 17) return new Color(0.70f, 0.50f, 0.15f, 1f);
                    return new Color(0.38f, 0.28f, 0.10f, 1f);
                }
                if (isToeKick)
                {
                    if (y == 4) return new Color(0.32f, 0.35f, 0.40f, 1f);
                    return new Color(0.15f, 0.17f, 0.20f, 1f);
                }
                if (isStanchion)
                {
                    if (x % 2 == 0) return new Color(0.60f, 0.45f, 0.15f, 1f);
                    return new Color(0.28f, 0.22f, 0.10f, 1f);
                }
                return Color.clear;
            });

            // 38. Chapter 1: Illuminated EXIT :) Sign (64x24) - Physical amber lightbox sign
            CreateSpriteAsset("ch1_exit_sign.png", 64, 24, (x, y) =>
            {
                if (x <= 2 || x >= 61 || y <= 2 || y >= 21) return new Color(0.14f, 0.13f, 0.12f, 1f);
                if (x <= 4 || x >= 59 || y <= 4 || y >= 19) return new Color(0.28f, 0.25f, 0.22f, 1f);

                Color amberGlow = new Color(1.0f, 0.72f, 0.20f, 1f);
                Color amberBright = new Color(1.0f, 0.90f, 0.45f, 1f);
                Color darkCharcoal = new Color(0.10f, 0.09f, 0.08f, 0.95f);

                bool e = (x >= 10 && x <= 15 && y >= 8 && y <= 15) &&
                         (x == 10 || x == 11 || y == 8 || y == 9 || y == 11 || y == 12 || y == 14 || y == 15);
                bool xChar = (x >= 18 && x <= 23 && y >= 8 && y <= 15) &&
                             (Mathf.Abs((x - 18) - (y - 8)) <= 1 || Mathf.Abs((x - 18) - (15 - y)) <= 1);
                bool i = (x >= 26 && x <= 28 && y >= 8 && y <= 15);
                bool t = (x >= 31 && x <= 37 && y >= 8 && y <= 15) &&
                         (y >= 14 || (x >= 33 && x <= 35));
                bool colon = (x >= 42 && x <= 43 && ((y >= 9 && y <= 10) || (y >= 13 && y <= 14)));
                bool paren = (x >= 46 && x <= 49 && y >= 8 && y <= 15) &&
                             ((y == 8 || y == 15) ? (x == 46 || x == 47) :
                              (y == 9 || y == 14) ? (x == 47 || x == 48) :
                              (x == 48 || x == 49));

                if (e || xChar || i || t || colon || paren) return darkCharcoal;

                float distFromCenter = Mathf.Abs(x - 32) / 32f + Mathf.Abs(y - 12) / 12f;
                return Color.Lerp(amberBright, amberGlow, distFromCenter * 0.7f);
            });

            // 39. Chapter 1: Cinematic Blast Door (64x96) - Heavy industrial containment blast door
            CreateSpriteAsset("ch1_blast_door_cinematic.png", 64, 96, (x, y) =>
            {
                if (x <= 3 || x >= 60 || y <= 2 || y >= 93) return new Color(0.12f, 0.13f, 0.15f, 1f);
                if (x <= 6 || x >= 57 || y <= 4 || y >= 91)
                {
                    if ((x == 5 || x == 58) && (y % 12 == 6)) return new Color(0.40f, 0.44f, 0.50f, 1f);
                    return new Color(0.24f, 0.27f, 0.32f, 1f);
                }

                if (x == 31 || x == 32) return new Color(0.08f, 0.09f, 0.10f, 1f);
                if (x == 30 || x == 33) return new Color(0.35f, 0.38f, 0.44f, 1f);

                int cy = y - 60;
                int cx = x - 32;
                bool isEmblem = false;
                if (cy >= -12 && cy <= 12)
                {
                    int halfWidth = (cy + 12);
                    if (Mathf.Abs(cx) <= halfWidth)
                    {
                        if (cy >= -5 && Mathf.Abs(cx) <= (cy + 5) * 0.75f)
                            isEmblem = false;
                        else
                            isEmblem = true;
                    }
                }

                if (y <= 22)
                {
                    if (y == 22) return new Color(0.10f, 0.11f, 0.13f, 1f);
                    if (y == 21) return new Color(0.35f, 0.38f, 0.42f, 1f);
                    float n = (Mathf.Sin(x * 47f + y * 89f) + 1f) * 0.5f;
                    if (n > 0.8f) return new Color(0.35f, 0.22f, 0.15f, 1f);
                    return new Color(0.20f, 0.22f, 0.26f, 1f);
                }

                if (isEmblem) return new Color(0.30f, 0.18f, 0.14f, 0.9f);

                float noise = (Mathf.Sin(x * 17f + y * 29f) + 1f) * 0.5f;
                float baseVal = 0.58f + noise * 0.08f;
                Color doorCol = new Color(baseVal * 1.02f, baseVal * 0.98f, baseVal * 0.92f, 1f);
                if (y < 40 && noise > 0.45f)
                {
                    float rf = (40f - y) / 20f * 0.5f;
                    doorCol = Color.Lerp(doorCol, new Color(0.40f, 0.24f, 0.14f, 1f), rf);
                }
                return doorCol;
            });

            // 40. Chapter 1: Corporate Mural Decal (128x96) - Inverted Triangle + "A SAFER TOMORROW"
            CreateSpriteAsset("ch1_mural_safer_tomorrow.png", 128, 96, (x, y) =>
            {
                Color stencilColor = new Color(0.45f, 0.18f, 0.14f, 0.85f);
                Color stencilFaded = new Color(0.35f, 0.14f, 0.11f, 0.60f);

                float noise = (Mathf.Sin(x * 43.12f + y * 71.34f) + 1f) * 0.5f;
                if (noise > 0.88f) return Color.clear;

                int cy = y - 62;
                int cx = x - 64;
                if (cy >= -18 && cy <= 18)
                {
                    int maxHalfW = (cy + 18);
                    if (Mathf.Abs(cx) <= maxHalfW)
                    {
                        if (cy >= -8 && Mathf.Abs(cx) <= (cy + 8) * 0.75f)
                        {
                            if (cy >= -3 && cy <= 5 && Mathf.Abs(cx) <= (cy + 3) * 0.5f)
                                return (noise > 0.7f) ? stencilFaded : stencilColor;
                            return Color.clear;
                        }
                        return (noise > 0.7f) ? stencilFaded : stencilColor;
                    }
                }

                if (y >= 28 && y <= 35)
                {
                    bool a = (x >= 32 && x <= 38) && (x == 32 || x == 38 || y == 35 || y == 31);
                    bool s = (x >= 46 && x <= 52) && (y == 35 || y == 31 || y == 28 || (x == 46 && y >= 31) || (x == 52 && y <= 31));
                    bool a2 = (x >= 55 && x <= 61) && (x == 55 || x == 61 || y == 35 || y == 31);
                    bool f = (x >= 64 && x <= 70) && (x == 64 || y == 35 || y == 31);
                    bool e = (x >= 73 && x <= 79) && (x == 73 || y == 35 || y == 31 || y == 28);
                    bool r = (x >= 82 && x <= 88) && (x == 82 || y == 35 || y == 31 || (x == 88 && y >= 31) || (x == y + 54 && y <= 31));

                    if (a || s || a2 || f || e || r) return (noise > 0.65f) ? stencilFaded : stencilColor;
                }

                if (y >= 15 && y <= 22)
                {
                    bool t = (x >= 24 && x <= 30) && (y == 22 || x == 27);
                    bool o = (x >= 34 && x <= 40) && (x == 34 || x == 40 || y == 22 || y == 15);
                    bool m = (x >= 44 && x <= 52) && (x == 44 || x == 52 || (y >= 18 && (x == 48 || Mathf.Abs(x - 48) == (22 - y))));
                    bool o2 = (x >= 56 && x <= 62) && (x == 56 || x == 62 || y == 22 || y == 15);
                    bool r1 = (x >= 66 && x <= 72) && (x == 66 || y == 22 || y == 18 || (x == 72 && y >= 18) || (x == y + 51 && y <= 18));
                    bool r2 = (x >= 76 && x <= 82) && (x == 76 || y == 22 || y == 18 || (x == 82 && y >= 18) || (x == y + 61 && y <= 18));
                    bool o3 = (x >= 86 && x <= 92) && (x == 86 || x == 92 || y == 22 || y == 15);
                    bool w = (x >= 96 && x <= 104) && (x == 96 || x == 104 || (y <= 19 && (x == 100 || Mathf.Abs(x - 100) == (y - 15))));

                    if (t || o || m || o2 || r1 || r2 || o3 || w) return (noise > 0.65f) ? stencilFaded : stencilColor;
                }

                return Color.clear;
            });

            // 41. Chapter 1: Architect Surveillance Desk Terminal (64x48) - Retro CRT with smiley face and desk lamp
            CreateSpriteAsset("ch1_desk_terminal.png", 64, 48, (x, y) =>
            {
                if (x >= 18 && x <= 42 && y >= 22 && y <= 45)
                {
                    bool isBezel = (x == 18 || x == 42 || y == 22 || y == 45 ||
                                   ((x <= 20 || x >= 40) && (y <= 24 || y >= 43)));
                    if (isBezel) return new Color(0.18f, 0.20f, 0.24f, 1f);
                    if (x == 19 || x == 41 || y == 23 || y == 44) return new Color(0.08f, 0.10f, 0.12f, 1f);

                    bool isScanline = (y % 2 == 0);
                    Color screenBase = isScanline ? new Color(0.03f, 0.10f, 0.08f, 1f) : new Color(0.05f, 0.15f, 0.12f, 1f);

                    Color phosphorGlow = new Color(0.2f, 0.95f, 0.70f, 1f);
                    Color phosphorCore = new Color(0.8f, 1.0f, 0.95f, 1f);

                    bool leftEye = (x >= 25 && x <= 28) && ((y == 37 && (x == 26 || x == 27)) || (y == 36 && (x == 25 || x == 28)));
                    bool rightEye = (x >= 32 && x <= 35) && ((y == 37 && (x == 33 || x == 34)) || (y == 36 && (x == 32 || x == 35)));
                    bool smile = (y == 28 && x >= 28 && x <= 32) ||
                                 (y == 29 && (x == 27 || x == 33)) ||
                                 (y == 30 && (x == 26 || x == 34));

                    if (leftEye || rightEye || smile) return phosphorCore;
                    if ((x >= 24 && x <= 36) && (y >= 27 && y <= 38))
                    {
                        if (Mathf.Abs(x - 30) <= 6 && Mathf.Abs(y - 33) <= 5) return Color.Lerp(screenBase, phosphorGlow, 0.35f);
                    }
                    return screenBase;
                }

                if (x >= 27 && x <= 33 && y >= 19 && y <= 21) return new Color(0.14f, 0.16f, 0.18f, 1f);

                if (x >= 47 && x <= 55 && y >= 21 && y <= 38)
                {
                    if (y == 21 && x >= 50 && x <= 54) return new Color(0.35f, 0.28f, 0.12f, 1f);
                    if (x == 52 && y >= 22 && y <= 30) return new Color(0.40f, 0.32f, 0.15f, 1f);
                    if ((x == 51 && y == 31) || (x == 50 && y == 32) || (x == 49 && y == 33)) return new Color(0.40f, 0.32f, 0.15f, 1f);
                    if (y >= 33 && y <= 37 && x >= 46 && x <= 50) return new Color(0.70f, 0.50f, 0.15f, 1f);
                    if (x >= 46 && x <= 48 && y >= 32 && y <= 34) return new Color(1.0f, 0.90f, 0.40f, 1f);
                }

                if (x >= 4 && x <= 60 && y >= 16 && y <= 20)
                {
                    if (y == 20) return new Color(0.40f, 0.35f, 0.30f, 1f);
                    if (y == 16) return new Color(0.12f, 0.10f, 0.08f, 1f);
                    return new Color(0.24f, 0.20f, 0.17f, 1f);
                }

                if (x >= 42 && x <= 58 && y <= 15)
                {
                    if (y == 10 || y == 5 || x == 42 || x == 58 || y == 0) return new Color(0.10f, 0.11f, 0.13f, 1f);
                    if (x >= 48 && x <= 52 && (y == 13 || y == 8 || y == 3)) return new Color(0.60f, 0.55f, 0.45f, 1f);
                    return new Color(0.22f, 0.24f, 0.27f, 1f);
                }

                if (x >= 6 && x <= 9 && y <= 15) return new Color(0.16f, 0.18f, 0.20f, 1f);

                if (x >= 12 && x <= 22 && y <= 16)
                {
                    if (y >= 6 && y <= 14 && x >= 13 && x <= 21) return new Color(0.10f, 0.12f, 0.14f, 0.85f);
                    if (x == 17 && y <= 6) return new Color(0.14f, 0.15f, 0.18f, 0.85f);
                    if (y == 0 && x >= 14 && x <= 20) return new Color(0.14f, 0.15f, 0.18f, 0.85f);
                }
                return Color.clear;
            });

            // 42. Chapter 1: Hanging Cable Bundle (64x64) - Drooping, tangled high-voltage power cables
            CreateSpriteAsset("ch1_hanging_cables.png", 64, 64, (x, y) =>
            {
                float t = (x - 32f) / 32f;
                float sag1 = 58f - (t * t * 38f);
                if (Mathf.Abs(y - sag1) <= 1.8f)
                {
                    if (y > sag1) return new Color(0.35f, 0.38f, 0.45f, 1f);
                    return new Color(0.12f, 0.13f, 0.16f, 1f);
                }

                float sag2 = 52f - (t * t * 44f);
                if (Mathf.Abs(y - sag2) <= 1.4f)
                {
                    if ((x / 4) % 2 == 0) return new Color(0.70f, 0.45f, 0.12f, 1f);
                    return new Color(0.10f, 0.11f, 0.14f, 1f);
                }

                float sag3 = 44f - (t * t * 40f);
                if (Mathf.Abs(y - sag3) <= 1.0f) return new Color(0.16f, 0.18f, 0.22f, 1f);

                if (x == 14 && y >= 25 && y <= 55) return new Color(0.14f, 0.16f, 0.20f, 1f);
                if (x == 15 && y == 24) return new Color(0.8f, 0.6f, 0.2f, 1f);

                if (x == 50 && y >= 18 && y <= 50) return new Color(0.14f, 0.16f, 0.20f, 1f);
                if ((x <= 6 || x >= 57) && y >= 50) return new Color(0.28f, 0.30f, 0.36f, 1f);
                return Color.clear;
            });

            // 43. Chapter 1: Wall Breaker Lockers (48x80) - Industrial electrical cabinets with louvers and latches
            CreateSpriteAsset("ch1_wall_lockers.png", 48, 80, (x, y) =>
            {
                if (x <= 2 || x >= 45 || y <= 2 || y >= 77) return new Color(0.12f, 0.13f, 0.15f, 1f);
                if (x <= 4 || x >= 43 || y <= 4 || y >= 75) return new Color(0.25f, 0.28f, 0.32f, 1f);

                if (x == 23 || x == 24) return new Color(0.10f, 0.11f, 0.12f, 1f);

                if (y >= 60 && y <= 70 && ((x >= 8 && x <= 20) || (x >= 28 && x <= 40)))
                {
                    if (y % 3 == 0) return new Color(0.08f, 0.09f, 0.10f, 1f);
                    return new Color(0.32f, 0.35f, 0.40f, 1f);
                }

                if (y >= 40 && y <= 46)
                {
                    if ((x >= 19 && x <= 21) || (x >= 26 && x <= 28)) return new Color(0.70f, 0.65f, 0.55f, 1f);
                }

                if (y >= 28 && y <= 34 && x >= 30 && x <= 38)
                {
                    if ((x + y) % 4 <= 1) return new Color(0.85f, 0.60f, 0.10f, 1f);
                    return new Color(0.10f, 0.11f, 0.12f, 1f);
                }

                float noise = (Mathf.Sin(x * 23f + y * 67f) + 1f) * 0.5f;
                float baseVal = 0.20f + noise * 0.04f;
                Color lockerCol = new Color(baseVal, baseVal * 1.05f, baseVal * 1.12f, 1f);
                if (y < 20 && noise > 0.5f) lockerCol = Color.Lerp(lockerCol, new Color(0.35f, 0.22f, 0.14f, 1f), 0.4f);
                return lockerCol;
            });

            // 44. Chapter 1: Cavernous Containment Chasm Backdrop (256x144) - Distant misty machinery and void
            CreateSpriteAsset("ch1_chasm_backdrop.png", 256, 144, (x, y) =>
            {
                float yNorm = y / 144f;
                // Rich atmospheric mist gradient: slate-blue mist fading into industrial chasm depth
                Color mistTop = new Color(0.20f, 0.25f, 0.32f, 1f);
                Color mistMid = new Color(0.28f, 0.34f, 0.42f, 1f);
                Color abyssDark = new Color(0.10f, 0.12f, 0.16f, 1f);
                Color bgSky = (yNorm > 0.45f) ? Color.Lerp(mistMid, mistTop, (yNorm - 0.45f) / 0.55f) : Color.Lerp(abyssDark, mistMid, yNorm / 0.45f);

                // Silhouetted industrial containment towers in distance
                if (x >= 70 && x <= 110)
                {
                    Color towerCol = new Color(0.12f, 0.15f, 0.20f, 1f);
                    int brace = (x + y) % 20;
                    if (brace <= 2) towerCol = new Color(0.18f, 0.22f, 0.28f, 1f);
                    if (Mathf.Abs(x - 90) <= 2 && Mathf.Abs(y - 85) <= 2) return new Color(1.0f, 0.70f, 0.25f, 1f); // Amber warning beacon
                    return towerCol;
                }

                if (x >= 180 && x <= 220)
                {
                    Color towerCol = new Color(0.10f, 0.13f, 0.17f, 1f);
                    int brace = (x - y + 144) % 18;
                    if (brace <= 2) towerCol = new Color(0.15f, 0.19f, 0.24f, 1f);
                    if (Mathf.Abs(x - 200) <= 2 && Mathf.Abs(y - 110) <= 2) return new Color(1.0f, 0.70f, 0.25f, 1f); // Amber warning beacon
                    return towerCol;
                }

                // Suspended gantry catwalks across chasm
                if ((y >= 50 && y <= 55) || (y >= 92 && y <= 96))
                {
                    if (x % 6 <= 4) return new Color(0.14f, 0.17f, 0.22f, 1f);
                }

                // Drooping suspension cables across abyss
                float sag = 120f - Mathf.Pow((x - 128f) / 128f, 2f) * 60f;
                if (Mathf.Abs(y - sag) <= 1.2f) return new Color(0.10f, 0.12f, 0.16f, 1f);
                return bgSky;
            });

            // 45. Chapter 1: Foreground Framing Silhouette (128x64) - Girders and hanging cables at sorting order 12
            CreateSpriteAsset("ch1_foreground_silhouette.png", 128, 64, (x, y) =>
            {
                if (y >= 48)
                {
                    if (y == 48) return new Color(0.12f, 0.14f, 0.16f, 0.95f);
                    return new Color(0.04f, 0.05f, 0.06f, 0.95f);
                }
                if (y >= 40 && y <= 46) return new Color(0.05f, 0.06f, 0.07f, 0.92f);

                float t = (x - 64f) / 64f;
                float c1 = 44f - (t * t * 24f);
                if (Mathf.Abs(y - c1) <= 1.5f) return new Color(0.03f, 0.04f, 0.05f, 0.95f);

                float c2 = 40f - (t * t * 32f);
                if (Mathf.Abs(y - c2) <= 1.2f) return new Color(0.03f, 0.04f, 0.05f, 0.95f);

                if (x + y >= 80 && x <= 32) return new Color(0.04f, 0.05f, 0.06f, 0.95f);
                return Color.clear;
            });

            // 46. Chapter 1: Wall Pipe Cluster (64x32) - Conduit pipes with red valve and mounting brackets
            CreateSpriteAsset("ch1_pipe_cluster.png", 64, 32, (x, y) =>
            {
                if (y >= 18 && y <= 26)
                {
                    if (y == 26) return new Color(0.48f, 0.52f, 0.58f, 1f);
                    if (y == 25) return new Color(0.38f, 0.42f, 0.48f, 1f);
                    if (y == 18) return new Color(0.10f, 0.12f, 0.14f, 1f);
                    if (x >= 28 && x <= 32) return new Color(0.42f, 0.46f, 0.52f, 1f);
                    return new Color(0.24f, 0.27f, 0.32f, 1f);
                }

                if (y >= 6 && y <= 12)
                {
                    if (y == 12) return new Color(0.70f, 0.42f, 0.22f, 1f);
                    if (y == 6) return new Color(0.20f, 0.10f, 0.05f, 1f);
                    return new Color(0.48f, 0.28f, 0.15f, 1f);
                }

                if (x >= 12 && x <= 16 && y >= 12 && y <= 18)
                {
                    if (y == 15 && x >= 10 && x <= 18) return new Color(0.85f, 0.20f, 0.15f, 1f);
                    return new Color(0.32f, 0.35f, 0.40f, 1f);
                }

                if ((x >= 4 && x <= 8) || (x >= 54 && x <= 58))
                {
                    if (y >= 4 && y <= 28) return new Color(0.16f, 0.18f, 0.22f, 1f);
                }
                return Color.clear;
            });

            // Chapter II: INVERSION Modular Environmental Art
            GenerateChapter02Sprites();

            AssetDatabase.Refresh();
        }

        #region Chapter II: Inversion Procedural Sprites
        private static void GenerateChapter02Sprites()
        {
            // 1. Chapter II Gravity Abyss Backdrop (256x256)
            // Vast broken anti-gravity industrial abyss with central gravity reactor tower, circular gantry rings, cyan core, and amber beacon pinlights
            CreateSpriteAsset("Environment/Chapter02/ch2_gravity_abyss_bg.png", 256, 256, (x, y) =>
            {
                // Deep background gradient (cold industrial slate-blue chasm)
                float ny = y / 255.0f;
                float nx = x / 255.0f;
                float vertDist = Mathf.Abs(ny - 0.5f) * 2.0f;
                float horizDist = Mathf.Abs(nx - 0.5f) * 2.0f;

                // Base slate chasm
                Color baseChasm = Color.Lerp(
                    new Color(0.04f, 0.07f, 0.12f, 1f), // Deep dark abyss shadow
                    new Color(0.10f, 0.17f, 0.24f, 1f), // Atmospheric steel-cyan haze
                    1.0f - vertDist * 0.4f
                );

                // Subtle organic rock/cavern noise
                float noise = (Mathf.Sin(x * 0.08f + y * 0.05f) + Mathf.Cos(x * 0.04f - y * 0.09f)) * 0.5f;
                baseChasm += new Color(noise * 0.02f, noise * 0.03f, noise * 0.04f, 0f);

                // Side cliff silhouettes (dark framing bunker architecture)
                if (x < 36)
                {
                    float cliffEdge = 24f + Mathf.Sin(y * 0.07f) * 8f;
                    if (x < cliffEdge)
                    {
                        if (x % 8 == 0 || y % 16 == 0) return new Color(0.03f, 0.04f, 0.06f, 1f);
                        return new Color(0.05f, 0.07f, 0.09f, 1f);
                    }
                }
                if (x > 220)
                {
                    float cliffEdge = 230f - Mathf.Sin(y * 0.08f) * 8f;
                    if (x > cliffEdge)
                    {
                        if (x % 8 == 0 || y % 16 == 0) return new Color(0.03f, 0.04f, 0.06f, 1f);
                        return new Color(0.05f, 0.07f, 0.09f, 1f);
                    }
                }

                // Central Gravity Reactor Tower (X: 145 to 185)
                float towerCenterX = 165f;
                float distFromTower = Mathf.Abs(x - towerCenterX);

                // Cyan core bloom / atmospheric radiance
                if (distFromTower < 45f)
                {
                    float bloomFactor = Mathf.Clamp01(1.0f - distFromTower / 45f);
                    float yBloom = 1.0f - Mathf.Abs(ny - 0.55f) * 0.8f;
                    baseChasm = Color.Lerp(baseChasm, new Color(0.18f, 0.55f, 0.72f, 1f), bloomFactor * yBloom * 0.45f);
                }

                // Vertical reactor shaft (X: 161 to 169)
                if (x >= 161 && x <= 169)
                {
                    if (x == 165) return new Color(0.92f, 0.98f, 1.0f, 1f); // White-hot plasma core line
                    if (x >= 163 && x <= 167) return new Color(0.20f, 0.88f, 1.0f, 1f); // Intense cyan core
                    return new Color(0.08f, 0.35f, 0.45f, 1f); // Conduit casing edge
                }

                // Structural reactor column struts (X: 157 to 160 and 170 to 173)
                if ((x >= 157 && x <= 160) || (x >= 170 && x <= 173))
                {
                    if (y % 12 <= 1) return new Color(0.04f, 0.06f, 0.08f, 1f); // Horizontal joint
                    return new Color(0.09f, 0.13f, 0.18f, 1f); // Heavy steel strut
                }

                // Multi-tier Circular Gantry Rings (Y centers: 55, 110, 165, 220)
                int[] ringYs = { 55, 110, 165, 220 };
                foreach (int ry in ringYs)
                {
                    float dx = (x - towerCenterX) / 34.0f;
                    float dy = (y - ry) / 7.0f;
                    float ringDistSq = dx * dx + dy * dy;

                    // Ring thickness between 0.7 and 1.0
                    if (ringDistSq >= 0.65f && ringDistSq <= 1.05f)
                    {
                        // Outer edge beacon pinlights at gantry ends
                        if (Mathf.Abs(x - towerCenterX) > 30f && Mathf.Abs(y - ry) <= 2)
                        {
                            return new Color(1.0f, 0.75f, 0.35f, 1f); // Warm maintenance amber pinlight
                        }
                        if (y % 2 == 0) return new Color(0.12f, 0.18f, 0.24f, 1f); // Catwalk deck
                        return new Color(0.06f, 0.10f, 0.14f, 1f); // Catwalk under-girder
                    }
                }

                // Suspended Crane Cables (vertical lines hanging through chasm)
                if ((x == 60 || x == 92 || x == 124 || x == 205) && (y > 40 && y < 240))
                {
                    if (y % 2 == 0) return new Color(0.15f, 0.20f, 0.26f, 1f);
                    return new Color(0.08f, 0.11f, 0.15f, 1f);
                }

                // Distant Skewed Cargo Silhouettes (tilted containers in zero-g)
                // Cube 1: near (75, 145), rotated
                float cx1 = (x - 75f) * 0.866f - (y - 145f) * 0.5f;
                float cy1 = (x - 75f) * 0.5f + (y - 145f) * 0.866f;
                if (Mathf.Abs(cx1) < 12f && Mathf.Abs(cy1) < 10f)
                {
                    if (Mathf.Abs(cx1) > 10f || Mathf.Abs(cy1) > 8f) return new Color(0.06f, 0.09f, 0.13f, 1f); // Silhouette edge
                    return new Color(0.09f, 0.14f, 0.19f, 1f);
                }

                // Cube 2: near (105, 80), rotated opposite
                float cx2 = (x - 105f) * 0.707f + (y - 80f) * 0.707f;
                float cy2 = -(x - 105f) * 0.707f + (y - 80f) * 0.707f;
                if (Mathf.Abs(cx2) < 9f && Mathf.Abs(cy2) < 8f)
                {
                    if (Mathf.Abs(cx2) > 7f || Mathf.Abs(cy2) > 6f) return new Color(0.05f, 0.08f, 0.11f, 1f);
                    return new Color(0.08f, 0.12f, 0.16f, 1f);
                }

                // Distant amber pinlights on deep background catwalks
                if ((x == 78 && y == 146) || (x == 106 && y == 82) || (x == 124 && y == 165))
                {
                    return new Color(1.0f, 0.72f, 0.35f, 1f);
                }

                return baseChasm;
            });

            // 2. Chapter II Floating Containment Cargo Pod (64x64)
            // Zero-gravity shipping container with corner reinforcement lugs, yellow hazard stripes, and status indicator
            CreateSpriteAsset("Environment/Chapter02/ch2_floating_cargo_pod.png", 64, 64, (x, y) =>
            {
                // Perimeter border / bevel
                if (x < 2 || x >= 62 || y < 2 || y >= 62) return Color.clear;
                if (x == 2 || x == 61 || y == 2 || y == 61) return new Color(0.12f, 0.15f, 0.18f, 1f);

                // Corner Reinforcement Brackets (14x14) with diagonal yellow/black hazard striping
                bool isCorner = (x <= 14 && y <= 14) || (x >= 49 && y <= 14) || (x <= 14 && y >= 49) || (x >= 49 && y >= 49);
                if (isCorner)
                {
                    if ((x + y) / 4 % 2 == 0) return new Color(0.85f, 0.68f, 0.12f, 1f); // Hazard yellow
                    return new Color(0.10f, 0.11f, 0.13f, 1f); // Hazard black
                }

                // Corner Hoisting Eyelet / Crane Rigging Lugs (at top edge)
                if ((x >= 8 && x <= 14 && y >= 58) || (x >= 49 && x <= 55 && y >= 58))
                {
                    return new Color(0.25f, 0.28f, 0.32f, 1f);
                }

                // Heavy armor plate seams and recessed channels
                if (x == 15 || x == 48 || y == 15 || y == 48) return new Color(0.08f, 0.10f, 0.13f, 1f);
                if (x == 31 || x == 32) return new Color(0.10f, 0.12f, 0.15f, 1f);

                // Rivet perimeter along armor panels
                bool isRivet = (y == 18 || y == 45) && (x % 6 == 0 && x > 15 && x < 48);
                if (isRivet) return new Color(0.35f, 0.39f, 0.44f, 1f);

                // Central Recessed Hatch (circle radius 11 at 32, 32)
                float dx = x - 31.5f;
                float dy = y - 31.5f;
                float distSq = dx * dx + dy * dy;
                if (distSq <= 121f)
                {
                    if (distSq >= 95f) return new Color(0.08f, 0.10f, 0.13f, 1f); // Lock ring channel
                    if (distSq <= 16f) return new Color(0.15f, 0.85f, 1.0f, 1f); // Central cyan telemetry indicator
                    if (distSq <= 25f) return new Color(0.08f, 0.45f, 0.55f, 1f);
                    return new Color(0.16f, 0.19f, 0.23f, 1f); // Hatch inner face
                }

                // Warning Decal: Stenciled containment triangle (\nabla)
                if (y >= 36 && y <= 42 && x >= 20 && x <= 26)
                {
                    int row = 42 - y;
                    int halfW = row;
                    if (Mathf.Abs(x - 23) <= halfW) return new Color(0.85f, 0.65f, 0.15f, 0.9f);
                }

                // Weathered dark steel surface with scratches and grime
                float n = Mathf.Sin(x * 15.3f + y * 37.1f) * 43758.5453f;
                float noise = n - Mathf.Floor(n);
                float baseVal = 0.22f + noise * 0.05f;
                return new Color(baseVal, baseVal * 1.08f, baseVal * 1.18f, 1f);
            });

            // 3. Chapter II Broken Bridge Girder (96x48)
            // Sheared industrial catwalk section with diamond tread deck, I-beam girder, cross trusses, and jagged severed rebar with dangling wires
            CreateSpriteAsset("Environment/Chapter02/ch2_broken_bridge_girder.png", 96, 48, (x, y) =>
            {
                // Jagged sheared break profile on right end (X >= 64)
                float breakThresholdY = 48f;
                if (x >= 64)
                {
                    // Downward sheared jagged fracture slope
                    float fracT = (x - 64f) / 32f;
                    breakThresholdY = 44f - fracT * 32f + Mathf.Sin(x * 0.8f) * 5f;
                    if (y > breakThresholdY)
                    {
                        // Dangling copper wires and sparks protruding from break
                        if ((x == 72 || x == 82 || x == 90) && y > breakThresholdY && y < breakThresholdY + 14f)
                        {
                            if (y == (int)(breakThresholdY + 12f)) return new Color(0.20f, 0.90f, 1.0f, 1f); // Severed spark
                            return new Color(0.85f, 0.45f, 0.15f, 1f); // Exposed copper core
                        }
                        return Color.clear;
                    }
                }

                if (y < 4 && x >= 70) return Color.clear; // Underside severed opening

                // Catwalk top walking deck (Y: 38 to 47)
                if (y >= 38)
                {
                    if (y == 47) return new Color(0.35f, 0.40f, 0.45f, 1f); // Top highlight edge
                    if (y == 38) return new Color(0.12f, 0.14f, 0.17f, 1f); // Deck base seam
                    if ((x + y) % 6 == 0) return new Color(0.28f, 0.32f, 0.38f, 1f); // Diamond tread highlight
                    return new Color(0.20f, 0.24f, 0.29f, 1f); // Diamond tread base
                }

                // Yellow/Black Hazard Stripe strip under top deck lip (Y: 34 to 37)
                if (y >= 34)
                {
                    if ((x + y) / 4 % 2 == 0) return new Color(0.88f, 0.70f, 0.12f, 1f);
                    return new Color(0.10f, 0.11f, 0.13f, 1f);
                }

                // Heavy I-Beam Girder Web with Truss Bracing (Y: 6 to 33)
                if (y <= 10) return new Color(0.22f, 0.25f, 0.30f, 1f); // Bottom girder flange
                if (x % 16 == 0) return new Color(0.14f, 0.16f, 0.19f, 1f); // Vertical stiffener rib

                // Diagonal truss members
                int cellX = x % 16;
                int normY = y - 10;
                if (Mathf.Abs(cellX - normY * 16 / 24) <= 1 || Mathf.Abs(cellX - (16 - normY * 16 / 24)) <= 1)
                {
                    return new Color(0.26f, 0.30f, 0.36f, 1f);
                }

                // Web plate recessed shadow
                return new Color(0.12f, 0.14f, 0.18f, 1f);
            });

            // 4. Chapter II Suspended Crane Crate (64x64)
            // Heavy cargo crate hanging from overhead crane cables with under-mounted warm amber maintenance lantern
            CreateSpriteAsset("Environment/Chapter02/ch2_suspended_crane_crate.png", 64, 64, (x, y) =>
            {
                // Overhead crane cables converging at (32, 63)
                if (y >= 46)
                {
                    if (y >= 60 && Mathf.Abs(x - 32) <= 3) return new Color(0.35f, 0.38f, 0.42f, 1f); // Hoisting hook/shackle
                    float cableT = (y - 46f) / 14f;
                    float leftCableX = Mathf.Lerp(14f, 32f, cableT);
                    float rightCableX = Mathf.Lerp(50f, 32f, cableT);
                    if (Mathf.Abs(x - leftCableX) < 1.2f || Mathf.Abs(x - rightCableX) < 1.2f)
                    {
                        return new Color(0.40f, 0.44f, 0.48f, 1f);
                    }
                    return Color.clear;
                }

                // Under-mounted warm amber maintenance lantern (Y < 12)
                if (y < 12)
                {
                    if (Mathf.Abs(x - 32) <= 6)
                    {
                        if (y >= 9) return new Color(0.20f, 0.22f, 0.25f, 1f); // Bracket
                        if (y <= 2) return new Color(0.18f, 0.20f, 0.22f, 1f); // Bottom cage cap
                        if (Mathf.Abs(x - 32) <= 3) return new Color(1.0f, 0.78f, 0.35f, 1f); // Glowing glass filament
                        return new Color(0.85f, 0.55f, 0.18f, 1f); // Amber glass globe
                    }
                    return Color.clear;
                }

                // Crate Body (X: 10 to 54, Y: 12 to 46)
                if (x < 10 || x > 54) return Color.clear;
                if (x == 10 || x == 54 || y == 12 || y == 46) return new Color(0.10f, 0.12f, 0.14f, 1f);

                // Outer steel cage frame
                if (x <= 14 || x >= 50 || y <= 16 || y >= 42)
                {
                    if ((x + y) / 3 % 2 == 0) return new Color(0.85f, 0.70f, 0.12f, 1f); // Hazard trim
                    return new Color(0.14f, 0.16f, 0.19f, 1f);
                }

                // Inner containment crate panel
                if (x == 32 || y == 29) return new Color(0.08f, 0.10f, 0.12f, 1f); // Seam
                float n = Mathf.Sin(x * 12f + y * 28f) * 43758.5f;
                float noise = n - Mathf.Floor(n);
                float val = 0.22f + noise * 0.05f;
                return new Color(val * 0.9f, val, val * 1.15f, 1f);
            });

            // 5. Chapter II Inverted Ceiling Deck (128x64)
            // Dual-orientation platform piece: top ceiling mounting brackets, middle riveted web girder with lightening holes,
            // bottom walkable face with diamond tread, hazard stripe trim, and recessed cyan LED walk lights
            CreateSpriteAsset("Environment/Chapter02/ch2_inverted_ceiling_deck.png", 128, 64, (x, y) =>
            {
                // Top Ceiling Anchor Flanges (Y: 54 to 63)
                if (y >= 54)
                {
                    if (y == 63) return new Color(0.12f, 0.14f, 0.18f, 1f); // Rock anchor seam
                    if ((x >= 12 && x <= 22) || (x >= 54 && x <= 64) || (x >= 96 && x <= 106))
                    {
                        if (y == 58 && (x == 17 || x == 59 || x == 101)) return new Color(0.55f, 0.58f, 0.62f, 1f); // Anchor bolt
                        return new Color(0.24f, 0.28f, 0.33f, 1f); // Heavy ceiling bracket
                    }
                    if (y >= 58) return new Color(0.14f, 0.16f, 0.20f, 1f);
                    return Color.clear;
                }

                // Middle Heavy Riveted Web Plate Girder (Y: 20 to 53)
                if (y >= 20)
                {
                    // Vertical stiffeners at intervals
                    if (x % 32 == 0 || x % 32 == 1) return new Color(0.12f, 0.14f, 0.18f, 1f);

                    // Circular Lightening Holes (radius 9 at X centers: 16, 48, 80, 112, Y: 36)
                    int cellCenter = (x / 32) * 32 + 16;
                    float hdx = x - cellCenter;
                    float hdy = y - 36f;
                    float holeDistSq = hdx * hdx + hdy * hdy;
                    if (holeDistSq <= 81f)
                    {
                        if (holeDistSq >= 64f) return new Color(0.10f, 0.12f, 0.15f, 1f); // Hole bevel rim
                        return Color.clear; // Cutout hole through girder
                    }

                    // Rivet lines along top and bottom of web plate
                    if ((y == 23 || y == 50) && (x % 8 == 4)) return new Color(0.40f, 0.44f, 0.49f, 1f);

                    return new Color(0.18f, 0.21f, 0.26f, 1f); // Web plate steel
                }

                // Walkable Ceiling Surface Face (Y: 4 to 19) - THIS IS WALKED ON WHEN INVERTED!
                // Safety Toe-Kick & Inverted Railing Sockets (Y: 0 to 4)
                if (y < 4)
                {
                    if (x % 32 <= 3) return new Color(0.22f, 0.26f, 0.30f, 1f); // Railing base socket
                    return Color.clear;
                }

                // Caution Yellow/Black Hazard Stripe Trim (Y: 4 to 8)
                if (y <= 8)
                {
                    if ((x + y) / 4 % 2 == 0) return new Color(0.88f, 0.70f, 0.12f, 1f); // Safety yellow
                    return new Color(0.10f, 0.11f, 0.13f, 1f); // Dark black
                }

                // Recessed Cyan LED Guide Lights along inverted walk path (Y: 13 to 15, at X: 16, 48, 80, 112)
                int ledCellCenter = (x / 32) * 32 + 16;
                if (Mathf.Abs(x - ledCellCenter) <= 3 && y >= 12 && y <= 16)
                {
                    if (Mathf.Abs(x - ledCellCenter) <= 1 && y >= 13 && y <= 15)
                    {
                        return new Color(0.20f, 0.92f, 1.0f, 1f); // Glowing cyan path marker
                    }
                    return new Color(0.06f, 0.35f, 0.45f, 1f); // LED fixture housing
                }

                // Walkable Diamond-Plate Tread Surface (Y: 9 to 19)
                if ((x + y) % 6 == 0) return new Color(0.32f, 0.36f, 0.42f, 1f); // Diamond plate pattern
                return new Color(0.22f, 0.26f, 0.32f, 1f); // Anti-slip tread base
            });

            // 6. Chapter II Catwalk Broken End (64x48)
            // Downward-sheared platform terminus for X = -4.8, showing torn deck plate, sheared girder, severed pipes, and sparks
            CreateSpriteAsset("Environment/Chapter02/ch2_catwalk_broken_end.png", 64, 48, (x, y) =>
            {
                // Sheared break curve on right (X >= 22)
                float breakThresholdY = 48f;
                if (x >= 22)
                {
                    float t = (x - 22f) / 42f;
                    breakThresholdY = 44f - t * 38f + Mathf.Sin(x * 0.9f) * 3f;
                    if (y > breakThresholdY)
                    {
                        // Severed conduit pipes protruding from break
                        if ((x == 32 || x == 44) && y > breakThresholdY && y < breakThresholdY + 10f)
                        {
                            return new Color(0.35f, 0.38f, 0.42f, 1f); // Exposed pipe
                        }
                        return Color.clear;
                    }
                }

                // Top Walk Deck (Y: 38 to 47 on left)
                if (y >= 38)
                {
                    if (y == 47) return new Color(0.38f, 0.42f, 0.48f, 1f);
                    if ((x + y) % 6 == 0) return new Color(0.30f, 0.34f, 0.40f, 1f);
                    return new Color(0.22f, 0.25f, 0.30f, 1f);
                }

                // Hazard warning lip (Y: 34 to 37)
                if (y >= 34)
                {
                    if ((x + y) / 4 % 2 == 0) return new Color(0.88f, 0.70f, 0.12f, 1f);
                    return new Color(0.10f, 0.11f, 0.13f, 1f);
                }

                // Underside web girder
                if (x == 20) return new Color(0.12f, 0.14f, 0.16f, 1f);
                return new Color(0.16f, 0.19f, 0.23f, 1f);
            });

            // 7. Chapter II Gravity Debris Cluster (64x64)
            // Jagged fragments of cracked concrete and sheared steel tumbling in anti-gravity
            CreateSpriteAsset("Environment/Chapter02/ch2_gravity_debris_cluster.png", 64, 64, (x, y) =>
            {
                // Fragment 1: Large concrete chunk near (24, 38)
                float d1x = Mathf.Abs(x - 24);
                float d1y = Mathf.Abs(y - 38);
                if (d1x + d1y < 16 && d1x < 12 && d1y < 10)
                {
                    if (d1x == 11 || d1y == 9) return new Color(0.12f, 0.14f, 0.16f, 1f);
                    if (x == 26 && y >= 32 && y <= 44) return new Color(0.65f, 0.45f, 0.20f, 1f); // Exposed rusty rebar
                    float n = Mathf.Sin(x * 14f + y * 31f) * 43758.5f;
                    float val = 0.26f + (n - Mathf.Floor(n)) * 0.08f;
                    return new Color(val, val * 1.05f, val * 1.15f, 1f);
                }

                // Fragment 2: Twisted steel plate near (46, 22)
                float d2x = (x - 46f) * 0.8f + (y - 22f) * 0.6f;
                float d2y = -(x - 46f) * 0.6f + (y - 22f) * 0.8f;
                if (Mathf.Abs(d2x) < 10f && Mathf.Abs(d2y) < 4f)
                {
                    if (Mathf.Abs(d2x) > 8f || Mathf.Abs(d2y) > 3f) return new Color(0.10f, 0.12f, 0.15f, 1f);
                    return new Color(0.32f, 0.36f, 0.42f, 1f);
                }

                // Fragment 3: Small floating chunk near (18, 14)
                if (Mathf.Abs(x - 18) < 5 && Mathf.Abs(y - 14) < 4)
                {
                    return new Color(0.24f, 0.27f, 0.32f, 1f);
                }

                return Color.clear;
            });

            // 8. Chapter II Maintenance Lantern (24x32)
            // Heavy cast-iron bulkhead lantern with glowing warm amber glass lens matching Reference 1
            CreateSpriteAsset("Environment/Chapter02/ch2_maintenance_lantern.png", 24, 32, (x, y) =>
            {
                if (x < 3 || x > 20) return Color.clear;

                // Wall mounting bracket and top cap (Y: 24 to 31)
                if (y >= 24)
                {
                    if (y >= 29) return new Color(0.22f, 0.24f, 0.28f, 1f); // Top mount flange
                    if (x >= 6 && x <= 17) return new Color(0.18f, 0.20f, 0.23f, 1f);
                    return Color.clear;
                }

                // Bottom iron cap and hook (Y: 0 to 6)
                if (y <= 6)
                {
                    if (y <= 2 && (x < 9 || x > 14)) return Color.clear;
                    if (x >= 6 && x <= 17) return new Color(0.18f, 0.20f, 0.23f, 1f);
                    return Color.clear;
                }

                // Glass Globe & Protective Cage Ribs (Y: 7 to 23)
                if (x == 6 || x == 17 || x == 11 || x == 12)
                {
                    return new Color(0.16f, 0.18f, 0.20f, 1f); // Protective steel cage wire ribs
                }

                // Glowing Warm Amber Filament Globe
                float cDist = Mathf.Abs(x - 11.5f);
                float yDist = Mathf.Abs(y - 15f);
                if (cDist <= 2f && yDist <= 4f)
                {
                    return new Color(1.0f, 0.88f, 0.55f, 1f); // White-amber filament core
                }
                return new Color(1.0f, 0.72f, 0.32f, 1f); // Warm amber glow
            });

            // 9. Chapter II Hazard Emitter Housing (32x32)
            // Heavy industrial containment housing for the laser hazards: cooling fins, caution stencil, red/pink plasma nozzle
            CreateSpriteAsset("Environment/Chapter02/ch2_hazard_emitter_housing.png", 32, 32, (x, y) =>
            {
                if (x < 2 || x > 29 || y < 2 || y > 29) return Color.clear;
                if (x == 2 || x == 29 || y == 2 || y == 29) return new Color(0.10f, 0.12f, 0.14f, 1f);

                // Horizontal cooling fins on sides
                if ((y % 4 == 0) && (x < 8 || x > 23)) return new Color(0.08f, 0.10f, 0.12f, 1f);

                // High voltage hazard triangle decal
                if (y >= 18 && y <= 24 && Mathf.Abs(x - 16) <= (24 - y))
                {
                    return new Color(0.95f, 0.75f, 0.15f, 1f);
                }

                // Red/Pink Plasma Emitter Nozzle in center (Y: 6 to 14, X: 11 to 20)
                if (x >= 11 && x <= 20 && y >= 6 && y <= 14)
                {
                    if (Mathf.Abs(x - 15.5f) <= 2f && Mathf.Abs(y - 10f) <= 2f)
                    {
                        return new Color(1.0f, 0.35f, 0.45f, 1f); // Intense plasma core
                    }
                    return new Color(0.85f, 0.12f, 0.22f, 1f); // Emitter ring
                }

                return new Color(0.20f, 0.23f, 0.27f, 1f); // Dark alloy casing
            });

            // 10. Chapter II Collapsing Floor Fracture Tile (64x64)
            // Platform tile with glowing amber stress fracture veins providing the mandatory diegetic visual warning clue
            CreateSpriteAsset("Environment/Chapter02/ch2_collapsing_floor_fracture.png", 64, 64, (x, y) =>
            {
                // Top edge bevel
                if (y >= 60) return new Color(0.35f, 0.38f, 0.42f, 1f);
                if (x % 16 == 0 || y % 16 == 0) return new Color(0.10f, 0.12f, 0.15f, 1f);

                // Spiderweb network of glowing amber hairline cracks radiating from (32, 32)
                float dx = x - 32f;
                float dy = y - 32f;
                float angle = Mathf.Atan2(dy, dx);
                float radius = Mathf.Sqrt(dx * dx + dy * dy);

                // 5 distinct jagged fracture branches
                bool isCrack = false;
                bool isCrackHalo = false;
                for (int branch = 0; branch < 5; branch++)
                {
                    float branchAngle = branch * Mathf.PI * 2f / 5f;
                    float angleDiff = Mathf.DeltaAngle(angle * Mathf.Rad2Deg, branchAngle * Mathf.Rad2Deg);
                    float wobble = Mathf.Sin(radius * 0.4f + branch) * 12f;
                    if (Mathf.Abs(angleDiff - wobble) < 3.5f && radius < 28f)
                    {
                        isCrack = true;
                    }
                    else if (Mathf.Abs(angleDiff - wobble) < 7f && radius < 28f)
                    {
                        isCrackHalo = true;
                    }
                }

                if (isCrack) return new Color(1.0f, 0.65f, 0.15f, 1f); // Glowing amber stress fracture
                if (isCrackHalo) return new Color(0.55f, 0.28f, 0.10f, 1f); // Fracture heat bloom

                // Base steel plate
                float n = Mathf.Sin(x * 12.3f + y * 28.7f) * 43758.5f;
                float val = 0.18f + (n - Mathf.Floor(n)) * 0.04f;
                return new Color(val, val * 1.05f, val * 1.15f, 1f);
            });

            // 11. Chapter II Left Containment Bunker Wall (128x128)
            // Heavy industrial bunker wall with high-pressure conduits, hydraulic door frame, and warning placards
            CreateSpriteAsset("Environment/Chapter02/ch2_left_containment_wall.png", 128, 128, (x, y) =>
            {
                if (x == 0 || x == 127 || y == 0 || y == 127) return new Color(0.08f, 0.10f, 0.12f, 1f);

                // Vertical Conduit Cluster (X: 18 to 26 and X: 104 to 112)
                if ((x >= 18 && x <= 26) || (x >= 104 && x <= 112))
                {
                    int pipeRel = (x < 50) ? (x - 18) : (x - 104);
                    if (pipeRel == 0 || pipeRel == 8) return new Color(0.09f, 0.11f, 0.13f, 1f);
                    if (y % 32 <= 2) return new Color(0.35f, 0.38f, 0.42f, 1f); // Mounting bracket
                    return new Color(0.25f, 0.30f, 0.36f, 1f);
                }

                // Horizontal Panel Seams
                if (y == 42 || y == 84) return new Color(0.08f, 0.10f, 0.12f, 1f);

                // Rivets along seams
                if ((y == 40 || y == 86) && (x % 16 == 8)) return new Color(0.42f, 0.46f, 0.50f, 1f);

                // Surface texture
                float n = Mathf.Sin(x * 15.3f + y * 41.7f) * 43758.5f;
                float noise = n - Mathf.Floor(n);
                float val = 0.20f + noise * 0.05f;
                return new Color(val, val * 1.06f, val * 1.16f, 1f);
            });

            // 12. Chapter II Foreground Silhouettes (128x64)
            // Dark framing silhouettes of industrial piping, crane rigging cables, and handrail silhouettes
            CreateSpriteAsset("Environment/Chapter02/ch2_foreground_silhouettes.png", 128, 64, (x, y) =>
            {
                // Heavy horizontal pipe run along bottom (Y: 6 to 18)
                if (y >= 6 && y <= 18)
                {
                    if (y == 18) return new Color(0.06f, 0.08f, 0.10f, 1f);
                    if (y == 17) return new Color(0.12f, 0.15f, 0.18f, 1f); // Top rim highlight
                    return new Color(0.04f, 0.05f, 0.07f, 1f);
                }

                // Vertical pipe risers and valves
                if ((x >= 24 && x <= 30) || (x >= 92 && x <= 98))
                {
                    if (y <= 45) return new Color(0.04f, 0.05f, 0.07f, 1f);
                }

                // Diagonal cable runs
                float cable1Y = x * 0.35f + 15f;
                if (Mathf.Abs(y - cable1Y) < 1.5f && x > 40)
                {
                    return new Color(0.05f, 0.07f, 0.09f, 1f);
                }

                return Color.clear;
            });

            // 13. Chapter II Heavy Platform Deck (128x64)
            // Walkable industrial platform deck for floor catwalks with diamond plate, yellow hazard stripe lip, and under-girder trusses
            CreateSpriteAsset("Environment/Chapter02/ch2_heavy_platform_deck.png", 128, 64, (x, y) =>
            {
                // Top Walking Deck Surface (Y: 48 to 63)
                if (y >= 48)
                {
                    if (y == 63) return new Color(0.42f, 0.46f, 0.52f, 1f); // Top highlight bevel
                    if (y == 48) return new Color(0.12f, 0.14f, 0.17f, 1f); // Deck seam
                    if ((x + y) % 6 == 0) return new Color(0.32f, 0.36f, 0.42f, 1f); // Diamond plate pattern
                    return new Color(0.22f, 0.25f, 0.30f, 1f);
                }

                // Yellow/Black Caution Hazard Lip (Y: 42 to 47)
                if (y >= 42)
                {
                    if ((x + y) / 4 % 2 == 0) return new Color(0.88f, 0.70f, 0.12f, 1f);
                    return new Color(0.10f, 0.11f, 0.13f, 1f);
                }

                // Under-girder with circular lightening cutouts (Y: 10 to 41)
                if (y >= 10)
                {
                    if (x % 32 <= 1) return new Color(0.12f, 0.14f, 0.17f, 1f); // Vertical rib

                    int holeCenter = (x / 32) * 32 + 16;
                    float hdx = x - holeCenter;
                    float hdy = y - 26f;
                    float distSq = hdx * hdx + hdy * hdy;
                    if (distSq <= 64f)
                    {
                        if (distSq >= 49f) return new Color(0.10f, 0.12f, 0.14f, 1f);
                        return Color.clear; // Cutout hole
                    }

                    if ((y == 14 || y == 38) && (x % 8 == 4)) return new Color(0.38f, 0.42f, 0.46f, 1f); // Rivet
                    return new Color(0.18f, 0.21f, 0.25f, 1f);
                }

                // Bottom flange (Y: 6 to 9)
                if (y >= 6) return new Color(0.24f, 0.28f, 0.33f, 1f);

                return Color.clear;
            });
        }
        #endregion

        private static void CreateSpriteAsset(string filename, int width, int height, System.Func<int, int, Color> pixelFunc)
        {
            string fullPath = Path.Combine(ArtPath, filename);
            string dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tex.SetPixel(x, y, pixelFunc(x, y));
                }
            }
            tex.Apply();

            byte[] pngData = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);
            File.WriteAllBytes(fullPath, pngData);

            AssetDatabase.ImportAsset(fullPath, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(fullPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 64;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }
        }
        #endregion

        #region Common Scene Builder Helper
        private struct BaseSceneRefs
        {
            public Camera camera;
            public Transform spawnPoint;
            public PlayerController2D player;
            public AbilityController abilityCtrl;
            public LevelHUD hud;
            public LevelDirector director;
            public ExitDoor exitDoor;
            public ArchitectDialogue dialogue;
            public ArchitectTriggers triggers;
            public GameObject envRoot;
            public GameObject puzzleRoot;
            public Text questionPromptText;
        }

        private static BaseSceneRefs SetupBaseChamber(string sceneTitle, int chamberNum, string nextScene, bool isFinal, ChapterAtmosphereConfig atmo, float? collapsingFloorGapX = null)
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            Sprite floorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_tile.png");
            Sprite wallSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/wall_panel.png");
            Sprite beamSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ceiling_beam.png");
            Sprite pipeSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/pipe_conduit.png");
            Sprite doorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/exit_door.png");
            Sprite playerSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/player_sprite.png");
            Sprite ghostSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/echo_ghost.png");
            Sprite monitorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/architect_monitor.png");
            Sprite pillarSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/bg_pillar.png");
            Sprite reactorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/bg_reactor.png");

            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") 
                            ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

            // 1. Camera
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            camObj.transform.position = new Vector3(0f, 1.0f, -10f);
            Camera cam = camObj.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6.0f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.02f, 0.03f, 0.05f, 1f);
            camObj.AddComponent<AudioListener>();
            camObj.AddComponent<UniversalAdditionalCameraData>();

            // 2. Lighting Rig (Chapter-specific atmospheric calibration)
            GameObject globalLightObj = new GameObject("Global Light 2D");
            Light2D globalLight = globalLightObj.AddComponent<Light2D>();
            globalLight.lightType = Light2D.LightType.Global;
            globalLight.color = atmo.ambientColor;
            globalLight.intensity = atmo.ambientIntensity;

            GameObject architectSpotObj = new GameObject("Architect_Spotlight");
            architectSpotObj.transform.position = new Vector3(0f, 4.2f, 0f);
            Light2D architectSpot = architectSpotObj.AddComponent<Light2D>();
            architectSpot.lightType = Light2D.LightType.Point;
            architectSpot.pointLightOuterRadius = 7.5f;
            architectSpot.color = new Color(0f, 0.94f, 1f, 1f);
            architectSpot.intensity = 0.85f;

            GameObject spawnLampObj = new GameObject("Spawn_EmergencyLamp");
            spawnLampObj.transform.position = new Vector3(-8.0f, -1.0f, 0f);
            Light2D spawnLamp = spawnLampObj.AddComponent<Light2D>();
            spawnLamp.lightType = Light2D.LightType.Point;
            spawnLamp.pointLightOuterRadius = 4.5f;
            spawnLamp.color = atmo.spawnLampColor;
            spawnLamp.intensity = atmo.spawnLampIntensity;

            FlickeringLight2D spawnFlicker = spawnLampObj.AddComponent<FlickeringLight2D>();
            spawnFlicker.Mode = atmo.spawnLampFlicker;
            spawnFlicker.BaseIntensity = atmo.spawnLampIntensity;

            // 3. Background Architecture (Deep BG: -25, Mid BG: -15)
            GameObject bgRoot = new GameObject("--- BACKGROUND ---");

            // Full industrial bulkhead background wall
            GameObject bgWallGrid = new GameObject("BG_BackWallGrid");
            bgWallGrid.transform.SetParent(bgRoot.transform);
            bgWallGrid.transform.position = Vector3.zero;

            for (float wx = -10f; wx <= 10f; wx += 2f)
            {
                for (float wy = -3f; wy <= 4f; wy += 2f)
                {
                    GameObject bgt = new GameObject($"BGTile_{wx}_{wy}");
                    bgt.transform.SetParent(bgWallGrid.transform);
                    bgt.transform.position = new Vector3(wx, wy, 0f);
                    bgt.transform.localScale = new Vector3(2f, 2f, 1f);
                    SpriteRenderer r = bgt.AddComponent<SpriteRenderer>();
                    r.sprite = wallSp;
                    r.sortingOrder = -22;
                    r.color = new Color(0.20f, 0.24f, 0.30f, 0.55f);
                }
            }

            GameObject reactorObj = new GameObject("BG_ReactorCore");
            reactorObj.transform.SetParent(bgRoot.transform);
            reactorObj.transform.position = new Vector3(2.5f, 1.0f, 5.0f);
            reactorObj.transform.localScale = new Vector3(3.5f, 3.5f, 1f);
            SpriteRenderer reactorRend = reactorObj.AddComponent<SpriteRenderer>();
            reactorRend.sprite = reactorSp;
            reactorRend.sortingOrder = -20;
            reactorRend.color = new Color(1f, 1f, 1f, 0.40f);

            float[] pillarXs = new float[] { -7f, -1.5f, 4.5f };
            foreach (var px in pillarXs)
            {
                GameObject colObj = new GameObject($"BG_Pillar_{px}");
                colObj.transform.SetParent(bgRoot.transform);
                colObj.transform.position = new Vector3(px, 1.0f, 2.0f);
                colObj.transform.localScale = new Vector3(1.5f, 1.8f, 1f);
                SpriteRenderer colRend = colObj.AddComponent<SpriteRenderer>();
                colRend.sprite = pillarSp;
                colRend.sortingOrder = -10;
                colRend.color = new Color(1f, 1f, 1f, 0.65f);
            }

            // 4. Perimeter Environment (Floor, Ceiling, Bulkheads)
            GameObject envRoot = new GameObject("--- ENVIRONMENT ---");

            // Atmospheric Particle Emitter (Non-colliding, sortingOrder -5)
            GameObject particleObj = new GameObject("Atmospheric_Particles");
            particleObj.transform.SetParent(envRoot.transform);
            particleObj.transform.position = Vector3.zero;
            AtmosphericParticleEmitter emitter = particleObj.AddComponent<AtmosphericParticleEmitter>();
            emitter.CurrentAtmosphere = atmo.particleType;

            // Floor
            GameObject floorComposite = new GameObject("Chamber_Floor");
            floorComposite.transform.SetParent(envRoot.transform);
            floorComposite.transform.position = new Vector3(0f, -4.5f, 0f);

            if (collapsingFloorGapX.HasValue)
            {
                float gapX = collapsingFloorGapX.Value;
                float gapHalfWidth = 0.6f;
                float leftMin = -12f;
                float leftMax = gapX - gapHalfWidth;
                float leftWidth = leftMax - leftMin;
                float leftCenterX = (leftMin + leftMax) * 0.5f;

                BoxCollider2D leftFloorCol = floorComposite.AddComponent<BoxCollider2D>();
                leftFloorCol.size = new Vector2(leftWidth, 1.0f);
                leftFloorCol.offset = new Vector2(leftCenterX, 0f);

                float rightMin = gapX + gapHalfWidth;
                float rightMax = 12f;
                float rightWidth = rightMax - rightMin;
                float rightCenterX = (rightMin + rightMax) * 0.5f;

                BoxCollider2D rightFloorCol = floorComposite.AddComponent<BoxCollider2D>();
                rightFloorCol.size = new Vector2(rightWidth, 1.0f);
                rightFloorCol.offset = new Vector2(rightCenterX, 0f);
            }
            else
            {
                BoxCollider2D floorCol = floorComposite.AddComponent<BoxCollider2D>();
                floorCol.size = new Vector2(24f, 1.0f);
            }

            for (float x = -11f; x <= 11f; x += 1.0f)
            {
                if (collapsingFloorGapX.HasValue && Mathf.Abs(x - collapsingFloorGapX.Value) < 0.6f)
                {
                    continue;
                }
                GameObject tile = new GameObject($"FloorTile_{x}");
                tile.transform.SetParent(floorComposite.transform);
                tile.transform.position = new Vector3(x, -4.0f, 0f);
                SpriteRenderer rend = tile.AddComponent<SpriteRenderer>();
                rend.sprite = floorSp;
                rend.sortingOrder = 0;
            }

            // Ceiling
            GameObject ceilingComposite = new GameObject("Chamber_Ceiling");
            ceilingComposite.transform.SetParent(envRoot.transform);
            ceilingComposite.transform.position = new Vector3(0f, 5.5f, 0f);
            BoxCollider2D ceilCol = ceilingComposite.AddComponent<BoxCollider2D>();
            ceilCol.size = new Vector2(24f, 1.0f);

            for (float x = -11f; x <= 11f; x += 1.0f)
            {
                GameObject tile = new GameObject($"CeilingBeam_{x}");
                tile.transform.SetParent(ceilingComposite.transform);
                tile.transform.position = new Vector3(x, 5.25f, 0f);
                tile.transform.localScale = new Vector3(1f, -1f, 1f);
                SpriteRenderer rend = tile.AddComponent<SpriteRenderer>();
                rend.sprite = beamSp;
                rend.sortingOrder = 0;
            }

            // Left Wall
            GameObject leftWall = new GameObject("Chamber_LeftWall");
            leftWall.transform.SetParent(envRoot.transform);
            leftWall.transform.position = new Vector3(-10.5f, 0.5f, 0f);
            BoxCollider2D leftCol = leftWall.AddComponent<BoxCollider2D>();
            leftCol.size = new Vector2(1.0f, 11.0f);
            for (float y = -4f; y <= 5f; y += 1.0f)
            {
                GameObject tile = new GameObject($"LeftWallTile_{y}");
                tile.transform.SetParent(leftWall.transform);
                tile.transform.position = new Vector3(-10.2f, y, 0f);
                SpriteRenderer rend = tile.AddComponent<SpriteRenderer>();
                rend.sprite = wallSp;
                rend.sortingOrder = 0;
            }

            // Right Wall
            GameObject rightWall = new GameObject("Chamber_RightWall");
            rightWall.transform.SetParent(envRoot.transform);
            rightWall.transform.position = new Vector3(10.5f, 0.5f, 0f);
            BoxCollider2D rightCol = rightWall.AddComponent<BoxCollider2D>();
            rightCol.size = new Vector2(1.0f, 11.0f);
            for (float y = -4f; y <= 5f; y += 1.0f)
            {
                GameObject tile = new GameObject($"RightWallTile_{y}");
                tile.transform.SetParent(rightWall.transform);
                tile.transform.position = new Vector3(10.2f, y, 0f);
                tile.transform.localScale = new Vector3(-1f, 1f, 1f);
                SpriteRenderer rend = tile.AddComponent<SpriteRenderer>();
                rend.sprite = wallSp;
                rend.sortingOrder = 0;
            }

            // Overhead pipes
            for (float hx = -9f; hx <= 9f; hx += 2.0f)
            {
                GameObject pipe = new GameObject($"OverheadPipe_{hx}");
                pipe.transform.SetParent(envRoot.transform);
                pipe.transform.position = new Vector3(hx, 4.6f, 0f);
                pipe.transform.localScale = new Vector3(2.0f, 1.0f, 1f);
                SpriteRenderer rend = pipe.AddComponent<SpriteRenderer>();
                rend.sprite = pipeSp;
                rend.sortingOrder = 1;
            }

            // Architect Surveillance Display
            GameObject monitorObj = new GameObject("Architect_Surveillance_Unit");
            monitorObj.transform.SetParent(envRoot.transform);
            monitorObj.transform.position = new Vector3(0f, 4.2f, 0f);
            monitorObj.transform.localScale = new Vector3(2.0f, 2.0f, 1f);
            SpriteRenderer monRend = monitorObj.AddComponent<SpriteRenderer>();
            monRend.sprite = monitorSp;
            monRend.sortingOrder = 2;

            GameObject monTextObj = new GameObject("Monitor_Text");
            monTextObj.transform.SetParent(monitorObj.transform);
            monTextObj.transform.localPosition = Vector3.zero;
            monTextObj.transform.localScale = new Vector3(0.015f, 0.015f, 1f);
            TextMesh monTm = monTextObj.AddComponent<TextMesh>();
            monTm.text = $"SUBJECT 075\nCHAMBER 0{chamberNum}";
            monTm.fontSize = 32;
            monTm.font = defaultFont;
            monTm.alignment = TextAlignment.Center;
            monTm.anchor = TextAnchor.MiddleCenter;
            monTm.color = atmo.architectMonitorColor;
            monTextObj.GetComponent<MeshRenderer>().sortingOrder = 3;

            // Foreground Framing Silhouettes (Corner Girders at sortingOrder 12, outside active play)
            GameObject fgRoot = new GameObject("--- FOREGROUND ---");
            GameObject tlGirder = new GameObject("FG_CornerGirder_TL");
            tlGirder.transform.SetParent(fgRoot.transform);
            tlGirder.transform.position = new Vector3(-10.5f, 5.2f, 0f);
            SpriteRenderer tlRend = tlGirder.AddComponent<SpriteRenderer>();
            tlRend.sprite = beamSp;
            tlRend.sortingOrder = 12;
            tlRend.color = new Color(0.10f, 0.12f, 0.15f, 0.85f);

            GameObject trGirder = new GameObject("FG_CornerGirder_TR");
            trGirder.transform.SetParent(fgRoot.transform);
            trGirder.transform.position = new Vector3(10.5f, 5.2f, 0f);
            SpriteRenderer trRend = trGirder.AddComponent<SpriteRenderer>();
            trRend.sprite = beamSp;
            trRend.sortingOrder = 12;
            trRend.color = new Color(0.10f, 0.12f, 0.15f, 0.85f);

            // 5. Exit Blast Door
            GameObject puzzleRoot = new GameObject("--- PUZZLE ---");
            GameObject doorObj = new GameObject("ExitDoor");
            doorObj.transform.SetParent(puzzleRoot.transform);
            doorObj.transform.position = new Vector3(8.5f, -2.7f, 0f);

            SpriteRenderer doorRend = doorObj.AddComponent<SpriteRenderer>();
            doorRend.sprite = doorSp;
            doorRend.sortingOrder = 4;

            BoxCollider2D doorBlocker = doorObj.AddComponent<BoxCollider2D>();
            doorBlocker.size = new Vector2(0.7f, 2.4f);

            GameObject exitTriggerObj = new GameObject("ExitTriggerZone");
            exitTriggerObj.transform.SetParent(doorObj.transform);
            exitTriggerObj.transform.localPosition = Vector3.zero;
            BoxCollider2D exitTrigger = exitTriggerObj.AddComponent<BoxCollider2D>();
            exitTrigger.isTrigger = true;
            exitTrigger.size = new Vector2(1.1f, 2.4f);
            exitTrigger.enabled = false;

            GameObject doorLightObj = new GameObject("Door_StatusLight");
            doorLightObj.transform.SetParent(doorObj.transform);
            doorLightObj.transform.localPosition = new Vector3(0f, 1.0f, 0f);
            Light2D doorLight = doorLightObj.AddComponent<Light2D>();
            doorLight.lightType = Light2D.LightType.Point;
            doorLight.pointLightOuterRadius = 2.8f;
            doorLight.color = new Color(1.0f, 0.15f, 0.25f, 1f);
            doorLight.intensity = 1.0f;

            ExitDoor exitComp = doorObj.AddComponent<ExitDoor>();
            SetPrivateField(exitComp, "solidBlockerCollider", doorBlocker);
            SetPrivateField(exitComp, "exitTriggerCollider", exitTrigger);
            SetPrivateField(exitComp, "doorFrameRenderer", doorRend);
            SetPrivateField(exitComp, "doorStatusLightRenderer", doorRend);
            SetPrivateField(exitComp, "statusLight2D", doorLight);

            // 6. Player Character (Physics preserved 100%, cosmetic squash/stretch on Player_Visual)
            GameObject playerRoot = new GameObject("--- PLAYER ---");
            GameObject spawnObj = new GameObject("PlayerSpawnPoint");
            spawnObj.transform.SetParent(playerRoot.transform);
            spawnObj.transform.position = new Vector3(-8.0f, -3.2f, 0f);

            GameObject playerObj = new GameObject("Player_Subject075");
            playerObj.transform.SetParent(playerRoot.transform);
            playerObj.transform.position = spawnObj.transform.position;
            playerObj.tag = "Player";

            BoxCollider2D playerCol = playerObj.AddComponent<BoxCollider2D>();
            playerCol.size = new Vector2(0.5f, 0.85f);

            Rigidbody2D playerRb = playerObj.AddComponent<Rigidbody2D>();
            playerRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            playerRb.interpolation = RigidbodyInterpolation2D.Interpolate;
            playerRb.freezeRotation = true;
            playerRb.mass = 1.0f;

            // Visual representation child
            GameObject visualObj = new GameObject("Player_Visual");
            visualObj.transform.SetParent(playerObj.transform, false);
            visualObj.transform.localPosition = Vector3.zero;

            SpriteRenderer playerRend = visualObj.AddComponent<SpriteRenderer>();
            playerRend.sprite = playerSp;
            playerRend.sortingOrder = 5;

            GameObject visorLightObj = new GameObject("Player_VisorLight");
            visorLightObj.transform.SetParent(visualObj.transform, false);
            visorLightObj.transform.localPosition = new Vector3(0.2f, 0.25f, 0f);
            Light2D visorLight = visorLightObj.AddComponent<Light2D>();
            visorLight.lightType = Light2D.LightType.Point;
            visorLight.pointLightOuterRadius = 1.6f;
            visorLight.color = new Color(0f, 0.94f, 1f, 1f);
            visorLight.intensity = 0.45f;

            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(playerObj.transform);
            groundCheckObj.transform.localPosition = new Vector3(0f, -0.45f, 0f);

            PlayerController2D playerCtrl = playerObj.AddComponent<PlayerController2D>();
            SetPrivateField(playerCtrl, "spriteRenderer", playerRend);
            SetPrivateField(playerCtrl, "groundCheckPoint", groundCheckObj.transform);
            SetPrivateField(playerCtrl, "moveSpeed", 6.5f);
            SetPrivateField(playerCtrl, "jumpForce", 11.5f);

            PlayerVisualAnimator visualAnim = playerObj.AddComponent<PlayerVisualAnimator>();
            visualAnim.VisualTransform = visualObj.transform;
            visualAnim.SpriteRend = playerRend;
            visualAnim.VisorLight = visorLight;

            AbilityController abilityCtrl = playerObj.AddComponent<AbilityController>();

            EchoRecorder echoRec = playerObj.AddComponent<EchoRecorder>();
            SetPrivateField(echoRec, "targetPlayer", playerCtrl);

            EchoPlayback echoPlay = playerObj.AddComponent<EchoPlayback>();
            SetPrivateField(echoPlay, "recorder", echoRec);
            SetPrivateField(echoPlay, "ghostSprite", ghostSp);

            // 7. UI Canvas & HUD (Preserved without redesign)
            GameObject uiRoot = new GameObject("--- UI_CANVAS ---");
            Canvas canvas = uiRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 5f;
            canvas.sortingOrder = 50;
            CanvasScaler scaler = uiRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            uiRoot.AddComponent<GraphicRaycaster>();

            Text titleTxt = CreateUIText(uiRoot.transform, "TitleText", 
                "GLITCHVERSE: ECHO PROTOCOL", defaultFont, 24, TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -30f), new Vector2(600f, 40f), 
                new Color(0f, 0.94f, 1f, 1f));

            Text levelTxt = CreateUIText(uiRoot.transform, "LevelText", 
                sceneTitle, defaultFont, 16, TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -65f), new Vector2(400f, 30f), 
                new Color(1f, 0.7f, 0.2f, 0.9f));

            Text timerTxt = CreateUIText(uiRoot.transform, "TimerText", 
                "TIME: 00:00.0", defaultFont, 16, TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -95f), new Vector2(300f, 30f), 
                new Color(0.8f, 0.9f, 1f, 0.8f));

            Text instTxt = CreateUIText(uiRoot.transform, "InstructionsText", 
                "[A/D] Move   [SPACE] Jump   [R] Restart", defaultFont, 18, TextAnchor.LowerLeft,
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(40f, 40f), new Vector2(600f, 35f), 
                new Color(0.85f, 0.88f, 0.92f, 1f));

            Text abilityTxt = CreateUIText(uiRoot.transform, "AbilityStatusText", 
                "", defaultFont, 16, TextAnchor.LowerLeft,
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(40f, 75f), new Vector2(800f, 35f), 
                new Color(0f, 0.94f, 1f, 1f));

            Text echoTxt = CreateUIText(uiRoot.transform, "EchoStatusText", 
                "ECHO: READY [SPACE RECORD / C PLAY / H CLEAR]", defaultFont, 16, TextAnchor.LowerLeft,
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(40f, 110f), new Vector2(800f, 35f), 
                new Color(0.85f, 0f, 1f, 1f));

            // 7.5 Emergency Fullscreen Flash Overlay (Phase 07)
            GameObject flashObj = new GameObject("EmergencyFlashOverlay");
            flashObj.transform.SetParent(uiRoot.transform, false);
            RectTransform flashRt = flashObj.AddComponent<RectTransform>();
            flashRt.anchorMin = Vector2.zero;
            flashRt.anchorMax = Vector2.one;
            flashRt.offsetMin = Vector2.zero;
            flashRt.offsetMax = Vector2.zero;
            Image flashImg = flashObj.AddComponent<Image>();
            flashImg.color = new Color(1f, 0f, 0.15f, 0f);
            flashImg.raycastTarget = false;

            // Architect Subtitle Banner (Phase 07)
            GameObject subBannerObj = new GameObject("ArchitectSubtitleBanner");
            subBannerObj.transform.SetParent(uiRoot.transform, false);
            RectTransform bannerRt = subBannerObj.AddComponent<RectTransform>();
            bannerRt.anchorMin = new Vector2(0.15f, 0.78f);
            bannerRt.anchorMax = new Vector2(0.85f, 0.90f);
            bannerRt.offsetMin = Vector2.zero;
            bannerRt.offsetMax = Vector2.zero;

            Image bannerBg = subBannerObj.AddComponent<Image>();
            bannerBg.color = new Color(0.02f, 0.04f, 0.08f, 0.92f);

            Text subSpeakerTxt = CreateUIText(subBannerObj.transform, "SpeakerNameText",
                "// THE ARCHITECT //", defaultFont, 14, TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(16f, -6f), new Vector2(300f, 20f),
                new Color(0f, 0.94f, 1f, 1f));

            Text subContentTxt = CreateUIText(subBannerObj.transform, "SubtitleContentText",
                "", defaultFont, 20, TextAnchor.MiddleLeft,
                new Vector2(0f, 0f), new Vector2(1f, 0.75f), new Vector2(20f, 0f), new Vector2(-40f, 0f),
                Color.white);
            subContentTxt.horizontalOverflow = HorizontalWrapMode.Wrap;

            ArchitectDialogue dialogue = uiRoot.AddComponent<ArchitectDialogue>();
            SetPrivateField(dialogue, "subtitleBannerPanel", subBannerObj);
            SetPrivateField(dialogue, "speakerNameText", subSpeakerTxt);
            SetPrivateField(dialogue, "dialogueContentText", subContentTxt);
            SetPrivateField(dialogue, "subtitleText", subContentTxt);
            SetPrivateField(dialogue, "flashOverlay", flashImg);

            GameObject completePanelObj = new GameObject("CompletionPanel");
            completePanelObj.transform.SetParent(uiRoot.transform, false);
            RectTransform compRt = completePanelObj.AddComponent<RectTransform>();
            compRt.anchorMin = new Vector2(0.2f, 0.2f);
            compRt.anchorMax = new Vector2(0.8f, 0.8f);
            compRt.offsetMin = Vector2.zero;
            compRt.offsetMax = Vector2.zero;

            Image compBg = completePanelObj.AddComponent<Image>();
            compBg.color = new Color(0.04f, 0.06f, 0.10f, 0.95f);

            Text compTitle = CreateUIText(completePanelObj.transform, "CompletionTitle", 
                isFinal ? "SIMULATION TERMINATED // REALITY BREACHED" : "CONTAINMENT PROTOCOL CLEARED", 
                defaultFont, 28, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f), Vector2.zero, new Vector2(800f, 50f), 
                new Color(0f, 0.94f, 1f, 1f));

            Text compStats = CreateUIText(completePanelObj.transform, "CompletionStats", 
                "", defaultFont, 18, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.45f), new Vector2(0.5f, 0.45f), Vector2.zero, new Vector2(600f, 80f), 
                Color.white);

            Text compPrompt = CreateUIText(completePanelObj.transform, "CompletionPrompt", 
                isFinal ? "[R] RESTART FROM CHAMBER 1" : "[SPACE] ADVANCE TO NEXT CHAMBER", 
                defaultFont, 20, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.2f), Vector2.zero, new Vector2(600f, 40f), 
                new Color(1f, 0.7f, 0.2f, 1f));

            completePanelObj.SetActive(false);

            // 8. Level HUD Controller
            LevelHUD hud = uiRoot.AddComponent<LevelHUD>();
            SetPrivateField(hud, "titleText", titleTxt);
            SetPrivateField(hud, "levelLabelText", levelTxt);
            SetPrivateField(hud, "timerText", timerTxt);
            SetPrivateField(hud, "abilityStatusText", abilityTxt);
            SetPrivateField(hud, "echoHintText", echoTxt);
            SetPrivateField(hud, "instructionsText", instTxt);
            SetPrivateField(hud, "completionPanel", completePanelObj);
            SetPrivateField(hud, "completionTitleText", compTitle);
            SetPrivateField(hud, "completionStatsText", compStats);
            SetPrivateField(hud, "completionPromptText", compPrompt);
            SetPrivateField(hud, "completionBannerText", compTitle);
            SetPrivateField(hud, "exitDoor", exitComp);
            SetPrivateField(hud, "abilityController", abilityCtrl);
            SetPrivateField(hud, "echoPlayback", echoPlay);

            // 9. Director and Triggers
            GameObject directorObj = new GameObject("LevelDirector");
            LevelDirector director = directorObj.AddComponent<LevelDirector>();
            SetPrivateField(director, "levelTitle", sceneTitle);
            SetPrivateField(director, "chamberNumber", chamberNum);
            SetPrivateField(director, "nextSceneName", nextScene);
            SetPrivateField(director, "isFinalLevel", isFinal);
            SetPrivateField(director, "exitDoor", exitComp);
            SetPrivateField(director, "localPlayer", playerCtrl);
            SetPrivateField(director, "spawnPoint", spawnObj.transform);
            SetPrivateField(hud, "levelDirector", director);

            ArchitectTriggers triggers = directorObj.AddComponent<ArchitectTriggers>();
            SetPrivateField(triggers, "levelDirector", director);
            SetPrivateField(triggers, "abilityController", abilityCtrl);
            SetPrivateField(triggers, "echoPlayback", echoPlay);

            // 10. Horror Chapter Introduction Overlay (Phase 07.5)
            GameObject introPanelObj = new GameObject("ChapterIntroPanel");
            introPanelObj.transform.SetParent(uiRoot.transform, false);
            RectTransform introRt = introPanelObj.AddComponent<RectTransform>();
            introRt.anchorMin = Vector2.zero;
            introRt.anchorMax = Vector2.one;
            introRt.offsetMin = Vector2.zero;
            introRt.offsetMax = Vector2.zero;

            Image introBg = introPanelObj.AddComponent<Image>();
            introBg.color = Color.black;

            Text introTitle = CreateUIText(introPanelObj.transform, "ChapterTitleText",
                "", defaultFont, 48, TextAnchor.MiddleCenter,
                new Vector2(0.1f, 0.48f), new Vector2(0.9f, 0.68f), Vector2.zero, Vector2.zero,
                Color.white);
            introTitle.rectTransform.offsetMin = Vector2.zero;
            introTitle.rectTransform.offsetMax = Vector2.zero;
            introTitle.horizontalOverflow = HorizontalWrapMode.Overflow;
            introTitle.verticalOverflow = VerticalWrapMode.Overflow;

            Text introSubtitle = CreateUIText(introPanelObj.transform, "ChapterSubtitleText",
                "", defaultFont, 24, TextAnchor.MiddleCenter,
                new Vector2(0.1f, 0.32f), new Vector2(0.9f, 0.44f), Vector2.zero, Vector2.zero,
                new Color(0.6f, 0.65f, 0.72f, 1f));
            introSubtitle.rectTransform.offsetMin = Vector2.zero;
            introSubtitle.rectTransform.offsetMax = Vector2.zero;
            introSubtitle.horizontalOverflow = HorizontalWrapMode.Overflow;
            introSubtitle.verticalOverflow = VerticalWrapMode.Overflow;

            Text introSkip = CreateUIText(introPanelObj.transform, "SkipHintText",
                "[SPACE] CONTINUE", defaultFont, 16, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.1f), new Vector2(0.5f, 0.1f), new Vector2(0f, 30f), new Vector2(300f, 30f),
                new Color(0.45f, 0.45f, 0.45f, 1f));

            ChapterIntroController introCtrl = uiRoot.AddComponent<ChapterIntroController>();
            SetPrivateField(introCtrl, "introPanel", introPanelObj);
            SetPrivateField(introCtrl, "backgroundOverlay", introBg);
            SetPrivateField(introCtrl, "chapterTitleText", introTitle);
            SetPrivateField(introCtrl, "chapterSubtitleText", introSubtitle);
            SetPrivateField(introCtrl, "skipHintText", introSkip);
            SetPrivateField(introCtrl, "levelDirector", director);
            SetPrivateField(introCtrl, "player", playerCtrl);

            // 11. Impossible Question Prompt UI
            Text questionTxt = CreateUIText(uiRoot.transform, "ImpossibleQuestionPromptText",
                "", defaultFont, 24, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 30f), new Vector2(850f, 220f),
                Color.white);
            questionTxt.verticalOverflow = VerticalWrapMode.Overflow;
            questionTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
            questionTxt.gameObject.SetActive(false);

            GameObject sessionObj = new GameObject("GameSession");
            sessionObj.AddComponent<GameSession>();

            subBannerObj.transform.SetAsLastSibling();
            questionTxt.transform.SetAsLastSibling();

            return new BaseSceneRefs
            {
                camera = cam,
                spawnPoint = spawnObj.transform,
                player = playerCtrl,
                abilityCtrl = abilityCtrl,
                hud = hud,
                director = director,
                exitDoor = exitComp,
                dialogue = dialogue,
                triggers = triggers,
                envRoot = envRoot,
                puzzleRoot = puzzleRoot,
                questionPromptText = questionTxt
            };
        }
        #endregion

        #region Level 01: The First Lie (Cinematic Industrial Containment Redesign)
        public static void ConstructLevel01Scene()
        {
            var atmo = new ChapterAtmosphereConfig
            {
                ambientColor = new Color(0.32f, 0.30f, 0.28f, 1f), // Moody industrial charcoal-slate base
                ambientIntensity = 0.75f,
                spawnLampColor = new Color(1.0f, 0.72f, 0.35f, 1f), // Warm amber bulkhead lighting
                spawnLampIntensity = 1.45f,
                spawnLampFlicker = FlickeringLight2D.FlickerMode.SubtleFluorescent,
                particleType = AtmosphericParticleEmitter.AtmosphereType.ContainmentDust,
                architectMonitorColor = new Color(0.2f, 0.95f, 0.75f, 0.95f) // Phosphor green/cyan
            };

            var refs = SetupBaseChamber("CONTAINMENT 01 // FIRST LIE", 1, "Level02", false, atmo);
            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

            // Load Chapter 1 Cinematic Modular Assets
            Sprite concreteSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_weathered_concrete.png");
            Sprite wainscotSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_wall_lower_wainscot.png");
            Sprite upperConcreteSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_wall_upper_concrete.png") ?? concreteSp;
            Sprite heavyPlatSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_heavy_platform.png");
            Sprite trussSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_overhead_truss.png");
            Sprite railingSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_catwalk_railing.png");
            Sprite exitSignSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_exit_sign.png");
            Sprite blastDoorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_blast_door_cinematic.png");
            Sprite muralSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_mural_safer_tomorrow.png");
            Sprite deskSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_desk_terminal.png");
            Sprite cablesSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_hanging_cables.png");
            Sprite lockersSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_wall_lockers.png");
            Sprite chasmSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_chasm_backdrop.png");
            Sprite fgSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_foreground_silhouette.png");
            Sprite pipeSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_pipe_cluster.png");

            Sprite cubeSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/cube_crate.png");
            Sprite plateSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/pressure_plate.png");
            Sprite hazardSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/hazard_stripes.png");
            Sprite subjDecalSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/decal_subject075.png");
            Sprite beaconSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/warning_beacon.png");
            Sprite barrierSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/security_barrier_grid.png");
            Sprite switchSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/echo_switch.png");
            Sprite laserSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/hazard_laser.png");

            // =========================================================================
            // 1. BACKGROUND ARCHITECTURE: WEATHERED CONCRETE WALL & CAVERNOUS CHASM
            // =========================================================================
            GameObject bgRoot = GameObject.Find("--- BACKGROUND ---");
            if (bgRoot != null)
            {
                Transform bgGrid = bgRoot.transform.Find("BG_BackWallGrid");
                if (bgGrid != null) Object.DestroyImmediate(bgGrid.gameObject);
                Transform bgReactor = bgRoot.transform.Find("BG_ReactorCore");
                if (bgReactor != null) Object.DestroyImmediate(bgReactor.gameObject);
                for (int i = bgRoot.transform.childCount - 1; i >= 0; i--)
                {
                    Transform child = bgRoot.transform.GetChild(i);
                    if (child.name.StartsWith("BG_Pillar")) Object.DestroyImmediate(child.gameObject);
                }
            }
            else
            {
                bgRoot = new GameObject("--- BACKGROUND ---");
            }

            // A. Weathered Concrete Bunker Wall (Left & Center: X from -10.0 to +3.0)
            GameObject concreteWallRoot = new GameObject("BG_WeatheredConcreteWall");
            concreteWallRoot.transform.SetParent(bgRoot.transform);
            concreteWallRoot.transform.position = Vector3.zero;

            for (float wx = -10.0f; wx <= 3.0f; wx += 2.0f)
            {
                for (float wy = -3.0f; wy <= 4.0f; wy += 2.0f)
                {
                    GameObject cTile = new GameObject($"ConcreteTile_{wx}_{wy}");
                    cTile.transform.SetParent(concreteWallRoot.transform);
                    cTile.transform.position = new Vector3(wx, wy, 0f);
                    SpriteRenderer cr = cTile.AddComponent<SpriteRenderer>();
                    cr.sprite = (wy <= -2.5f && wainscotSp != null) ? wainscotSp : upperConcreteSp;
                    cr.sortingOrder = -22;
                    cr.color = new Color(0.95f, 0.92f, 0.90f, 1f);
                }
            }

            // Unblock right wall renderers to reveal cavernous containment chasm
            Transform chamberRightWall = refs.envRoot.transform.Find("Chamber_RightWall");
            if (chamberRightWall != null)
            {
                for (int i = chamberRightWall.childCount - 1; i >= 0; i--)
                {
                    Object.DestroyImmediate(chamberRightWall.GetChild(i).gameObject);
                }
            }

            // B. Cavernous Containment Chasm Backdrop (Right: X from 3.0 to 12.0)
            GameObject chasmBackdrop = new GameObject("BG_ContainmentChasm");
            chasmBackdrop.transform.SetParent(bgRoot.transform);
            chasmBackdrop.transform.position = new Vector3(7.8f, 0.5f, 2.0f);
            chasmBackdrop.transform.localScale = new Vector3(5.5f, 3.0f, 1f);
            SpriteRenderer chasmRend = chasmBackdrop.AddComponent<SpriteRenderer>();
            chasmRend.sprite = chasmSp;
            chasmRend.sortingOrder = -25;
            var unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default") ?? Shader.Find("Sprites/Default");
            if (unlitShader != null) chasmRend.material = new Material(unlitShader);
            chasmRend.color = Color.white;

            // Distant pin-lights on machinery towers
            GameObject pinLight1 = new GameObject("Chasm_TowerBeacon_1");
            pinLight1.transform.SetParent(chasmBackdrop.transform);
            pinLight1.transform.localPosition = new Vector3(-0.2f, 0.2f, 0f);
            Light2D pl1 = pinLight1.AddComponent<Light2D>();
            pl1.lightType = Light2D.LightType.Point;
            pl1.pointLightOuterRadius = 3.5f;
            pl1.pointLightInnerRadius = 0.3f;
            pl1.color = new Color(1.0f, 0.70f, 0.25f, 1f);
            pl1.intensity = 1.6f;

            GameObject pinLight2 = new GameObject("Chasm_TowerBeacon_2");
            pinLight2.transform.SetParent(chasmBackdrop.transform);
            pinLight2.transform.localPosition = new Vector3(0.5f, 0.6f, 0f);
            Light2D pl2 = pinLight2.AddComponent<Light2D>();
            pl2.lightType = Light2D.LightType.Point;
            pl2.pointLightOuterRadius = 3.0f;
            pl2.pointLightInnerRadius = 0.2f;
            pl2.color = new Color(1.0f, 0.70f, 0.25f, 1f);
            pl2.intensity = 1.4f;

            // C. Heavy Vertical Structural Pillars framing boundary at X = 3.6
            GameObject strutCol = new GameObject("BG_ChasmBoundaryPillar");
            strutCol.transform.SetParent(bgRoot.transform);
            strutCol.transform.position = new Vector3(3.6f, 0.5f, 0f);
            strutCol.transform.localScale = new Vector3(0.8f, 9.5f, 1f);
            SpriteRenderer strutRend = strutCol.AddComponent<SpriteRenderer>();
            strutRend.sprite = trussSp;
            strutRend.sortingOrder = -15;
            strutRend.color = new Color(0.22f, 0.25f, 0.30f, 0.95f);

            // =========================================================================
            // 2. FLOOR & CEILING UPGRADE: HEAVY RIVETED GIRDER PLATFORMS & TRUSSES
            // =========================================================================
            Transform chamberFloor = refs.envRoot.transform.Find("Chamber_Floor");
            if (chamberFloor != null)
            {
                for (int i = chamberFloor.childCount - 1; i >= 0; i--)
                {
                    Object.DestroyImmediate(chamberFloor.GetChild(i).gameObject);
                }

                for (float fx = -10.0f; fx <= 10.0f; fx += 2.0f)
                {
                    GameObject pTile = new GameObject($"HeavyFloorDeck_{fx}");
                    pTile.transform.SetParent(chamberFloor);
                    pTile.transform.position = new Vector3(fx, -4.0f, 0f);
                    SpriteRenderer pr = pTile.AddComponent<SpriteRenderer>();
                    pr.sprite = heavyPlatSp;
                    pr.sortingOrder = 0;

                    GameObject underStrut = new GameObject($"UnderStrut_{fx}");
                    underStrut.transform.SetParent(chamberFloor);
                    underStrut.transform.position = new Vector3(fx, -4.8f, 0f);
                    underStrut.transform.localScale = new Vector3(1.0f, 0.8f, 1f);
                    SpriteRenderer usr = underStrut.AddComponent<SpriteRenderer>();
                    usr.sprite = heavyPlatSp;
                    usr.sortingOrder = -2;
                    usr.color = new Color(0.12f, 0.14f, 0.18f, 0.9f);
                }
            }

            Transform chamberCeiling = refs.envRoot.transform.Find("Chamber_Ceiling");
            if (chamberCeiling != null)
            {
                for (int i = chamberCeiling.childCount - 1; i >= 0; i--)
                {
                    Object.DestroyImmediate(chamberCeiling.GetChild(i).gameObject);
                }

                for (float cx = -10.0f; cx <= 10.0f; cx += 2.0f)
                {
                    GameObject cTruss = new GameObject($"OverheadTruss_{cx}");
                    cTruss.transform.SetParent(chamberCeiling);
                    cTruss.transform.position = new Vector3(cx, 5.2f, 0f);
                    SpriteRenderer ctr = cTruss.AddComponent<SpriteRenderer>();
                    ctr.sprite = trussSp;
                    ctr.sortingOrder = 0;
                }
            }

            for (int i = refs.envRoot.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = refs.envRoot.transform.GetChild(i);
                if (child.name.StartsWith("OverheadPipe")) Object.DestroyImmediate(child.gameObject);
            }

            float[] cableXs = new float[] { -8f, -4.5f, -1.0f, 2.5f, 6.5f };
            foreach (var cx in cableXs)
            {
                GameObject cableObj = new GameObject($"Ceiling_Cables_{cx}");
                cableObj.transform.SetParent(refs.envRoot.transform);
                cableObj.transform.position = new Vector3(cx, 4.4f, 0f);
                cableObj.transform.localScale = new Vector3(1.8f, 1.2f, 1f);
                SpriteRenderer cRend = cableObj.AddComponent<SpriteRenderer>();
                cRend.sprite = cablesSp;
                cRend.sortingOrder = -5;
            }

            // =========================================================================
            // 3. LEFT-SIDE BUNKER INSTALLATIONS (ELECTRICAL LOCKERS, CONDUITS, SPAWN)
            // =========================================================================
            GameObject lockersObj = new GameObject("Deco_WallBreakerLockers");
            lockersObj.transform.SetParent(refs.envRoot.transform);
            lockersObj.transform.position = new Vector3(-9.6f, -2.8f, 0f);
            lockersObj.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
            SpriteRenderer lockRend = lockersObj.AddComponent<SpriteRenderer>();
            lockRend.sprite = lockersSp;
            lockRend.sortingOrder = -5;

            GameObject pipeCluster1 = new GameObject("Deco_PipeCluster_Spawn");
            pipeCluster1.transform.SetParent(refs.envRoot.transform);
            pipeCluster1.transform.position = new Vector3(-9.5f, 0.2f, 0f);
            pipeCluster1.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
            SpriteRenderer pcRend1 = pipeCluster1.AddComponent<SpriteRenderer>();
            pcRend1.sprite = pipeSp;
            pcRend1.sortingOrder = -5;

            GameObject pipeCluster2 = new GameObject("Deco_PipeCluster_Mid");
            pipeCluster2.transform.SetParent(refs.envRoot.transform);
            pipeCluster2.transform.position = new Vector3(-6.5f, 2.5f, 0f);
            pipeCluster2.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
            SpriteRenderer pcRend2 = pipeCluster2.AddComponent<SpriteRenderer>();
            pcRend2.sprite = pipeSp;
            pcRend2.sortingOrder = -5;

            GameObject subjDecalObj = new GameObject("Deco_Subject075_Decal");
            subjDecalObj.transform.SetParent(refs.envRoot.transform);
            subjDecalObj.transform.position = new Vector3(-7.5f, 1.2f, 0f);
            SpriteRenderer sdRend = subjDecalObj.AddComponent<SpriteRenderer>();
            sdRend.sprite = subjDecalSp;
            sdRend.sortingOrder = -5;

            // =========================================================================
            // 4. ARCHITECT WORKSTATION & CORPORATE MURAL ("A SAFER TOMORROW")
            // =========================================================================
            Transform oldMonitor = refs.envRoot.transform.Find("Architect_Surveillance_Unit");
            if (oldMonitor != null) Object.DestroyImmediate(oldMonitor.gameObject);

            GameObject muralObj = new GameObject("Deco_CorporateMural");
            muralObj.transform.SetParent(refs.envRoot.transform);
            muralObj.transform.position = new Vector3(0.5f, 0.4f, 0f);
            muralObj.transform.localScale = new Vector3(1.6f, 1.6f, 1f);
            SpriteRenderer muralRend = muralObj.AddComponent<SpriteRenderer>();
            muralRend.sprite = muralSp;
            muralRend.sortingOrder = -20;

            GameObject muralSpotObj = new GameObject("Mural_OverheadSpot");
            muralSpotObj.transform.SetParent(refs.envRoot.transform);
            muralSpotObj.transform.position = new Vector3(0.5f, 2.2f, 0f);
            Light2D mSpot = muralSpotObj.AddComponent<Light2D>();
            mSpot.lightType = Light2D.LightType.Point;
            mSpot.pointLightOuterRadius = 5.5f;
            mSpot.pointLightInnerRadius = 0.6f;
            mSpot.color = new Color(1.0f, 0.78f, 0.42f, 1f);
            mSpot.intensity = 2.0f;

            GameObject deskObj = new GameObject("Deco_ArchitectWorkstation");
            deskObj.transform.SetParent(refs.envRoot.transform);
            deskObj.transform.position = new Vector3(0.2f, -3.25f, 0f);
            deskObj.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            SpriteRenderer deskRend = deskObj.AddComponent<SpriteRenderer>();
            deskRend.sprite = deskSp;
            deskRend.sortingOrder = 2;

            GameObject crtGlowObj = new GameObject("Desk_CRTGlow");
            crtGlowObj.transform.SetParent(deskObj.transform);
            crtGlowObj.transform.localPosition = new Vector3(0.0f, 0.3f, 0f);
            Light2D crtLight = crtGlowObj.AddComponent<Light2D>();
            crtLight.lightType = Light2D.LightType.Point;
            crtLight.pointLightOuterRadius = 2.5f;
            crtLight.pointLightInnerRadius = 0.3f;
            crtLight.color = new Color(0.2f, 0.95f, 0.75f, 1f);
            crtLight.intensity = 1.0f;

            GameObject deskLampObj = new GameObject("Desk_GooseneckLamp");
            deskLampObj.transform.SetParent(deskObj.transform);
            deskLampObj.transform.localPosition = new Vector3(0.35f, 0.45f, 0f);
            Light2D deskLamp = deskLampObj.AddComponent<Light2D>();
            deskLamp.lightType = Light2D.LightType.Point;
            deskLamp.pointLightOuterRadius = 3.2f;
            deskLamp.pointLightInnerRadius = 0.4f;
            deskLamp.color = new Color(1.0f, 0.82f, 0.42f, 1f);
            deskLamp.intensity = 1.8f;

            GameObject hazardObj = new GameObject("Deco_HazardStripes");
            hazardObj.transform.SetParent(refs.envRoot.transform);
            hazardObj.transform.position = new Vector3(-3.0f, -3.88f, 0f);
            hazardObj.transform.localScale = new Vector3(3.2f, 0.25f, 1f);
            SpriteRenderer hzRend = hazardObj.AddComponent<SpriteRenderer>();
            hzRend.sprite = hazardSp;
            hzRend.sortingOrder = 1;

            GameObject deskLockersObj = new GameObject("Deco_DeskWallLockers");
            deskLockersObj.transform.SetParent(refs.envRoot.transform);
            deskLockersObj.transform.position = new Vector3(2.0f, -2.8f, 0f);
            deskLockersObj.transform.localScale = new Vector3(1.1f, 1.2f, 1f);
            SpriteRenderer deskLockRend = deskLockersObj.AddComponent<SpriteRenderer>();
            deskLockRend.sprite = lockersSp;
            deskLockRend.sortingOrder = -5;

            // =========================================================================
            // 5. OBSERVATION PLATFORM A (ELEVATED CATWALK, BLAST DOOR & REAL EXIT)
            // =========================================================================
            GameObject platformA = new GameObject("Observation_Platform_A");
            platformA.transform.SetParent(refs.envRoot.transform);
            platformA.transform.position = new Vector3(-3.0f, -1.0f, 0f);
            BoxCollider2D platCol = platformA.AddComponent<BoxCollider2D>();
            platCol.size = new Vector2(3.2f, 0.4f);

            for (float px = -4.0f; px <= -2.0f; px += 2.0f)
            {
                GameObject seg = new GameObject($"PlatSeg_{px}");
                seg.transform.SetParent(platformA.transform);
                seg.transform.position = new Vector3(px, -1.0f, 0f);
                seg.transform.localScale = new Vector3(1.0f, 0.4f, 1f);
                SpriteRenderer rend = seg.AddComponent<SpriteRenderer>();
                rend.sprite = heavyPlatSp;
                rend.sortingOrder = 1;
            }

            GameObject platALeftRail = new GameObject("PlatARail_Left");
            platALeftRail.transform.SetParent(platformA.transform);
            platALeftRail.transform.position = new Vector3(-4.0f, -0.6f, 0f);
            platALeftRail.transform.localScale = new Vector3(1.1f, 0.6f, 1f);
            SpriteRenderer rrL = platALeftRail.AddComponent<SpriteRenderer>();
            rrL.sprite = railingSp;
            rrL.sortingOrder = 3;

            GameObject platARightRail = new GameObject("PlatARail_Right");
            platARightRail.transform.SetParent(platformA.transform);
            platARightRail.transform.position = new Vector3(-2.0f, -0.6f, 0f);
            platARightRail.transform.localScale = new Vector3(1.1f, 0.6f, 1f);
            SpriteRenderer rrR = platARightRail.AddComponent<SpriteRenderer>();
            rrR.sprite = railingSp;
            rrR.sortingOrder = 3;

            float[] platAColXs = new float[] { -4.4f, -1.6f };
            foreach (var colX in platAColXs)
            {
                GameObject colObj = new GameObject($"PlatASupport_{colX}");
                colObj.transform.SetParent(platformA.transform);
                colObj.transform.position = new Vector3(colX, -2.5f, 0f);
                colObj.transform.localScale = new Vector3(0.5f, 3.0f, 1f);
                SpriteRenderer colRend = colObj.AddComponent<SpriteRenderer>();
                colRend.sprite = trussSp;
                colRend.sortingOrder = -1;
                colRend.color = new Color(0.24f, 0.27f, 0.32f, 0.95f);
            }

            GameObject platABeam = new GameObject("PlatA_UnderBeam");
            platABeam.transform.SetParent(platformA.transform);
            platABeam.transform.position = new Vector3(-3.0f, -1.3f, 0f);
            platABeam.transform.localScale = new Vector3(1.6f, 0.5f, 1f);
            SpriteRenderer pabRend = platABeam.AddComponent<SpriteRenderer>();
            pabRend.sprite = trussSp;
            pabRend.sortingOrder = 0;
            pabRend.color = new Color(0.35f, 0.38f, 0.44f, 1f);

            GameObject blastDoorFrameA = new GameObject("PlatformA_BlastDoorFrame");
            blastDoorFrameA.transform.SetParent(platformA.transform);
            blastDoorFrameA.transform.position = new Vector3(-3.0f, 0.4f, 0f);
            SpriteRenderer bdfRend = blastDoorFrameA.AddComponent<SpriteRenderer>();
            bdfRend.sprite = blastDoorSp;
            bdfRend.sortingOrder = -5;

            GameObject exitSignA = new GameObject("PlatformA_ExitLightbox");
            exitSignA.transform.SetParent(platformA.transform);
            exitSignA.transform.position = new Vector3(-3.0f, 1.7f, 0f);
            exitSignA.transform.localScale = new Vector3(1.2f, 1.0f, 1f);
            SpriteRenderer esRendA = exitSignA.AddComponent<SpriteRenderer>();
            esRendA.sprite = exitSignSp;
            esRendA.sortingOrder = 4;

            GameObject platALightObj = new GameObject("PlatformA_DoorwayLamp");
            platALightObj.transform.SetParent(platformA.transform);
            platALightObj.transform.position = new Vector3(-3.0f, 2.1f, 0f);
            Light2D platALight = platALightObj.AddComponent<Light2D>();
            platALight.lightType = Light2D.LightType.Point;
            platALight.pointLightOuterRadius = 6.0f;
            platALight.pointLightInnerRadius = 0.8f;
            platALight.color = new Color(1.0f, 0.72f, 0.35f, 1f);
            platALight.intensity = 2.2f;

            FlickeringLight2D platAFlicker = platALightObj.AddComponent<FlickeringLight2D>();
            platAFlicker.Mode = FlickeringLight2D.FlickerMode.SubtleFluorescent;
            platAFlicker.BaseIntensity = 2.2f;

            // =========================================================================
            // 6. PUZZLE MECHANICS: PRESSURE PLATE & MOVABLE CUBE
            // =========================================================================
            GameObject plateObj = new GameObject("PressurePlate");
            plateObj.transform.SetParent(refs.puzzleRoot.transform);
            plateObj.transform.position = new Vector3(3.0f, -3.9f, 0f);

            SpriteRenderer plateRend = plateObj.AddComponent<SpriteRenderer>();
            plateRend.sprite = plateSp;
            plateRend.sortingOrder = 2;

            BoxCollider2D plateTrigger = plateObj.AddComponent<BoxCollider2D>();
            plateTrigger.isTrigger = true;
            plateTrigger.size = new Vector2(1.2f, 0.35f);
            plateTrigger.offset = new Vector2(0f, 0.15f);

            GameObject plateLightObj = new GameObject("Plate_GlowLight");
            plateLightObj.transform.SetParent(plateObj.transform);
            plateLightObj.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            Light2D plateLight = plateLightObj.AddComponent<Light2D>();
            plateLight.lightType = Light2D.LightType.Point;
            plateLight.pointLightOuterRadius = 2.5f;
            plateLight.color = new Color(0f, 0.94f, 1f, 1f);
            plateLight.intensity = 0.4f;

            PressurePlate plateComp = plateObj.AddComponent<PressurePlate>();
            SetPrivateField(plateComp, "plateTopRenderer", plateRend);
            SetPrivateField(plateComp, "plateGlowLight", plateLight);
            SetPrivateField(plateComp, "depressTarget", plateObj.transform);

            GameObject cubeObj = new GameObject("MovableCube");
            cubeObj.transform.SetParent(refs.puzzleRoot.transform);
            cubeObj.transform.position = new Vector3(-2.0f, -3.4f, 0f);

            SpriteRenderer cubeRend = cubeObj.AddComponent<SpriteRenderer>();
            cubeRend.sprite = cubeSp;
            cubeRend.sortingOrder = 3;

            BoxCollider2D cubeCol = cubeObj.AddComponent<BoxCollider2D>();
            cubeCol.size = new Vector2(0.95f, 0.95f);

            Rigidbody2D cubeRb = cubeObj.AddComponent<Rigidbody2D>();
            cubeRb.mass = 2.0f;
            cubeRb.linearDamping = 1.8f;
            cubeRb.angularDamping = 3.0f;
            cubeRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            GameObject cubeLightObj = new GameObject("Cube_AuraLight");
            cubeLightObj.transform.SetParent(cubeObj.transform);
            cubeLightObj.transform.localPosition = Vector3.zero;
            Light2D cubeLight = cubeLightObj.AddComponent<Light2D>();
            cubeLight.lightType = Light2D.LightType.Point;
            cubeLight.pointLightOuterRadius = 1.8f;
            cubeLight.color = new Color(0f, 0.94f, 1f, 1f);
            cubeLight.intensity = 0.6f;

            MovableCube cubeComp = cubeObj.AddComponent<MovableCube>();
            SetPrivateField(cubeComp, "spriteRenderer", cubeRend);

            SetPrivateField(refs.hud, "pressurePlate", plateComp);
            SetPrivateField(refs.director, "pressurePlate", plateComp);
            SetPrivateField(refs.triggers, "pressurePlate", plateComp);

            // =========================================================================
            // 7. DECOY EXIT BLAST DOOR & ILLUMINATED EXIT :) LIGHTBOX (X: 8.5)
            // =========================================================================
            refs.exitDoor.transform.position = new Vector3(8.5f, -2.7f, 0f);
            var decoyRend = refs.exitDoor.GetComponent<SpriteRenderer>();
            if (decoyRend != null)
            {
                decoyRend.sprite = blastDoorSp;
                decoyRend.sortingOrder = 4;
            }

            GameObject decoySignObj = new GameObject("Decoy_ExitLightbox");
            decoySignObj.transform.SetParent(refs.puzzleRoot.transform);
            decoySignObj.transform.position = new Vector3(8.5f, -1.3f, 0f);
            decoySignObj.transform.localScale = new Vector3(1.2f, 1.0f, 1f);
            SpriteRenderer dSignRend = decoySignObj.AddComponent<SpriteRenderer>();
            dSignRend.sprite = exitSignSp;
            dSignRend.sortingOrder = 5;

            GameObject decoySpotObj = new GameObject("Decoy_DoorwayLamp");
            decoySpotObj.transform.SetParent(refs.puzzleRoot.transform);
            decoySpotObj.transform.position = new Vector3(8.5f, -0.8f, 0f);
            Light2D dSpot = decoySpotObj.AddComponent<Light2D>();
            dSpot.lightType = Light2D.LightType.Point;
            dSpot.pointLightOuterRadius = 5.5f;
            dSpot.pointLightInnerRadius = 0.6f;
            dSpot.color = new Color(1.0f, 0.72f, 0.35f, 1f);
            dSpot.intensity = 2.0f;

            GameObject beaconObj = new GameObject("Deco_WarningBeacon_Exit");
            beaconObj.transform.SetParent(refs.puzzleRoot.transform);
            beaconObj.transform.position = new Vector3(8.5f, -0.5f, 0f);
            SpriteRenderer bRend = beaconObj.AddComponent<SpriteRenderer>();
            bRend.sprite = beaconSp;
            bRend.sortingOrder = 5;

            GameObject beaconLightObj = new GameObject("Beacon_Light");
            beaconLightObj.transform.SetParent(beaconObj.transform);
            beaconLightObj.transform.localPosition = Vector3.zero;
            Light2D bLight = beaconLightObj.AddComponent<Light2D>();
            bLight.lightType = Light2D.LightType.Point;
            bLight.pointLightOuterRadius = 2.4f;
            bLight.color = new Color(1.0f, 0.65f, 0.15f, 1f);
            bLight.intensity = 0.65f;

            FlickeringLight2D bFlicker = beaconLightObj.AddComponent<FlickeringLight2D>();
            bFlicker.Mode = FlickeringLight2D.FlickerMode.SubtleFluorescent;
            bFlicker.BaseIntensity = 0.65f;

            GameObject realDoorObj = Object.Instantiate(refs.exitDoor.gameObject, refs.puzzleRoot.transform);
            realDoorObj.name = "RealExitDoor_PlatformA";
            realDoorObj.transform.position = new Vector3(-3.0f, 0.4f, 0f);
            realDoorObj.SetActive(false);
            ExitDoor realDoorComp = realDoorObj.GetComponent<ExitDoor>();

            var realDoorRend = realDoorObj.GetComponent<SpriteRenderer>();
            if (realDoorRend != null)
            {
                realDoorRend.sprite = blastDoorSp;
                realDoorRend.sortingOrder = 4;
            }

            for (int i = realDoorObj.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = realDoorObj.transform.GetChild(i);
                if (child.name == "ExitTriggerZone") continue;
                if (child.name == "Door_StatusLight") continue;
                Object.DestroyImmediate(child.gameObject);
            }

            // =========================================================================
            // 8. SECURITY BARRIER ON OBSERVATION PLATFORM A
            // =========================================================================
            GameObject barrierObj = new GameObject("RealExit_SecurityBarrier");
            barrierObj.transform.SetParent(refs.puzzleRoot.transform);
            barrierObj.transform.position = new Vector3(-3.0f, 0.4f, 0f);

            SpriteRenderer barRend = barrierObj.AddComponent<SpriteRenderer>();
            barRend.sprite = barrierSp;
            barRend.drawMode = SpriteDrawMode.Tiled;
            barRend.size = new Vector2(1.6f, 2.6f);
            barRend.sortingOrder = 6;
            barRend.color = new Color(1.0f, 0.15f, 0.25f, 0.95f);

            BoxCollider2D barCol = barrierObj.AddComponent<BoxCollider2D>();
            barCol.size = new Vector2(1.6f, 2.6f);

            GameObject barLightObj = new GameObject("Barrier_AlertLight");
            barLightObj.transform.SetParent(barrierObj.transform);
            barLightObj.transform.localPosition = new Vector3(0f, 0.8f, 0f);
            Light2D barLight = barLightObj.AddComponent<Light2D>();
            barLight.lightType = Light2D.LightType.Point;
            barLight.pointLightOuterRadius = 3.2f;
            barLight.color = new Color(1.0f, 0.2f, 0.25f, 1f);
            barLight.intensity = 1.4f;

            GameObject barCanvasObj = new GameObject("Barrier_SignUI");
            barCanvasObj.transform.SetParent(barrierObj.transform);
            barCanvasObj.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            Canvas barCanvas = barCanvasObj.AddComponent<Canvas>();
            barCanvas.renderMode = RenderMode.WorldSpace;
            RectTransform barRt = barCanvasObj.GetComponent<RectTransform>();
            barRt.sizeDelta = new Vector2(300f, 80f);
            barRt.localScale = new Vector3(0.007f, 0.007f, 1f);
            Text barText = CreateUIText(barCanvasObj.transform, "SignText", 
                "<color=#FF1E40>[SECURITY INTERLOCK: ACTIVE]</color>", defaultFont, 14, TextAnchor.MiddleCenter,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Color.white);

            SecurityBarrier barrierComp = barrierObj.AddComponent<SecurityBarrier>();
            SetPrivateField(barrierComp, "barrierCollider", barCol);
            SetPrivateField(barrierComp, "barrierRenderer", barRend);
            SetPrivateField(barrierComp, "barrierLight", barLight);
            SetPrivateField(barrierComp, "signText", barText);
            SetPrivateField(barrierComp, "isLocked", true);
            barrierObj.SetActive(false);

            // =========================================================================
            // 9. OBSERVATION PLATFORM B (SUSPENDED CHASM CATWALK & ECHO SWITCH)
            // =========================================================================
            GameObject platformB = new GameObject("Observation_Platform_B");
            platformB.transform.SetParent(refs.envRoot.transform);
            platformB.transform.position = new Vector3(5.5f, -1.4f, 0f);
            BoxCollider2D platBCol = platformB.AddComponent<BoxCollider2D>();
            platBCol.size = new Vector2(3.0f, 0.4f);

            for (float px = 4.5f; px <= 6.5f; px += 2.0f)
            {
                GameObject seg = new GameObject($"PlatBSeg_{px}");
                seg.transform.SetParent(platformB.transform);
                seg.transform.position = new Vector3(px, -1.4f, 0f);
                seg.transform.localScale = new Vector3(1.0f, 0.4f, 1f);
                SpriteRenderer rend = seg.AddComponent<SpriteRenderer>();
                rend.sprite = heavyPlatSp;
                rend.sortingOrder = 1;
            }

            for (float rx = 4.8f; rx <= 6.2f; rx += 1.4f)
            {
                GameObject rail = new GameObject($"PlatBRail_{rx}");
                rail.transform.SetParent(platformB.transform);
                rail.transform.position = new Vector3(rx, -1.0f, 0f);
                rail.transform.localScale = new Vector3(1.5f, 0.6f, 1f);
                SpriteRenderer rr = rail.AddComponent<SpriteRenderer>();
                rr.sprite = railingSp;
                rr.sortingOrder = 3;
            }

            float[] platBColXs = new float[] { 4.2f, 6.8f };
            foreach (var colX in platBColXs)
            {
                GameObject colObj = new GameObject($"PlatBSuspension_{colX}");
                colObj.transform.SetParent(platformB.transform);
                colObj.transform.position = new Vector3(colX, 1.8f, 0f);
                colObj.transform.localScale = new Vector3(0.4f, 6.0f, 1f);
                SpriteRenderer colRend = colObj.AddComponent<SpriteRenderer>();
                colRend.sprite = trussSp;
                colRend.sortingOrder = -1;
                colRend.color = new Color(0.24f, 0.27f, 0.32f, 0.95f);
            }

            GameObject pbbLightObj = new GameObject("PlatB_LanternLight");
            pbbLightObj.transform.SetParent(platformB.transform);
            pbbLightObj.transform.position = new Vector3(5.5f, 0.1f, 0f);
            Light2D pbbLight = pbbLightObj.AddComponent<Light2D>();
            pbbLight.lightType = Light2D.LightType.Point;
            pbbLight.pointLightOuterRadius = 4.5f;
            pbbLight.pointLightInnerRadius = 0.5f;
            pbbLight.color = new Color(1.0f, 0.70f, 0.30f, 1f);
            pbbLight.intensity = 1.8f;

            FlickeringLight2D pbbFlicker = pbbLightObj.AddComponent<FlickeringLight2D>();
            pbbFlicker.Mode = FlickeringLight2D.FlickerMode.SubtleFluorescent;
            pbbFlicker.BaseIntensity = 1.15f;

            GameObject platBBeacon = new GameObject("Deco_PlatB_Beacon");
            platBBeacon.transform.SetParent(refs.envRoot.transform);
            platBBeacon.transform.position = new Vector3(5.5f, 0.4f, 0f);
            SpriteRenderer pbbRend = platBBeacon.AddComponent<SpriteRenderer>();
            pbbRend.sprite = beaconSp;
            pbbRend.sortingOrder = 3;

            float[] conduitXs = new float[] { 5.5f, 3.5f, 1.5f, -0.5f, -2.5f };
            foreach (var cx in conduitXs)
            {
                GameObject interlockCable = new GameObject($"Deco_ConduitLink_{cx}");
                interlockCable.transform.SetParent(refs.envRoot.transform);
                interlockCable.transform.position = new Vector3(cx, 3.8f, 0f);
                interlockCable.transform.localScale = new Vector3(1.8f, 0.7f, 1f);
                SpriteRenderer icRend = interlockCable.AddComponent<SpriteRenderer>();
                icRend.sprite = cablesSp;
                icRend.color = new Color(0.85f, 0.1f, 1.0f, 0.8f);
                icRend.sortingOrder = -3;
            }

            GameObject switchObj = new GameObject("SecondarySwitch_ConsoleB");
            switchObj.transform.SetParent(refs.puzzleRoot.transform);
            switchObj.transform.position = new Vector3(5.5f, -1.0f, 0f);

            SpriteRenderer swRend = switchObj.AddComponent<SpriteRenderer>();
            swRend.sprite = switchSp;
            swRend.sortingOrder = 4;
            swRend.color = new Color(0.25f, 0.28f, 0.35f, 0.7f);

            BoxCollider2D swCol = switchObj.AddComponent<BoxCollider2D>();
            swCol.isTrigger = true;
            swCol.size = new Vector2(1.5f, 0.6f);

            GameObject swLightObj = new GameObject("Switch_StatusLight");
            swLightObj.transform.SetParent(switchObj.transform);
            swLightObj.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            Light2D swLight = swLightObj.AddComponent<Light2D>();
            swLight.lightType = Light2D.LightType.Point;
            swLight.pointLightOuterRadius = 2.4f;
            swLight.color = new Color(0.85f, 0f, 1f, 1f);
            swLight.intensity = 0.25f;

            GameObject swCanvasObj = new GameObject("Switch_ConsoleUI");
            swCanvasObj.transform.SetParent(switchObj.transform);
            swCanvasObj.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            Canvas swCanvas = swCanvasObj.AddComponent<Canvas>();
            swCanvas.renderMode = RenderMode.WorldSpace;
            RectTransform swRt = swCanvasObj.GetComponent<RectTransform>();
            swRt.sizeDelta = new Vector2(300f, 80f);
            swRt.localScale = new Vector3(0.007f, 0.007f, 1f);
            Text swText = CreateUIText(swCanvasObj.transform, "StatusText", 
                "<color=#666666>[STANDBY]</color>", defaultFont, 14, TextAnchor.MiddleCenter,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Color.white);

            SecurityOverrideSwitch switchComp = switchObj.AddComponent<SecurityOverrideSwitch>();
            SetPrivateField(switchComp, "targetBarrier", barrierComp);
            SetPrivateField(switchComp, "switchRenderer", swRend);
            SetPrivateField(switchComp, "glowLight", swLight);
            SetPrivateField(switchComp, "depressVisual", switchObj.transform);
            SetPrivateField(switchComp, "consoleStatusText", swText);
            SetPrivateField(switchComp, "allowPlayerActivation", false);
            SetPrivateField(switchComp, "allowEchoActivation", true);
            SetPrivateField(switchComp, "holdGraceDuration", 2.5f);
            SetPrivateField(switchComp, "latching", false);
            SetPrivateField(switchComp, "isPowered", false);
            SetPrivateField(switchComp, "isActivated", false);

            // =========================================================================
            // 10. DUAL MOVING RED LASERS (LASER A & LASER B)
            // =========================================================================
            GameObject laserAObj = new GameObject("LaserA_Vertical");
            laserAObj.transform.SetParent(refs.puzzleRoot.transform);
            laserAObj.transform.position = new Vector3(1.15f, -2.5f, 0f);

            SpriteRenderer laRend = laserAObj.AddComponent<SpriteRenderer>();
            laRend.sprite = laserSp;
            laRend.color = new Color(1.0f, 0.12f, 0.22f, 1f);
            laRend.sortingOrder = 7;
            laRend.drawMode = SpriteDrawMode.Sliced;
            laRend.size = new Vector2(0.35f, 2.6f);

            BoxCollider2D laCol = laserAObj.AddComponent<BoxCollider2D>();
            laCol.isTrigger = true;
            laCol.size = new Vector2(0.35f, 2.6f);

            GameObject laLightObj = new GameObject("LaserA_Light");
            laLightObj.transform.SetParent(laserAObj.transform);
            laLightObj.transform.localPosition = Vector3.zero;
            Light2D laLight = laLightObj.AddComponent<Light2D>();
            laLight.lightType = Light2D.LightType.Point;
            laLight.pointLightOuterRadius = 2.8f;
            laLight.color = new Color(1.0f, 0.12f, 0.22f, 1f);
            laLight.intensity = 1.4f;

            MovingLaserHazard laserAComp = laserAObj.AddComponent<MovingLaserHazard>();
            laserAComp.ConfigurePath(MovingLaserHazard.MotionAxis.Horizontal, new Vector3(0.5f, -2.5f, 0f), new Vector3(1.8f, -2.5f, 0f), 1.4f, 0.5f, startActive: true);
            SetPrivateField(laserAComp, "laserRenderer", laRend);
            SetPrivateField(laserAComp, "laserLight", laLight);
            SetPrivateField(laserAComp, "activeOnStart", true);
            laserAComp.SetHazardActive(true);

            GameObject laserBObj = new GameObject("LaserB_Horizontal");
            laserBObj.transform.SetParent(refs.puzzleRoot.transform);
            laserBObj.transform.position = new Vector3(-0.5f, -2.2f, 0f);

            SpriteRenderer lbRend = laserBObj.AddComponent<SpriteRenderer>();
            lbRend.sprite = laserSp;
            lbRend.color = new Color(1.0f, 0.12f, 0.22f, 1f);
            lbRend.sortingOrder = 7;
            lbRend.drawMode = SpriteDrawMode.Sliced;
            lbRend.size = new Vector2(2.0f, 0.3f);

            BoxCollider2D lbCol = laserBObj.AddComponent<BoxCollider2D>();
            lbCol.isTrigger = true;
            lbCol.size = new Vector2(2.0f, 0.3f);

            GameObject lbLightObj = new GameObject("LaserB_Light");
            lbLightObj.transform.SetParent(laserBObj.transform);
            lbLightObj.transform.localPosition = Vector3.zero;
            Light2D lbLight = lbLightObj.AddComponent<Light2D>();
            lbLight.lightType = Light2D.LightType.Point;
            lbLight.pointLightOuterRadius = 2.8f;
            lbLight.color = new Color(1.0f, 0.12f, 0.22f, 1f);
            lbLight.intensity = 1.4f;

            MovingLaserHazard laserBComp = laserBObj.AddComponent<MovingLaserHazard>();
            laserBComp.ConfigurePath(MovingLaserHazard.MotionAxis.Vertical, new Vector3(-0.5f, -3.2f, 0f), new Vector3(-0.5f, -1.3f, 0f), 1.1f, 0.0f, startActive: false);
            SetPrivateField(laserBComp, "laserRenderer", lbRend);
            SetPrivateField(laserBComp, "laserLight", lbLight);
            SetPrivateField(laserBComp, "activeOnStart", false);
            laserBComp.SetHazardActive(false);

            // =========================================================================
            // 11. WIRE SCRIPTED BETRAYAL PRANK
            // =========================================================================
            FakeExitPrank prank = refs.exitDoor.gameObject.AddComponent<FakeExitPrank>();
            SetPrivateField(prank, "decoyDoor", refs.exitDoor);
            SetPrivateField(prank, "realDoor", realDoorComp);
            SetPrivateField(prank, "relocatedPosition", new Vector3(-3.0f, 0.4f, 0f));
            SetPrivateField(prank, "securityBarrier", barrierComp);
            SetPrivateField(prank, "secondarySwitch", switchComp);
            SetPrivateField(prank, "laserA", laserAComp);
            SetPrivateField(prank, "laserB", laserBComp);

            SetPrivateField(refs.hud, "echoSwitch", switchComp);
            SetPrivateField(refs.director, "echoSwitch", switchComp);
            SetPrivateField(refs.triggers, "echoSwitch", switchComp);

            // =========================================================================
            // 12. FOREGROUND FRAMING SILHOUETTES (SORTING ORDER 12)
            // =========================================================================
            GameObject fgRoot = GameObject.Find("--- FOREGROUND ---");
            if (fgRoot != null)
            {
                for (int i = fgRoot.transform.childCount - 1; i >= 0; i--)
                {
                    Object.DestroyImmediate(fgRoot.transform.GetChild(i).gameObject);
                }
            }
            else
            {
                fgRoot = new GameObject("--- FOREGROUND ---");
            }

            GameObject fgTopLeft = new GameObject("FG_Framing_TopLeft");
            fgTopLeft.transform.SetParent(fgRoot.transform);
            fgTopLeft.transform.position = new Vector3(-6.0f, 5.4f, 0f);
            fgTopLeft.transform.localScale = new Vector3(2.5f, 1.2f, 1f);
            SpriteRenderer fgRendTL = fgTopLeft.AddComponent<SpriteRenderer>();
            fgRendTL.sprite = fgSp;
            fgRendTL.sortingOrder = 12;

            GameObject fgTopRight = new GameObject("FG_Framing_TopRight");
            fgTopRight.transform.SetParent(fgRoot.transform);
            fgTopRight.transform.position = new Vector3(6.0f, 5.4f, 0f);
            fgTopRight.transform.localScale = new Vector3(-2.5f, 1.2f, 1f);
            SpriteRenderer fgRendTR = fgTopRight.AddComponent<SpriteRenderer>();
            fgRendTR.sprite = fgSp;
            fgRendTR.sortingOrder = 12;

            // =========================================================================
            // 13. STREAMLINED SURVEILLANCE HUD
            // =========================================================================
            var hudCanvas = refs.hud.GetComponent<Canvas>();
            if (hudCanvas != null)
            {
                Text titleTxt = refs.hud.transform.Find("TitleText")?.GetComponent<Text>();
                if (titleTxt != null)
                {
                    titleTxt.fontSize = 14;
                    titleTxt.color = new Color(0.45f, 0.65f, 0.75f, 0.70f);
                    titleTxt.rectTransform.anchoredPosition = new Vector2(35f, -25f);
                }

                Text levelTxt = refs.hud.transform.Find("LevelText")?.GetComponent<Text>();
                if (levelTxt != null)
                {
                    levelTxt.text = "CONTAINMENT 01 // FIRST LIE";
                    levelTxt.fontSize = 16;
                    levelTxt.color = new Color(0.95f, 0.80f, 0.55f, 0.90f);
                    levelTxt.rectTransform.anchoredPosition = new Vector2(35f, -48f);
                }

                Text timerTxt = refs.hud.transform.Find("TimerText")?.GetComponent<Text>();
                if (timerTxt != null)
                {
                    timerTxt.fontSize = 13;
                    timerTxt.color = new Color(0.65f, 0.72f, 0.80f, 0.65f);
                    timerTxt.rectTransform.anchoredPosition = new Vector2(35f, -70f);
                }

                Text instTxt = refs.hud.transform.Find("InstructionsText")?.GetComponent<Text>();
                if (instTxt != null)
                {
                    instTxt.fontSize = 13;
                    instTxt.color = new Color(0.65f, 0.68f, 0.72f, 0.65f);
                    instTxt.rectTransform.anchoredPosition = new Vector2(35f, 25f);
                }

                Text echoTxt = refs.hud.transform.Find("EchoStatusText")?.GetComponent<Text>();
                if (echoTxt != null)
                {
                    echoTxt.fontSize = 13;
                    echoTxt.color = new Color(0.85f, 0.20f, 0.95f, 0.70f);
                    echoTxt.rectTransform.anchoredPosition = new Vector2(35f, 48f);
                }
            }

            // In edit mode, disable fullscreen overlays so scene and camera views are pristine
            var introPanel = GameObject.Find("ChapterIntroPanel");
            if (introPanel != null) introPanel.SetActive(false);
            var subBanner = GameObject.Find("ArchitectSubtitleBanner");
            if (subBanner != null) subBanner.SetActive(false);

            SaveChamberScene("Level01.unity");
        }
        #endregion

        #region Level 02: Gravity Is A Suggestion (Gravity Inversion & Echo Replay)
        public static void ConstructLevel02Scene()
        {
            var atmo = new ChapterAtmosphereConfig
            {
                ambientColor = new Color(0.22f, 0.28f, 0.36f, 1f), // Moody slate-blue / steel cyan abyss
                ambientIntensity = 0.75f,
                spawnLampColor = new Color(1.0f, 0.72f, 0.35f, 1f), // Warm amber maintenance bulkhead lamp
                spawnLampIntensity = 1.45f,
                spawnLampFlicker = FlickeringLight2D.FlickerMode.SubtleFluorescent,
                particleType = AtmosphericParticleEmitter.AtmosphereType.GravityAnomalies,
                architectMonitorColor = new Color(0.20f, 0.88f, 1.0f, 0.95f)
            };

            var refs = SetupBaseChamber("CONTAINMENT 02 // GRAVITY ECHO", 2, "Level03", false, atmo, collapsingFloorGapX: 1.0f);

            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") 
                            ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

            // Load Chapter II Modular Assets
            Sprite ch2AbyssBgSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/Environment/Chapter02/ch2_gravity_abyss_bg.png");
            Sprite ch2CargoPodSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/Environment/Chapter02/ch2_floating_cargo_pod.png");
            Sprite ch2BrokenGirderSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/Environment/Chapter02/ch2_broken_bridge_girder.png");
            Sprite ch2CraneCrateSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/Environment/Chapter02/ch2_suspended_crane_crate.png");
            Sprite ch2InvertedCeilSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/Environment/Chapter02/ch2_inverted_ceiling_deck.png");
            Sprite ch2BrokenEndSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/Environment/Chapter02/ch2_catwalk_broken_end.png");
            Sprite ch2DebrisSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/Environment/Chapter02/ch2_gravity_debris_cluster.png");
            Sprite ch2LanternSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/Environment/Chapter02/ch2_maintenance_lantern.png");
            Sprite ch2EmitterHousingSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/Environment/Chapter02/ch2_hazard_emitter_housing.png");
            Sprite ch2FractureSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/Environment/Chapter02/ch2_collapsing_floor_fracture.png");
            Sprite ch2LeftWallSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/Environment/Chapter02/ch2_left_containment_wall.png");
            Sprite ch2ForegroundSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/Environment/Chapter02/ch2_foreground_silhouettes.png");
            Sprite ch2HeavyPlatSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/Environment/Chapter02/ch2_heavy_platform_deck.png");

            Sprite floorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_tile.png");
            Sprite echoSwitchSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/echo_switch.png");
            Sprite cableSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/cable_bundle.png");
            Sprite hazardSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/hazard_stripes.png");
            Sprite laserSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/hazard_laser.png");
            Sprite railingSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_catwalk_railing.png");
            Sprite exitSignSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_exit_sign.png");
            Sprite blastDoorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ch1_blast_door_cinematic.png");

            // Enable Gravity Inversion power on player
            refs.abilityCtrl.AllowGravity = true;

            // =========================================================================
            // 1. CLEANUP DEFAULT BACKGROUND & BUILD CHAPTER II VISUAL ENVIRONMENT
            // =========================================================================
            GameObject bgRoot = GameObject.Find("--- BACKGROUND ---");
            if (bgRoot != null)
            {
                Transform bgGrid = bgRoot.transform.Find("BG_BackWallGrid");
                if (bgGrid != null) Object.DestroyImmediate(bgGrid.gameObject);
                Transform bgReactor = bgRoot.transform.Find("BG_ReactorCore");
                if (bgReactor != null) Object.DestroyImmediate(bgReactor.gameObject);
                for (int i = bgRoot.transform.childCount - 1; i >= 0; i--)
                {
                    Transform child = bgRoot.transform.GetChild(i);
                    if (child.name.StartsWith("BG_Pillar")) Object.DestroyImmediate(child.gameObject);
                }
            }
            else
            {
                bgRoot = new GameObject("--- BACKGROUND ---");
            }

            // Remove right wall and ceiling visual renderers to reveal the towering gravity chasm
            Transform chamberRightWall = refs.envRoot.transform.Find("Chamber_RightWall");
            if (chamberRightWall != null)
            {
                for (int i = chamberRightWall.childCount - 1; i >= 0; i--)
                {
                    Object.DestroyImmediate(chamberRightWall.GetChild(i).gameObject);
                }
            }
            Transform chamberCeiling = refs.envRoot.transform.Find("Chamber_Ceiling");
            if (chamberCeiling != null)
            {
                for (int i = chamberCeiling.childCount - 1; i >= 0; i--)
                {
                    Object.DestroyImmediate(chamberCeiling.GetChild(i).gameObject);
                }
            }

            // Remove generic monitor, overhead pipes, and old spotlight to unblock the soaring gravity abyss
            Transform oldMonitor = refs.envRoot.transform.Find("Architect_Surveillance_Unit");
            if (oldMonitor != null) Object.DestroyImmediate(oldMonitor.gameObject);

            for (int i = refs.envRoot.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = refs.envRoot.transform.GetChild(i);
                if (child.name.StartsWith("OverheadPipe")) Object.DestroyImmediate(child.gameObject);
            }

            GameObject archSpot = GameObject.Find("Architect_Spotlight");
            if (archSpot != null) Object.DestroyImmediate(archSpot);

            GameObject fgOldRoot = GameObject.Find("--- FOREGROUND ---");
            if (fgOldRoot != null)
            {
                for (int i = fgOldRoot.transform.childCount - 1; i >= 0; i--)
                {
                    Object.DestroyImmediate(fgOldRoot.transform.GetChild(i).gameObject);
                }
            }

            GameObject ch02EnvRoot = new GameObject("CH02_VisualEnvironment");
            ch02EnvRoot.transform.position = Vector3.zero;

            // A. Deep Atmospheric Gravity Abyss Backdrop (Sorting Order: -25, Unlit Material)
            GameObject abyssBackdrop = new GameObject("BG_GravityAbyssBackdrop");
            abyssBackdrop.transform.SetParent(ch02EnvRoot.transform);
            abyssBackdrop.transform.position = new Vector3(1.2f, 0.8f, 2.0f);
            abyssBackdrop.transform.localScale = new Vector3(7.5f, 4.5f, 1f);
            SpriteRenderer abyssRend = abyssBackdrop.AddComponent<SpriteRenderer>();
            abyssRend.sprite = ch2AbyssBgSp;
            abyssRend.sortingOrder = -25;
            var unlitShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default") ?? Shader.Find("Sprites/Default");
            if (unlitShader != null) abyssRend.material = new Material(unlitShader);
            abyssRend.color = Color.white;

            // B. Distant Gravity Reactor Central Core Glow
            GameObject coreGlowObj = new GameObject("Core_CyanLuminescence");
            coreGlowObj.transform.SetParent(ch02EnvRoot.transform);
            coreGlowObj.transform.position = new Vector3(2.5f, 1.2f, 0f);
            Light2D coreGlow = coreGlowObj.AddComponent<Light2D>();
            coreGlow.lightType = Light2D.LightType.Point;
            coreGlow.pointLightOuterRadius = 9.5f;
            coreGlow.pointLightInnerRadius = 0.8f;
            coreGlow.color = new Color(0.15f, 0.85f, 1.0f, 1f);
            coreGlow.intensity = 1.25f;

            FlickeringLight2D coreFlicker = coreGlowObj.AddComponent<FlickeringLight2D>();
            coreFlicker.Mode = FlickeringLight2D.FlickerMode.SlowBreathing;
            coreFlicker.BaseIntensity = 1.25f;

            // C. Zero-G Suspended & Skewed Cargo Containers (Sorting Order: -16)
            // Cargo Pod 1 (Above left floor, tilted 22 degrees)
            GameObject pod1 = new GameObject("Deco_ZeroG_CargoPod_1");
            pod1.transform.SetParent(ch02EnvRoot.transform);
            pod1.transform.position = new Vector3(-2.6f, 2.2f, 0f);
            pod1.transform.rotation = Quaternion.Euler(0f, 0f, 22f);
            pod1.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
            SpriteRenderer p1Rend = pod1.AddComponent<SpriteRenderer>();
            p1Rend.sprite = ch2CargoPodSp;
            p1Rend.sortingOrder = -16;

            // Cargo Pod 2 (High center chasm, tilted -32 degrees)
            GameObject pod2 = new GameObject("Deco_ZeroG_CargoPod_2");
            pod2.transform.SetParent(ch02EnvRoot.transform);
            pod2.transform.position = new Vector3(0.6f, 3.4f, 0f);
            pod2.transform.rotation = Quaternion.Euler(0f, 0f, -32f);
            pod2.transform.localScale = new Vector3(1.0f, 1.0f, 1f);
            SpriteRenderer p2Rend = pod2.AddComponent<SpriteRenderer>();
            p2Rend.sprite = ch2CargoPodSp;
            p2Rend.sortingOrder = -16;

            // Cargo Pod 3 (Right vertical shaft, tilted 16 degrees)
            GameObject pod3 = new GameObject("Deco_ZeroG_CargoPod_3");
            pod3.transform.SetParent(ch02EnvRoot.transform);
            pod3.transform.position = new Vector3(3.8f, 1.8f, 0f);
            pod3.transform.rotation = Quaternion.Euler(0f, 0f, 16f);
            pod3.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
            SpriteRenderer p3Rend = pod3.AddComponent<SpriteRenderer>();
            p3Rend.sprite = ch2CargoPodSp;
            p3Rend.sortingOrder = -16;

            // Cargo Pod 4 (Deep background, scale 0.75, misty alpha)
            GameObject pod4 = new GameObject("Deco_ZeroG_CargoPod_4_Deep");
            pod4.transform.SetParent(ch02EnvRoot.transform);
            pod4.transform.position = new Vector3(-0.4f, 0.3f, 0f);
            pod4.transform.rotation = Quaternion.Euler(0f, 0f, -14f);
            pod4.transform.localScale = new Vector3(0.85f, 0.85f, 1f);
            SpriteRenderer p4Rend = pod4.AddComponent<SpriteRenderer>();
            p4Rend.sprite = ch2CargoPodSp;
            p4Rend.sortingOrder = -18;
            p4Rend.color = new Color(0.7f, 0.85f, 0.95f, 0.65f);

            // D. Suspended Crane Crates Hanging from Vertical Rigging
            // Crane Crate 1 (Right chasm at X: 5.2, Y: 3.2 with warm maintenance amber beacon)
            GameObject craneCrate1 = new GameObject("Deco_CraneCrate_Right");
            craneCrate1.transform.SetParent(ch02EnvRoot.transform);
            craneCrate1.transform.position = new Vector3(5.2f, 3.2f, 0f);
            SpriteRenderer cc1Rend = craneCrate1.AddComponent<SpriteRenderer>();
            cc1Rend.sprite = ch2CraneCrateSp;
            cc1Rend.sortingOrder = -15;

            GameObject cc1LightObj = new GameObject("Crane1_AmberBeacon");
            cc1LightObj.transform.SetParent(craneCrate1.transform);
            cc1LightObj.transform.localPosition = new Vector3(0f, -0.6f, 0f);
            Light2D cc1Light = cc1LightObj.AddComponent<Light2D>();
            cc1Light.lightType = Light2D.LightType.Point;
            cc1Light.pointLightOuterRadius = 3.2f;
            cc1Light.color = new Color(1.0f, 0.72f, 0.35f, 1f);
            cc1Light.intensity = 1.3f;

            // Crane Crate 2 (Left chasm at X: -1.0, Y: 4.2 with warm maintenance amber beacon)
            GameObject craneCrate2 = new GameObject("Deco_CraneCrate_Left");
            craneCrate2.transform.SetParent(ch02EnvRoot.transform);
            craneCrate2.transform.position = new Vector3(-1.0f, 4.2f, 0f);
            SpriteRenderer cc2Rend = craneCrate2.AddComponent<SpriteRenderer>();
            cc2Rend.sprite = ch2CraneCrateSp;
            cc2Rend.sortingOrder = -15;

            GameObject cc2LightObj = new GameObject("Crane2_AmberBeacon");
            cc2LightObj.transform.SetParent(craneCrate2.transform);
            cc2LightObj.transform.localPosition = new Vector3(0f, -0.6f, 0f);
            Light2D cc2Light = cc2LightObj.AddComponent<Light2D>();
            cc2Light.lightType = Light2D.LightType.Point;
            cc2Light.pointLightOuterRadius = 2.8f;
            cc2Light.color = new Color(1.0f, 0.72f, 0.35f, 1f);
            cc2Light.intensity = 1.2f;

            // E. Floating Sheared Bridge Girder (Midground at X: 1.8, Y: -0.2, rotated -12 deg)
            GameObject floatGirder = new GameObject("Deco_Floating_BridgeGirder");
            floatGirder.transform.SetParent(ch02EnvRoot.transform);
            floatGirder.transform.position = new Vector3(1.8f, -0.2f, 0f);
            floatGirder.transform.rotation = Quaternion.Euler(0f, 0f, -12f);
            floatGirder.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            SpriteRenderer fgRend = floatGirder.AddComponent<SpriteRenderer>();
            fgRend.sprite = ch2BrokenGirderSp;
            fgRend.sortingOrder = -14;

            // F. Zero-G Debris Clusters
            float[,] debrisCoords = new float[,] {
                { -4.2f, 0.8f, 1.1f },
                { 2.6f, 0.4f, 0.9f },
                { -0.2f, 2.4f, 1.0f },
                { 6.4f, 2.6f, 0.8f }
            };
            for (int d = 0; d < 4; d++)
            {
                GameObject debObj = new GameObject($"Deco_DebrisCluster_{d}");
                debObj.transform.SetParent(ch02EnvRoot.transform);
                debObj.transform.position = new Vector3(debrisCoords[d, 0], debrisCoords[d, 1], 0f);
                debObj.transform.localScale = Vector3.one * debrisCoords[d, 2];
                SpriteRenderer debRend = debObj.AddComponent<SpriteRenderer>();
                debRend.sprite = ch2DebrisSp;
                debRend.sortingOrder = -14;
            }

            // G. Left Bunker Entrance Wall (X from -10.5 to -6.5, Y from -4.0 to 4.5)
            GameObject leftBunkerWall = new GameObject("BG_LeftBunkerWall");
            leftBunkerWall.transform.SetParent(ch02EnvRoot.transform);
            leftBunkerWall.transform.position = Vector3.zero;
            for (float wx = -10.0f; wx <= -6.5f; wx += 2.0f)
            {
                for (float wy = -3.0f; wy <= 4.0f; wy += 2.0f)
                {
                    GameObject bTile = new GameObject($"BunkerTile_{wx}_{wy}");
                    bTile.transform.SetParent(leftBunkerWall.transform);
                    bTile.transform.position = new Vector3(wx, wy, 0f);
                    SpriteRenderer bRend = bTile.AddComponent<SpriteRenderer>();
                    bRend.sprite = ch2LeftWallSp;
                    bRend.sortingOrder = -22;
                }
            }

            // Heavy Entrance Blast Door Frame at Player Spawn
            GameObject spawnBlastDoor = new GameObject("Deco_SpawnBlastDoor");
            spawnBlastDoor.transform.SetParent(ch02EnvRoot.transform);
            spawnBlastDoor.transform.position = new Vector3(-8.8f, -2.4f, 0f);
            spawnBlastDoor.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
            SpriteRenderer sbdRend = spawnBlastDoor.AddComponent<SpriteRenderer>();
            sbdRend.sprite = blastDoorSp;
            sbdRend.sortingOrder = -12;

            // =========================================================================
            // 2. FLOOR CATWALK DRESSING & BROKEN PLATFORM TERMINUS (X: -10.5 to -4.8)
            // =========================================================================
            // Clean up default floor visual tiles to replace with heavy industrial catwalk pieces
            Transform chamberFloor = refs.envRoot.transform.Find("Chamber_Floor");
            if (chamberFloor != null)
            {
                for (int i = chamberFloor.childCount - 1; i >= 0; i--)
                {
                    Transform child = chamberFloor.GetChild(i);
                    if (child.name.StartsWith("FloorTile_")) Object.DestroyImmediate(child.gameObject);
                }
            }

            GameObject floorDressRoot = new GameObject("Floor_Catwalk_Dressed");
            floorDressRoot.transform.SetParent(refs.envRoot.transform);
            floorDressRoot.transform.position = Vector3.zero;

            for (float fx = -10.5f; fx <= -5.5f; fx += 2.0f)
            {
                GameObject fSeg = new GameObject($"FloorSeg_{fx}");
                fSeg.transform.SetParent(floorDressRoot.transform);
                fSeg.transform.position = new Vector3(fx, -4.0f, 0f);
                SpriteRenderer fsRend = fSeg.AddComponent<SpriteRenderer>();
                fsRend.sprite = ch2HeavyPlatSp;
                fsRend.sortingOrder = 0;
            }

            // Broken sheared catwalk terminus at X: -4.8 pointing into the plasma abyss
            GameObject brokenFloorTerminus = new GameObject("Catwalk_BrokenTerminus_Left");
            brokenFloorTerminus.transform.SetParent(floorDressRoot.transform);
            brokenFloorTerminus.transform.position = new Vector3(-4.8f, -4.0f, 0f);
            SpriteRenderer bftRend = brokenFloorTerminus.AddComponent<SpriteRenderer>();
            bftRend.sprite = ch2BrokenEndSp;
            bftRend.sortingOrder = 1;

            // Warm maintenance lantern at broken edge
            GameObject lanternFloorObj = new GameObject("Lantern_BrokenFloorEdge");
            lanternFloorObj.transform.SetParent(floorDressRoot.transform);
            lanternFloorObj.transform.position = new Vector3(-5.3f, -3.6f, 0f);
            SpriteRenderer lfoRend = lanternFloorObj.AddComponent<SpriteRenderer>();
            lfoRend.sprite = ch2LanternSp;
            lfoRend.sortingOrder = 5;

            GameObject lfoLightObj = new GameObject("LanternLight_BrokenFloor");
            lfoLightObj.transform.SetParent(lanternFloorObj.transform);
            lfoLightObj.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            Light2D lfoLight = lfoLightObj.AddComponent<Light2D>();
            lfoLight.lightType = Light2D.LightType.Point;
            lfoLight.pointLightOuterRadius = 2.8f;
            lfoLight.color = new Color(1.0f, 0.72f, 0.35f, 1f);
            lfoLight.intensity = 1.35f;

            // Safety railings along left spawn catwalk
            for (float rx = -10.0f; rx <= -6.0f; rx += 1.0f)
            {
                GameObject rail = new GameObject($"Rail_{rx}");
                rail.transform.SetParent(floorDressRoot.transform);
                rail.transform.position = new Vector3(rx, -3.75f, 0f);
                SpriteRenderer rr = rail.AddComponent<SpriteRenderer>();
                rr.sprite = railingSp;
                rr.sortingOrder = 3;
            }

            // =========================================================================
            // 3. COLLAPSING FLOOR BETRAYAL TRAP WITH VISUAL WARNING CLUE
            // =========================================================================
            GameObject trapTileObj = new GameObject("CollapsingFloorTrap");
            trapTileObj.transform.SetParent(refs.puzzleRoot.transform);
            trapTileObj.transform.position = new Vector3(1.0f, -4.0f, 0f);

            SpriteRenderer trapRend = trapTileObj.AddComponent<SpriteRenderer>();
            trapRend.sprite = ch2FractureSp != null ? ch2FractureSp : floorSp;
            trapRend.sortingOrder = 1;

            BoxCollider2D trapCol = trapTileObj.AddComponent<BoxCollider2D>();
            trapCol.size = new Vector2(1.2f, 1.0f);

            // Visual clue: Caution light glowing with amber structural stress pulse
            GameObject trapLightObj = new GameObject("Trap_CautionLight");
            trapLightObj.transform.SetParent(trapTileObj.transform);
            trapLightObj.transform.localPosition = new Vector3(0f, 0.3f, 0f);
            Light2D trapLight = trapLightObj.AddComponent<Light2D>();
            trapLight.lightType = Light2D.LightType.Point;
            trapLight.pointLightOuterRadius = 1.8f;
            trapLight.color = new Color(1.0f, 0.55f, 0.12f, 1f);
            trapLight.intensity = 0.85f;

            FlickeringLight2D trapFlicker = trapLightObj.AddComponent<FlickeringLight2D>();
            trapFlicker.Mode = FlickeringLight2D.FlickerMode.SlowBreathing;
            trapFlicker.BaseIntensity = 0.85f;

            CollapsingFloor trapComp = trapTileObj.AddComponent<CollapsingFloor>();
            SetPrivateField(trapComp, "floorRenderer", trapRend);
            SetPrivateField(trapComp, "solidCollider", trapCol);
            SetPrivateField(trapComp, "cautionLight", trapLight);

            // =========================================================================
            // 4. ZONE 1: FLOOR HAZARD (TEACHES "FLOOR IS DANGER, FLIP UP")
            // =========================================================================
            // Blocks walking forward along floor between X: -4.8 and -1.8
            GameObject floorHazardObj = new GameObject("FloorHazard_Zone1");
            floorHazardObj.transform.SetParent(refs.puzzleRoot.transform);
            floorHazardObj.transform.position = new Vector3(-3.3f, -3.6f, 0f);

            SpriteRenderer fhRend = floorHazardObj.AddComponent<SpriteRenderer>();
            fhRend.sprite = laserSp;
            fhRend.color = new Color(1.0f, 0.15f, 0.25f, 1f);
            fhRend.sortingOrder = 7;
            fhRend.drawMode = SpriteDrawMode.Sliced;
            fhRend.size = new Vector2(3.2f, 0.8f);

            BoxCollider2D fhCol = floorHazardObj.AddComponent<BoxCollider2D>();
            fhCol.isTrigger = true;
            fhCol.size = new Vector2(3.2f, 0.8f);

            GameObject fhLightObj = new GameObject("FloorHazard_Light");
            fhLightObj.transform.SetParent(floorHazardObj.transform);
            fhLightObj.transform.localPosition = Vector3.zero;
            Light2D fhLight = fhLightObj.AddComponent<Light2D>();
            fhLight.lightType = Light2D.LightType.Point;
            fhLight.pointLightInnerRadius = 0.3f;
            fhLight.pointLightOuterRadius = 2.0f;
            fhLight.color = new Color(1.0f, 0.12f, 0.22f, 1f);
            fhLight.intensity = 1.0f;

            MovingLaserHazard fhComp = floorHazardObj.AddComponent<MovingLaserHazard>();
            fhComp.ConfigurePath(MovingLaserHazard.MotionAxis.Horizontal, new Vector3(-3.3f, -3.6f, 0f), new Vector3(-3.3f, -3.6f, 0f), 0f, 0f, startActive: true);
            SetPrivateField(fhComp, "laserRenderer", fhRend);
            SetPrivateField(fhComp, "laserLight", fhLight);
            SetPrivateField(fhComp, "activeOnStart", true);
            fhComp.SetHazardActive(true);

            // Heavy containment laser projector housings at both ends of the floor pit
            GameObject fEmitLeft = new GameObject("Emitter_FloorPit_Left");
            fEmitLeft.transform.SetParent(floorHazardObj.transform);
            fEmitLeft.transform.position = new Vector3(-4.9f, -3.6f, 0f);
            SpriteRenderer felRend = fEmitLeft.AddComponent<SpriteRenderer>();
            felRend.sprite = ch2EmitterHousingSp;
            felRend.sortingOrder = 8;

            GameObject fEmitRight = new GameObject("Emitter_FloorPit_Right");
            fEmitRight.transform.SetParent(floorHazardObj.transform);
            fEmitRight.transform.position = new Vector3(-1.7f, -3.6f, 0f);
            fEmitRight.transform.localScale = new Vector3(-1f, 1f, 1f);
            SpriteRenderer ferRend = fEmitRight.AddComponent<SpriteRenderer>();
            ferRend.sprite = ch2EmitterHousingSp;
            ferRend.sortingOrder = 8;

            // =========================================================================
            // 5. INVERTED CEILING CATWALK & ECHO SWITCH (X: -6.0 to -1.5, Y: 3.8)
            // =========================================================================
            GameObject ceilingCatwalk = new GameObject("Ceiling_Catwalk");
            ceilingCatwalk.transform.SetParent(refs.envRoot.transform);
            ceilingCatwalk.transform.position = new Vector3(-3.75f, 3.8f, 0f);
            BoxCollider2D ceilCatCol = ceilingCatwalk.AddComponent<BoxCollider2D>();
            ceilCatCol.size = new Vector2(4.8f, 0.4f);

            // Dual-orientation ceiling deck pieces: top anchors bolted to ceiling, bottom diamond tread for upside-down walk
            for (float px = -5.75f; px <= -1.75f; px += 2.0f)
            {
                GameObject seg = new GameObject($"CeilDeck_{px}");
                seg.transform.SetParent(ceilingCatwalk.transform);
                seg.transform.position = new Vector3(px, 3.8f, 0f);
                SpriteRenderer rend = seg.AddComponent<SpriteRenderer>();
                rend.sprite = ch2InvertedCeilSp;
                rend.sortingOrder = 1;
            }

            // Inverted Safety Railings hanging below ceiling catwalk
            for (float px = -5.5f; px <= -2.0f; px += 1.0f)
            {
                GameObject invRail = new GameObject($"InvRail_{px}");
                invRail.transform.SetParent(ceilingCatwalk.transform);
                invRail.transform.position = new Vector3(px, 3.65f, 0f);
                invRail.transform.localScale = new Vector3(1.0f, -0.7f, 1f);
                SpriteRenderer irRend = invRail.AddComponent<SpriteRenderer>();
                irRend.sprite = railingSp;
                irRend.sortingOrder = 3;
            }

            // EchoSwitch mounted inverted on the ceiling catwalk at (-3.8, 3.5)
            GameObject echoSwitchObj = new GameObject("EchoSwitch_Ceiling");
            echoSwitchObj.transform.SetParent(refs.puzzleRoot.transform);
            echoSwitchObj.transform.position = new Vector3(-3.8f, 3.5f, 0f);
            echoSwitchObj.transform.localScale = new Vector3(1.0f, -1.0f, 1f);

            SpriteRenderer switchRend = echoSwitchObj.AddComponent<SpriteRenderer>();
            switchRend.sprite = echoSwitchSp;
            switchRend.sortingOrder = 4;

            BoxCollider2D switchCol = echoSwitchObj.AddComponent<BoxCollider2D>();
            switchCol.isTrigger = true;
            switchCol.size = new Vector2(1.2f, 0.4f);

            GameObject switchLightObj = new GameObject("Switch_Light");
            switchLightObj.transform.SetParent(echoSwitchObj.transform);
            switchLightObj.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            Light2D switchLight = switchLightObj.AddComponent<Light2D>();
            switchLight.lightType = Light2D.LightType.Point;
            switchLight.pointLightInnerRadius = 0.2f;
            switchLight.pointLightOuterRadius = 1.8f;
            switchLight.color = new Color(0.85f, 0f, 1f, 1f);
            switchLight.intensity = 0.85f;

            SecurityOverrideSwitch echoSwitchComp = echoSwitchObj.AddComponent<SecurityOverrideSwitch>();
            SetPrivateField(echoSwitchComp, "switchRenderer", switchRend);
            SetPrivateField(echoSwitchComp, "glowLight", switchLight);
            SetPrivateField(echoSwitchComp, "depressVisual", echoSwitchObj.transform);
            SetPrivateField(echoSwitchComp, "allowPlayerActivation", false);
            SetPrivateField(echoSwitchComp, "allowEchoActivation", true);
            SetPrivateField(echoSwitchComp, "holdGraceDuration", 3.5f);
            SetPrivateField(echoSwitchComp, "latching", false);
            SetPrivateField(echoSwitchComp, "isPowered", true);

            // Heavy conduit run powering the ceiling switch from the ceiling
            GameObject ceilConduits = new GameObject("Deco_CeilingSwitchConduits");
            ceilConduits.transform.SetParent(ceilingCatwalk.transform);
            ceilConduits.transform.position = new Vector3(-3.8f, 4.3f, 0f);
            ceilConduits.transform.localScale = new Vector3(1.8f, -1.0f, 1f);
            SpriteRenderer cscRend = ceilConduits.AddComponent<SpriteRenderer>();
            cscRend.sprite = cableSp;
            cscRend.sortingOrder = -5;

            // =========================================================================
            // 6. ZONE 3: HANGING CEILING LASER HAZARD (TEACHES "UP IS DANGER, FLIP DOWN")
            // =========================================================================
            // Hangs down from ceiling at X: 1.3, covering Y: 2.3 to 5.2 (height 2.9m)
            GameObject ceilLaserObj = new GameObject("CeilingHazard_Zone3");
            ceilLaserObj.transform.SetParent(refs.puzzleRoot.transform);
            ceilLaserObj.transform.position = new Vector3(1.3f, 3.75f, 0f);

            SpriteRenderer clRend = ceilLaserObj.AddComponent<SpriteRenderer>();
            clRend.sprite = laserSp;
            clRend.color = new Color(1.0f, 0.15f, 0.25f, 1f);
            clRend.sortingOrder = 7;
            clRend.drawMode = SpriteDrawMode.Sliced;
            clRend.size = new Vector2(0.4f, 2.9f);

            BoxCollider2D clCol = ceilLaserObj.AddComponent<BoxCollider2D>();
            clCol.isTrigger = true;
            clCol.size = new Vector2(0.4f, 2.9f);

            GameObject clLightObj = new GameObject("CeilLaser_Light");
            clLightObj.transform.SetParent(ceilLaserObj.transform);
            clLightObj.transform.localPosition = Vector3.zero;
            Light2D clLight = clLightObj.AddComponent<Light2D>();
            clLight.lightType = Light2D.LightType.Point;
            clLight.pointLightInnerRadius = 0.3f;
            clLight.pointLightOuterRadius = 1.8f;
            clLight.color = new Color(1.0f, 0.12f, 0.22f, 1f);
            clLight.intensity = 1.0f;

            MovingLaserHazard clComp = ceilLaserObj.AddComponent<MovingLaserHazard>();
            clComp.ConfigurePath(MovingLaserHazard.MotionAxis.Vertical, new Vector3(1.3f, 3.6f, 0f), new Vector3(1.3f, 3.9f, 0f), 0.6f, 0f, startActive: true);
            SetPrivateField(clComp, "laserRenderer", clRend);
            SetPrivateField(clComp, "laserLight", clLight);
            SetPrivateField(clComp, "activeOnStart", true);
            clComp.SetHazardActive(true);

            // Overhead heavy emitter projector housing at Y: 5.2
            GameObject ceilEmitHousing = new GameObject("Emitter_CeilingLaserHousing");
            ceilEmitHousing.transform.SetParent(ceilLaserObj.transform);
            ceilEmitHousing.transform.position = new Vector3(1.3f, 5.25f, 0f);
            ceilEmitHousing.transform.localScale = new Vector3(1.2f, -1.2f, 1f);
            SpriteRenderer cehRend = ceilEmitHousing.AddComponent<SpriteRenderer>();
            cehRend.sprite = ch2EmitterHousingSp;
            cehRend.sortingOrder = 8;

            // =========================================================================
            // 7. MID TRANSITION CATWALK (SAFE LANDING ZONE AT X: -0.8, Y: -1.4)
            // =========================================================================
            GameObject midCatwalk = new GameObject("Mid_TransitionCatwalk");
            midCatwalk.transform.SetParent(refs.envRoot.transform);
            midCatwalk.transform.position = new Vector3(-0.8f, -1.4f, 0f);
            BoxCollider2D midCol = midCatwalk.AddComponent<BoxCollider2D>();
            midCol.size = new Vector2(1.8f, 0.35f);

            GameObject midDeck = new GameObject("Mid_CatwalkDeck");
            midDeck.transform.SetParent(midCatwalk.transform);
            midDeck.transform.position = new Vector3(-0.8f, -1.4f, 0f);
            midDeck.transform.localScale = new Vector3(0.9f, 0.5f, 1f);
            SpriteRenderer mdRend = midDeck.AddComponent<SpriteRenderer>();
            mdRend.sprite = ch2HeavyPlatSp;
            mdRend.sortingOrder = 1;

            // Warm maintenance lantern on mid catwalk edge
            GameObject midLantern = new GameObject("Lantern_MidCatwalk");
            midLantern.transform.SetParent(midCatwalk.transform);
            midLantern.transform.position = new Vector3(-0.1f, -1.1f, 0f);
            SpriteRenderer mlRend = midLantern.AddComponent<SpriteRenderer>();
            mlRend.sprite = ch2LanternSp;
            mlRend.sortingOrder = 5;

            GameObject mlLightObj = new GameObject("MidLantern_Light");
            mlLightObj.transform.SetParent(midLantern.transform);
            mlLightObj.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            Light2D mlLight = mlLightObj.AddComponent<Light2D>();
            mlLight.lightType = Light2D.LightType.Point;
            mlLight.pointLightOuterRadius = 2.4f;
            mlLight.color = new Color(1.0f, 0.72f, 0.35f, 1f);
            mlLight.intensity = 1.25f;

            // =========================================================================
            // 8. STEP CATWALK (TRANSITION UP TO EXIT CATWALK AT X: 4.2, Y: -2.4)
            // =========================================================================
            GameObject stepCatwalk = new GameObject("Step_Catwalk");
            stepCatwalk.transform.SetParent(refs.envRoot.transform);
            stepCatwalk.transform.position = new Vector3(4.2f, -2.4f, 0f);
            BoxCollider2D stepCol = stepCatwalk.AddComponent<BoxCollider2D>();
            stepCol.size = new Vector2(1.4f, 0.35f);

            GameObject stepDeck = new GameObject("Step_CatwalkDeck");
            stepDeck.transform.SetParent(stepCatwalk.transform);
            stepDeck.transform.position = new Vector3(4.2f, -2.4f, 0f);
            stepDeck.transform.localScale = new Vector3(0.7f, 0.5f, 1f);
            SpriteRenderer sdRend = stepDeck.AddComponent<SpriteRenderer>();
            sdRend.sprite = ch2HeavyPlatSp;
            sdRend.sortingOrder = 1;

            // =========================================================================
            // 9. EXIT CATWALK, SECURITY BARRIER, & ELEVATED EXIT DOOR
            // =========================================================================
            refs.exitDoor.transform.position = new Vector3(8.5f, 0.5f, 0f);

            GameObject exitCatwalk = new GameObject("Exit_Catwalk");
            exitCatwalk.transform.SetParent(refs.envRoot.transform);
            exitCatwalk.transform.position = new Vector3(8.0f, -0.8f, 0f);
            BoxCollider2D catCol = exitCatwalk.AddComponent<BoxCollider2D>();
            catCol.size = new Vector2(5.5f, 0.4f);

            for (float px = 5.5f; px <= 10.5f; px += 2.0f)
            {
                GameObject seg = new GameObject($"ExitDeckSeg_{px}");
                seg.transform.SetParent(exitCatwalk.transform);
                seg.transform.position = new Vector3(px, -0.8f, 0f);
                SpriteRenderer rend = seg.AddComponent<SpriteRenderer>();
                rend.sprite = ch2HeavyPlatSp;
                rend.sortingOrder = 1;
            }

            // Yellow safety railings along exit catwalk
            for (float rx = 5.5f; rx <= 10.5f; rx += 1.0f)
            {
                if (Mathf.Abs(rx - 6.2f) < 0.4f || Mathf.Abs(rx - 8.5f) < 0.6f) continue; // Leave gaps for barrier and door
                GameObject rail = new GameObject($"ExitRail_{rx}");
                rail.transform.SetParent(exitCatwalk.transform);
                rail.transform.position = new Vector3(rx, -0.55f, 0f);
                SpriteRenderer er = rail.AddComponent<SpriteRenderer>();
                er.sprite = railingSp;
                er.sortingOrder = 3;
            }

            // Security Barrier in front of the exit door on the catwalk at X: 6.2, Y: 0.35
            GameObject barrierObj = new GameObject("SecurityBarrier_Exit");
            barrierObj.transform.SetParent(refs.puzzleRoot.transform);
            barrierObj.transform.position = new Vector3(6.2f, 0.35f, 0f);

            SpriteRenderer barRend = barrierObj.AddComponent<SpriteRenderer>();
            barRend.sprite = laserSp;
            barRend.color = new Color(1.0f, 0.15f, 0.25f, 0.95f);
            barRend.sortingOrder = 8;
            barRend.drawMode = SpriteDrawMode.Sliced;
            barRend.size = new Vector2(0.35f, 2.3f);

            BoxCollider2D barCol = barrierObj.AddComponent<BoxCollider2D>();
            barCol.size = new Vector2(0.35f, 2.3f);

            GameObject barLightObj = new GameObject("Barrier_Light");
            barLightObj.transform.SetParent(barrierObj.transform);
            barLightObj.transform.localPosition = Vector3.zero;
            Light2D barLight = barLightObj.AddComponent<Light2D>();
            barLight.lightType = Light2D.LightType.Point;
            barLight.pointLightInnerRadius = 0.2f;
            barLight.pointLightOuterRadius = 1.6f;
            barLight.color = new Color(1.0f, 0.2f, 0.25f, 1f);
            barLight.intensity = 0.95f;

            // Barrier status sign
            GameObject signCanvasObj = new GameObject("BarrierSignCanvas");
            signCanvasObj.transform.SetParent(barrierObj.transform);
            signCanvasObj.transform.localPosition = new Vector3(0f, 1.35f, 0f);
            Canvas signCanvas = signCanvasObj.AddComponent<Canvas>();
            signCanvas.renderMode = RenderMode.WorldSpace;
            RectTransform signRect = signCanvasObj.GetComponent<RectTransform>();
            signRect.sizeDelta = new Vector2(250f, 60f);
            signCanvasObj.transform.localScale = new Vector3(0.01f, 0.01f, 1f);

            GameObject signTextObj = new GameObject("SignText");
            signTextObj.transform.SetParent(signCanvasObj.transform);
            signTextObj.transform.localPosition = Vector3.zero;
            Text signText = signTextObj.AddComponent<Text>();
            signText.font = defaultFont;
            signText.fontSize = 14;
            signText.alignment = TextAnchor.MiddleCenter;
            signText.text = "<color=#FF1E40>[GRAVITY INTERLOCK: ACTIVE]</color>\n<size=12><color=#D600FF>TEMPORAL ECHO OVERRIDE REQUIRED</color></size>";
            RectTransform textRect = signTextObj.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(250f, 60f);

            SecurityBarrier barrierComp = barrierObj.AddComponent<SecurityBarrier>();
            SetPrivateField(barrierComp, "barrierCollider", barCol);
            SetPrivateField(barrierComp, "barrierRenderer", barRend);
            SetPrivateField(barrierComp, "barrierLight", barLight);
            SetPrivateField(barrierComp, "signText", signText);

            // Connect ceiling switch to security barrier
            SetPrivateField(echoSwitchComp, "targetBarrier", barrierComp);
            echoSwitchComp.TargetBarrier = barrierComp;

            SetPrivateField(refs.hud, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.director, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.triggers, "echoSwitch", echoSwitchComp);

            // Exit Blast Door Framing & Warm Conical Bulkhead Spotlight
            GameObject exitSignObj = new GameObject("Deco_ExitSign");
            exitSignObj.transform.SetParent(refs.exitDoor.transform);
            exitSignObj.transform.localPosition = new Vector3(0f, 1.35f, 0f);
            SpriteRenderer esRend = exitSignObj.AddComponent<SpriteRenderer>();
            esRend.sprite = exitSignSp;
            esRend.sortingOrder = 5;

            GameObject exitDoorSpotObj = new GameObject("ExitDoor_Spotlight");
            exitDoorSpotObj.transform.SetParent(refs.exitDoor.transform);
            exitDoorSpotObj.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            Light2D exitDoorSpot = exitDoorSpotObj.AddComponent<Light2D>();
            exitDoorSpot.lightType = Light2D.LightType.Point;
            exitDoorSpot.pointLightOuterRadius = 3.8f;
            exitDoorSpot.color = new Color(1.0f, 0.72f, 0.35f, 1f);
            exitDoorSpot.intensity = 1.45f;

            // Warm maintenance lantern on exit platform corner
            GameObject exitLantern = new GameObject("Lantern_ExitPlatformCorner");
            exitLantern.transform.SetParent(exitCatwalk.transform);
            exitLantern.transform.position = new Vector3(10.2f, -0.45f, 0f);
            SpriteRenderer elRend = exitLantern.AddComponent<SpriteRenderer>();
            elRend.sprite = ch2LanternSp;
            elRend.sortingOrder = 5;

            GameObject elLightObj = new GameObject("ExitLantern_Light");
            elLightObj.transform.SetParent(exitLantern.transform);
            elLightObj.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            Light2D elLight = elLightObj.AddComponent<Light2D>();
            elLight.lightType = Light2D.LightType.Point;
            elLight.pointLightOuterRadius = 2.6f;
            elLight.color = new Color(1.0f, 0.72f, 0.35f, 1f);
            elLight.intensity = 1.3f;

            // =========================================================================
            // 10. CINEMATIC FOREGROUND SILHOUETTES (Sorting Order: 12)
            // =========================================================================
            GameObject fgObjLeft = new GameObject("FG_Silhouette_Left");
            fgObjLeft.transform.SetParent(ch02EnvRoot.transform);
            fgObjLeft.transform.position = new Vector3(-8.0f, -4.8f, 0f);
            fgObjLeft.transform.localScale = new Vector3(2.5f, 1.8f, 1f);
            SpriteRenderer fglRend = fgObjLeft.AddComponent<SpriteRenderer>();
            fglRend.sprite = ch2ForegroundSp;
            fglRend.sortingOrder = 12;

            GameObject fgObjRight = new GameObject("FG_Silhouette_Right");
            fgObjRight.transform.SetParent(ch02EnvRoot.transform);
            fgObjRight.transform.position = new Vector3(7.5f, -4.8f, 0f);
            fgObjRight.transform.localScale = new Vector3(-2.5f, 1.8f, 1f);
            SpriteRenderer fgrRend = fgObjRight.AddComponent<SpriteRenderer>();
            fgrRend.sprite = ch2ForegroundSp;
            fgrRend.sortingOrder = 12;

            // =========================================================================
            // 11. CHECKPOINT 4: FAKE EMERGENCY COUNTDOWN PRANK (LEVEL 02)
            // =========================================================================
            GameObject emergencyPrankObj = new GameObject("FakeEmergencyPrankController");
            emergencyPrankObj.transform.SetParent(refs.puzzleRoot.transform);
            emergencyPrankObj.AddComponent<FakeEmergencyPrank>();

            // In edit mode, disable fullscreen overlays so scene and camera views are pristine
            var introPanel = GameObject.Find("ChapterIntroPanel");
            if (introPanel != null) introPanel.SetActive(false);
            var subBanner = GameObject.Find("ArchitectSubtitleBanner");
            if (subBanner != null) subBanner.SetActive(false);

            SaveChamberScene("Level02.unity");
        }
        #endregion

        #region Level 03: The Past Is Your Enemy (Time Freeze & Moving Hazards)
        public static void ConstructLevel03Scene()
        {
            var atmo = new ChapterAtmosphereConfig
            {
                ambientColor = new Color(0.30f, 0.72f, 0.90f, 1f), // Cold cobalt/cyan laboratory lighting
                ambientIntensity = 0.70f,
                spawnLampColor = new Color(0f, 0.94f, 1f, 1f),
                spawnLampIntensity = 1.05f,
                spawnLampFlicker = FlickeringLight2D.FlickerMode.SlowBreathing,
                particleType = AtmosphericParticleEmitter.AtmosphereType.CryoStasisMist,
                architectMonitorColor = new Color(0f, 0.94f, 1f, 0.95f)
            };

            var refs = SetupBaseChamber("CONTAINMENT 03 // TEMPORAL FREEZE", 3, "Level04", false, atmo);

            Sprite floorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_tile.png");
            Sprite hazardSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/hazard_laser.png");
            Sprite plateSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/pressure_plate.png");
            Sprite cubeSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/cube_crate.png");
            Sprite terminalSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/lab_terminal.png");
            Sprite podSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/containment_pod.png");
            Sprite quarantineSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/decal_quarantine.png");
            Sprite wallSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/wall_panel.png");

            // Enable Time Freeze (F)
            refs.abilityCtrl.AllowTimeFreeze = true;

            // Environmental Dressing: Damaged Scientific Terminals
            GameObject terminal1 = new GameObject("Deco_LabTerminal_1");
            terminal1.transform.SetParent(refs.envRoot.transform);
            terminal1.transform.position = new Vector3(-7.5f, -2.5f, 0f);
            SpriteRenderer termRend1 = terminal1.AddComponent<SpriteRenderer>();
            termRend1.sprite = terminalSp;
            termRend1.sortingOrder = -5;

            GameObject terminal2 = new GameObject("Deco_LabTerminal_2");
            terminal2.transform.SetParent(refs.envRoot.transform);
            terminal2.transform.position = new Vector3(8.5f, -0.5f, 0f);
            SpriteRenderer termRend2 = terminal2.AddComponent<SpriteRenderer>();
            termRend2.sprite = terminalSp;
            termRend2.sortingOrder = -5;

            // Cryo Containment Specimen Pods
            GameObject pod1 = new GameObject("Deco_CryoPod_1");
            pod1.transform.SetParent(refs.envRoot.transform);
            pod1.transform.position = new Vector3(-4.8f, -2.0f, 0f);
            SpriteRenderer podRend1 = pod1.AddComponent<SpriteRenderer>();
            podRend1.sprite = podSp;
            podRend1.sortingOrder = -8;

            GameObject podLight1Obj = new GameObject("Pod_Glow_1");
            podLight1Obj.transform.SetParent(pod1.transform);
            podLight1Obj.transform.localPosition = Vector3.zero;
            Light2D podLight1 = podLight1Obj.AddComponent<Light2D>();
            podLight1.lightType = Light2D.LightType.Point;
            podLight1.pointLightOuterRadius = 2.2f;
            podLight1.color = new Color(0f, 0.94f, 1f, 1f);
            podLight1.intensity = 0.45f;

            FlickeringLight2D podFlicker1 = podLight1Obj.AddComponent<FlickeringLight2D>();
            podFlicker1.Mode = FlickeringLight2D.FlickerMode.SlowBreathing;
            podFlicker1.BaseIntensity = 0.45f;

            GameObject pod2 = new GameObject("Deco_CryoPod_2");
            pod2.transform.SetParent(refs.envRoot.transform);
            pod2.transform.position = new Vector3(4.5f, -2.0f, 0f);
            SpriteRenderer podRend2 = pod2.AddComponent<SpriteRenderer>();
            podRend2.sprite = podSp;
            podRend2.sortingOrder = -8;

            // Quarantine Warning Decal
            GameObject guarDecal = new GameObject("Deco_QuarantineAlert");
            guarDecal.transform.SetParent(refs.envRoot.transform);
            guarDecal.transform.position = new Vector3(0f, 2.5f, 0f);
            SpriteRenderer qdRend = guarDecal.AddComponent<SpriteRenderer>();
            qdRend.sprite = quarantineSp;
            qdRend.sortingOrder = -5;

            // High Contrast Dark Backdrop behind the Echo puzzle flight path (X: -2 to 3)
            GameObject echoContrastBackdrop = new GameObject("EchoPath_ContrastBackdrop");
            echoContrastBackdrop.transform.SetParent(refs.envRoot.transform);
            echoContrastBackdrop.transform.position = new Vector3(0.5f, -1.5f, 0f);
            echoContrastBackdrop.transform.localScale = new Vector3(5.5f, 3.5f, 1f);
            SpriteRenderer ecbRend = echoContrastBackdrop.AddComponent<SpriteRenderer>();
            ecbRend.sprite = wallSp;
            ecbRend.sortingOrder = -12;
            ecbRend.color = new Color(0.04f, 0.06f, 0.09f, 0.95f); // Deep dark backdrop ensures Echo ghost stands out!

            // Checkpoint 3: Impossible Question Forced-Death Prank (Level 03)
            GameObject questionPrankObj = new GameObject("ImpossibleQuestionPrankController");
            questionPrankObj.transform.SetParent(refs.puzzleRoot.transform);
            ImpossibleQuestionPrank questionPrank = questionPrankObj.AddComponent<ImpossibleQuestionPrank>();
            SetPrivateField(questionPrank, "questionPromptText", refs.questionPromptText);
            SetPrivateField(questionPrank, "triggerX", -5.0f);

            // Movable Cube before Laser 1 at (X: -4.2, Y: -3.4)
            GameObject cubeObj = new GameObject("MovableCube");
            cubeObj.transform.SetParent(refs.puzzleRoot.transform);
            cubeObj.transform.position = new Vector3(-4.2f, -3.4f, 0f);

            SpriteRenderer cubeRend = cubeObj.AddComponent<SpriteRenderer>();
            cubeRend.sprite = cubeSp;
            cubeRend.sortingOrder = 3;

            BoxCollider2D cubeCol = cubeObj.AddComponent<BoxCollider2D>();
            cubeCol.size = new Vector2(0.95f, 0.95f);

            Rigidbody2D cubeRb = cubeObj.AddComponent<Rigidbody2D>();
            cubeRb.mass = 2.0f;
            cubeRb.linearDamping = 1.8f;
            cubeRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            MovableCube cubeComp = cubeObj.AddComponent<MovableCube>();
            SetPrivateField(cubeComp, "spriteRenderer", cubeRend);

            // Moving Freezable Hazard Laser 1 (Blocks cube path; requires Time Freeze F)
            GameObject hazardObj1 = new GameObject("FreezableLaser_Barrier_1");
            hazardObj1.transform.SetParent(refs.puzzleRoot.transform);
            hazardObj1.transform.position = new Vector3(-1.8f, -3.5f, 0f);

            SpriteRenderer hazRend1 = hazardObj1.AddComponent<SpriteRenderer>();
            hazRend1.sprite = hazardSp;
            hazRend1.sortingOrder = 3;

            GameObject hazLight1 = new GameObject("Hazard1_Glow");
            hazLight1.transform.SetParent(hazardObj1.transform);
            hazLight1.transform.localPosition = Vector3.zero;
            Light2D hl1 = hazLight1.AddComponent<Light2D>();
            hl1.lightType = Light2D.LightType.Point;
            hl1.pointLightOuterRadius = 2.0f;
            hl1.color = new Color(1.0f, 0.15f, 0.25f, 1f);
            hl1.intensity = 0.8f;

            BoxCollider2D hazCol1 = hazardObj1.AddComponent<BoxCollider2D>();
            hazCol1.size = new Vector2(0.5f, 1.8f);
            hazCol1.isTrigger = true;

            FreezableHazard hazardComp1 = hazardObj1.AddComponent<FreezableHazard>();
            SetPrivateField(hazardComp1, "offsetA", Vector3.zero);
            SetPrivateField(hazardComp1, "offsetB", new Vector3(0f, 4.0f, 0f));
            SetPrivateField(hazardComp1, "moveSpeed", 3.2f);
            SetPrivateField(hazardComp1, "hazardRenderer", hazRend1);

            // Pressure Plate A at (X: 0.5, Y: -3.9) — Cube opens Blast Gate A
            GameObject plateObj = new GameObject("PressurePlate");
            plateObj.transform.SetParent(refs.puzzleRoot.transform);
            plateObj.transform.position = new Vector3(0.5f, -3.9f, 0f);

            SpriteRenderer plateRend = plateObj.AddComponent<SpriteRenderer>();
            plateRend.sprite = plateSp;
            plateRend.sortingOrder = 2;

            BoxCollider2D plateTrigger = plateObj.AddComponent<BoxCollider2D>();
            plateTrigger.isTrigger = true;
            plateTrigger.size = new Vector2(1.2f, 0.35f);
            plateTrigger.offset = new Vector2(0f, 0.15f);

            PressurePlate plateComp = plateObj.AddComponent<PressurePlate>();
            SetPrivateField(plateComp, "plateTopRenderer", plateRend);
            SetPrivateField(plateComp, "depressTarget", plateObj.transform);

            // Blast Gate A (Security Barrier interlocked with Pressure Plate A) at X: 1.6, Y: -2.7
            GameObject gateAObj = new GameObject("SecurityBarrier_GateA");
            gateAObj.transform.SetParent(refs.puzzleRoot.transform);
            gateAObj.transform.position = new Vector3(1.6f, -2.7f, 0f);

            SpriteRenderer gaRend = gateAObj.AddComponent<SpriteRenderer>();
            gaRend.sprite = hazardSp;
            gaRend.color = new Color(1.0f, 0.15f, 0.25f, 0.95f);
            gaRend.sortingOrder = 8;
            gaRend.drawMode = SpriteDrawMode.Sliced;
            gaRend.size = new Vector2(0.35f, 2.4f);

            BoxCollider2D gaCol = gateAObj.AddComponent<BoxCollider2D>();
            gaCol.size = new Vector2(0.35f, 2.4f);

            GameObject gaLightObj = new GameObject("GateA_Light");
            gaLightObj.transform.SetParent(gateAObj.transform);
            gaLightObj.transform.localPosition = Vector3.zero;
            Light2D gaLight = gaLightObj.AddComponent<Light2D>();
            gaLight.lightType = Light2D.LightType.Point;
            gaLight.pointLightOuterRadius = 2.2f;
            gaLight.color = new Color(1.0f, 0.2f, 0.25f, 1f);
            gaLight.intensity = 1.2f;

            SecurityBarrier gateAComp = gateAObj.AddComponent<SecurityBarrier>();
            SetPrivateField(gateAComp, "barrierCollider", gaCol);
            SetPrivateField(gateAComp, "barrierRenderer", gaRend);
            SetPrivateField(gateAComp, "barrierLight", gaLight);
            plateComp.TargetBarrier = gateAComp;

            // Catwalk Gantry beyond Gate A at X: 2.0 to 4.2, Y: -1.2
            GameObject gantryCatwalk = new GameObject("Gantry_Catwalk");
            gantryCatwalk.transform.SetParent(refs.envRoot.transform);
            gantryCatwalk.transform.position = new Vector3(3.1f, -1.2f, 0f);
            BoxCollider2D gantryCol = gantryCatwalk.AddComponent<BoxCollider2D>();
            gantryCol.size = new Vector2(2.4f, 0.35f);

            for (float px = 2.0f; px <= 4.2f; px += 0.7f)
            {
                GameObject seg = new GameObject($"GantrySeg_{px}");
                seg.transform.SetParent(gantryCatwalk.transform);
                seg.transform.position = new Vector3(px, -1.2f, 0f);
                seg.transform.localScale = new Vector3(0.7f, 0.35f, 1f);
                SpriteRenderer rend = seg.AddComponent<SpriteRenderer>();
                rend.sprite = floorSp;
                rend.sortingOrder = 1;
            }

            // Step up to gantry at X: 1.8, Y: -2.6
            GameObject stepGantry = new GameObject("Step_Gantry");
            stepGantry.transform.SetParent(refs.envRoot.transform);
            stepGantry.transform.position = new Vector3(1.8f, -2.6f, 0f);
            BoxCollider2D sgCol = stepGantry.AddComponent<BoxCollider2D>();
            sgCol.size = new Vector2(0.8f, 0.35f);

            // EchoSwitch Terminal on Gantry at (X: 2.8, Y: -0.85)
            GameObject echoSwitchObj = new GameObject("EchoSwitch_Terminal");
            echoSwitchObj.transform.SetParent(refs.puzzleRoot.transform);
            echoSwitchObj.transform.position = new Vector3(2.8f, -0.85f, 0f);

            SpriteRenderer switchRend = echoSwitchObj.AddComponent<SpriteRenderer>();
            switchRend.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/echo_switch.png");
            switchRend.sortingOrder = 2;

            BoxCollider2D switchCol = echoSwitchObj.AddComponent<BoxCollider2D>();
            switchCol.isTrigger = true;
            switchCol.size = new Vector2(1.2f, 0.4f);

            GameObject switchLightObj = new GameObject("Switch_Light");
            switchLightObj.transform.SetParent(echoSwitchObj.transform);
            switchLightObj.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            Light2D switchLight = switchLightObj.AddComponent<Light2D>();
            switchLight.lightType = Light2D.LightType.Point;
            switchLight.pointLightOuterRadius = 3.0f;
            switchLight.color = new Color(0.85f, 0f, 1f, 1f);
            switchLight.intensity = 0.7f;

            SecurityOverrideSwitch echoSwitchComp = echoSwitchObj.AddComponent<SecurityOverrideSwitch>();
            SetPrivateField(echoSwitchComp, "switchRenderer", switchRend);
            SetPrivateField(echoSwitchComp, "glowLight", switchLight);
            SetPrivateField(echoSwitchComp, "depressVisual", echoSwitchObj.transform);
            SetPrivateField(echoSwitchComp, "allowPlayerActivation", false);
            SetPrivateField(echoSwitchComp, "allowEchoActivation", true);
            SetPrivateField(echoSwitchComp, "holdGraceDuration", 3.0f);
            SetPrivateField(echoSwitchComp, "latching", false);
            SetPrivateField(echoSwitchComp, "isPowered", true);

            // Moving Freezable Hazard Laser 2 (Across lower floor corridor at X: 4.8)
            GameObject hazardObj2 = new GameObject("FreezableLaser_Barrier_2");
            hazardObj2.transform.SetParent(refs.puzzleRoot.transform);
            hazardObj2.transform.position = new Vector3(4.8f, 0.0f, 0f);

            SpriteRenderer hazRend2 = hazardObj2.AddComponent<SpriteRenderer>();
            hazRend2.sprite = hazardSp;
            hazRend2.sortingOrder = 3;

            GameObject hazLight2 = new GameObject("Hazard2_Glow");
            hazLight2.transform.SetParent(hazardObj2.transform);
            hazLight2.transform.localPosition = Vector3.zero;
            Light2D hl2 = hazLight2.AddComponent<Light2D>();
            hl2.lightType = Light2D.LightType.Point;
            hl2.pointLightOuterRadius = 2.0f;
            hl2.color = new Color(1.0f, 0.15f, 0.25f, 1f);
            hl2.intensity = 0.8f;

            BoxCollider2D hazCol2 = hazardObj2.AddComponent<BoxCollider2D>();
            hazCol2.size = new Vector2(0.5f, 1.8f);
            hazCol2.isTrigger = true;

            FreezableHazard hazardComp2 = hazardObj2.AddComponent<FreezableHazard>();
            SetPrivateField(hazardComp2, "offsetA", Vector3.zero);
            SetPrivateField(hazardComp2, "offsetB", new Vector3(0f, -3.5f, 0f));
            SetPrivateField(hazardComp2, "moveSpeed", 3.5f);
            SetPrivateField(hazardComp2, "hazardRenderer", hazRend2);

            // Security Barrier Exit at X: 6.0, Y: -2.7 (Controlled by EchoSwitch_Terminal)
            GameObject exitBarrierObj = new GameObject("SecurityBarrier_Exit");
            exitBarrierObj.transform.SetParent(refs.puzzleRoot.transform);
            exitBarrierObj.transform.position = new Vector3(6.0f, -2.7f, 0f);

            SpriteRenderer ebRend = exitBarrierObj.AddComponent<SpriteRenderer>();
            ebRend.sprite = hazardSp;
            ebRend.color = new Color(1.0f, 0.15f, 0.25f, 0.95f);
            ebRend.sortingOrder = 8;
            ebRend.drawMode = SpriteDrawMode.Sliced;
            ebRend.size = new Vector2(0.35f, 2.4f);

            BoxCollider2D ebCol = exitBarrierObj.AddComponent<BoxCollider2D>();
            ebCol.size = new Vector2(0.35f, 2.4f);

            GameObject ebLightObj = new GameObject("ExitBarrier_Light");
            ebLightObj.transform.SetParent(exitBarrierObj.transform);
            ebLightObj.transform.localPosition = Vector3.zero;
            Light2D ebLight = ebLightObj.AddComponent<Light2D>();
            ebLight.lightType = Light2D.LightType.Point;
            ebLight.pointLightOuterRadius = 2.4f;
            ebLight.color = new Color(1.0f, 0.2f, 0.25f, 1f);
            ebLight.intensity = 1.3f;

            SecurityBarrier exitBarrierComp = exitBarrierObj.AddComponent<SecurityBarrier>();
            SetPrivateField(exitBarrierComp, "barrierCollider", ebCol);
            SetPrivateField(exitBarrierComp, "barrierRenderer", ebRend);
            SetPrivateField(exitBarrierComp, "barrierLight", ebLight);

            // Connect echo switch to exit security barrier
            SetPrivateField(echoSwitchComp, "targetBarrier", exitBarrierComp);
            echoSwitchComp.TargetBarrier = exitBarrierComp;

            // Elevated Exit Doorway at (X: 8.5, Y: -2.5)
            refs.exitDoor.transform.position = new Vector3(8.5f, -2.5f, 0f);

            SetPrivateField(refs.hud, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.director, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.triggers, "echoSwitch", echoSwitchComp);

            SetPrivateField(refs.hud, "pressurePlate", plateComp);
            SetPrivateField(refs.director, "pressurePlate", plateComp);
            SetPrivateField(refs.triggers, "pressurePlate", plateComp);

            SaveChamberScene("Level03.unity");
        }
        #endregion

        #region Level 04: Break The System (Dimension Shift & Grand Finale)
        public static void ConstructLevel04Scene()
        {
            var atmo = new ChapterAtmosphereConfig
            {
                ambientColor = new Color(0.92f, 0.32f, 0.36f, 1f), // Deep emergency crimson/red
                ambientIntensity = 0.76f,
                spawnLampColor = new Color(1.0f, 0.22f, 0.18f, 1f),
                spawnLampIntensity = 1.2f,
                spawnLampFlicker = FlickeringLight2D.FlickerMode.EmergencyStrobe,
                particleType = AtmosphericParticleEmitter.AtmosphereType.EmergencyEmbers,
                architectMonitorColor = new Color(1.0f, 0.25f, 0.20f, 0.95f)
            };

            var refs = SetupBaseChamber("CONTAINMENT 04 // REALITY BREACH", 4, "Level05", false, atmo);

            Sprite alphaSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/dim_alpha_platform.png");
            Sprite betaSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/dim_beta_platform.png");
            Sprite echoSwitchSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/echo_switch.png");
            Sprite skylineSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/distant_skyline_vent.png");
            Sprite beaconSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/warning_beacon.png");
            Sprite quarantineSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/decal_quarantine.png");
            Sprite pillarSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/bg_pillar.png");

            // Unlock ALL powers for the finale: Gravity (G), Time Freeze (F), Dimension Swap (Q)
            refs.abilityCtrl.AllowGravity = true;
            refs.abilityCtrl.AllowTimeFreeze = true;
            refs.abilityCtrl.AllowDimensionSwap = true;

            // Environmental Storytelling: Distant Skyline Observation Slit (The Last City)
            GameObject skylineObj = new GameObject("Deco_DistantSkyline_Vent");
            skylineObj.transform.SetParent(refs.envRoot.transform);
            skylineObj.transform.position = new Vector3(0f, 2.2f, 0f);
            skylineObj.transform.localScale = new Vector3(1.8f, 1.4f, 1f);
            SpriteRenderer skyRend = skylineObj.AddComponent<SpriteRenderer>();
            skyRend.sprite = skylineSp;
            skyRend.sortingOrder = -25; // Deepest background layer

            // Heavy Brutalist Bulkhead Columns
            float[] colXs = new float[] { -8.5f, -2.5f, 3.5f };
            foreach (var cx in colXs)
            {
                GameObject col = new GameObject($"Deco_BulkheadCol_{cx}");
                col.transform.SetParent(refs.envRoot.transform);
                col.transform.position = new Vector3(cx, 1.0f, 0f);
                col.transform.localScale = new Vector3(1.6f, 2.2f, 1f);
                SpriteRenderer colR = col.AddComponent<SpriteRenderer>();
                colR.sprite = pillarSp;
                colR.sortingOrder = -15;
                colR.color = new Color(0.18f, 0.12f, 0.14f, 0.85f);
            }

            // Stenciled Quarantine Warning Decals
            GameObject qd1 = new GameObject("Deco_Quarantine_1");
            qd1.transform.SetParent(refs.envRoot.transform);
            qd1.transform.position = new Vector3(-6.0f, -1.0f, 0f);
            SpriteRenderer qdR1 = qd1.AddComponent<SpriteRenderer>();
            qdR1.sprite = quarantineSp;
            qdR1.sortingOrder = -5;

            GameObject qd2 = new GameObject("Deco_Quarantine_2");
            qd2.transform.SetParent(refs.envRoot.transform);
            qd2.transform.position = new Vector3(5.0f, -0.5f, 0f);
            SpriteRenderer qdR2 = qd2.AddComponent<SpriteRenderer>();
            qdR2.sprite = quarantineSp;
            qdR2.sortingOrder = -5;

            // Flashing Emergency Wall Alarm Beacons
            GameObject leftBeacon = new GameObject("Deco_EmergencyBeacon_L");
            leftBeacon.transform.SetParent(refs.envRoot.transform);
            leftBeacon.transform.position = new Vector3(-9.5f, 1.0f, 0f);
            SpriteRenderer lbRend = leftBeacon.AddComponent<SpriteRenderer>();
            lbRend.sprite = beaconSp;
            lbRend.sortingOrder = 5;

            GameObject lbLightObj = new GameObject("Beacon_Light_L");
            lbLightObj.transform.SetParent(leftBeacon.transform);
            lbLightObj.transform.localPosition = Vector3.zero;
            Light2D lbLight = lbLightObj.AddComponent<Light2D>();
            lbLight.lightType = Light2D.LightType.Point;
            lbLight.pointLightOuterRadius = 3.5f;
            lbLight.color = new Color(1.0f, 0.12f, 0.15f, 1f);
            lbLight.intensity = 0.8f;

            FlickeringLight2D lbFlicker = lbLightObj.AddComponent<FlickeringLight2D>();
            lbFlicker.Mode = FlickeringLight2D.FlickerMode.EmergencyStrobe;
            lbFlicker.BaseIntensity = 0.8f;

            GameObject rightBeacon = new GameObject("Deco_EmergencyBeacon_R");
            rightBeacon.transform.SetParent(refs.envRoot.transform);
            rightBeacon.transform.position = new Vector3(9.5f, 1.0f, 0f);
            SpriteRenderer rbRend = rightBeacon.AddComponent<SpriteRenderer>();
            rbRend.sprite = beaconSp;
            rbRend.sortingOrder = 5;

            GameObject rbLightObj = new GameObject("Beacon_Light_R");
            rbLightObj.transform.SetParent(rightBeacon.transform);
            rbLightObj.transform.localPosition = Vector3.zero;
            Light2D rbLight = rbLightObj.AddComponent<Light2D>();
            rbLight.lightType = Light2D.LightType.Point;
            rbLight.pointLightOuterRadius = 3.5f;
            rbLight.color = new Color(1.0f, 0.12f, 0.15f, 1f);
            rbLight.intensity = 0.8f;

            FlickeringLight2D rbFlicker = rbLightObj.AddComponent<FlickeringLight2D>();
            rbFlicker.Mode = FlickeringLight2D.FlickerMode.EmergencyStrobe;
            rbFlicker.BaseIntensity = 0.8f;

            // Checkpoint 4: Fake Checkpoint Dialogue Prank (Level 04)
            GameObject fakeCheckpointObj = new GameObject("FakeCheckpointPrankController");
            fakeCheckpointObj.transform.SetParent(refs.puzzleRoot.transform);
            FakeCheckpointPrank fakeCheckpoint = fakeCheckpointObj.AddComponent<FakeCheckpointPrank>();
            SetPrivateField(fakeCheckpoint, "triggerX", 0.0f);

            // Floor Hazard across the chasm pit at X: -4.5 to 3.5, Y: -4.0
            Sprite laserSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/hazard_laser.png");
            GameObject floorChasmHazard = new GameObject("FloorChasm_Hazard");
            floorChasmHazard.transform.SetParent(refs.puzzleRoot.transform);
            floorChasmHazard.transform.position = new Vector3(-0.5f, -3.8f, 0f);

            SpriteRenderer fchRend = floorChasmHazard.AddComponent<SpriteRenderer>();
            fchRend.sprite = laserSp;
            fchRend.color = new Color(1.0f, 0.15f, 0.25f, 1f);
            fchRend.sortingOrder = 7;
            fchRend.drawMode = SpriteDrawMode.Sliced;
            fchRend.size = new Vector2(8.0f, 0.7f);

            BoxCollider2D fchCol = floorChasmHazard.AddComponent<BoxCollider2D>();
            fchCol.isTrigger = true;
            fchCol.size = new Vector2(8.0f, 0.7f);

            MovingLaserHazard fchComp = floorChasmHazard.AddComponent<MovingLaserHazard>();
            fchComp.ConfigurePath(MovingLaserHazard.MotionAxis.Horizontal, new Vector3(-0.5f, -3.8f, 0f), new Vector3(-0.5f, -3.8f, 0f), 0f, 0f, startActive: true);
            SetPrivateField(fchComp, "laserRenderer", fchRend);
            SetPrivateField(fchComp, "activeOnStart", true);
            fchComp.SetHazardActive(true);

            // 1. DIMENSION ALPHA: Inverted Ceiling Observation Catwalk at X: -3.5, Y: 3.8
            GameObject alphaCeilingCatwalk = CreateDimensionPlatform(refs.puzzleRoot.transform, "DimAlpha_CeilingCatwalk", 
                new Vector3(-3.5f, 3.8f, 0f), new Vector2(4.2f, 0.4f), alphaSp, DimensionType.Alpha);

            // Step platform to ceiling in Alpha at X: -6.0, Y: -1.5
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimAlpha_Step_1", 
                new Vector3(-6.0f, -1.5f, 0f), new Vector2(2.0f, 0.4f), alphaSp, DimensionType.Alpha);

            // EchoSwitch mounted inverted on Alpha Ceiling Catwalk at (-3.5, 3.5)
            GameObject echoSwitchObj = new GameObject("EchoSwitch_Revelation");
            echoSwitchObj.transform.SetParent(alphaCeilingCatwalk.transform);
            echoSwitchObj.transform.localPosition = new Vector3(0f, -0.3f, 0f);
            echoSwitchObj.transform.localRotation = Quaternion.Euler(0, 0, 180f);

            SpriteRenderer switchRend = echoSwitchObj.AddComponent<SpriteRenderer>();
            switchRend.sprite = echoSwitchSp;
            switchRend.sortingOrder = 4;

            BoxCollider2D switchCol = echoSwitchObj.AddComponent<BoxCollider2D>();
            switchCol.isTrigger = true;
            switchCol.size = new Vector2(1.2f, 0.4f);

            GameObject switchLightObj = new GameObject("Switch_Light");
            switchLightObj.transform.SetParent(echoSwitchObj.transform);
            switchLightObj.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            Light2D switchLight = switchLightObj.AddComponent<Light2D>();
            switchLight.lightType = Light2D.LightType.Point;
            switchLight.pointLightOuterRadius = 3.0f;
            switchLight.color = new Color(0.85f, 0f, 1f, 1f);
            switchLight.intensity = 0.7f;

            SecurityOverrideSwitch echoSwitchComp = echoSwitchObj.AddComponent<SecurityOverrideSwitch>();
            SetPrivateField(echoSwitchComp, "switchRenderer", switchRend);
            SetPrivateField(echoSwitchComp, "glowLight", switchLight);
            SetPrivateField(echoSwitchComp, "depressVisual", echoSwitchObj.transform);
            SetPrivateField(echoSwitchComp, "allowPlayerActivation", false);
            SetPrivateField(echoSwitchComp, "allowEchoActivation", true);
            SetPrivateField(echoSwitchComp, "holdGraceDuration", 4.0f);
            SetPrivateField(echoSwitchComp, "latching", false);
            SetPrivateField(echoSwitchComp, "isPowered", true);

            // Revelation Schematic Display on Ceiling bulkheads (Visible in Alpha)
            GameObject schematicObj = new GameObject("Deco_AlphaSchematic");
            schematicObj.transform.SetParent(refs.envRoot.transform);
            schematicObj.transform.position = new Vector3(-1.0f, 3.2f, 0f);
            SpriteRenderer schemRend = schematicObj.AddComponent<SpriteRenderer>();
            schemRend.sprite = quarantineSp;
            schemRend.color = new Color(0f, 0.94f, 1f, 0.75f);
            schemRend.sortingOrder = -5;

            // 2. DIMENSION BETA: Lower Chasm Crossing & Ascent Platforms
            // Dimension Beta Platform Bridge 1 (X: -2.0, Y: -1.5)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimBeta_Platform_Bridge1", 
                new Vector3(-2.0f, -1.5f, 0f), new Vector2(2.5f, 0.4f), betaSp, DimensionType.Beta);

            // Dimension Beta Platform Bridge 2 (X: 0.5, Y: -0.8)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimBeta_Platform_Bridge2", 
                new Vector3(0.5f, -0.8f, 0f), new Vector2(2.5f, 0.4f), betaSp, DimensionType.Beta);

            // Dimension Beta Ascent Platform 1 (X: 2.8, Y: 0.5)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimBeta_Platform_Ascent1", 
                new Vector3(2.8f, 0.5f, 0f), new Vector2(2.0f, 0.4f), betaSp, DimensionType.Beta);

            // Dimension Beta Ascent Platform 2 (X: 4.6, Y: 1.8)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimBeta_Platform_Ascent2", 
                new Vector3(4.6f, 1.8f, 0f), new Vector2(2.2f, 0.4f), betaSp, DimensionType.Beta);

            // 3. DIMENSION ALPHA: Final Exit Catwalk at X: 6.6, Y: 2.5
            // Requires mid-air switch from Beta jump into Alpha phase!
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimAlpha_Catwalk_Exit", 
                new Vector3(6.6f, 2.5f, 0f), new Vector2(2.8f, 0.4f), alphaSp, DimensionType.Alpha);

            // Security Barrier Exit at X: 7.0, Y: 3.4 (Controlled by EchoSwitch_Revelation)
            GameObject exitBarrierObj = new GameObject("SecurityBarrier_Exit");
            exitBarrierObj.transform.SetParent(refs.puzzleRoot.transform);
            exitBarrierObj.transform.position = new Vector3(7.0f, 3.4f, 0f);

            SpriteRenderer ebRend = exitBarrierObj.AddComponent<SpriteRenderer>();
            ebRend.sprite = laserSp;
            ebRend.color = new Color(1.0f, 0.15f, 0.25f, 0.95f);
            ebRend.sortingOrder = 8;
            ebRend.drawMode = SpriteDrawMode.Sliced;
            ebRend.size = new Vector2(0.35f, 2.2f);

            BoxCollider2D ebCol = exitBarrierObj.AddComponent<BoxCollider2D>();
            ebCol.size = new Vector2(0.35f, 2.2f);

            GameObject ebLightObj = new GameObject("ExitBarrier_Light");
            ebLightObj.transform.SetParent(exitBarrierObj.transform);
            ebLightObj.transform.localPosition = Vector3.zero;
            Light2D ebLight = ebLightObj.AddComponent<Light2D>();
            ebLight.lightType = Light2D.LightType.Point;
            ebLight.pointLightOuterRadius = 2.4f;
            ebLight.color = new Color(1.0f, 0.2f, 0.25f, 1f);
            ebLight.intensity = 1.3f;

            SecurityBarrier exitBarrierComp = exitBarrierObj.AddComponent<SecurityBarrier>();
            SetPrivateField(exitBarrierComp, "barrierCollider", ebCol);
            SetPrivateField(exitBarrierComp, "barrierRenderer", ebRend);
            SetPrivateField(exitBarrierComp, "barrierLight", ebLight);

            // Connect echo switch to exit security barrier
            SetPrivateField(echoSwitchComp, "targetBarrier", exitBarrierComp);
            echoSwitchComp.TargetBarrier = exitBarrierComp;

            // Final Breach Exit Door mounted high on the right bulkhead at (X: 8.5, Y: 3.5)
            refs.exitDoor.transform.position = new Vector3(8.5f, 3.5f, 0f);

            SetPrivateField(refs.hud, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.director, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.triggers, "echoSwitch", echoSwitchComp);

            SaveChamberScene("Level04.unity");
        }

        public static void ConstructLevel05Scene()
        {
            var atmo = new ChapterAtmosphereConfig
            {
                ambientColor = new Color(0.12f, 0.05f, 0.18f, 1f), // Cold deep surveillance violet
                ambientIntensity = 0.74f,
                spawnLampColor = new Color(0.75f, 0.25f, 1.0f, 1f),
                spawnLampIntensity = 1.1f,
                spawnLampFlicker = FlickeringLight2D.FlickerMode.SurveillanceGlow,
                particleType = AtmosphericParticleEmitter.AtmosphereType.SurveillanceMotes,
                architectMonitorColor = new Color(0.78f, 0.22f, 1.0f, 0.95f)
            };

            var refs = SetupBaseChamber("CONTAINMENT 05 // PARANOIA", 5, "Level06", false, atmo);

            Sprite floorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_tile.png");
            Sprite wallSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/wall_panel.png");
            Sprite beamSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ceiling_beam.png");
            Sprite pipeSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/pipe_conduit.png");
            Sprite alphaSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/dim_alpha_platform.png");
            Sprite betaSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/dim_beta_platform.png");
            Sprite echoSwitchSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/echo_switch.png");
            Sprite beaconSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/warning_beacon.png");
            Sprite stripesSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/hazard_stripes.png");
            Sprite cableSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/cable_bundle.png");
            Sprite terminalSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/lab_terminal.png");
            Sprite pillarSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/bg_pillar.png");

            Sprite hazardLaserSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/hazard_laser.png");

            // Powers: Gravity (G), Dimension Swap (Q), Echo (H/C)
            refs.abilityCtrl.AllowGravity = true;
            refs.abilityCtrl.AllowDimensionSwap = true;
            refs.abilityCtrl.AllowTimeFreeze = false;

            // Environmental Dressing: Surveillance Monitors & Cold Violet Lighting
            float[] colXs = new float[] { -7.0f, -1.0f, 5.0f };
            foreach (var cx in colXs)
            {
                GameObject col = new GameObject($"Deco_SurveillanceCol_{cx}");
                col.transform.SetParent(refs.envRoot.transform);
                col.transform.position = new Vector3(cx, 0.5f, 0f);
                col.transform.localScale = new Vector3(1.5f, 2.0f, 1f);
                SpriteRenderer colR = col.AddComponent<SpriteRenderer>();
                colR.sprite = pillarSp;
                colR.sortingOrder = -15;
                colR.color = new Color(0.12f, 0.08f, 0.16f, 0.9f);
            }

            // Violet Surveillance Terminal
            GameObject termObj = new GameObject("Deco_SurveillanceTerminal");
            termObj.transform.SetParent(refs.envRoot.transform);
            termObj.transform.position = new Vector3(-6.5f, -2.5f, 0f);
            SpriteRenderer termR = termObj.AddComponent<SpriteRenderer>();
            termR.sprite = terminalSp;
            termR.sortingOrder = -5;
            termR.color = new Color(0.8f, 0.4f, 1.0f, 1f);

            // True Route Visual Clue: Intact Neon Conduits leading UP from spawn bulkhead
            GameObject verticalConduit = new GameObject("Deco_TruePath_Conduit");
            verticalConduit.transform.SetParent(refs.envRoot.transform);
            verticalConduit.transform.position = new Vector3(-7.5f, 0.5f, 0f);
            verticalConduit.transform.rotation = Quaternion.Euler(0, 0, 90f);
            verticalConduit.transform.localScale = new Vector3(7.5f, 1.2f, 1f);
            SpriteRenderer vcRend = verticalConduit.AddComponent<SpriteRenderer>();
            vcRend.sprite = pipeSp;
            vcRend.sortingOrder = -6;
            vcRend.color = new Color(0.2f, 0.85f, 1.0f, 0.95f);

            GameObject vLightObj = new GameObject("TruePath_Conduit_Light");
            vLightObj.transform.SetParent(verticalConduit.transform);
            vLightObj.transform.localPosition = Vector3.zero;
            Light2D vLight = vLightObj.AddComponent<Light2D>();
            vLight.lightType = Light2D.LightType.Point;
            vLight.pointLightOuterRadius = 3.5f;
            vLight.color = new Color(0.1f, 0.85f, 1.0f, 1f);
            vLight.intensity = 0.8f;

            // Ceiling Walkway 1 (Y = 3.8)
            GameObject ceilPlat1 = new GameObject("Ceiling_Walkway_1");
            ceilPlat1.transform.SetParent(refs.puzzleRoot.transform);
            ceilPlat1.transform.position = new Vector3(-2.5f, 3.8f, 0f);
            SpriteRenderer cpRend1 = ceilPlat1.AddComponent<SpriteRenderer>();
            cpRend1.sprite = beamSp;
            cpRend1.drawMode = SpriteDrawMode.Tiled;
            cpRend1.size = new Vector2(6f, 0.6f);
            cpRend1.sortingOrder = 2;
            BoxCollider2D cpCol1 = ceilPlat1.AddComponent<BoxCollider2D>();
            cpCol1.size = new Vector2(6f, 0.6f);

            // Intact Cyan Conduit along ceiling walkway
            GameObject ceilConduit = new GameObject("Deco_Ceiling_Conduit");
            ceilConduit.transform.SetParent(refs.envRoot.transform);
            ceilConduit.transform.position = new Vector3(-2.5f, 4.15f, 0f);
            ceilConduit.transform.localScale = new Vector3(6.0f, 1.0f, 1f);
            SpriteRenderer ccRend = ceilConduit.AddComponent<SpriteRenderer>();
            ccRend.sprite = pipeSp;
            ccRend.sortingOrder = -6;
            ccRend.color = new Color(0.2f, 0.85f, 1.0f, 0.95f);

            // Echo-only Security Switch inverted on the ceiling walkway at (X: -2.0, Y: 3.4)
            GameObject echoSwitchObj = new GameObject("EchoSwitch_Ceiling");
            echoSwitchObj.transform.SetParent(ceilPlat1.transform);
            echoSwitchObj.transform.localPosition = new Vector3(0.5f, -0.35f, 0f);
            echoSwitchObj.transform.localRotation = Quaternion.Euler(0, 0, 180f); // Inverted on ceiling

            SpriteRenderer switchRend = echoSwitchObj.AddComponent<SpriteRenderer>();
            switchRend.sprite = echoSwitchSp;
            switchRend.sortingOrder = 4;

            BoxCollider2D switchCol = echoSwitchObj.AddComponent<BoxCollider2D>();
            switchCol.isTrigger = true;
            switchCol.size = new Vector2(1.2f, 0.4f);

            GameObject switchLightObj = new GameObject("EchoSwitch_Light");
            switchLightObj.transform.SetParent(echoSwitchObj.transform);
            switchLightObj.transform.localPosition = Vector3.zero;
            Light2D switchLight = switchLightObj.AddComponent<Light2D>();
            switchLight.lightType = Light2D.LightType.Point;
            switchLight.pointLightOuterRadius = 2.2f;
            switchLight.color = new Color(0.85f, 0f, 1f, 1f);
            switchLight.intensity = 1.0f;

            SecurityOverrideSwitch echoSwitchComp = echoSwitchObj.AddComponent<SecurityOverrideSwitch>();
            SetPrivateField(echoSwitchComp, "switchRenderer", switchRend);
            SetPrivateField(echoSwitchComp, "glowLight", switchLight);
            SetPrivateField(echoSwitchComp, "depressVisual", echoSwitchObj.transform);
            SetPrivateField(echoSwitchComp, "allowPlayerActivation", false);
            SetPrivateField(echoSwitchComp, "allowEchoActivation", true);
            SetPrivateField(echoSwitchComp, "holdGraceDuration", 4.5f);
            SetPrivateField(echoSwitchComp, "latching", false);
            SetPrivateField(echoSwitchComp, "isPowered", true);

            // Mid-span Platform: Dimension Beta Platform 1 (X: 1.2, Y: 3.0) with intact conduit
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimBeta_Ceiling_1",
                new Vector3(1.2f, 3.0f, 0f), new Vector2(2.5f, 0.4f), betaSp, DimensionType.Beta);

            GameObject midConduit = new GameObject("Deco_MidConduit_Beta");
            midConduit.transform.SetParent(refs.envRoot.transform);
            midConduit.transform.position = new Vector3(1.2f, 3.3f, 0f);
            midConduit.transform.localScale = new Vector3(2.5f, 0.8f, 1f);
            SpriteRenderer mcRend = midConduit.AddComponent<SpriteRenderer>();
            mcRend.sprite = pipeSp;
            mcRend.sortingOrder = -6;
            mcRend.color = new Color(0.2f, 0.85f, 1.0f, 0.95f);

            // SECTION 2: Final Test of the Discovered Rule (Conflicting choices)
            // Choice A: Dimension Alpha Platform (Deceptive route: severed cables & sweeping laser)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimAlpha_TrapPlatform",
                new Vector3(4.5f, 1.4f, 0f), new Vector2(3.0f, 0.4f), alphaSp, DimensionType.Alpha);

            GameObject frayedAlpha = new GameObject("Deco_FrayedCables_Alpha");
            frayedAlpha.transform.SetParent(refs.envRoot.transform);
            frayedAlpha.transform.position = new Vector3(4.5f, 1.0f, 0f);
            SpriteRenderer faRend = frayedAlpha.AddComponent<SpriteRenderer>();
            faRend.sprite = cableSp;
            faRend.sortingOrder = 1;
            faRend.color = new Color(0.9f, 0.2f, 0.1f, 0.9f);

            // Sweeping laser hazard across the deceptive Alpha platform
            GameObject alphaLaserObj = new GameObject("MovingLaser_AlphaTrap");
            alphaLaserObj.transform.SetParent(refs.puzzleRoot.transform);
            alphaLaserObj.transform.position = new Vector3(3.5f, 1.7f, 0f);
            SpriteRenderer alRend = alphaLaserObj.AddComponent<SpriteRenderer>();
            alRend.sprite = hazardLaserSp;
            alRend.color = new Color(1.0f, 0.1f, 0.2f, 0.9f);
            alRend.sortingOrder = 6;
            alRend.drawMode = SpriteDrawMode.Sliced;
            alRend.size = new Vector2(0.25f, 0.8f);
            BoxCollider2D alCol = alphaLaserObj.AddComponent<BoxCollider2D>();
            alCol.size = new Vector2(0.25f, 0.8f);
            alCol.isTrigger = true;
            MovingLaserHazard alHazard = alphaLaserObj.AddComponent<MovingLaserHazard>();
            alHazard.ConfigurePath(MovingLaserHazard.MotionAxis.Horizontal, new Vector3(3.2f, 1.7f, 0f), new Vector3(5.5f, 1.7f, 0f), 2.2f, 0.5f, startActive: true);
            SetPrivateField(alHazard, "laserRenderer", alRend);
            SetPrivateField(alHazard, "activeOnStart", true);
            alHazard.SetHazardActive(true);

            // Choice B: Dimension Beta Platform (Truthful route: intact cyan conduit into exit)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimBeta_TrueBridge",
                new Vector3(4.8f, 3.2f, 0f), new Vector2(3.5f, 0.4f), betaSp, DimensionType.Beta);

            GameObject betaConduit = new GameObject("Deco_TrueConduit_Beta");
            betaConduit.transform.SetParent(refs.envRoot.transform);
            betaConduit.transform.position = new Vector3(4.8f, 3.5f, 0f);
            betaConduit.transform.localScale = new Vector3(3.5f, 0.8f, 1f);
            SpriteRenderer bc2Rend = betaConduit.AddComponent<SpriteRenderer>();
            bc2Rend.sprite = pipeSp;
            bc2Rend.sortingOrder = -6;
            bc2Rend.color = new Color(0.2f, 0.85f, 1.0f, 0.95f);

            GameObject betaLightObj = new GameObject("TrueBeta_Light");
            betaLightObj.transform.SetParent(betaConduit.transform);
            betaLightObj.transform.localPosition = Vector3.zero;
            Light2D betaLight = betaLightObj.AddComponent<Light2D>();
            betaLight.lightType = Light2D.LightType.Point;
            betaLight.pointLightOuterRadius = 3.0f;
            betaLight.color = new Color(0.1f, 0.85f, 1.0f, 1f);
            betaLight.intensity = 0.85f;

            // Exit Landing Platform (X: 8.5, Y: 1.8)
            GameObject exitPlat = new GameObject("Exit_LandingPlatform");
            exitPlat.transform.SetParent(refs.puzzleRoot.transform);
            exitPlat.transform.position = new Vector3(8.5f, 1.8f, 0f);
            SpriteRenderer epRend = exitPlat.AddComponent<SpriteRenderer>();
            epRend.sprite = floorSp;
            epRend.drawMode = SpriteDrawMode.Tiled;
            epRend.size = new Vector2(4f, 0.6f);
            epRend.sortingOrder = 2;
            BoxCollider2D epCol = exitPlat.AddComponent<BoxCollider2D>();
            epCol.size = new Vector2(4f, 0.6f);

            // Security Barrier Exit at X: 7.0, Y: 2.7 (Guarding ExitDoor, unlocked by EchoSwitch_Ceiling)
            GameObject exitBarrierObj = new GameObject("SecurityBarrier_Exit");
            exitBarrierObj.transform.SetParent(refs.puzzleRoot.transform);
            exitBarrierObj.transform.position = new Vector3(7.0f, 2.7f, 0f);

            SpriteRenderer ebRend = exitBarrierObj.AddComponent<SpriteRenderer>();
            ebRend.sprite = hazardLaserSp;
            ebRend.color = new Color(1.0f, 0.15f, 0.25f, 0.95f);
            ebRend.sortingOrder = 8;
            ebRend.drawMode = SpriteDrawMode.Sliced;
            ebRend.size = new Vector2(0.35f, 2.4f);

            BoxCollider2D ebCol = exitBarrierObj.AddComponent<BoxCollider2D>();
            ebCol.size = new Vector2(0.35f, 2.4f);

            GameObject ebLightObj = new GameObject("ExitBarrier_Light");
            ebLightObj.transform.SetParent(exitBarrierObj.transform);
            ebLightObj.transform.localPosition = Vector3.zero;
            Light2D ebLight = ebLightObj.AddComponent<Light2D>();
            ebLight.lightType = Light2D.LightType.Point;
            ebLight.pointLightOuterRadius = 2.5f;
            ebLight.color = new Color(1.0f, 0.2f, 0.25f, 1f);
            ebLight.intensity = 1.3f;

            SecurityBarrier exitBarrierComp = exitBarrierObj.AddComponent<SecurityBarrier>();
            SetPrivateField(exitBarrierComp, "barrierCollider", ebCol);
            SetPrivateField(exitBarrierComp, "barrierRenderer", ebRend);
            SetPrivateField(exitBarrierComp, "barrierLight", ebLight);

            SetPrivateField(echoSwitchComp, "targetBarrier", exitBarrierComp);
            echoSwitchComp.TargetBarrier = exitBarrierComp;

            // Position Exit Door on the high security upper landing
            refs.exitDoor.transform.position = new Vector3(8.5f, 2.7f, 0f);

            // Trap Route (Lower Floor): Visual deception & ParanoiaTrapPrank
            GameObject trapBeacon = new GameObject("Deco_TrapWarningBeacon");
            trapBeacon.transform.SetParent(refs.envRoot.transform);
            trapBeacon.transform.position = new Vector3(1.5f, -2.8f, 0f);
            SpriteRenderer tbRend = trapBeacon.AddComponent<SpriteRenderer>();
            tbRend.sprite = beaconSp;
            tbRend.sortingOrder = 5;

            GameObject tbLightObj = new GameObject("TrapBeacon_Light");
            tbLightObj.transform.SetParent(trapBeacon.transform);
            tbLightObj.transform.localPosition = Vector3.zero;
            Light2D tbLight = tbLightObj.AddComponent<Light2D>();
            tbLight.lightType = Light2D.LightType.Point;
            tbLight.pointLightOuterRadius = 3.0f;
            tbLight.color = new Color(1.0f, 0.15f, 0.2f, 1f);
            tbLight.intensity = 0.9f;
            FlickeringLight2D tbFlicker = tbLightObj.AddComponent<FlickeringLight2D>();
            tbFlicker.Mode = FlickeringLight2D.FlickerMode.EmergencyStrobe;

            GameObject brokenCables = new GameObject("Deco_BrokenCables_Trap");
            brokenCables.transform.SetParent(refs.envRoot.transform);
            brokenCables.transform.position = new Vector3(1.0f, -3.7f, 0f);
            SpriteRenderer bcRend = brokenCables.AddComponent<SpriteRenderer>();
            bcRend.sprite = cableSp;
            bcRend.sortingOrder = 1;
            bcRend.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);

            GameObject hazardStripes = new GameObject("Deco_HazardStripes_Trap");
            hazardStripes.transform.SetParent(refs.envRoot.transform);
            hazardStripes.transform.position = new Vector3(2.5f, -3.7f, 0f);
            hazardStripes.transform.localScale = new Vector3(3.0f, 0.4f, 1f);
            SpriteRenderer hsRend = hazardStripes.AddComponent<SpriteRenderer>();
            hsRend.sprite = stripesSp;
            hsRend.sortingOrder = 3;

            // Stationary floor pit hazard under the trap zone
            GameObject pitLaser = new GameObject("Hazard_FloorPit");
            pitLaser.transform.SetParent(refs.puzzleRoot.transform);
            pitLaser.transform.position = new Vector3(2.0f, -3.85f, 0f);
            SpriteRenderer plRend = pitLaser.AddComponent<SpriteRenderer>();
            plRend.sprite = hazardLaserSp;
            plRend.color = new Color(1.0f, 0.15f, 0.25f, 0.9f);
            plRend.sortingOrder = 6;
            plRend.drawMode = SpriteDrawMode.Sliced;
            plRend.size = new Vector2(4.5f, 0.3f);
            BoxCollider2D plCol = pitLaser.AddComponent<BoxCollider2D>();
            plCol.size = new Vector2(4.5f, 0.3f);
            plCol.isTrigger = true;
            MovingLaserHazard plHazard = pitLaser.AddComponent<MovingLaserHazard>();
            plHazard.ConfigurePath(MovingLaserHazard.MotionAxis.Horizontal, new Vector3(2.0f, -3.85f, 0f), new Vector3(2.0f, -3.85f, 0f), 0f, 0f, startActive: true);
            SetPrivateField(plHazard, "laserRenderer", plRend);
            SetPrivateField(plHazard, "activeOnStart", true);
            plHazard.SetHazardActive(true);

            // Add ParanoiaTrapPrank
            GameObject paranoiaPrankObj = new GameObject("ParanoiaTrapPrankController");
            paranoiaPrankObj.transform.SetParent(refs.puzzleRoot.transform);
            ParanoiaTrapPrank paranoiaPrank = paranoiaPrankObj.AddComponent<ParanoiaTrapPrank>();
            SetPrivateField(paranoiaPrank, "triggerX", 1.8f);
            SetPrivateField(paranoiaPrank, "maxY", -2.5f);
            SetPrivateField(paranoiaPrank, "warningSignRenderer", hsRend);

            // Wire ExitDoor and EchoSwitch into Director and HUD
            SetPrivateField(refs.hud, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.director, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.triggers, "echoSwitch", echoSwitchComp);

            SaveChamberScene("Level05.unity");
        }

        public static void ConstructLevel06Scene()
        {
            var atmo = new ChapterAtmosphereConfig
            {
                ambientColor = new Color(0.20f, 0.04f, 0.06f, 1f), // Deep quarantine emergency crimson
                ambientIntensity = 0.78f,
                spawnLampColor = new Color(1.0f, 0.15f, 0.18f, 1f),
                spawnLampIntensity = 1.25f,
                spawnLampFlicker = FlickeringLight2D.FlickerMode.QuarantineAlarm,
                particleType = AtmosphericParticleEmitter.AtmosphereType.QuarantineStatic,
                architectMonitorColor = new Color(1.0f, 0.15f, 0.20f, 0.95f)
            };

            var refs = SetupBaseChamber("CONTAINMENT 06 // TRUTH", 6, "Level07", false, atmo);

            Sprite floorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_tile.png");
            Sprite wallSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/wall_panel.png");
            Sprite beamSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ceiling_beam.png");
            Sprite hazardLaserSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/hazard_laser.png");
            Sprite alphaSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/dim_alpha_platform.png");
            Sprite betaSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/dim_beta_platform.png");
            Sprite echoSwitchSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/echo_switch.png");
            Sprite plateSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/pressure_plate.png");
            Sprite beaconSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/warning_beacon.png");
            Sprite quarantineSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/decal_quarantine.png");
            Sprite reactorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/bg_reactor.png");
            Sprite pillarSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/bg_pillar.png");

            // ALL 3 powers enabled for grand finale
            refs.abilityCtrl.AllowGravity = true;
            refs.abilityCtrl.AllowTimeFreeze = true;
            refs.abilityCtrl.AllowDimensionSwap = true;

            // Environmental Dressing: Ominous Dormant Containment Reactor
            GameObject reactorObj = new GameObject("Deco_QuarantineCoreReactor");
            reactorObj.transform.SetParent(refs.envRoot.transform);
            reactorObj.transform.position = new Vector3(0.5f, 1.2f, 0f);
            reactorObj.transform.localScale = new Vector3(2.4f, 2.4f, 1f);
            SpriteRenderer reactorRend = reactorObj.AddComponent<SpriteRenderer>();
            reactorRend.sprite = reactorSp;
            reactorRend.sortingOrder = -20;
            reactorRend.color = new Color(0.8f, 0.15f, 0.2f, 0.7f);

            // Cold White Sterile Core Floodlights contrasting the deep blood red
            GameObject whiteCoreLightObj = new GameObject("Core_SterileFloodlight");
            whiteCoreLightObj.transform.position = new Vector3(0.5f, 2.5f, 0f);
            Light2D whiteLight = whiteCoreLightObj.AddComponent<Light2D>();
            whiteLight.lightType = Light2D.LightType.Point;
            whiteLight.pointLightOuterRadius = 6.5f;
            whiteLight.color = new Color(0.9f, 0.95f, 1.0f, 1f); // Cold sterile white
            whiteLight.intensity = 0.65f;

            // Massive Brutalist Reinforced Bulkhead Pilasters
            float[] pilasterXs = new float[] { -8.5f, -2.5f, 4.0f };
            foreach (var px in pilasterXs)
            {
                GameObject pCol = new GameObject($"Deco_ReinforcedPilaster_{px}");
                pCol.transform.SetParent(refs.envRoot.transform);
                pCol.transform.position = new Vector3(px, 0.5f, 0f);
                pCol.transform.localScale = new Vector3(1.6f, 2.4f, 1f);
                SpriteRenderer pRend = pCol.AddComponent<SpriteRenderer>();
                pRend.sprite = pillarSp;
                pRend.sortingOrder = -15;
                pRend.color = new Color(0.20f, 0.08f, 0.10f, 0.9f);
            }

            // Quarantine Decals
            GameObject qDecal1 = new GameObject("Deco_QuarantineSign_1");
            qDecal1.transform.SetParent(refs.envRoot.transform);
            qDecal1.transform.position = new Vector3(-6.0f, -1.0f, 0f);
            SpriteRenderer qdR1 = qDecal1.AddComponent<SpriteRenderer>();
            qdR1.sprite = quarantineSp;
            qdR1.sortingOrder = -5;

            // In-world Lore Consoles showing escalating external threat
            GameObject console1 = new GameObject("Deco_LoreConsole_1");
            console1.transform.SetParent(refs.envRoot.transform);
            console1.transform.position = new Vector3(-6.8f, -0.6f, 0f);
            SpriteRenderer c1R = console1.AddComponent<SpriteRenderer>();
            c1R.sprite = beaconSp;
            c1R.sortingOrder = 2;
            c1R.color = new Color(0f, 0.85f, 1f, 1f);

            GameObject console2 = new GameObject("Deco_LoreConsole_2");
            console2.transform.SetParent(refs.envRoot.transform);
            console2.transform.position = new Vector3(2.8f, -3.2f, 0f);
            SpriteRenderer c2R = console2.AddComponent<SpriteRenderer>();
            c2R.sprite = beaconSp;
            c2R.sortingOrder = 2;
            c2R.color = new Color(1.0f, 0.75f, 0.1f, 1f);

            GameObject console3 = new GameObject("Deco_LoreConsole_3");
            console3.transform.SetParent(refs.envRoot.transform);
            console3.transform.position = new Vector3(7.2f, 3.6f, 0f);
            SpriteRenderer c3R = console3.AddComponent<SpriteRenderer>();
            c3R.sprite = beaconSp;
            c3R.sortingOrder = 2;
            c3R.color = new Color(1.0f, 0.2f, 0.25f, 1f);

            // Add Narrative Progression Trigger Controller
            GameObject truthTriggersObj = new GameObject("TruthChamberNarrativeTriggers");
            truthTriggersObj.transform.SetParent(refs.puzzleRoot.transform);
            TruthChamberTriggers truthTriggers = truthTriggersObj.AddComponent<TruthChamberTriggers>();
            SetPrivateField(truthTriggers, "stage1TriggerX", -2.0f);
            SetPrivateField(truthTriggers, "stage2TriggerX", 2.5f);
            SetPrivateField(truthTriggers, "stage3TriggerX", 7.6f);

            // --- STAGE 1: Blast Gate & Elevated Echo-Only Switch ---
            // Elevated Platform at X: -5.5, Y: -1.2
            GameObject echoPlat = new GameObject("Stage1_EchoPlatform");
            echoPlat.transform.SetParent(refs.puzzleRoot.transform);
            echoPlat.transform.position = new Vector3(-5.5f, -1.2f, 0f);
            SpriteRenderer epRend = echoPlat.AddComponent<SpriteRenderer>();
            epRend.sprite = floorSp;
            epRend.drawMode = SpriteDrawMode.Tiled;
            epRend.size = new Vector2(3f, 0.5f);
            epRend.sortingOrder = 2;
            BoxCollider2D epCol = echoPlat.AddComponent<BoxCollider2D>();
            epCol.size = new Vector2(3f, 0.5f);

            // EchoSwitch on elevated platform at (-5.5, -0.7)
            GameObject echoSwitchObj = new GameObject("EchoSwitch_QuarantineGate");
            echoSwitchObj.transform.SetParent(echoPlat.transform);
            echoSwitchObj.transform.localPosition = new Vector3(0f, 0.45f, 0f);

            SpriteRenderer switchRend = echoSwitchObj.AddComponent<SpriteRenderer>();
            switchRend.sprite = echoSwitchSp;
            switchRend.sortingOrder = 4;

            BoxCollider2D switchCol = echoSwitchObj.AddComponent<BoxCollider2D>();
            switchCol.isTrigger = true;
            switchCol.size = new Vector2(1.2f, 0.4f);

            GameObject s1LightObj = new GameObject("EchoSwitch_Light");
            s1LightObj.transform.SetParent(echoSwitchObj.transform);
            s1LightObj.transform.localPosition = Vector3.zero;
            Light2D s1Light = s1LightObj.AddComponent<Light2D>();
            s1Light.lightType = Light2D.LightType.Point;
            s1Light.pointLightOuterRadius = 2.4f;
            s1Light.color = new Color(0.85f, 0f, 1f, 1f);
            s1Light.intensity = 1.0f;

            SecurityOverrideSwitch echoSwitchComp = echoSwitchObj.AddComponent<SecurityOverrideSwitch>();
            SetPrivateField(echoSwitchComp, "switchRenderer", switchRend);
            SetPrivateField(echoSwitchComp, "glowLight", s1Light);
            SetPrivateField(echoSwitchComp, "depressVisual", echoSwitchObj.transform);
            SetPrivateField(echoSwitchComp, "allowPlayerActivation", false);
            SetPrivateField(echoSwitchComp, "allowEchoActivation", true);
            SetPrivateField(echoSwitchComp, "holdGraceDuration", 5.0f);
            SetPrivateField(echoSwitchComp, "latching", false);
            SetPrivateField(echoSwitchComp, "isPowered", true);

            // Stage 1 Retracting Blast Gate at X: -3.0
            GameObject blastGateObj = new GameObject("Stage1_BlastGate");
            blastGateObj.transform.SetParent(refs.puzzleRoot.transform);
            blastGateObj.transform.position = new Vector3(-3.0f, -2.5f, 0f);
            SpriteRenderer bgRend = blastGateObj.AddComponent<SpriteRenderer>();
            bgRend.sprite = hazardLaserSp;
            bgRend.drawMode = SpriteDrawMode.Sliced;
            bgRend.size = new Vector2(0.35f, 3.2f);
            bgRend.sortingOrder = 8;
            bgRend.color = new Color(1.0f, 0.15f, 0.25f, 0.95f);
            BoxCollider2D bgCol = blastGateObj.AddComponent<BoxCollider2D>();
            bgCol.size = new Vector2(0.35f, 3.2f);

            GameObject bgLightObj = new GameObject("BlastGate_Light");
            bgLightObj.transform.SetParent(blastGateObj.transform);
            bgLightObj.transform.localPosition = Vector3.zero;
            Light2D bgLight = bgLightObj.AddComponent<Light2D>();
            bgLight.lightType = Light2D.LightType.Point;
            bgLight.pointLightOuterRadius = 3.0f;
            bgLight.color = new Color(1.0f, 0.2f, 0.25f, 1f);
            bgLight.intensity = 1.2f;

            SecurityBarrier seal1BarrierComp = blastGateObj.AddComponent<SecurityBarrier>();
            SetPrivateField(seal1BarrierComp, "barrierCollider", bgCol);
            SetPrivateField(seal1BarrierComp, "barrierRenderer", bgRend);
            SetPrivateField(seal1BarrierComp, "barrierLight", bgLight);

            SetPrivateField(echoSwitchComp, "targetBarrier", seal1BarrierComp);
            echoSwitchComp.TargetBarrier = seal1BarrierComp;

            SetPrivateField(truthTriggers, "echoSwitch", echoSwitchComp);
            SetPrivateField(truthTriggers, "blastGate", blastGateObj);

            // --- STAGE 2: Electrified Pit & Freezable Moving Laser Hazard ---
            // Lethal floor hazard pit across X: -2.0 to 2.0 at Y: -3.85
            GameObject pitHazard = new GameObject("Hazard_PitFloor");
            pitHazard.transform.SetParent(refs.puzzleRoot.transform);
            pitHazard.transform.position = new Vector3(0f, -3.85f, 0f);
            SpriteRenderer phRend = pitHazard.AddComponent<SpriteRenderer>();
            phRend.sprite = hazardLaserSp;
            phRend.drawMode = SpriteDrawMode.Sliced;
            phRend.size = new Vector2(4.5f, 0.3f);
            phRend.sortingOrder = 6;
            phRend.color = new Color(1.0f, 0.15f, 0.25f, 0.95f);
            BoxCollider2D phCol = pitHazard.AddComponent<BoxCollider2D>();
            phCol.size = new Vector2(4.5f, 0.3f);
            phCol.isTrigger = true;
            MovingLaserHazard phHazard = pitHazard.AddComponent<MovingLaserHazard>();
            phHazard.ConfigurePath(MovingLaserHazard.MotionAxis.Horizontal, new Vector3(0f, -3.85f, 0f), new Vector3(0f, -3.85f, 0f), 0f, 0f, startActive: true);
            SetPrivateField(phHazard, "laserRenderer", phRend);
            SetPrivateField(phHazard, "activeOnStart", true);
            phHazard.SetHazardActive(true);

            // Create FreezableHazard that sweeps across the pit at Y = -2.5
            GameObject hazardObj = new GameObject("Stage2_FreezableLaserHazard");
            hazardObj.transform.SetParent(refs.puzzleRoot.transform);
            hazardObj.transform.position = new Vector3(-0.5f, -2.5f, 0f);

            SpriteRenderer hzRend = hazardObj.AddComponent<SpriteRenderer>();
            hzRend.sprite = hazardLaserSp;
            hzRend.drawMode = SpriteDrawMode.Sliced;
            hzRend.size = new Vector2(1.8f, 0.6f);
            hzRend.sortingOrder = 7;
            hzRend.color = new Color(1.0f, 0.15f, 0.25f, 1f);

            BoxCollider2D hzCol = hazardObj.AddComponent<BoxCollider2D>();
            hzCol.size = new Vector2(1.8f, 0.6f);

            FreezableHazard fHazard = hazardObj.AddComponent<FreezableHazard>();
            SetPrivateField(fHazard, "offsetA", new Vector3(-1.2f, 0f, 0f));
            SetPrivateField(fHazard, "offsetB", new Vector3(2.5f, 0f, 0f));
            SetPrivateField(fHazard, "moveSpeed", 3.2f);
            SetPrivateField(fHazard, "hazardRenderer", hzRend);

            // Ground platform after the pit at X: 2.8, Y: -4.0
            GameObject pitExitPlat = new GameObject("Stage2_PitExitFloor");
            pitExitPlat.transform.SetParent(refs.puzzleRoot.transform);
            pitExitPlat.transform.position = new Vector3(2.8f, -4.0f, 0f);
            SpriteRenderer peRend = pitExitPlat.AddComponent<SpriteRenderer>();
            peRend.sprite = floorSp;
            peRend.drawMode = SpriteDrawMode.Tiled;
            peRend.size = new Vector2(2.5f, 1.0f);
            peRend.sortingOrder = 2;
            BoxCollider2D peCol = pitExitPlat.AddComponent<BoxCollider2D>();
            peCol.size = new Vector2(2.5f, 1.0f);

            // --- STAGE 3: Massive Lower Barrier & Inverted Ceiling Dimension Shaft ---
            // Huge vertical bulkhead from floor up to Y: 1.8 at X: 4.5
            GameObject vaultBulkhead = new GameObject("Stage3_VaultBarrier");
            vaultBulkhead.transform.SetParent(refs.puzzleRoot.transform);
            vaultBulkhead.transform.position = new Vector3(4.5f, -1.2f, 0f);
            SpriteRenderer vbRend = vaultBulkhead.AddComponent<SpriteRenderer>();
            vbRend.sprite = wallSp;
            vbRend.drawMode = SpriteDrawMode.Tiled;
            vbRend.size = new Vector2(1.0f, 6.0f);
            vbRend.sortingOrder = 3;
            vbRend.color = new Color(0.2f, 0.22f, 0.28f, 1f);
            BoxCollider2D vbCol = vaultBulkhead.AddComponent<BoxCollider2D>();
            vbCol.size = new Vector2(1.0f, 6.0f);

            // Upper Ceiling Dimension Platforms (requires Gravity Inversion [G] to walk on ceiling):
            // Dimension Alpha Platform (Ceiling, X: 4.2, Y: 3.2)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimAlpha_Ceiling_Stage3",
                new Vector3(4.2f, 3.2f, 0f), new Vector2(3.0f, 0.4f), alphaSp, DimensionType.Alpha);

            // Dimension Beta Platform (Ceiling, X: 6.2, Y: 3.2)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimBeta_Ceiling_Stage3",
                new Vector3(6.2f, 3.2f, 0f), new Vector2(3.0f, 0.4f), betaSp, DimensionType.Beta);

            // Mid-shaft barrier preventing dropping early
            GameObject midShaftHazard = new GameObject("Hazard_MidShaftDrop");
            midShaftHazard.transform.SetParent(refs.puzzleRoot.transform);
            midShaftHazard.transform.position = new Vector3(5.2f, 0.8f, 0f);
            SpriteRenderer msRend = midShaftHazard.AddComponent<SpriteRenderer>();
            msRend.sprite = hazardLaserSp;
            msRend.drawMode = SpriteDrawMode.Sliced;
            msRend.size = new Vector2(2.2f, 0.3f);
            msRend.sortingOrder = 6;
            msRend.color = new Color(1.0f, 0.15f, 0.25f, 0.9f);
            BoxCollider2D msCol = midShaftHazard.AddComponent<BoxCollider2D>();
            msCol.size = new Vector2(2.2f, 0.3f);
            msCol.isTrigger = true;
            MovingLaserHazard msHazard = midShaftHazard.AddComponent<MovingLaserHazard>();
            msHazard.ConfigurePath(MovingLaserHazard.MotionAxis.Horizontal, new Vector3(5.2f, 0.8f, 0f), new Vector3(5.2f, 0.8f, 0f), 0f, 0f, startActive: true);
            SetPrivateField(msHazard, "laserRenderer", msRend);
            SetPrivateField(msHazard, "activeOnStart", true);
            msHazard.SetHazardActive(true);

            // --- STAGE 4: Final Quarantine Vault & Exit Door ---
            // Floor of final vault at X: 8.5, Y: -2.0
            GameObject vaultFloor = new GameObject("Stage4_VaultFloor");
            vaultFloor.transform.SetParent(refs.puzzleRoot.transform);
            vaultFloor.transform.position = new Vector3(8.5f, -2.0f, 0f);
            SpriteRenderer vfRend = vaultFloor.AddComponent<SpriteRenderer>();
            vfRend.sprite = floorSp;
            vfRend.drawMode = SpriteDrawMode.Tiled;
            vfRend.size = new Vector2(4.5f, 0.6f);
            vfRend.sortingOrder = 2;
            BoxCollider2D vfCol = vaultFloor.AddComponent<BoxCollider2D>();
            vfCol.size = new Vector2(4.5f, 0.6f);

            // Security Barrier guarding the Exit Door at X: 7.6, Y: -1.1
            GameObject finalBarrierObj = new GameObject("SecurityBarrier_FinalVault");
            finalBarrierObj.transform.SetParent(refs.puzzleRoot.transform);
            finalBarrierObj.transform.position = new Vector3(7.6f, -1.1f, 0f);

            SpriteRenderer fbRend = finalBarrierObj.AddComponent<SpriteRenderer>();
            fbRend.sprite = hazardLaserSp;
            fbRend.color = new Color(1.0f, 0.15f, 0.25f, 0.95f);
            fbRend.sortingOrder = 8;
            fbRend.drawMode = SpriteDrawMode.Sliced;
            fbRend.size = new Vector2(0.35f, 2.4f);

            BoxCollider2D fbCol = finalBarrierObj.AddComponent<BoxCollider2D>();
            fbCol.size = new Vector2(0.35f, 2.4f);

            GameObject fbLightObj = new GameObject("FinalBarrier_Light");
            fbLightObj.transform.SetParent(finalBarrierObj.transform);
            fbLightObj.transform.localPosition = Vector3.zero;
            Light2D fbLight = fbLightObj.AddComponent<Light2D>();
            fbLight.lightType = Light2D.LightType.Point;
            fbLight.pointLightOuterRadius = 2.4f;
            fbLight.color = new Color(1.0f, 0.2f, 0.25f, 1f);
            fbLight.intensity = 1.3f;

            SecurityBarrier finalBarrierComp = finalBarrierObj.AddComponent<SecurityBarrier>();
            SetPrivateField(finalBarrierComp, "barrierCollider", fbCol);
            SetPrivateField(finalBarrierComp, "barrierRenderer", fbRend);
            SetPrivateField(finalBarrierComp, "barrierLight", fbLight);

            // Pressure Plate inside vault to trigger door seal
            GameObject plateObj = new GameObject("Vault_FinalTriggerPlate");
            plateObj.transform.SetParent(refs.puzzleRoot.transform);
            plateObj.transform.position = new Vector3(6.8f, -1.6f, 0f);
            SpriteRenderer plateRend = plateObj.AddComponent<SpriteRenderer>();
            plateRend.sprite = plateSp;
            plateRend.sortingOrder = 2;
            BoxCollider2D plateCol = plateObj.AddComponent<BoxCollider2D>();
            plateCol.isTrigger = true;
            plateCol.size = new Vector2(1.2f, 0.3f);
            PressurePlate vaultPlate = plateObj.AddComponent<PressurePlate>();
            SetPrivateField(vaultPlate, "plateRenderer", plateRend);
            SetPrivateField(vaultPlate, "depressVisual", plateObj.transform);
            vaultPlate.TargetBarrier = finalBarrierComp;

            // Final Quarantine Exit Door at X: 8.5, Y: -1.1
            refs.exitDoor.transform.position = new Vector3(8.5f, -1.1f, 0f);
            refs.exitDoor.SetUnlocked(true); // Exit door ready; physical passage guarded by SecurityBarrier_FinalVault

            // Wire Director and HUD
            SetPrivateField(refs.hud, "pressurePlate", vaultPlate);
            SetPrivateField(refs.hud, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.director, "pressurePlate", vaultPlate);
            SetPrivateField(refs.director, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.triggers, "echoSwitch", echoSwitchComp);

            SaveChamberScene("Level06.unity");
        }

        public static void ConstructLevel07Scene()
        {
            var atmo = new ChapterAtmosphereConfig
            {
                ambientColor = new Color(0.24f, 0.04f, 0.08f, 1f), // Harsh crimson emergency
                ambientIntensity = 0.85f,
                spawnLampColor = new Color(1.0f, 0.15f, 0.20f, 1f),
                spawnLampIntensity = 1.35f,
                spawnLampFlicker = FlickeringLight2D.FlickerMode.BossCorePulse,
                particleType = AtmosphericParticleEmitter.AtmosphereType.BossCoreAnomalies,
                architectMonitorColor = new Color(1.0f, 0.15f, 0.25f, 0.95f)
            };

            var refs = SetupBaseChamber("CONTAINMENT 07 // JUDGMENT", 7, "", true, atmo);

            Sprite floorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_tile.png");
            Sprite wallSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/wall_panel.png");
            Sprite beamSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ceiling_beam.png");
            Sprite alphaSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/dim_alpha_platform.png");
            Sprite betaSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/dim_beta_platform.png");
            Sprite echoSwitchSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/echo_switch.png");
            Sprite nodeSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/boss_control_node.png");
            Sprite surgeSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/energy_surge_hazard.png");
            Sprite bossCoreSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/architect_boss_core.png");
            Sprite hazardLaserSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/hazard_laser.png");

            // ALL 3 reality powers enabled for final confrontation
            refs.abilityCtrl.AllowGravity = true;
            refs.abilityCtrl.AllowTimeFreeze = true;
            refs.abilityCtrl.AllowDimensionSwap = true;

            // --- ARENA PERIMETER & CHASM GEOMETRY ---
            GameObject baseFloor = GameObject.Find("Floor");
            if (baseFloor != null) Object.DestroyImmediate(baseFloor);

            GameObject floorLeft = new GameObject("Floor_LeftArena");
            floorLeft.transform.SetParent(refs.envRoot.transform);
            floorLeft.transform.position = new Vector3(-6.5f, -4.0f, 0f);
            SpriteRenderer flR = floorLeft.AddComponent<SpriteRenderer>();
            flR.sprite = floorSp;
            flR.drawMode = SpriteDrawMode.Tiled;
            flR.size = new Vector2(7.0f, 1.0f);
            flR.sortingOrder = 2;
            BoxCollider2D flCol = floorLeft.AddComponent<BoxCollider2D>();
            flCol.size = new Vector2(7.0f, 1.0f);

            GameObject floorRight = new GameObject("Floor_RightArena");
            floorRight.transform.SetParent(refs.envRoot.transform);
            floorRight.transform.position = new Vector3(6.5f, -4.0f, 0f);
            SpriteRenderer frR = floorRight.AddComponent<SpriteRenderer>();
            frR.sprite = floorSp;
            frR.drawMode = SpriteDrawMode.Tiled;
            frR.size = new Vector2(7.0f, 1.0f);
            frR.sortingOrder = 2;
            BoxCollider2D frCol = floorRight.AddComponent<BoxCollider2D>();
            frCol.size = new Vector2(7.0f, 1.0f);

            GameObject pitObj = new GameObject("Arena_AbyssHazardZone");
            pitObj.transform.SetParent(refs.envRoot.transform);
            pitObj.transform.position = new Vector3(0f, -6.5f, 0f);
            BoxCollider2D pitCol = pitObj.AddComponent<BoxCollider2D>();
            pitCol.isTrigger = true;
            pitCol.size = new Vector2(24.0f, 2.0f);

            GameObject ceiling = GameObject.Find("Ceiling");
            if (ceiling != null)
            {
                ceiling.transform.position = new Vector3(0f, 4.2f, 0f);
            }

            // --- CENTRAL ARCHITECT BOSS CORE ---
            GameObject bossCoreObj = new GameObject("ArchitectBossCore");
            bossCoreObj.transform.SetParent(refs.envRoot.transform);
            bossCoreObj.transform.position = new Vector3(0f, 2.2f, 0f);
            bossCoreObj.transform.localScale = new Vector3(1.8f, 1.8f, 1f);

            SpriteRenderer coreRend = bossCoreObj.AddComponent<SpriteRenderer>();
            coreRend.sprite = bossCoreSp;
            coreRend.sortingOrder = -4;

            GameObject coreAlarmObj = new GameObject("Core_EmergencyAlarmStrobe");
            coreAlarmObj.transform.SetParent(bossCoreObj.transform);
            coreAlarmObj.transform.localPosition = Vector3.zero;
            Light2D alarmLight = coreAlarmObj.AddComponent<Light2D>();
            alarmLight.lightType = Light2D.LightType.Point;
            alarmLight.pointLightOuterRadius = 10.0f;
            alarmLight.color = new Color(1.0f, 0.15f, 0.25f, 1f);
            alarmLight.intensity = 1.4f;
            FlickeringLight2D alarmFlicker = coreAlarmObj.AddComponent<FlickeringLight2D>();
            alarmFlicker.Mode = FlickeringLight2D.FlickerMode.BossCorePulse;

            GameObject coreCyanObj = new GameObject("Core_CyanAnomalyBacklight");
            coreCyanObj.transform.SetParent(bossCoreObj.transform);
            coreCyanObj.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            Light2D cyanLight = coreCyanObj.AddComponent<Light2D>();
            cyanLight.lightType = Light2D.LightType.Point;
            cyanLight.pointLightOuterRadius = 6.0f;
            cyanLight.color = new Color(0f, 0.94f, 1f, 1f);
            cyanLight.intensity = 0.8f;

            // --- STAGE 1: GRAVITY ASSAULT ---
            GameObject lowSurgeObj = new GameObject("Stage1_LowSurgeHazard");
            lowSurgeObj.transform.SetParent(refs.puzzleRoot.transform);
            lowSurgeObj.transform.position = new Vector3(-0.5f, -3.5f, 0f);

            SpriteRenderer lsRend = lowSurgeObj.AddComponent<SpriteRenderer>();
            lsRend.sprite = surgeSp;
            lsRend.drawMode = SpriteDrawMode.Tiled;
            lsRend.size = new Vector2(13.0f, 0.7f);
            lsRend.sortingOrder = 4;

            BoxCollider2D lsCol = lowSurgeObj.AddComponent<BoxCollider2D>();
            lsCol.isTrigger = true;
            lsCol.size = new Vector2(13.0f, 0.7f);

            GameObject lsWarnObj = new GameObject("LowHazard_WarningLight");
            lsWarnObj.transform.SetParent(lowSurgeObj.transform);
            lsWarnObj.transform.localPosition = Vector3.zero;
            Light2D lsLight = lsWarnObj.AddComponent<Light2D>();
            lsLight.lightType = Light2D.LightType.Point;
            lsLight.pointLightOuterRadius = 4.0f;
            lsLight.color = new Color(1.0f, 0.7f, 0.1f, 1f);
            lsLight.intensity = 0f;

            BossHazardEmitter lowEmitter = lowSurgeObj.AddComponent<BossHazardEmitter>();
            SetPrivateField(lowEmitter, "initialDelay", 0.5f);
            SetPrivateField(lowEmitter, "idleDuration", 2.4f);
            SetPrivateField(lowEmitter, "warningDuration", 1.4f);
            SetPrivateField(lowEmitter, "surgeDuration", 1.6f);
            SetPrivateField(lowEmitter, "hazardRenderer", lsRend);
            SetPrivateField(lowEmitter, "warningLight", lsLight);

            GameObject highSurgeObj = new GameObject("Stage1_HighSurgeHazard");
            highSurgeObj.transform.SetParent(refs.puzzleRoot.transform);
            highSurgeObj.transform.position = new Vector3(-0.5f, 3.5f, 0f);

            SpriteRenderer hsRend = highSurgeObj.AddComponent<SpriteRenderer>();
            hsRend.sprite = surgeSp;
            hsRend.drawMode = SpriteDrawMode.Tiled;
            hsRend.size = new Vector2(13.0f, 0.7f);
            hsRend.sortingOrder = 4;

            BoxCollider2D hsCol = highSurgeObj.AddComponent<BoxCollider2D>();
            hsCol.isTrigger = true;
            hsCol.size = new Vector2(13.0f, 0.7f);

            GameObject hsWarnObj = new GameObject("HighHazard_WarningLight");
            hsWarnObj.transform.SetParent(highSurgeObj.transform);
            hsWarnObj.transform.localPosition = Vector3.zero;
            Light2D hsLight = hsWarnObj.AddComponent<Light2D>();
            hsLight.lightType = Light2D.LightType.Point;
            hsLight.pointLightOuterRadius = 4.0f;
            hsLight.color = new Color(1.0f, 0.7f, 0.1f, 1f);
            hsLight.intensity = 0f;

            BossHazardEmitter highEmitter = highSurgeObj.AddComponent<BossHazardEmitter>();
            SetPrivateField(highEmitter, "initialDelay", 2.8f);
            SetPrivateField(highEmitter, "idleDuration", 2.4f);
            SetPrivateField(highEmitter, "warningDuration", 1.4f);
            SetPrivateField(highEmitter, "surgeDuration", 1.6f);
            SetPrivateField(highEmitter, "hazardRenderer", hsRend);
            SetPrivateField(highEmitter, "warningLight", hsLight);

            // Elevated Platform for Node 1: Mounted high on right ceiling bulkhead at (X: 5.5, Y: 2.8)
            // Requires gravity inversion to traverse across ceiling and land
            GameObject node1Plat = new GameObject("Stage1_Node1Platform");
            node1Plat.transform.SetParent(refs.puzzleRoot.transform);
            node1Plat.transform.position = new Vector3(5.5f, 2.8f, 0f);
            SpriteRenderer n1pR = node1Plat.AddComponent<SpriteRenderer>();
            n1pR.sprite = beamSp;
            n1pR.drawMode = SpriteDrawMode.Tiled;
            n1pR.size = new Vector2(3.2f, 0.4f);
            n1pR.sortingOrder = 2;
            BoxCollider2D n1pCol = node1Plat.AddComponent<BoxCollider2D>();
            n1pCol.size = new Vector2(3.2f, 0.4f);

            GameObject node1Obj = new GameObject("Stage1_OverrideNode1");
            node1Obj.transform.SetParent(refs.puzzleRoot.transform);
            node1Obj.transform.position = new Vector3(5.5f, 3.2f, 0f);

            SpriteRenderer n1Rend = node1Obj.AddComponent<SpriteRenderer>();
            n1Rend.sprite = nodeSp;
            n1Rend.sortingOrder = 4;

            BoxCollider2D n1Col = node1Obj.AddComponent<BoxCollider2D>();
            n1Col.isTrigger = true;
            n1Col.size = new Vector2(1.2f, 0.8f);

            GameObject n1LightObj = new GameObject("Node1_GlowLight");
            n1LightObj.transform.SetParent(node1Obj.transform);
            n1LightObj.transform.localPosition = Vector3.zero;
            Light2D n1Light = n1LightObj.AddComponent<Light2D>();
            n1Light.lightType = Light2D.LightType.Point;
            n1Light.pointLightOuterRadius = 3.0f;
            n1Light.color = new Color(1.0f, 0.25f, 0.25f, 1f);
            n1Light.intensity = 0.8f;

            BossControlNode node1Comp = node1Obj.AddComponent<BossControlNode>();
            SetPrivateField(node1Comp, "stageNumber", 1);
            SetPrivateField(node1Comp, "nodeName", "OVERRIDE NODE 01 // GRAVITY MATRIX");
            SetPrivateField(node1Comp, "nodeRenderer", n1Rend);
            SetPrivateField(node1Comp, "glowLight", n1Light);
            SetPrivateField(node1Comp, "pulseVisual", node1Obj.transform);

            // --- STAGE 2: REALITY COLLAPSE ---
            GameObject stage2Root = new GameObject("Stage2_PuzzleRoot");
            stage2Root.transform.SetParent(refs.puzzleRoot.transform);

            CreateDimensionPlatform(stage2Root.transform, "DimAlpha_Stage2_Plat1",
                new Vector3(-2.2f, -1.8f, 0f), new Vector2(2.6f, 0.4f), alphaSp, DimensionType.Alpha);

            CreateDimensionPlatform(stage2Root.transform, "DimBeta_Stage2_Plat2",
                new Vector3(0.5f, -0.6f, 0f), new Vector2(2.6f, 0.4f), betaSp, DimensionType.Beta);

            CreateDimensionPlatform(stage2Root.transform, "DimAlpha_Stage2_Plat3",
                new Vector3(3.2f, 0.6f, 0f), new Vector2(2.6f, 0.4f), alphaSp, DimensionType.Alpha);

            GameObject stage2HazardObj = new GameObject("Stage2_MovingSecurityHazard");
            stage2HazardObj.transform.SetParent(stage2Root.transform);
            stage2HazardObj.transform.position = new Vector3(0.5f, 0.0f, 0f);

            SpriteRenderer s2hRend = stage2HazardObj.AddComponent<SpriteRenderer>();
            s2hRend.sprite = hazardLaserSp;
            s2hRend.drawMode = SpriteDrawMode.Sliced;
            s2hRend.size = new Vector2(1.8f, 0.6f);
            s2hRend.sortingOrder = 4;
            s2hRend.color = new Color(1.0f, 0.2f, 0.3f, 1f);

            BoxCollider2D s2hCol = stage2HazardObj.AddComponent<BoxCollider2D>();
            s2hCol.size = new Vector2(1.8f, 0.6f);

            FreezableHazard stage2HazardComp = stage2HazardObj.AddComponent<FreezableHazard>();
            SetPrivateField(stage2HazardComp, "offsetA", new Vector3(-2.0f, 0f, 0f));
            SetPrivateField(stage2HazardComp, "offsetB", new Vector3(2.0f, 0f, 0f));
            SetPrivateField(stage2HazardComp, "moveSpeed", 3.2f);
            SetPrivateField(stage2HazardComp, "hazardRenderer", s2hRend);

            GameObject node2Plat = new GameObject("Stage2_Node2Platform");
            node2Plat.transform.SetParent(stage2Root.transform);
            node2Plat.transform.position = new Vector3(5.5f, 0.8f, 0f);
            SpriteRenderer n2pR = node2Plat.AddComponent<SpriteRenderer>();
            n2pR.sprite = floorSp;
            n2pR.drawMode = SpriteDrawMode.Tiled;
            n2pR.size = new Vector2(2.5f, 0.4f);
            n2pR.sortingOrder = 2;
            BoxCollider2D n2pCol = node2Plat.AddComponent<BoxCollider2D>();
            n2pCol.size = new Vector2(2.5f, 0.4f);

            GameObject node2Obj = new GameObject("Stage2_OverrideNode2");
            node2Obj.transform.SetParent(stage2Root.transform);
            node2Obj.transform.position = new Vector3(5.5f, 1.3f, 0f);

            SpriteRenderer n2Rend = node2Obj.AddComponent<SpriteRenderer>();
            n2Rend.sprite = nodeSp;
            n2Rend.sortingOrder = 4;

            BoxCollider2D n2Col = node2Obj.AddComponent<BoxCollider2D>();
            n2Col.isTrigger = true;
            n2Col.size = new Vector2(1.2f, 0.8f);

            GameObject n2LightObj = new GameObject("Node2_GlowLight");
            n2LightObj.transform.SetParent(node2Obj.transform);
            n2LightObj.transform.localPosition = Vector3.zero;
            Light2D n2Light = n2LightObj.AddComponent<Light2D>();
            n2Light.lightType = Light2D.LightType.Point;
            n2Light.pointLightOuterRadius = 3.0f;
            n2Light.color = new Color(1.0f, 0.25f, 0.25f, 1f);
            n2Light.intensity = 0.8f;

            BossControlNode node2Comp = node2Obj.AddComponent<BossControlNode>();
            SetPrivateField(node2Comp, "stageNumber", 2);
            SetPrivateField(node2Comp, "nodeName", "OVERRIDE NODE 02 // REALITY SYNTHESIS");
            SetPrivateField(node2Comp, "nodeRenderer", n2Rend);
            SetPrivateField(node2Comp, "glowLight", n2Light);
            SetPrivateField(node2Comp, "pulseVisual", node2Obj.transform);

            // --- STAGE 3: ECHO PROTOCOL ---
            GameObject echoPlat = new GameObject("Stage3_EchoLedge");
            echoPlat.transform.SetParent(refs.puzzleRoot.transform);
            echoPlat.transform.position = new Vector3(-6.0f, 0.5f, 0f);
            SpriteRenderer epR = echoPlat.AddComponent<SpriteRenderer>();
            epR.sprite = floorSp;
            epR.drawMode = SpriteDrawMode.Tiled;
            epR.size = new Vector2(3.5f, 0.5f);
            epR.sortingOrder = 2;
            BoxCollider2D epCol = echoPlat.AddComponent<BoxCollider2D>();
            epCol.size = new Vector2(3.5f, 0.5f);

            GameObject stage3SwitchObj = new GameObject("Stage3_EchoSwitch");
            stage3SwitchObj.transform.SetParent(echoPlat.transform);
            stage3SwitchObj.transform.localPosition = new Vector3(0f, 0.45f, 0f);

            SpriteRenderer s3sRend = stage3SwitchObj.AddComponent<SpriteRenderer>();
            s3sRend.sprite = echoSwitchSp;
            s3sRend.sortingOrder = 4;

            BoxCollider2D s3sCol = stage3SwitchObj.AddComponent<BoxCollider2D>();
            s3sCol.isTrigger = true;
            s3sCol.size = new Vector2(1.2f, 0.4f);

            GameObject s3LightObj = new GameObject("Stage3_EchoSwitch_Light");
            s3LightObj.transform.SetParent(stage3SwitchObj.transform);
            s3LightObj.transform.localPosition = Vector3.zero;
            Light2D s3Light = s3LightObj.AddComponent<Light2D>();
            s3Light.lightType = Light2D.LightType.Point;
            s3Light.pointLightOuterRadius = 2.4f;
            s3Light.color = new Color(0.85f, 0f, 1f, 1f);
            s3Light.intensity = 1.0f;

            SecurityOverrideSwitch echoSwitchComp = stage3SwitchObj.AddComponent<SecurityOverrideSwitch>();
            SetPrivateField(echoSwitchComp, "switchRenderer", s3sRend);
            SetPrivateField(echoSwitchComp, "glowLight", s3Light);
            SetPrivateField(echoSwitchComp, "depressVisual", stage3SwitchObj.transform);
            SetPrivateField(echoSwitchComp, "allowPlayerActivation", false);
            SetPrivateField(echoSwitchComp, "allowEchoActivation", true);
            SetPrivateField(echoSwitchComp, "holdGraceDuration", 6.0f);
            SetPrivateField(echoSwitchComp, "latching", false);
            SetPrivateField(echoSwitchComp, "isPowered", true);

            // Forcefield Shield Barrier guarding Node 3 at X: 8.0, Y: -2.5
            GameObject shieldObj = new GameObject("Stage3_ShieldBarrier");
            shieldObj.transform.SetParent(refs.puzzleRoot.transform);
            shieldObj.transform.position = new Vector3(8.0f, -2.5f, 0f);

            SpriteRenderer sbRend = shieldObj.AddComponent<SpriteRenderer>();
            sbRend.sprite = hazardLaserSp;
            sbRend.drawMode = SpriteDrawMode.Sliced;
            sbRend.size = new Vector2(0.35f, 3.5f);
            sbRend.sortingOrder = 8;
            sbRend.color = new Color(1.0f, 0.15f, 0.25f, 0.95f);

            BoxCollider2D sbCol = shieldObj.AddComponent<BoxCollider2D>();
            sbCol.size = new Vector2(0.35f, 3.5f);

            GameObject sbLightObj = new GameObject("ShieldBarrier_Light");
            sbLightObj.transform.SetParent(shieldObj.transform);
            sbLightObj.transform.localPosition = Vector3.zero;
            Light2D sbLight = sbLightObj.AddComponent<Light2D>();
            sbLight.lightType = Light2D.LightType.Point;
            sbLight.pointLightOuterRadius = 3.0f;
            sbLight.color = new Color(1.0f, 0.2f, 0.25f, 1f);
            sbLight.intensity = 1.3f;

            SecurityBarrier shieldBarrierComp = shieldObj.AddComponent<SecurityBarrier>();
            SetPrivateField(shieldBarrierComp, "barrierCollider", sbCol);
            SetPrivateField(shieldBarrierComp, "barrierRenderer", sbRend);
            SetPrivateField(shieldBarrierComp, "barrierLight", sbLight);

            SetPrivateField(echoSwitchComp, "targetBarrier", shieldBarrierComp);
            echoSwitchComp.TargetBarrier = shieldBarrierComp;

            GameObject node3Obj = new GameObject("Stage3_OverrideNode3");
            node3Obj.transform.SetParent(refs.puzzleRoot.transform);
            node3Obj.transform.position = new Vector3(9.0f, -2.8f, 0f);

            SpriteRenderer n3Rend = node3Obj.AddComponent<SpriteRenderer>();
            n3Rend.sprite = nodeSp;
            n3Rend.sortingOrder = 4;

            BoxCollider2D n3Col = node3Obj.AddComponent<BoxCollider2D>();
            n3Col.isTrigger = true;
            n3Col.size = new Vector2(1.2f, 0.8f);

            GameObject n3LightObj = new GameObject("Node3_GlowLight");
            n3LightObj.transform.SetParent(node3Obj.transform);
            n3LightObj.transform.localPosition = Vector3.zero;
            Light2D n3Light = n3LightObj.AddComponent<Light2D>();
            n3Light.lightType = Light2D.LightType.Point;
            n3Light.pointLightOuterRadius = 3.5f;
            n3Light.color = new Color(1.0f, 0.25f, 0.25f, 1f);
            n3Light.intensity = 1.0f;

            BossControlNode node3Comp = node3Obj.AddComponent<BossControlNode>();
            SetPrivateField(node3Comp, "stageNumber", 3);
            SetPrivateField(node3Comp, "nodeName", "MASTER OVERLOAD NODE // ARCHITECT SHUTDOWN");
            SetPrivateField(node3Comp, "nodeRenderer", n3Rend);
            SetPrivateField(node3Comp, "glowLight", n3Light);
            SetPrivateField(node3Comp, "pulseVisual", node3Obj.transform);

            // --- BOSS UI OVERLAY (CORE INTEGRITY) ---
            Text bossIntegrityText = CreateUIText(
                refs.hud.transform,
                "BossCoreIntegrityText",
                "<color=#FF1E40>CORE INTEGRITY: 3/3</color>   <size=14><color=#888888>// PHASE 1: GRAVITY ASSAULT</color></size>",
                Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"),
                20,
                TextAnchor.MiddleCenter,
                new Vector2(0.5f, 1.0f),
                new Vector2(0.5f, 1.0f),
                new Vector2(0f, -48f),
                new Vector2(700f, 40f),
                Color.white
            );

            // --- BOSS DIRECTOR CONTROLLER ---
            GameObject bossDirectorObj = new GameObject("BossDirectorController");
            bossDirectorObj.transform.SetParent(refs.puzzleRoot.transform);
            BossDirector bossDirector = bossDirectorObj.AddComponent<BossDirector>();

            SetPrivateField(bossDirector, "stage1Hazards", new BossHazardEmitter[] { lowEmitter, highEmitter });
            SetPrivateField(bossDirector, "node1", node1Comp);
            SetPrivateField(bossDirector, "stage2Chasm", pitObj);
            SetPrivateField(bossDirector, "stage2Hazard", stage2HazardComp);
            SetPrivateField(bossDirector, "stage2PuzzleRoot", stage2Root);
            SetPrivateField(bossDirector, "node2", node2Comp);
            SetPrivateField(bossDirector, "stage3EchoSwitch", echoSwitchComp);
            SetPrivateField(bossDirector, "stage3ShieldBarrier", shieldObj);
            SetPrivateField(bossDirector, "node3", node3Comp);
            SetPrivateField(bossDirector, "architectEyeCore", bossCoreObj);
            SetPrivateField(bossDirector, "coreRenderer", coreRend);
            SetPrivateField(bossDirector, "coreLight", cyanLight);
            SetPrivateField(bossDirector, "coreAlarmStrobe", alarmLight);
            SetPrivateField(bossDirector, "bossStatusText", bossIntegrityText);

            // Position Exit Door behind the final core node
            refs.exitDoor.transform.position = new Vector3(9.2f, -2.8f, 0f);

            // Wire HUD and Director
            SetPrivateField(refs.hud, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.director, "echoSwitch", echoSwitchComp);

            SaveChamberScene("Level07.unity");
        }

        private static GameObject CreateDimensionPlatform(Transform parent, string name, Vector3 pos, Vector2 size, Sprite sp, DimensionType dim)
        {
            GameObject plat = new GameObject(name);
            plat.transform.SetParent(parent);
            plat.transform.position = pos;

            SpriteRenderer rend = plat.AddComponent<SpriteRenderer>();
            rend.sprite = sp;
            rend.sortingOrder = 2;

            BoxCollider2D col = plat.AddComponent<BoxCollider2D>();
            col.size = size;

            DimensionObject dimComp = plat.AddComponent<DimensionObject>();
            SetPrivateField(dimComp, "activeInDimension", dim);
            SetPrivateField(dimComp, "spriteRenderer", rend);

            return plat;
        }
        #endregion

        #region Build Settings & File Utilities
        private static void SaveChamberScene(string sceneFileName)
        {
            string fullPath = Path.Combine(LevelsPath, sceneFileName);
            if (!Directory.Exists(LevelsPath))
            {
                Directory.CreateDirectory(LevelsPath);
            }
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[ChamberBuilder] Saved {fullPath}");
        }

        public static void RegisterScenesInBuildSettings()
        {
            string[] scenePaths = new string[]
            {
                "Assets/_Game/Scenes/Levels/Level01.unity",
                "Assets/_Game/Scenes/Levels/Level02.unity",
                "Assets/_Game/Scenes/Levels/Level03.unity",
                "Assets/_Game/Scenes/Levels/Level04.unity",
                "Assets/_Game/Scenes/Levels/Level05.unity",
                "Assets/_Game/Scenes/Levels/Level06.unity",
                "Assets/_Game/Scenes/Levels/Level07.unity"
            };

            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[scenePaths.Length];
            for (int i = 0; i < scenePaths.Length; i++)
            {
                buildScenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
            }

            EditorBuildSettings.scenes = buildScenes;
            AssetDatabase.SaveAssets();
            Debug.Log($"[ChamberBuilder] Registered {buildScenes.Length} scenes in EditorBuildSettings.");
        }

        private static Text CreateUIText(Transform parent, string name, string content, Font font, int size, 
            TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta, Color color)
        {
            GameObject txtObj = new GameObject(name);
            txtObj.transform.SetParent(parent, false);

            RectTransform rt = txtObj.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = anchorMin;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            Text text = txtObj.AddComponent<Text>();
            text.text = content;
            text.font = font;
            text.fontSize = size;
            text.alignment = alignment;
            text.color = color;
            text.supportRichText = true;

            return text;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var type = target.GetType();
            System.Reflection.FieldInfo field = null;
            while (type != null && field == null)
            {
                field = type.GetField(fieldName, 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance | 
                    System.Reflection.BindingFlags.Public);
                type = type.BaseType;
            }
            if (field != null)
            {
                field.SetValue(target, value);
            }
        }
        #endregion
    }
}
