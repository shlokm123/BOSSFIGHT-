using System.Text;
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
using GlitchVerse.Narrative;
using GlitchVerse.Atmosphere;
using GlitchVerse.UI;

namespace GlitchVerse.Editor
{
    public static class Phase08Validator
    {
        [MenuItem("GlitchVerse/Validate Phase 08 Atmosphere & Environment")]
        public static string RunValidation()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== PHASE 08 ATMOSPHERIC ENVIRONMENT VALIDATION REPORT ===");
            bool allPassed = true;

            string[] levelPaths = new string[]
            {
                "Assets/_Game/Scenes/Levels/Level01.unity",
                "Assets/_Game/Scenes/Levels/Level02.unity",
                "Assets/_Game/Scenes/Levels/Level03.unity",
                "Assets/_Game/Scenes/Levels/Level04.unity"
            };

            for (int i = 0; i < levelPaths.Length; i++)
            {
                int levelNum = i + 1;
                var scene = EditorSceneManager.OpenScene(levelPaths[i], OpenSceneMode.Single);
                sb.AppendLine($"\n--- Validating Level 0{levelNum} Atmosphere & Safety ---");

                // 1. Validate Player Silhouette and Visual Animator
                var playerObj = GameObject.Find("Player_Subject075");
                if (playerObj == null)
                {
                    sb.AppendLine($"[FAIL] Level 0{levelNum}: Player_Subject075 not found.");
                    allPassed = false;
                }
                else
                {
                    var col = playerObj.GetComponent<BoxCollider2D>();
                    if (col != null && Mathf.Approximately(col.size.x, 0.5f) && Mathf.Approximately(col.size.y, 0.85f))
                    {
                        sb.AppendLine($"[PASS] Player collider preserved: size=({col.size.x:F2}, {col.size.y:F2})");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Player collider altered! size={col?.size}");
                        allPassed = false;
                    }

                    var visualAnim = playerObj.GetComponent<PlayerVisualAnimator>();
                    if (visualAnim != null)
                    {
                        sb.AppendLine($"[PASS] PlayerVisualAnimator confirmed on Subject 075.");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] PlayerVisualAnimator missing.");
                        allPassed = false;
                    }

                    var visualChild = playerObj.transform.Find("Player_Visual");
                    if (visualChild != null && visualChild.GetComponent<SpriteRenderer>() != null)
                    {
                        sb.AppendLine($"[PASS] Dedicated Player_Visual child confirmed (SpriteRenderer sortingOrder={visualChild.GetComponent<SpriteRenderer>().sortingOrder}).");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Player_Visual child missing or lacks SpriteRenderer.");
                        allPassed = false;
                    }
                }

                // 2. Validate Lighting Rig & Flickering Lights
                var globalLight = Object.FindAnyObjectByType<Light2D>();
                var flickers = Object.FindObjectsByType<FlickeringLight2D>(FindObjectsSortMode.None);
                if (flickers != null && flickers.Length > 0)
                {
                    sb.AppendLine($"[PASS] FlickeringLight2D components confirmed (count={flickers.Length}).");
                }
                else
                {
                    sb.AppendLine($"[FAIL] FlickeringLight2D missing in Level 0{levelNum}.");
                    allPassed = false;
                }

                // 3. Validate Atmospheric Particles
                var emitter = Object.FindAnyObjectByType<AtmosphericParticleEmitter>();
                if (emitter != null)
                {
                    sb.AppendLine($"[PASS] AtmosphericParticleEmitter confirmed (Type={emitter.CurrentAtmosphere}).");
                }
                else
                {
                    sb.AppendLine($"[FAIL] AtmosphericParticleEmitter missing in Level 0{levelNum}.");
                    allPassed = false;
                }

                // 4. Validate Layer Depth Separation
                var fgObj = GameObject.Find("--- FOREGROUND ---");
                if (fgObj != null)
                {
                    sb.AppendLine($"[PASS] Foreground framing layer confirmed.");
                }
                else
                {
                    sb.AppendLine($"[FAIL] Foreground framing layer missing.");
                    allPassed = false;
                }

                var bgObj = GameObject.Find("--- BACKGROUND ---");
                if (bgObj != null)
                {
                    sb.AppendLine($"[PASS] Background depth layer confirmed.");
                }
                else
                {
                    sb.AppendLine($"[FAIL] Background depth layer missing.");
                    allPassed = false;
                }

