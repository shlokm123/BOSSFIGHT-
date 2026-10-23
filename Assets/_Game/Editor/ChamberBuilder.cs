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

namespace GlitchVerse.Editor
{
    /// <summary>
    /// Autonomous Builder that procedurally creates stylized 2D sci-fi containment assets
    /// and constructs all four playable levels (Level01 to Level04) for GlitchVerse: Echo Protocol.
    /// Follows emergency completion directive requirements.
    /// </summary>
    public static class ChamberBuilder
    {
        private const string ArtPath = "Assets/_Game/Art";
        private const string LevelsPath = "Assets/_Game/Scenes/Levels";

        [MenuItem("GlitchVerse/Build All Four Levels")]
        public static void BuildAll()
        {
            Debug.Log("[ChamberBuilder] Beginning autonomous build of all 4 levels...");
            GenerateProceduralSprites();
            
            ConstructLevel01Scene();
            ConstructLevel02Scene();
            ConstructLevel03Scene();
            ConstructLevel04Scene();

            RegisterScenesInBuildSettings();
            Debug.Log("[ChamberBuilder] All four levels successfully built and registered!");
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
                if (y >= 6 && y <= 14 && x >= 10 && x <= 53) return new Color(0.85f, 0f, 1f, 1f); // Magenta glow
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
                if (y <= 4 || y >= 59) return new Color(0.2f, 0.2f, 0.25f, 1f); // Metal emitter housing
                if (x >= 12 && x <= 19) return new Color(1.0f, 0.2f, 0.3f, 1f); // Core laser beam
                if (x >= 8 && x <= 23) return new Color(1.0f, 0.1f, 0.2f, 0.5f); // Red hazard aura
                return Color.clear;
            });

            // 10. Dimension Alpha Platform (64x16) - Electric Cyan
            CreateSpriteAsset("dim_alpha_platform.png", 64, 16, (x, y) =>
            {
                if (y >= 13) return new Color(0f, 0.94f, 1f, 1f); // Neon top edge
                if (x <= 2 || x >= 61 || y <= 2) return new Color(0f, 0.6f, 0.8f, 1f);
                return new Color(0.08f, 0.20f, 0.28f, 1f);
            });

            // 11. Dimension Beta Platform (64x16) - Vibrant Magenta
            CreateSpriteAsset("dim_beta_platform.png", 64, 16, (x, y) =>
            {
                if (y >= 13) return new Color(1.0f, 0.15f, 0.85f, 1f); // Neon top edge
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
            public GameObject envRoot;
            public GameObject puzzleRoot;
        }

        private static BaseSceneRefs SetupBaseChamber(string sceneTitle, int chamberNum, string nextScene, bool isFinal)
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
            cam.backgroundColor = new Color(0.03f, 0.04f, 0.07f, 1f);
            camObj.AddComponent<AudioListener>();
            camObj.AddComponent<UniversalAdditionalCameraData>();

            // 2. Lighting Rig
            GameObject globalLightObj = new GameObject("Global Light 2D");
            Light2D globalLight = globalLightObj.AddComponent<Light2D>();
            globalLight.lightType = Light2D.LightType.Global;
            globalLight.color = new Color(0.08f, 0.13f, 0.22f, 1f);
            globalLight.intensity = 0.32f;

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
            spawnLamp.color = new Color(1.0f, 0.65f, 0.2f, 1f);
            spawnLamp.intensity = 0.65f;

            // 3. Background Architecture
            GameObject bgRoot = new GameObject("--- BACKGROUND ---");
            GameObject reactorObj = new GameObject("BG_ReactorCore");
            reactorObj.transform.SetParent(bgRoot.transform);
            reactorObj.transform.position = new Vector3(2.5f, 1.0f, 5.0f);
            reactorObj.transform.localScale = new Vector3(3.5f, 3.5f, 1f);
            SpriteRenderer reactorRend = reactorObj.AddComponent<SpriteRenderer>();
            reactorRend.sprite = reactorSp;
            reactorRend.sortingOrder = -20;
            reactorRend.color = new Color(1f, 1f, 1f, 0.45f);

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

            // Floor
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
            monTm.color = new Color(0f, 0.94f, 1f, 0.95f);
            monTextObj.GetComponent<MeshRenderer>().sortingOrder = 3;

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

            // 6. Player Character
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

            Rigidbody2D playerRb = playerObj.AddComponent<Rigidbody2D>();
            playerRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            playerRb.interpolation = RigidbodyInterpolation2D.Interpolate;
            playerRb.freezeRotation = true;
            playerRb.mass = 1.0f;

            GameObject visorLightObj = new GameObject("Player_VisorLight");
            visorLightObj.transform.SetParent(playerObj.transform);
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

            AbilityController abilityCtrl = playerObj.AddComponent<AbilityController>();

            EchoRecorder echoRec = playerObj.AddComponent<EchoRecorder>();
            SetPrivateField(echoRec, "targetPlayer", playerCtrl);

            EchoPlayback echoPlay = playerObj.AddComponent<EchoPlayback>();
            SetPrivateField(echoPlay, "recorder", echoRec);
            SetPrivateField(echoPlay, "ghostSprite", ghostSp);

            // 7. UI Canvas & HUD
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

            Text echoTxt = CreateUIText(uiRoot.transform, "EchoHintText", 
                "[H] Replay Echo   [C] Clear", defaultFont, 18, TextAnchor.LowerRight,
                new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-40f, 40f), new Vector2(400f, 35f), 
                new Color(0f, 0.94f, 1f, 1f));

