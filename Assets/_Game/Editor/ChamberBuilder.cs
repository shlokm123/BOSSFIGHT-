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
using GlitchVerse.UI;

namespace GlitchVerse.Editor
{
    /// <summary>
    /// Autonomous Builder that procedurally creates stylized 2D sci-fi containment assets
    /// and constructs the complete Level01.unity chamber scene with physics, lighting, and HUD.
    /// </summary>
    public static class ChamberBuilder
    {
        private const string ArtPath = "Assets/_Game/Art";
        private const string ScenePath = "Assets/_Game/Scenes/Levels/Level01.unity";

        [MenuItem("GlitchVerse/Build Level 01 Chamber")]
        public static void BuildAll()
        {
            Debug.Log("[ChamberBuilder] Beginning autonomous environment build...");
            GenerateProceduralSprites();
            ConstructLevel01Scene();
            Debug.Log("[ChamberBuilder] Build complete! Level01.unity is ready.");
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
                if (x % 16 == 0 || y % 16 == 0) return new Color(0.10f, 0.12f, 0.16f, 1f); // Grid seam
                if ((x + y) % 8 == 0) return new Color(0.18f, 0.22f, 0.28f, 1f); // Tread texture
                return new Color(0.14f, 0.17f, 0.22f, 1f); // Base dark steel
            });

            // 2. Wall Panel (64x64) - Monolithic brutalist steel bulkhead
            CreateSpriteAsset("wall_panel.png", 64, 64, (x, y) =>
            {
                if (x <= 3 || x >= 60 || y <= 3 || y >= 60) return new Color(0.08f, 0.10f, 0.13f, 1f); // Outer bezel
                if ((x == 6 || x == 57) && (y == 6 || y == 57)) return new Color(0.4f, 0.45f, 0.5f, 1f); // Rivet
                if (y >= 45 && y <= 47) return new Color(0.85f, 0.45f, 0.1f, 0.8f); // Amber warning stripe
                if (y >= 41 && y <= 43) return new Color(0.10f, 0.10f, 0.12f, 1f);
                return new Color(0.16f, 0.19f, 0.24f, 1f); // Base concrete/steel
            });

            // 3. Ceiling Beam (64x32) - Heavy structural I-beam
            CreateSpriteAsset("ceiling_beam.png", 64, 32, (x, y) =>
            {
                if (y <= 4 || y >= 27) return new Color(0.11f, 0.13f, 0.17f, 1f); // Flanges
                if (x % 32 == 0) return new Color(0.07f, 0.08f, 0.11f, 1f); // Vertical strut
                if (y >= 14 && y <= 17) return new Color(0.20f, 0.24f, 0.30f, 1f); // Center web
                return new Color(0.13f, 0.16f, 0.20f, 1f);
            });

            // 4. Pipe Conduit (64x16) - Industrial pipe
            CreateSpriteAsset("pipe_conduit.png", 64, 16, (x, y) =>
            {
                if (x % 32 <= 2) return new Color(0.35f, 0.40f, 0.45f, 1f); // Coupling clamp
                if (y == 14 || y == 1) return new Color(0.09f, 0.11f, 0.14f, 1f); // Outline
                if (y >= 8 && y <= 11) return new Color(0.30f, 0.35f, 0.42f, 1f); // Highlight
                return new Color(0.18f, 0.22f, 0.27f, 1f); // Shadow side
            });

            // 5. Movable Cube (64x64) - High-tech containment crate with electric cyan core
            CreateSpriteAsset("cube_crate.png", 64, 64, (x, y) =>
            {
                // Border frame
                if (x <= 3 || x >= 60 || y <= 3 || y >= 60) return new Color(0f, 0.94f, 1f, 1f); // #00F0FF border
                // Corner inner supports
                if ((x <= 8 && y <= 8) || (x >= 55 && y <= 8) || (x <= 8 && y >= 55) || (x >= 55 && y >= 55))
                    return new Color(0f, 0.7f, 0.85f, 1f);
                // Glowing cyan diamond center core
                int distFromCenter = Mathf.Abs(x - 32) + Mathf.Abs(y - 32);
                if (distFromCenter <= 10) return new Color(0f, 0.94f, 1f, 1f);
                if (distFromCenter <= 14) return new Color(0f, 0.4f, 0.6f, 0.8f);
                // Dark inner panel
                return new Color(0.10f, 0.14f, 0.18f, 1f);
            });