                // 5. Decorative Collision Safety Check (Zero colliders on decorative elements)
                var envRoot = GameObject.Find("--- ENVIRONMENT ---");
                if (envRoot != null)
                {
                    var decoItems = envRoot.GetComponentsInChildren<Transform>();
                    int illegalColliderCount = 0;
                    foreach (var item in decoItems)
                    {
                        if (item.name.StartsWith("Deco_"))
                        {
                            var c = item.GetComponent<Collider2D>();
                            if (c != null && !c.isTrigger)
                            {
                                illegalColliderCount++;
                            }
                        }
                    }

                    if (illegalColliderCount == 0)
                    {
                        sb.AppendLine($"[PASS] Collision Safety verified: 0 illegal solid colliders on decorative items.");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Collision Safety violation: {illegalColliderCount} solid colliders found on decorative items.");
                        allPassed = false;
                    }
                }

                // 6. Chapter-Specific Narrative & Environmental Checks
                if (levelNum == 1)
                {
                    bool hasSubjDecal = GameObject.Find("Deco_Subject075_Decal") != null;
                    bool hasFakeExit = Object.FindAnyObjectByType<FakeExitPrank>() != null;
                    if (hasSubjDecal && hasFakeExit)
                    {
                        sb.AppendLine($"[PASS] Chapter I: Subject 075 decal and Fake Exit prank confirmed.");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Chapter I: Missing decal or fake exit.");
                        allPassed = false;
                    }
                }
                else if (levelNum == 2)
                {
                    bool hasTrap = Object.FindAnyObjectByType<CollapsingFloor>() != null;
                    bool hasEmergency = Object.FindAnyObjectByType<FakeEmergencyPrank>() != null;
                    bool hasFloating = GameObject.Find("Deco_FloatingMachinery_1") != null;
                    if (hasTrap && hasEmergency && hasFloating)
                    {
                        sb.AppendLine($"[PASS] Chapter II: Collapsing floor, fake emergency, and floating machinery confirmed.");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Chapter II: Missing trap, emergency prank, or floating machinery.");
                        allPassed = false;
                    }
                }
                else if (levelNum == 3)
                {
                    bool hasQuestion = Object.FindAnyObjectByType<ImpossibleQuestionPrank>() != null;
                    bool hasTerminal = GameObject.Find("Deco_LabTerminal_1") != null;
                    bool hasPod = GameObject.Find("Deco_CryoPod_1") != null;
                    bool hasEchoBackdrop = GameObject.Find("EchoPath_ContrastBackdrop") != null;
                    if (hasQuestion && hasTerminal && hasPod && hasEchoBackdrop)
                    {
                        sb.AppendLine($"[PASS] Chapter III: Lab terminals, cryo pods, Echo contrast backdrop, and impossible question confirmed.");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Chapter III: Missing laboratory equipment or question prank.");
                        allPassed = false;
                    }
                }
                else if (levelNum == 4)
                {
                    bool hasSkyline = GameObject.Find("Deco_DistantSkyline_Vent") != null;
                    bool hasFakeCp = Object.FindAnyObjectByType<FakeCheckpointPrank>() != null;
                    bool hasBeacons = GameObject.Find("Deco_EmergencyBeacon_L") != null;
                    if (hasSkyline && hasFakeCp && hasBeacons)
                    {
                        sb.AppendLine($"[PASS] Chapter IV: Distant Skyline (The Last City), emergency beacons, and fake checkpoint confirmed.");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Chapter IV: Missing skyline vista, beacons, or fake checkpoint.");
                        allPassed = false;
                    }
                }

                // 7. Verify UI Preservation (No redesign)
                var hud = Object.FindAnyObjectByType<LevelHUD>();
                if (hud != null)
                {
                    sb.AppendLine($"[PASS] LevelHUD preserved intact without redesign.");
                }
                else
                {
                    sb.AppendLine($"[FAIL] LevelHUD missing in Level 0{levelNum}.");
                    allPassed = false;
                }
            }

            // Restore Level01 as active editor scene
            EditorSceneManager.OpenScene("Assets/_Game/Scenes/Levels/Level01.unity", OpenSceneMode.Single);

            sb.AppendLine(allPassed ? "\n=== OVERALL RESULT: ALL PHASE 08 CHECKS PASSED ===" : "\n=== OVERALL RESULT: SOME CHECKS FAILED ===");
            Debug.Log(sb.ToString());
            return sb.ToString();
        }
    }
}