            Text puzzleTxt = CreateUIText(uiRoot.transform, "PuzzleStatusText", 
                "○ SENSOR: INACTIVE", defaultFont, 18, TextAnchor.UpperRight,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -35f), new Vector2(400f, 35f), 
                new Color(0.6f, 0.65f, 0.72f, 1f));

            Text doorTxt = CreateUIText(uiRoot.transform, "DoorStatusText", 
                "EXIT DOOR: LOCKED", defaultFont, 18, TextAnchor.UpperRight,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -65f), new Vector2(400f, 35f), 
                new Color(1.0f, 0.2f, 0.3f, 1f));

            Text completeTxt = CreateUIText(uiRoot.transform, "CompletionBanner", 
                "TEST COMPLETE\n<size=20><color=#00F0FF>SIMULATION PROTOCOL VALIDATED</color></size>", 
                defaultFont, 36, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 140f), 
                Color.white);
            completeTxt.gameObject.SetActive(false);

            LevelHUD hud = uiRoot.AddComponent<LevelHUD>();
            SetPrivateField(hud, "titleText", titleTxt);
            SetPrivateField(hud, "levelLabelText", levelTxt);
            SetPrivateField(hud, "timerText", timerTxt);
            SetPrivateField(hud, "instructionsText", instTxt);
            SetPrivateField(hud, "abilityStatusText", abilityTxt);
            SetPrivateField(hud, "puzzleStatusText", puzzleTxt);
            SetPrivateField(hud, "doorStatusText", doorTxt);
            SetPrivateField(hud, "echoHintText", echoTxt);
            SetPrivateField(hud, "completionBannerText", completeTxt);
            SetPrivateField(hud, "exitDoor", exitComp);
            SetPrivateField(hud, "echoPlayback", echoPlay);
            SetPrivateField(hud, "abilityController", abilityCtrl);

            // 8. Core Management
            GameObject coreRoot = new GameObject("--- CORE_MANAGEMENT ---");
            GameObject directorObj = new GameObject("LevelDirector");
            directorObj.transform.SetParent(coreRoot.transform);
            LevelDirector director = directorObj.AddComponent<LevelDirector>();
            SetPrivateField(director, "spawnPoint", spawnObj.transform);
            SetPrivateField(director, "exitDoor", exitComp);
            SetPrivateField(director, "levelTitle", sceneTitle);
            SetPrivateField(director, "chamberNumber", chamberNum);
            SetPrivateField(director, "nextSceneName", nextScene);
            SetPrivateField(director, "isFinalLevel", isFinal);
            SetPrivateField(hud, "levelDirector", director);

            GameObject sessionObj = new GameObject("GameSession");
            sessionObj.AddComponent<GameSession>();

            return new BaseSceneRefs
            {
                camera = cam,
                spawnPoint = spawnObj.transform,
                player = playerCtrl,
                abilityCtrl = abilityCtrl,
                hud = hud,
                director = director,
                exitDoor = exitComp,
                envRoot = envRoot,
                puzzleRoot = puzzleRoot
            };
        }
        #endregion

        #region Level 01: The First Lie (Tutorial & Basic Push)
        public static void ConstructLevel01Scene()
        {
            var refs = SetupBaseChamber("CONTAINMENT 01 // FIRST LIE", 1, "Level02", false);

            Sprite floorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_tile.png");
            Sprite cubeSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/cube_crate.png");
            Sprite plateSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/pressure_plate.png");

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

            SaveChamberScene("Level01.unity");
        }
        #endregion

        #region Level 02: Gravity Is A Suggestion (Gravity Inversion & Echo Replay)
        public static void ConstructLevel02Scene()
        {
            var refs = SetupBaseChamber("CONTAINMENT 02 // GRAVITY ECHO", 2, "Level03", false);

            Sprite floorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_tile.png");
            Sprite echoSwitchSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/echo_switch.png");

            // Enable Gravity Inversion power on player
            refs.abilityCtrl.AllowGravity = true;

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

            // EchoSwitch mounted on the ceiling catwalk at (-4.0, 3.5)
            GameObject echoSwitchObj = new GameObject("EchoSwitch_Ceiling");
            echoSwitchObj.transform.SetParent(refs.puzzleRoot.transform);
            echoSwitchObj.transform.position = new Vector3(-4.0f, 3.5f, 0f);
            echoSwitchObj.transform.localScale = new Vector3(1.0f, -1.0f, 1f); // Inverted mounting

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

            SaveChamberScene("Level02.unity");
        }
        #endregion

        #region Level 03: The Past Is Your Enemy (Time Freeze & Moving Hazards)
        public static void ConstructLevel03Scene()
        {
            var refs = SetupBaseChamber("CONTAINMENT 03 // TEMPORAL FREEZE", 3, "Level04", false);

            Sprite floorSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/floor_tile.png");
            Sprite hazardSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/hazard_laser.png");
            Sprite plateSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/pressure_plate.png");
            Sprite cubeSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/cube_crate.png");

            // Enable Time Freeze (F)
            refs.abilityCtrl.AllowTimeFreeze = true;

            // Moving Freezable Hazard Laser 1 patrolling X = -1.5, sweeping Y between -3.5 and 0.5
            GameObject hazardObj1 = new GameObject("FreezableLaser_Barrier_1");
            hazardObj1.transform.SetParent(refs.puzzleRoot.transform);
            hazardObj1.transform.position = new Vector3(-2.0f, -3.5f, 0f);

            SpriteRenderer hazRend1 = hazardObj1.AddComponent<SpriteRenderer>();
            hazRend1.sprite = hazardSp;
            hazRend1.sortingOrder = 3;

            BoxCollider2D hazCol1 = hazardObj1.AddComponent<BoxCollider2D>();
            hazCol1.size = new Vector2(0.5f, 1.8f);
            hazCol1.isTrigger = true;

            FreezableHazard hazardComp1 = hazardObj1.AddComponent<FreezableHazard>();
            SetPrivateField(hazardComp1, "offsetA", Vector3.zero);
            SetPrivateField(hazardComp1, "offsetB", new Vector3(0f, 4.0f, 0f));
            SetPrivateField(hazardComp1, "moveSpeed", 2.8f);
            SetPrivateField(hazardComp1, "hazardRenderer", hazRend1);

            // Moving Freezable Hazard Laser 2 patrolling X = +2.5, sweeping vertically opposite
            GameObject hazardObj2 = new GameObject("FreezableLaser_Barrier_2");
            hazardObj2.transform.SetParent(refs.puzzleRoot.transform);
            hazardObj2.transform.position = new Vector3(2.0f, 0.5f, 0f);

            SpriteRenderer hazRend2 = hazardObj2.AddComponent<SpriteRenderer>();
            hazRend2.sprite = hazardSp;
            hazRend2.sortingOrder = 3;

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

            SaveChamberScene("Level03.unity");
        }
        #endregion

        #region Level 04: Break The System (Dimension Shift & Grand Finale)
        public static void ConstructLevel04Scene()
        {
            var refs = SetupBaseChamber("CONTAINMENT 04 // REALITY BREACH", 4, "", true);

            Sprite alphaSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/dim_alpha_platform.png");
            Sprite betaSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/dim_beta_platform.png");
            Sprite echoSwitchSp = AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath + "/echo_switch.png");

            // Unlock ALL powers for the finale: Gravity (G), Time Freeze (F), Dimension Swap (Q)
            refs.abilityCtrl.AllowGravity = true;
            refs.abilityCtrl.AllowTimeFreeze = true;
            refs.abilityCtrl.AllowDimensionSwap = true;

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

            SaveChamberScene("Level04.unity");
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
                "Assets/Scenes/SampleScene.unity",
                "Assets/_Game/Scenes/Levels/Level01.unity",
                "Assets/_Game/Scenes/Levels/Level02.unity",
                "Assets/_Game/Scenes/Levels/Level03.unity",
                "Assets/_Game/Scenes/Levels/Level04.unity"
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