            // 6. Pressure Plate (64x16) - Floor-embedded sensor pad
            CreateSpriteAsset("pressure_plate.png", 64, 16, (x, y) =>
            {
                if (y <= 2) return new Color(0.08f, 0.09f, 0.12f, 1f); // Base plate
                if (x <= 4 || x >= 59) return new Color(0.20f, 0.24f, 0.30f, 1f); // Edges
                if (y >= 8 && y <= 12 && x >= 8 && x <= 55) return new Color(0f, 0.94f, 1f, 1f); // Neon sensor strip
                return new Color(0.14f, 0.18f, 0.22f, 1f);
            });

            // 7. Exit Door (48x80) - Heavy sealed blast door with status light bar
            CreateSpriteAsset("exit_door.png", 48, 80, (x, y) =>
            {
                if (x <= 3 || x >= 44 || y <= 3 || y >= 76) return new Color(0.08f, 0.10f, 0.14f, 1f); // Heavy frame
                // Top status bar
                if (y >= 68 && y <= 73 && x >= 8 && x <= 39) return new Color(1.0f, 0.15f, 0.25f, 1f); // Status light (Red default)
                // Door panels
                if (x == 24) return new Color(0.05f, 0.06f, 0.08f, 1f); // Center seam
                if (y == 25 || y == 50) return new Color(0.05f, 0.06f, 0.08f, 1f); // Horizontal seams
                if ((x + y) % 12 <= 2) return new Color(0.85f, 0.55f, 0.1f, 0.7f); // Diagonal hazard markings on bottom
                return new Color(0.18f, 0.21f, 0.26f, 1f);
            });

            // 8. Player Sprite (32x56) - Stylized test subject silhouette with glowing cyan visor
            CreateSpriteAsset("player_sprite.png", 32, 56, (x, y) =>
            {
                // Head (y: 40 to 52, x: 10 to 22)
                if (y >= 40 && y <= 52 && x >= 10 && x <= 22)
                {
                    if (y >= 44 && y <= 47 && x >= 15 && x <= 22) return new Color(0f, 0.94f, 1f, 1f); // Visor
                    return new Color(0.15f, 0.18f, 0.23f, 1f); // Helmet
                }
                // Torso (y: 22 to 39, x: 8 to 24)
                if (y >= 22 && y <= 39 && x >= 8 && x <= 24)
                {
                    if (y >= 29 && y <= 33 && x >= 14 && x <= 18) return new Color(0f, 0.94f, 1f, 0.9f); // Chest power core
                    return new Color(0.20f, 0.25f, 0.32f, 1f); // Armor suit
                }
                // Legs (y: 2 to 21, left leg x: 8-14, right leg x: 18-24)
                if (y >= 2 && y <= 21)
                {
                    if ((x >= 9 && x <= 14) || (x >= 18 && x <= 23)) return new Color(0.14f, 0.17f, 0.21f, 1f);
                }
                return Color.clear; // Transparent background
            });

            // 9. Echo Ghost Sprite (32x56) - Translucent cyan holographic wireframe
            CreateSpriteAsset("echo_ghost.png", 32, 56, (x, y) =>
            {
                // Head silhouette
                if (y >= 40 && y <= 52 && x >= 10 && x <= 22)
                {
                    bool isEdge = (x == 10 || x == 22 || y == 40 || y == 52);
                    return isEdge ? new Color(0f, 0.94f, 1f, 0.8f) : new Color(0f, 0.7f, 0.9f, 0.35f);
                }
                // Torso
                if (y >= 22 && y <= 39 && x >= 8 && x <= 24)
                {
                    bool isEdge = (x == 8 || x == 24 || y == 22 || y == 39);
                    return isEdge ? new Color(0f, 0.94f, 1f, 0.8f) : new Color(0f, 0.7f, 0.9f, 0.35f);
                }
                // Legs
                if (y >= 2 && y <= 21 && ((x >= 9 && x <= 14) || (x >= 18 && x <= 23)))
                {
                    return new Color(0f, 0.94f, 1f, 0.45f);
                }
                return Color.clear;
            });

