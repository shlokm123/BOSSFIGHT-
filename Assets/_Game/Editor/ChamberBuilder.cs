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

        [MenuItem("GlitchVerse/Build All Six Levels")]
        public static void BuildAll()
        {
            Debug.Log("[ChamberBuilder] Beginning autonomous build of all 6 levels (Phase 09 Upgrade)...");
            GenerateProceduralSprites();
            
            ConstructLevel01Scene();
            ConstructLevel02Scene();
            ConstructLevel03Scene();
            ConstructLevel04Scene();
            ConstructLevel05Scene();
            ConstructLevel06Scene();

            RegisterScenesInBuildSettings();
            Debug.Log("[ChamberBuilder] All six levels successfully built and registered!");
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

            // 17. Trap Floor (64x64) - Floor tile with subtle warning hairline fissure cracks
            CreateSpriteAsset("trap_floor.png", 64, 64, (x, y) =>
            {
                bool isCrack = (x >= 20 && x <= 22 && y >= 48 && y <= 59) ||
                               (x >= 22 && x <= 25 && y >= 38 && y <= 48) ||
                               (x >= 25 && x <= 28 && y >= 25 && y <= 38) ||
                               (x >= 28 && x <= 33 && y >= 12 && y <= 25) ||
                               (x >= 33 && x <= 36 && y >= 4 && y <= 12);

                if (isCrack) return new Color(0.42f, 0.12f, 0.15f, 1f);
                if (y >= 60) return new Color(0f, 0.94f, 1f, 1f);
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

            AssetDatabase.Refresh();
        }

        private static void CreateSpriteAsset(string filename, int width, int height, System.Func<int, int, Color> pixelFunc)
        {
            string fullPath = Path.Combine(ArtPath, filename);
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
            SetPrivateField(director, "levelTitle", isFinal ? "CONTAINMENT 04" : $"CONTAINMENT 0{chamberNum}");
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

        #region Level 01: The First Lie (Tutorial & Basic Push)
        public static void ConstructLevel01Scene()
        {
            var atmo = new ChapterAtmosphereConfig
            {
                ambientColor = new Color(0.85f, 0.62f, 0.38f, 1f), // Warm amber emergency lighting
                ambientIntensity = 0.78f,
                spawnLampColor = new Color(1.0f, 0.68f, 0.22f, 1f),
                spawnLampIntensity = 1.1f,
                spawnLampFlicker = FlickeringLight2D.FlickerMode.SubtleFluorescent,
                particleType = AtmosphericParticleEmitter.AtmosphereType.ContainmentDust,
                architectMonitorColor = new Color(1.0f, 0.70f, 0.20f, 0.95f)
            };

            var refs = SetupBaseChamber("CONTAINMENT 01 // FIRST LIE", 1, "Level02", false, atmo);

            Sprite floorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_tile.png");
            Sprite cubeSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/cube_crate.png");
            Sprite plateSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/pressure_plate.png");
            Sprite grateSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/wall_grate.png");
            Sprite cableSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/cable_bundle.png");
            Sprite hazardSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/hazard_stripes.png");
            Sprite subjDecalSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/decal_subject075.png");
            Sprite debrisSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_debris.png");
            Sprite beaconSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/warning_beacon.png");

            // Environmental Dressing: Ceiling Cables
            float[] cableXs = new float[] { -6f, -2f, 2f, 6f };
            foreach (var cx in cableXs)
            {
                GameObject cableObj = new GameObject($"Ceiling_Cables_{cx}");
                cableObj.transform.SetParent(refs.envRoot.transform);
                cableObj.transform.position = new Vector3(cx, 4.4f, 0f);
                cableObj.transform.localScale = new Vector3(1.5f, 1.0f, 1f);
                SpriteRenderer cRend = cableObj.AddComponent<SpriteRenderer>();
                cRend.sprite = cableSp;
                cRend.sortingOrder = -5;
            }

            // Wall Grate & Subject 075 Identification Decal near spawn
            GameObject grateObj = new GameObject("Deco_WallGrate");
            grateObj.transform.SetParent(refs.envRoot.transform);
            grateObj.transform.position = new Vector3(-8.0f, -0.6f, 0f);
            SpriteRenderer gRend = grateObj.AddComponent<SpriteRenderer>();
            gRend.sprite = grateSp;
            gRend.sortingOrder = -5;

            GameObject subjDecalObj = new GameObject("Deco_Subject075_Decal");
            subjDecalObj.transform.SetParent(refs.envRoot.transform);
            subjDecalObj.transform.position = new Vector3(-8.0f, 1.2f, 0f);
            SpriteRenderer sdRend = subjDecalObj.AddComponent<SpriteRenderer>();
            sdRend.sprite = subjDecalSp;
            sdRend.sortingOrder = -5;

            // Hazard Warning Stripes on floor beneath Platform A
            GameObject hazardObj = new GameObject("Deco_HazardStripes");
            hazardObj.transform.SetParent(refs.envRoot.transform);
            hazardObj.transform.position = new Vector3(-3.0f, -3.88f, 0f);
            hazardObj.transform.localScale = new Vector3(3.2f, 0.25f, 1f);
            SpriteRenderer hzRend = hazardObj.AddComponent<SpriteRenderer>();
            hzRend.sprite = hazardSp;
            hzRend.sortingOrder = 1;

            // Floor Debris
            GameObject debris1 = new GameObject("Deco_FloorDebris_1");
            debris1.transform.SetParent(refs.envRoot.transform);
            debris1.transform.position = new Vector3(-9.2f, -3.9f, 0f);
            SpriteRenderer debRend1 = debris1.AddComponent<SpriteRenderer>();
            debRend1.sprite = debrisSp;
            debRend1.sortingOrder = 1;

            GameObject debris2 = new GameObject("Deco_FloorDebris_2");
            debris2.transform.SetParent(refs.envRoot.transform);
            debris2.transform.position = new Vector3(5.5f, -3.9f, 0f);
            SpriteRenderer debRend2 = debris2.AddComponent<SpriteRenderer>();
            debRend2.sprite = debrisSp;
            debRend2.sortingOrder = 1;

            // Warning Beacon above Decoy Exit
            GameObject beaconObj = new GameObject("Deco_WarningBeacon_Exit");
            beaconObj.transform.SetParent(refs.puzzleRoot.transform);
            beaconObj.transform.position = new Vector3(8.5f, -1.2f, 0f);
            SpriteRenderer bRend = beaconObj.AddComponent<SpriteRenderer>();
            bRend.sprite = beaconSp;
            bRend.sortingOrder = 5;

            GameObject beaconLightObj = new GameObject("Beacon_Light");
            beaconLightObj.transform.SetParent(beaconObj.transform);
            beaconLightObj.transform.localPosition = Vector3.zero;
            Light2D bLight = beaconLightObj.AddComponent<Light2D>();
            bLight.lightType = Light2D.LightType.Point;
            bLight.pointLightOuterRadius = 2.2f;
            bLight.color = new Color(1.0f, 0.65f, 0.15f, 1f);
            bLight.intensity = 0.55f;

            FlickeringLight2D bFlicker = beaconLightObj.AddComponent<FlickeringLight2D>();
            bFlicker.Mode = FlickeringLight2D.FlickerMode.SubtleFluorescent;
            bFlicker.BaseIntensity = 0.55f;

            // Observation Platform A (X: -4.5 to -1.5, Y: -1.0)
            GameObject platformA = new GameObject("Observation_Platform_A");
            platformA.transform.SetParent(refs.envRoot.transform);
            platformA.transform.position = new Vector3(-3.0f, -1.0f, 0f);
            BoxCollider2D platCol = platformA.AddComponent<BoxCollider2D>();
            platCol.size = new Vector2(3.2f, 0.4f);

            for (float px = -4.5f; px <= -1.5f; px += 1.0f)
            {
                GameObject seg = new GameObject($"PlatSeg_{px}");
                seg.transform.SetParent(platformA.transform);
                seg.transform.position = new Vector3(px, -1.0f, 0f);
                seg.transform.localScale = new Vector3(1.0f, 0.4f, 1f);
                SpriteRenderer rend = seg.AddComponent<SpriteRenderer>();
                rend.sprite = floorSp;
                rend.sortingOrder = 1;
            }

            // Pressure Plate at (3.0, -3.9)
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

            // Movable Cube at (-2.0, -3.4)
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

            // Checkpoint 2: Scripted Fake Exit Prank (Level 01)
            GameObject realDoorObj = Object.Instantiate(refs.exitDoor.gameObject, refs.puzzleRoot.transform);
            realDoorObj.name = "RealExitDoor_PlatformA";
            realDoorObj.transform.position = new Vector3(-3.0f, 0.4f, 0f);
            realDoorObj.SetActive(false);
            ExitDoor realDoorComp = realDoorObj.GetComponent<ExitDoor>();

            FakeExitPrank prank = refs.exitDoor.gameObject.AddComponent<FakeExitPrank>();
            SetPrivateField(prank, "decoyDoor", refs.exitDoor);
            SetPrivateField(prank, "realDoor", realDoorComp);
            SetPrivateField(prank, "relocatedPosition", new Vector3(-3.0f, 0.4f, 0f));

            SaveChamberScene("Level01.unity");
        }
        #endregion

        #region Level 02: Gravity Is A Suggestion (Gravity Inversion & Echo Replay)
        public static void ConstructLevel02Scene()
        {
            var atmo = new ChapterAtmosphereConfig
            {
                ambientColor = new Color(0.38f, 0.68f, 0.82f, 1f), // Cool desaturated slate-blue/teal
                ambientIntensity = 0.72f,
                spawnLampColor = new Color(0.40f, 0.85f, 1.0f, 1f),
                spawnLampIntensity = 1.0f,
                spawnLampFlicker = FlickeringLight2D.FlickerMode.MalfunctioningArc,
                particleType = AtmosphericParticleEmitter.AtmosphereType.GravityAnomalies,
                architectMonitorColor = new Color(0.25f, 0.85f, 1.0f, 0.95f)
            };

            var refs = SetupBaseChamber("CONTAINMENT 02 // GRAVITY ECHO", 2, "Level03", false, atmo, collapsingFloorGapX: 1.0f);

            Sprite floorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_tile.png");
            Sprite trapFloorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/trap_floor.png");
            Sprite echoSwitchSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/echo_switch.png");
            Sprite cableSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/cable_bundle.png");
            Sprite hazardSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/hazard_stripes.png");
            Sprite debrisSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_debris.png");
            Sprite pillarSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/bg_pillar.png");

            // Collapsing Floor Betrayal Trap (Level 02)
            GameObject trapTileObj = new GameObject("CollapsingFloorTrap");
            trapTileObj.transform.SetParent(refs.puzzleRoot.transform);
            trapTileObj.transform.position = new Vector3(1.0f, -4.0f, 0f);

            SpriteRenderer trapRend = trapTileObj.AddComponent<SpriteRenderer>();
            trapRend.sprite = trapFloorSp != null ? trapFloorSp : floorSp;
            trapRend.sortingOrder = 1;

            BoxCollider2D trapCol = trapTileObj.AddComponent<BoxCollider2D>();
            trapCol.size = new Vector2(1.2f, 1.0f);

            CollapsingFloor trapComp = trapTileObj.AddComponent<CollapsingFloor>();
            SetPrivateField(trapComp, "floorRenderer", trapRend);
            SetPrivateField(trapComp, "solidCollider", trapCol);

            // Enable Gravity Inversion power on player
            refs.abilityCtrl.AllowGravity = true;

            // Environmental Dressing: Suspended inverted machinery in mid-air background
            GameObject floatDebris1 = new GameObject("Deco_FloatingMachinery_1");
            floatDebris1.transform.SetParent(refs.envRoot.transform);
            floatDebris1.transform.position = new Vector3(-1.5f, 1.8f, 0f);
            floatDebris1.transform.localScale = new Vector3(1.2f, -1.2f, 1f);
            SpriteRenderer fdRend1 = floatDebris1.AddComponent<SpriteRenderer>();
            fdRend1.sprite = pillarSp;
            fdRend1.sortingOrder = -15;
            fdRend1.color = new Color(0.6f, 0.75f, 0.85f, 0.35f);

            GameObject floatDebris2 = new GameObject("Deco_FloatingMachinery_2");
            floatDebris2.transform.SetParent(refs.envRoot.transform);
            floatDebris2.transform.position = new Vector3(2.5f, 2.4f, 0f);
            floatDebris2.transform.localScale = new Vector3(0.8f, -0.8f, 1f);
            SpriteRenderer fdRend2 = floatDebris2.AddComponent<SpriteRenderer>();
            fdRend2.sprite = debrisSp;
            fdRend2.sortingOrder = -15;
            fdRend2.color = new Color(0.7f, 0.85f, 0.95f, 0.45f);

            // Inverted Ceiling Cable Bundles
            GameObject ceilCables = new GameObject("Deco_CeilingCables_Inverted");
            ceilCables.transform.SetParent(refs.envRoot.transform);
            ceilCables.transform.position = new Vector3(-4.0f, 4.3f, 0f);
            ceilCables.transform.localScale = new Vector3(1.6f, -1.0f, 1f);
            SpriteRenderer ccRend = ceilCables.AddComponent<SpriteRenderer>();
            ccRend.sprite = cableSp;
            ccRend.sortingOrder = -5;

            // Malfunctioning light fixture near ceiling catwalk
            GameObject malfLightObj = new GameObject("Malf_CeilingLamp");
            malfLightObj.transform.SetParent(refs.envRoot.transform);
            malfLightObj.transform.position = new Vector3(-4.0f, 2.8f, 0f);
            Light2D malfLight = malfLightObj.AddComponent<Light2D>();
            malfLight.lightType = Light2D.LightType.Point;
            malfLight.pointLightOuterRadius = 3.5f;
            malfLight.color = new Color(0.3f, 0.8f, 1.0f, 1f);
            malfLight.intensity = 0.5f;

            FlickeringLight2D malfFlicker = malfLightObj.AddComponent<FlickeringLight2D>();
            malfFlicker.Mode = FlickeringLight2D.FlickerMode.MalfunctioningArc;
            malfFlicker.BaseIntensity = 0.5f;

            // Elevated Exit Doorway at (X: 8.5, Y: 0.5)
            refs.exitDoor.transform.position = new Vector3(8.5f, 0.5f, 0f);

            // Midground Access Catwalk under Exit Door (X: 5.5 to 10.5, Y: -0.8)
            GameObject exitCatwalk = new GameObject("Exit_Catwalk");
            exitCatwalk.transform.SetParent(refs.envRoot.transform);
            exitCatwalk.transform.position = new Vector3(8.0f, -0.8f, 0f);
            BoxCollider2D catCol = exitCatwalk.AddComponent<BoxCollider2D>();
            catCol.size = new Vector2(5.5f, 0.4f);

            for (float px = 5.5f; px <= 10.5f; px += 1.0f)
            {
                GameObject seg = new GameObject($"CatwalkSeg_{px}");
                seg.transform.SetParent(exitCatwalk.transform);
                seg.transform.position = new Vector3(px, -0.8f, 0f);
                seg.transform.localScale = new Vector3(1.0f, 0.4f, 1f);
                SpriteRenderer rend = seg.AddComponent<SpriteRenderer>();
                rend.sprite = floorSp;
                rend.sortingOrder = 1;
            }

            // Inverted Ceiling Catwalk where the EchoSwitch is mounted (X: -6.0 to -2.0, Y: 3.8)
            GameObject ceilingCatwalk = new GameObject("Ceiling_Catwalk");
            ceilingCatwalk.transform.SetParent(refs.envRoot.transform);
            ceilingCatwalk.transform.position = new Vector3(-4.0f, 3.8f, 0f);
            BoxCollider2D ceilCatCol = ceilingCatwalk.AddComponent<BoxCollider2D>();
            ceilCatCol.size = new Vector2(4.5f, 0.4f);

            for (float px = -6.0f; px <= -2.0f; px += 1.0f)
            {
                GameObject seg = new GameObject($"CeilSeg_{px}");
                seg.transform.SetParent(ceilingCatwalk.transform);
                seg.transform.position = new Vector3(px, 3.8f, 0f);
                seg.transform.localScale = new Vector3(1.0f, -0.4f, 1f);
                SpriteRenderer rend = seg.AddComponent<SpriteRenderer>();
                rend.sprite = floorSp;
                rend.sortingOrder = 1;
            }

            // Hazard stripe trim on ceiling catwalk
            GameObject ceilHazard = new GameObject("Deco_CeilCatwalk_Hazard");
            ceilHazard.transform.SetParent(ceilingCatwalk.transform);
            ceilHazard.transform.position = new Vector3(-4.0f, 3.98f, 0f);
            ceilHazard.transform.localScale = new Vector3(4.2f, -0.2f, 1f);
            SpriteRenderer chRend = ceilHazard.AddComponent<SpriteRenderer>();
            chRend.sprite = hazardSp;
            chRend.sortingOrder = 2;

            // EchoSwitch mounted on the ceiling catwalk at (-4.0, 3.5)
            GameObject echoSwitchObj = new GameObject("EchoSwitch_Ceiling");
            echoSwitchObj.transform.SetParent(refs.puzzleRoot.transform);
            echoSwitchObj.transform.position = new Vector3(-4.0f, 3.5f, 0f);
            echoSwitchObj.transform.localScale = new Vector3(1.0f, -1.0f, 1f);

            SpriteRenderer switchRend = echoSwitchObj.AddComponent<SpriteRenderer>();
            switchRend.sprite = echoSwitchSp;
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
            switchLight.intensity = 0.5f;

            EchoSwitch echoSwitchComp = echoSwitchObj.AddComponent<EchoSwitch>();
            SetPrivateField(echoSwitchComp, "switchRenderer", switchRend);
            SetPrivateField(echoSwitchComp, "glowLight", switchLight);
            SetPrivateField(echoSwitchComp, "depressVisual", echoSwitchObj.transform);

            SetPrivateField(refs.hud, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.director, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.triggers, "echoSwitch", echoSwitchComp);

            // Checkpoint 4: Fake Emergency Countdown Prank (Level 02)
            GameObject emergencyPrankObj = new GameObject("FakeEmergencyPrankController");
            emergencyPrankObj.transform.SetParent(refs.puzzleRoot.transform);
            emergencyPrankObj.AddComponent<FakeEmergencyPrank>();

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
            SetPrivateField(questionPrank, "triggerX", -4.0f);

            // Moving Freezable Hazard Laser 1
            GameObject hazardObj1 = new GameObject("FreezableLaser_Barrier_1");
            hazardObj1.transform.SetParent(refs.puzzleRoot.transform);
            hazardObj1.transform.position = new Vector3(-2.0f, -3.5f, 0f);

            SpriteRenderer hazRend1 = hazardObj1.AddComponent<SpriteRenderer>();
            hazRend1.sprite = hazardSp;
            hazRend1.sortingOrder = 3;

            GameObject hazLight1 = new GameObject("Hazard1_Glow");
            hazLight1.transform.SetParent(hazardObj1.transform);
            hazLight1.transform.localPosition = Vector3.zero;
            Light2D hl1 = hazLight1.AddComponent<Light2D>();
            hl1.lightType = Light2D.LightType.Point;
            hl1.pointLightOuterRadius = 1.8f;
            hl1.color = new Color(1.0f, 0.15f, 0.25f, 1f);
            hl1.intensity = 0.6f;

            BoxCollider2D hazCol1 = hazardObj1.AddComponent<BoxCollider2D>();
            hazCol1.size = new Vector2(0.5f, 1.8f);
            hazCol1.isTrigger = true;

            FreezableHazard hazardComp1 = hazardObj1.AddComponent<FreezableHazard>();
            SetPrivateField(hazardComp1, "offsetA", Vector3.zero);
            SetPrivateField(hazardComp1, "offsetB", new Vector3(0f, 4.0f, 0f));
            SetPrivateField(hazardComp1, "moveSpeed", 2.8f);
            SetPrivateField(hazardComp1, "hazardRenderer", hazRend1);

            // Moving Freezable Hazard Laser 2
            GameObject hazardObj2 = new GameObject("FreezableLaser_Barrier_2");
            hazardObj2.transform.SetParent(refs.puzzleRoot.transform);
            hazardObj2.transform.position = new Vector3(2.0f, 0.5f, 0f);

            SpriteRenderer hazRend2 = hazardObj2.AddComponent<SpriteRenderer>();
            hazRend2.sprite = hazardSp;
            hazRend2.sortingOrder = 3;

            GameObject hazLight2 = new GameObject("Hazard2_Glow");
            hazLight2.transform.SetParent(hazardObj2.transform);
            hazLight2.transform.localPosition = Vector3.zero;
            Light2D hl2 = hazLight2.AddComponent<Light2D>();
            hl2.lightType = Light2D.LightType.Point;
            hl2.pointLightOuterRadius = 1.8f;
            hl2.color = new Color(1.0f, 0.15f, 0.25f, 1f);
            hl2.intensity = 0.6f;

            BoxCollider2D hazCol2 = hazardObj2.AddComponent<BoxCollider2D>();
            hazCol2.size = new Vector2(0.5f, 1.8f);
            hazCol2.isTrigger = true;

            FreezableHazard hazardComp2 = hazardObj2.AddComponent<FreezableHazard>();
            SetPrivateField(hazardComp2, "offsetA", Vector3.zero);
            SetPrivateField(hazardComp2, "offsetB", new Vector3(0f, -4.0f, 0f));
            SetPrivateField(hazardComp2, "moveSpeed", 3.2f);
            SetPrivateField(hazardComp2, "hazardRenderer", hazRend2);

            // Puzzle Plate across the hazards at (X: 6.0, Y: -3.9)
            GameObject plateObj = new GameObject("PressurePlate");
            plateObj.transform.SetParent(refs.puzzleRoot.transform);
            plateObj.transform.position = new Vector3(6.0f, -3.9f, 0f);

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

            // Movable Cube before the hazards at (X: -5.0, Y: -3.4)
            GameObject cubeObj = new GameObject("MovableCube");
            cubeObj.transform.SetParent(refs.puzzleRoot.transform);
            cubeObj.transform.position = new Vector3(-5.0f, -3.4f, 0f);

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

            // Dimension Alpha Platform 1 (X: -4.0, Y: -1.5)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimAlpha_Platform_1", 
                new Vector3(-4.0f, -1.5f, 0f), new Vector2(3.5f, 0.4f), alphaSp, DimensionType.Alpha);

            // Dimension Alpha Platform 2 with EchoSwitch (X: 1.0, Y: 1.0)
            GameObject alphaPlat2 = CreateDimensionPlatform(refs.puzzleRoot.transform, "DimAlpha_Platform_2", 
                new Vector3(1.0f, 1.0f, 0f), new Vector2(3.5f, 0.4f), alphaSp, DimensionType.Alpha);

            // EchoSwitch mounted on Alpha Platform 2 at (1.0, 1.3)
            GameObject echoSwitchObj = new GameObject("EchoSwitch_Dimension");
            echoSwitchObj.transform.SetParent(alphaPlat2.transform);
            echoSwitchObj.transform.localPosition = new Vector3(0f, 0.35f, 0f);

            SpriteRenderer switchRend = echoSwitchObj.AddComponent<SpriteRenderer>();
            switchRend.sprite = echoSwitchSp;
            switchRend.sortingOrder = 4;

            BoxCollider2D switchCol = echoSwitchObj.AddComponent<BoxCollider2D>();
            switchCol.isTrigger = true;
            switchCol.size = new Vector2(1.2f, 0.4f);

            EchoSwitch echoSwitchComp = echoSwitchObj.AddComponent<EchoSwitch>();
            SetPrivateField(echoSwitchComp, "switchRenderer", switchRend);
            SetPrivateField(echoSwitchComp, "depressVisual", echoSwitchObj.transform);

            // Dimension Beta Platform 1 (Bridge spanning chasm, X: -1.5, Y: -1.5)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimBeta_Platform_1", 
                new Vector3(-1.0f, -1.5f, 0f), new Vector2(3.5f, 0.4f), betaSp, DimensionType.Beta);

            // Dimension Beta Platform 2 (Approaching final ceiling breach, X: 5.0, Y: 1.5)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimBeta_Platform_2", 
                new Vector3(5.0f, 1.5f, 0f), new Vector2(3.5f, 0.4f), betaSp, DimensionType.Beta);

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
            verticalConduit.transform.position = new Vector3(-7.5f, 0f, 0f);
            verticalConduit.transform.rotation = Quaternion.Euler(0, 0, 90f);
            verticalConduit.transform.localScale = new Vector3(8f, 1.2f, 1f);
            SpriteRenderer vcRend = verticalConduit.AddComponent<SpriteRenderer>();
            vcRend.sprite = pipeSp;
            vcRend.sortingOrder = -6;
            vcRend.color = new Color(0.2f, 0.8f, 1.0f, 0.9f);

            // Ceiling Bulkhead Pathway (Y = 3.8)
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

            // EchoSwitch inverted on the ceiling walkway at (X: -2.0, Y: 3.4)
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

            EchoSwitch echoSwitchComp = echoSwitchObj.AddComponent<EchoSwitch>();
            SetPrivateField(echoSwitchComp, "switchRenderer", switchRend);
            SetPrivateField(echoSwitchComp, "depressVisual", echoSwitchObj.transform);

            // Dimension Alpha Platform (Ceiling mid-span, X: 1.5, Y: 3.0)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimAlpha_Ceiling_1",
                new Vector3(1.5f, 3.0f, 0f), new Vector2(3.5f, 0.4f), alphaSp, DimensionType.Alpha);

            // Dimension Beta Platform (Ceiling bridge to vault, X: 4.8, Y: 3.2)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimBeta_Ceiling_2",
                new Vector3(4.8f, 3.2f, 0f), new Vector2(3.5f, 0.4f), betaSp, DimensionType.Beta);

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

            var refs = SetupBaseChamber("CONTAINMENT 06 // TRUTH", 6, "", true, atmo);

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

            // Add Narrative Progression Trigger Controller
            GameObject truthTriggersObj = new GameObject("TruthChamberNarrativeTriggers");
            truthTriggersObj.transform.SetParent(refs.puzzleRoot.transform);
            TruthChamberTriggers truthTriggers = truthTriggersObj.AddComponent<TruthChamberTriggers>();
            SetPrivateField(truthTriggers, "stage2TriggerX", 1.0f);
            SetPrivateField(truthTriggers, "stage3TriggerX", 6.5f);

            // --- STAGE 1: Blast Gate & Elevated Echo Switch ---
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

            EchoSwitch echoSwitchComp = echoSwitchObj.AddComponent<EchoSwitch>();
            SetPrivateField(echoSwitchComp, "switchRenderer", switchRend);
            SetPrivateField(echoSwitchComp, "depressVisual", echoSwitchObj.transform);

            // Stage 1 Retracting Blast Gate at X: -3.0
            GameObject blastGateObj = new GameObject("Stage1_BlastGate");
            blastGateObj.transform.SetParent(refs.puzzleRoot.transform);
            blastGateObj.transform.position = new Vector3(-3.0f, -2.5f, 0f);
            SpriteRenderer bgRend = blastGateObj.AddComponent<SpriteRenderer>();
            bgRend.sprite = wallSp;
            bgRend.drawMode = SpriteDrawMode.Tiled;
            bgRend.size = new Vector2(0.8f, 3.2f);
            bgRend.sortingOrder = 3;
            bgRend.color = new Color(0.8f, 0.2f, 0.25f, 1f);
            BoxCollider2D bgCol = blastGateObj.AddComponent<BoxCollider2D>();
            bgCol.size = new Vector2(0.8f, 3.2f);

            // Wire Stage 1 EchoSwitch to control retracting blast gate via TruthChamberTriggers
            SetPrivateField(truthTriggers, "echoSwitch", echoSwitchComp);
            SetPrivateField(truthTriggers, "blastGate", blastGateObj);

            // --- STAGE 2: Electrified Pit & Freezable Moving Laser Hazard ---
            // Create FreezableHazard that sweeps across the pit at Y = -2.5
            GameObject hazardObj = new GameObject("Stage2_FreezableLaserHazard");
            hazardObj.transform.SetParent(refs.puzzleRoot.transform);
            hazardObj.transform.position = new Vector3(-1.0f, -2.5f, 0f);

            SpriteRenderer hzRend = hazardObj.AddComponent<SpriteRenderer>();
            hzRend.sprite = hazardLaserSp;
            hzRend.drawMode = SpriteDrawMode.Tiled;
            hzRend.size = new Vector2(1.8f, 0.8f);
            hzRend.sortingOrder = 4;
            hzRend.color = new Color(1.0f, 0.15f, 0.25f, 1f);

            BoxCollider2D hzCol = hazardObj.AddComponent<BoxCollider2D>();
            hzCol.size = new Vector2(1.8f, 0.8f);

            FreezableHazard fHazard = hazardObj.AddComponent<FreezableHazard>();
            SetPrivateField(fHazard, "offsetA", new Vector3(-1.2f, 0f, 0f));
            SetPrivateField(fHazard, "offsetB", new Vector3(2.5f, 0f, 0f));
            SetPrivateField(fHazard, "moveSpeed", 3.2f);
            SetPrivateField(fHazard, "hazardRenderer", hzRend);

            // Ground platform after the pit at X: 3.2, Y: -4.0
            GameObject pitExitPlat = new GameObject("Stage2_PitExitFloor");
            pitExitPlat.transform.SetParent(refs.puzzleRoot.transform);
            pitExitPlat.transform.position = new Vector3(3.2f, -4.0f, 0f);
            SpriteRenderer peRend = pitExitPlat.AddComponent<SpriteRenderer>();
            peRend.sprite = floorSp;
            peRend.drawMode = SpriteDrawMode.Tiled;
            peRend.size = new Vector2(2.5f, 1.0f);
            peRend.sortingOrder = 2;
            BoxCollider2D peCol = pitExitPlat.AddComponent<BoxCollider2D>();
            peCol.size = new Vector2(2.5f, 1.0f);

            // --- STAGE 3: Massive Lower Barrier & Inverted Ceiling Dimension Shaft ---
            // Huge vertical bulkhead from floor up to Y: 1.5 at X: 4.5
            GameObject vaultBulkhead = new GameObject("Stage3_VaultBarrier");
            vaultBulkhead.transform.SetParent(refs.puzzleRoot.transform);
            vaultBulkhead.transform.position = new Vector3(4.5f, -1.5f, 0f);
            SpriteRenderer vbRend = vaultBulkhead.AddComponent<SpriteRenderer>();
            vbRend.sprite = wallSp;
            vbRend.drawMode = SpriteDrawMode.Tiled;
            vbRend.size = new Vector2(1.0f, 5.5f);
            vbRend.sortingOrder = 3;
            vbRend.color = new Color(0.2f, 0.22f, 0.28f, 1f);
            BoxCollider2D vbCol = vaultBulkhead.AddComponent<BoxCollider2D>();
            vbCol.size = new Vector2(1.0f, 5.5f);

            // Upper Ceiling Dimension Platforms (requires Gravity Inversion [G] to walk on ceiling):
            // Dimension Alpha Platform (Ceiling, X: 4.2, Y: 2.8)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimAlpha_Ceiling_Stage3",
                new Vector3(4.2f, 2.8f, 0f), new Vector2(3.0f, 0.4f), alphaSp, DimensionType.Alpha);

            // Dimension Beta Platform (Ceiling, X: 7.0, Y: 2.8)
            CreateDimensionPlatform(refs.puzzleRoot.transform, "DimBeta_Ceiling_Stage3",
                new Vector3(7.0f, 2.8f, 0f), new Vector2(3.0f, 0.4f), betaSp, DimensionType.Beta);

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

            // Final Quarantine Exit Door at X: 8.5, Y: -1.0
            refs.exitDoor.transform.position = new Vector3(8.5f, -1.1f, 0f);
            refs.exitDoor.SetUnlocked(true); // Reaching vault door allows breach into ending sequence

            // Pressure Plate inside vault to trigger door seal
            GameObject plateObj = new GameObject("Vault_FinalTriggerPlate");
            plateObj.transform.SetParent(refs.puzzleRoot.transform);
            plateObj.transform.position = new Vector3(7.0f, -1.6f, 0f);
            SpriteRenderer plateRend = plateObj.AddComponent<SpriteRenderer>();
            plateRend.sprite = plateSp;
            plateRend.sortingOrder = 2;
            BoxCollider2D plateCol = plateObj.AddComponent<BoxCollider2D>();
            plateCol.isTrigger = true;
            plateCol.size = new Vector2(1.2f, 0.3f);
            PressurePlate vaultPlate = plateObj.AddComponent<PressurePlate>();
            SetPrivateField(vaultPlate, "plateRenderer", plateRend);
            SetPrivateField(vaultPlate, "depressVisual", plateObj.transform);

            // Wire Director and HUD
            SetPrivateField(refs.hud, "pressurePlate", vaultPlate);
            SetPrivateField(refs.hud, "echoSwitch", echoSwitchComp);
            SetPrivateField(refs.director, "pressurePlate", vaultPlate);
            SetPrivateField(refs.director, "echoSwitch", null);
            SetPrivateField(refs.triggers, "echoSwitch", echoSwitchComp);

            SaveChamberScene("Level06.unity");
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
                "Assets/_Game/Scenes/Levels/Level06.unity"
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
            var field = target.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.Public);
            if (field != null)
            {
                field.SetValue(target, value);
            }
        }
        #endregion
    }
}