            // 10. Architect Monitor (128x64) - Overhead surveillance screen
            CreateSpriteAsset("architect_monitor.png", 128, 64, (x, y) =>
            {
                if (x <= 4 || x >= 123 || y <= 4 || y >= 59) return new Color(0.08f, 0.09f, 0.12f, 1f); // Bezel
                // Inner screen area
                if (x >= 8 && x <= 119 && y >= 8 && y <= 55)
                {
                    if (y % 4 == 0) return new Color(0.04f, 0.14f, 0.18f, 1f); // Scanline
                    if (x == 8 || x == 119 || y == 8 || y == 55) return new Color(0f, 0.94f, 1f, 0.8f); // Screen boundary
                    return new Color(0.06f, 0.18f, 0.22f, 1f); // Cyan CRT phosphor glow
                }
                return new Color(0.12f, 0.14f, 0.18f, 1f);
            });

            // 11. Background Column (48x128) - Receding structural pillar
            CreateSpriteAsset("bg_pillar.png", 48, 128, (x, y) =>
            {
                if (x <= 2 || x >= 45) return new Color(0.05f, 0.06f, 0.08f, 1f);
                if (y % 32 == 0) return new Color(0.06f, 0.07f, 0.09f, 1f);
                return new Color(0.09f, 0.11f, 0.14f, 1f); // Very dark muted slate
            });

            // 12. Background Reactor Core (128x128) - Giant circular containment vessel
            CreateSpriteAsset("bg_reactor.png", 128, 128, (x, y) =>
            {
                float dx = x - 64;
                float dy = y - 64;
                float r = Mathf.Sqrt(dx * dx + dy * dy);
                if (r > 60) return Color.clear;
                if (r >= 56 && r <= 60) return new Color(0.08f, 0.10f, 0.13f, 1f); // Outer ring
                if (r >= 38 && r <= 42) return new Color(0f, 0.5f, 0.7f, 0.6f); // Inner glowing conduit
                if (r < 20) return new Color(0f, 0.6f, 0.8f, 0.4f); // Core glow
                return new Color(0.07f, 0.08f, 0.11f, 0.9f);
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
                importer.spritePixelsPerUnit = 64; // 1 Unity unit = 64 pixels (standard 1m grid)
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }
        }
        #endregion

        #region Scene Construction
        public static void ConstructLevel01Scene()
        {
            // Create a fresh empty scene
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Load generated sprites
            Sprite floorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_tile.png");
            Sprite wallSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/wall_panel.png");
            Sprite beamSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/ceiling_beam.png");
            Sprite pipeSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/pipe_conduit.png");
            Sprite cubeSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/cube_crate.png");
            Sprite plateSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/pressure_plate.png");
            Sprite doorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/exit_door.png");
            Sprite playerSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/player_sprite.png");
            Sprite ghostSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/echo_ghost.png");
            Sprite monitorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/architect_monitor.png");
            Sprite pillarSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/bg_pillar.png");
            Sprite reactorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/bg_reactor.png");

            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") 
                            ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

            // --- 1. Camera & AudioListener ---
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            camObj.transform.position = new Vector3(0f, 1.0f, -10f);
            Camera cam = camObj.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6.0f; // Shows 21.3m width x 12m height
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.03f, 0.04f, 0.07f, 1f); // Deep midnight void
            camObj.AddComponent<AudioListener>();
            camObj.AddComponent<UniversalAdditionalCameraData>();

            // --- 2. Lighting Rig ---
            // 2A. Global Ambient Light
            GameObject globalLightObj = new GameObject("Global Light 2D");
            Light2D globalLight = globalLightObj.AddComponent<Light2D>();
            globalLight.lightType = Light2D.LightType.Global;
            globalLight.color = new Color(0.08f, 0.13f, 0.22f, 1f); // Cold dark navy
            globalLight.intensity = 0.32f;

            // 2B. Architect Monitor Spotlight (downward cone)
            GameObject architectSpotObj = new GameObject("Architect_Spotlight");
            architectSpotObj.transform.position = new Vector3(0f, 4.2f, 0f);
            architectSpotObj.transform.rotation = Quaternion.Euler(0f, 0f, 180f); // Point downwards
            Light2D architectSpot = architectSpotObj.AddComponent<Light2D>();
            architectSpot.lightType = Light2D.LightType.Point;
            architectSpot.pointLightInnerRadius = 0.5f;
            architectSpot.pointLightOuterRadius = 7.5f;
            architectSpot.color = new Color(0f, 0.94f, 1f, 1f);
            architectSpot.intensity = 0.85f;

            // 2C. Spawn Emergency Lamp (Amber)
            GameObject spawnLampObj = new GameObject("Spawn_EmergencyLamp");
            spawnLampObj.transform.position = new Vector3(-8.0f, -1.0f, 0f);
            Light2D spawnLamp = spawnLampObj.AddComponent<Light2D>();
            spawnLamp.lightType = Light2D.LightType.Point;
            spawnLamp.pointLightOuterRadius = 4.5f;
            spawnLamp.color = new Color(1.0f, 0.65f, 0.2f, 1f);
            spawnLamp.intensity = 0.65f;

            // --- 3. Background Architecture (Parallax Depth) ---
            GameObject bgRoot = new GameObject("--- BACKGROUND ---");
            
            // Giant Reactor silhouette (Layer -20)
            GameObject reactorObj = new GameObject("BG_ReactorCore");
            reactorObj.transform.SetParent(bgRoot.transform);
            reactorObj.transform.position = new Vector3(2.5f, 1.0f, 5.0f);
            reactorObj.transform.localScale = new Vector3(3.5f, 3.5f, 1f);
            SpriteRenderer reactorRend = reactorObj.AddComponent<SpriteRenderer>();
            reactorRend.sprite = reactorSp;
            reactorRend.sortingOrder = -20;
            reactorRend.color = new Color(1f, 1f, 1f, 0.45f);

            // Industrial Support Columns (Layer -10)
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

            // --- 4. Foreground / Midground Geometry ---
            GameObject envRoot = new GameObject("--- ENVIRONMENT ---");

            // 4A. Floor (Y = -4.0, from X = -11 to +11)
            GameObject floorComposite = new GameObject("Chamber_Floor");
            floorComposite.transform.SetParent(envRoot.transform);
            floorComposite.transform.position = new Vector3(0f, -4.5f, 0f);
            BoxCollider2D floorCol = floorComposite.AddComponent<BoxCollider2D>();
            floorCol.size = new Vector2(24f, 1.0f);

            for (float x = -11f; x <= 11f; x += 1.0f)
            {
                GameObject tile = new GameObject($"FloorTile_{x}");
                tile.transform.SetParent(floorComposite.transform);
                tile.transform.position = new Vector3(x, -4.0f, 0f);
                SpriteRenderer rend = tile.AddComponent<SpriteRenderer>();
                rend.sprite = floorSp;
                rend.sortingOrder = 0;
            }

            // 4B. Ceiling (Y = +5.5, from X = -11 to +11)
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
                tile.transform.localScale = new Vector3(1f, -1f, 1f); // Inverted beam
                SpriteRenderer rend = tile.AddComponent<SpriteRenderer>();
                rend.sprite = beamSp;
                rend.sortingOrder = 0;
            }

            // 4C. Left Wall (X = -10.5)
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

            // 4D. Right Wall (X = +10.5)
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

            // 4E. Elevated Observation Platform A (X: -4.5 to -1.5, Y: -1.0)
            GameObject platformA = new GameObject("Observation_Platform_A");
            platformA.transform.SetParent(envRoot.transform);
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

            // 4F. Horizontal Industrial Pipes (Y = 4.2 across room)
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

            // 4G. Overhead THE ARCHITECT Surveillance Display Unit
            GameObject monitorObj = new GameObject("Architect_Surveillance_Unit");
            monitorObj.transform.SetParent(envRoot.transform);
            monitorObj.transform.position = new Vector3(0f, 4.2f, 0f);
            monitorObj.transform.localScale = new Vector3(2.0f, 2.0f, 1f);
            SpriteRenderer monRend = monitorObj.AddComponent<SpriteRenderer>();
            monRend.sprite = monitorSp;
            monRend.sortingOrder = 2;

            // Monitor world text "SUBJECT 075 // EVALUATING"
            GameObject monTextObj = new GameObject("Monitor_Text");
            monTextObj.transform.SetParent(monitorObj.transform);
            monTextObj.transform.localPosition = new Vector3(0f, 0f, 0f);
            monTextObj.transform.localScale = new Vector3(0.015f, 0.015f, 1f);
            TextMesh monTm = monTextObj.AddComponent<TextMesh>();
            monTm.text = "SUBJECT 075\nEVALUATING";
            monTm.fontSize = 32;
            monTm.font = defaultFont;
            monTm.alignment = TextAlignment.Center;
            monTm.anchor = TextAnchor.MiddleCenter;
            monTm.color = new Color(0f, 0.94f, 1f, 0.95f);
            monTextObj.GetComponent<MeshRenderer>().sortingOrder = 3;

            // --- 5. Interactive Puzzle Components ---
            GameObject puzzleRoot = new GameObject("--- PUZZLE ---");

            // 5A. Pressure Plate (X: +3.0, Y: -3.85)
            GameObject plateObj = new GameObject("PressurePlate");
            plateObj.transform.SetParent(puzzleRoot.transform);
            plateObj.transform.position = new Vector3(3.0f, -3.9f, 0f);
            plateObj.tag = "Untagged";

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

            // 5B. Movable Cube (X: -2.0, Y: -3.4)
            GameObject cubeObj = new GameObject("MovableCube");
            cubeObj.transform.SetParent(puzzleRoot.transform);
            cubeObj.transform.position = new Vector3(-2.0f, -3.4f, 0f);
            cubeObj.tag = "Untagged";

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

            // Cyan point light attached to the cube
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

            // 5C. Exit Blast Door (X: +8.5, Y: -2.7)
            GameObject doorObj = new GameObject("ExitDoor");
            doorObj.transform.SetParent(puzzleRoot.transform);
            doorObj.transform.position = new Vector3(8.5f, -2.7f, 0f);

            SpriteRenderer doorRend = doorObj.AddComponent<SpriteRenderer>();
            doorRend.sprite = doorSp;
            doorRend.sortingOrder = 4;

            // Blocker collider
            BoxCollider2D doorBlocker = doorObj.AddComponent<BoxCollider2D>();
            doorBlocker.size = new Vector2(0.7f, 2.4f);
            doorBlocker.offset = new Vector2(0f, 0f);

            // Trigger collider for room completion
            GameObject exitTriggerObj = new GameObject("ExitTriggerZone");
            exitTriggerObj.transform.SetParent(doorObj.transform);
            exitTriggerObj.transform.localPosition = Vector3.zero;
            BoxCollider2D exitTrigger = exitTriggerObj.AddComponent<BoxCollider2D>();
            exitTrigger.isTrigger = true;
            exitTrigger.size = new Vector2(1.1f, 2.4f);
            exitTrigger.enabled = false;

            // Status light (Red default, turns Cyan when unlocked)
            GameObject doorLightObj = new GameObject("Door_StatusLight");
            doorLightObj.transform.SetParent(doorObj.transform);
            doorLightObj.transform.localPosition = new Vector3(0f, 1.0f, 0f);
            Light2D doorLight = doorLightObj.AddComponent<Light2D>();
            doorLight.lightType = Light2D.LightType.Point;
            doorLight.pointLightOuterRadius = 2.8f;
            doorLight.color = new Color(1.0f, 0.15f, 0.25f, 1f); // Emergency Red
            doorLight.intensity = 1.0f;

            ExitDoor exitComp = doorObj.AddComponent<ExitDoor>();
            SetPrivateField(exitComp, "solidBlockerCollider", doorBlocker);
            SetPrivateField(exitComp, "exitTriggerCollider", exitTrigger);
            SetPrivateField(exitComp, "doorFrameRenderer", doorRend);
            SetPrivateField(exitComp, "doorStatusLightRenderer", doorRend);
            SetPrivateField(exitComp, "statusLight2D", doorLight);

            // --- 6. Player Character & Spawn Point ---
            GameObject playerRoot = new GameObject("--- PLAYER ---");

            GameObject spawnObj = new GameObject("PlayerSpawnPoint");
            spawnObj.transform.SetParent(playerRoot.transform);
            spawnObj.transform.position = new Vector3(-8.0f, -3.2f, 0f);

            GameObject playerObj = new GameObject("Player_Subject075");
            playerObj.transform.SetParent(playerRoot.transform);
            playerObj.transform.position = spawnObj.transform.position;
            playerObj.tag = "Player";

            SpriteRenderer playerRend = playerObj.AddComponent<SpriteRenderer>();
            playerRend.sprite = playerSp;
            playerRend.sortingOrder = 5;

            BoxCollider2D playerCol = playerObj.AddComponent<BoxCollider2D>();
            playerCol.size = new Vector2(0.5f, 0.85f);
            playerCol.offset = new Vector2(0f, 0f);

            Rigidbody2D playerRb = playerObj.AddComponent<Rigidbody2D>();
            playerRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            playerRb.interpolation = RigidbodyInterpolation2D.Interpolate;
            playerRb.freezeRotation = true;
            playerRb.mass = 1.0f;

            // Subtle personal cyan visor light
            GameObject visorLightObj = new GameObject("Player_VisorLight");
            visorLightObj.transform.SetParent(playerObj.transform);
            visorLightObj.transform.localPosition = new Vector3(0.2f, 0.25f, 0f);
            Light2D visorLight = visorLightObj.AddComponent<Light2D>();
            visorLight.lightType = Light2D.LightType.Point;
            visorLight.pointLightOuterRadius = 1.6f;
            visorLight.color = new Color(0f, 0.94f, 1f, 1f);
            visorLight.intensity = 0.45f;

            // Ground check transform
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(playerObj.transform);
            groundCheckObj.transform.localPosition = new Vector3(0f, -0.45f, 0f);

            PlayerController2D playerCtrl = playerObj.AddComponent<PlayerController2D>();
            SetPrivateField(playerCtrl, "spriteRenderer", playerRend);
            SetPrivateField(playerCtrl, "groundCheckPoint", groundCheckObj.transform);
            SetPrivateField(playerCtrl, "moveSpeed", 6.5f);
            SetPrivateField(playerCtrl, "jumpForce", 11.5f);

            // Echo components
            EchoRecorder echoRec = playerObj.AddComponent<EchoRecorder>();
            SetPrivateField(echoRec, "targetPlayer", playerCtrl);

            EchoPlayback echoPlay = playerObj.AddComponent<EchoPlayback>();
            SetPrivateField(echoPlay, "recorder", echoRec);
            SetPrivateField(echoPlay, "ghostSprite", ghostSp);

            // --- 7. UI Canvas & HUD ---
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

            // Title Header (Top-Left)
            Text titleTxt = CreateUIText(uiRoot.transform, "TitleText", 
                "GLITCHVERSE: ECHO PROTOCOL", defaultFont, 24, TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -30f), new Vector2(600f, 40f), 
                new Color(0f, 0.94f, 1f, 1f));

            // Level Sub-Header (Top-Left)
            Text levelTxt = CreateUIText(uiRoot.transform, "LevelText", 
                "CONTAINMENT 01 // FIRST LIE", defaultFont, 16, TextAnchor.UpperLeft,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -65f), new Vector2(400f, 30f), 
                new Color(1f, 0.7f, 0.2f, 0.9f));

            // Instructions (Bottom-Left)
            Text instTxt = CreateUIText(uiRoot.transform, "InstructionsText", 
                "[A/D] Move   [SPACE] Jump   [R] Restart", defaultFont, 18, TextAnchor.LowerLeft,
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(40f, 40f), new Vector2(600f, 35f), 
                new Color(0.85f, 0.88f, 0.92f, 1f));

            // Echo Hint (Bottom-Right)
            Text echoTxt = CreateUIText(uiRoot.transform, "EchoHintText", 
                "[H] Replay Echo", defaultFont, 18, TextAnchor.LowerRight,
                new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-40f, 40f), new Vector2(300f, 35f), 
                new Color(0f, 0.94f, 1f, 1f));

            // Puzzle Telemetry (Top-Right)
            Text puzzleTxt = CreateUIText(uiRoot.transform, "PuzzleStatusText", 
                "○ PRESSURE PLATE: INACTIVE", defaultFont, 18, TextAnchor.UpperRight,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -35f), new Vector2(400f, 35f), 
                new Color(0.6f, 0.65f, 0.72f, 1f));

            // Door Telemetry (Top-Right below plate)
            Text doorTxt = CreateUIText(uiRoot.transform, "DoorStatusText", 
                "EXIT DOOR: LOCKED", defaultFont, 18, TextAnchor.UpperRight,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -65f), new Vector2(400f, 35f), 
                new Color(1.0f, 0.2f, 0.3f, 1f));

            // Completion Banner (Center)
            Text completeTxt = CreateUIText(uiRoot.transform, "CompletionBanner", 
                "TEST COMPLETE\n<size=20><color=#00F0FF>SIMULATION PROTOCOL VALIDATED — PRESS R TO RESET</color></size>", 
                defaultFont, 36, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(800f, 120f), 
                Color.white);
            completeTxt.gameObject.SetActive(false);

            LevelHUD hud = uiRoot.AddComponent<LevelHUD>();
            SetPrivateField(hud, "titleText", titleTxt);
            SetPrivateField(hud, "levelLabelText", levelTxt);
            SetPrivateField(hud, "instructionsText", instTxt);
            SetPrivateField(hud, "puzzleStatusText", puzzleTxt);
            SetPrivateField(hud, "doorStatusText", doorTxt);
            SetPrivateField(hud, "echoHintText", echoTxt);
            SetPrivateField(hud, "completionBannerText", completeTxt);
            SetPrivateField(hud, "pressurePlate", plateComp);
            SetPrivateField(hud, "exitDoor", exitComp);
            SetPrivateField(hud, "echoPlayback", echoPlay);

            // --- 8. Core Management Objects ---
            GameObject coreRoot = new GameObject("--- CORE_MANAGEMENT ---");
            
            // Level Director
            GameObject directorObj = new GameObject("LevelDirector");
            directorObj.transform.SetParent(coreRoot.transform);
            LevelDirector director = directorObj.AddComponent<LevelDirector>();
            SetPrivateField(director, "spawnPoint", spawnObj.transform);
            SetPrivateField(director, "pressurePlate", plateComp);
            SetPrivateField(director, "exitDoor", exitComp);
            SetPrivateField(director, "levelTitle", "CONTAINMENT 01");
            SetPrivateField(director, "chamberNumber", 1);
            SetPrivateField(hud, "levelDirector", director);

            // Game Session
            GameObject sessionObj = new GameObject("GameSession");
            sessionObj.transform.SetParent(coreRoot.transform);
            sessionObj.AddComponent<GameSession>();

            // Save Scene
            string sceneDir = Path.GetDirectoryName(ScenePath);
            if (!Directory.Exists(sceneDir))
            {
                Directory.CreateDirectory(sceneDir);
            }
            EditorSceneManager.SaveScene(newScene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[ChamberBuilder] Scene saved to {ScenePath}");
        }

        private static Text CreateUIText(Transform parent, string name, string content, Font font, int size, 
            TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta, Color color)
        {
            GameObject txtObj = new GameObject(name);
            txtObj.transform.SetParent(parent, false);

            RectTransform rt = txtObj.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = anchorMin; // Align pivot with anchor for natural offset
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
            else
            {
                Debug.LogWarning($"[ChamberBuilder] Field '{fieldName}' not found on type {target.GetType().Name}");
            }
        }
        #endregion
    }
}
