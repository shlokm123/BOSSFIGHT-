using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using GlitchVerse.Core;
using GlitchVerse.Player;
using GlitchVerse.Interactables;
using GlitchVerse.Narrative;
using GlitchVerse.UI;

namespace GlitchVerse.Editor
{
    public static class Phase081RegressionRepairValidator
    {
        [MenuItem("GlitchVerse/Validate Phase 08.1 Progression & Dialogue")]
        public static string RunValidation()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== PHASE 08.1 PROGRESSION & DIALOGUE REPAIR VALIDATION ===");
            bool allPassed = true;

            // 1. Build Settings Verification
            sb.AppendLine("\n--- 1. Build Settings Verification ---");
            var scenes = EditorBuildSettings.scenes;
            if (scenes.Length < 4)
            {
                sb.AppendLine($"[FAIL] Less than 4 scenes in BuildSettings: {scenes.Length}");
                allPassed = false;
            }
            else
            {
                if (scenes[0].path.EndsWith("Level01.unity") && scenes[0].enabled)
                {
                    sb.AppendLine("[PASS] Level01 is enabled at Build Index 0.");
                }
                else
                {
                    sb.AppendLine($"[FAIL] Index 0 is not enabled Level01: {scenes[0].path}");
                    allPassed = false;
                }
            }

            // 2. Scene Field & Narrative Wiring Verification
            sb.AppendLine("\n--- 2. Scene Field & Narrative Wiring Verification ---");
            string[] expectedScenes = { "Level01", "Level02", "Level03", "Level04" };
            string[] expectedNext = { "Level02", "Level03", "Level04", "" };
            bool[] expectedFinal = { false, false, false, true };

            for (int i = 0; i < 4; i++)
            {
                int levelNum = i + 1;
                string scenePath = $"Assets/_Game/Scenes/Levels/{expectedScenes[i]}.unity";
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                var director = Object.FindAnyObjectByType<LevelDirector>();
                var dialogue = Object.FindAnyObjectByType<ArchitectDialogue>();
                var hud = Object.FindAnyObjectByType<LevelHUD>();
                var intro = Object.FindAnyObjectByType<ChapterIntroController>();

                if (director == null)
                {
                    sb.AppendLine($"[FAIL] {expectedScenes[i]}: LevelDirector missing.");
                    allPassed = false;
                    continue;
                }

                if (director.ChamberNumber == levelNum)
                {
                    sb.AppendLine($"[PASS] {expectedScenes[i]}: chamberNumber = {director.ChamberNumber}");
                }
                else
                {
                    sb.AppendLine($"[FAIL] {expectedScenes[i]}: chamberNumber = {director.ChamberNumber} (expected {levelNum})");
                    allPassed = false;
                }

                if (director.NextSceneName == expectedNext[i])
                {
                    sb.AppendLine($"[PASS] {expectedScenes[i]}: nextSceneName = '{director.NextSceneName}'");
                }
                else
                {
                    sb.AppendLine($"[FAIL] {expectedScenes[i]}: nextSceneName = '{director.NextSceneName}' (expected '{expectedNext[i]}')");
                    allPassed = false;
                }

                if (director.IsFinalLevel == expectedFinal[i])
                {
                    sb.AppendLine($"[PASS] {expectedScenes[i]}: isFinalLevel = {director.IsFinalLevel}");
                }
                else
                {
                    sb.AppendLine($"[FAIL] {expectedScenes[i]}: isFinalLevel = {director.IsFinalLevel} (expected {expectedFinal[i]})");
                    allPassed = false;
                }

                if (dialogue == null)
                {
                    sb.AppendLine($"[FAIL] {expectedScenes[i]}: ArchitectDialogue missing.");
                    allPassed = false;
                }
                else
                {
                    var fSub = dialogue.GetType().GetField("subtitleText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var sub = fSub?.GetValue(dialogue) as Text;
                    var fBanner = dialogue.GetType().GetField("subtitleBannerPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var banner = fBanner?.GetValue(dialogue) as GameObject;
                    var fFlash = dialogue.GetType().GetField("flashOverlay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var flash = fFlash?.GetValue(dialogue) as Image;

                    if (sub != null && banner != null && flash != null)
                    {
                        sb.AppendLine($"[PASS] {expectedScenes[i]}: SubtitleText='{sub.name}', Banner='{banner.name}', Flash='{flash.name}' wired.");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] {expectedScenes[i]}: Dialogue missing components (sub={sub != null}, banner={banner != null}, flash={flash != null})");
                        allPassed = false;
                    }
                }

                // Check specific pranks per level
                if (levelNum == 1)
                {
                    var prank = Object.FindAnyObjectByType<FakeExitPrank>();
                    ExitDoor realDoor = null;
                    var allDoors = Object.FindObjectsByType<ExitDoor>(FindObjectsInactive.Include);
                    foreach (var d in allDoors)
                    {
                        if (d != null && d.gameObject.name == "RealExitDoor_PlatformA")
                        {
                            realDoor = d;
                            break;
                        }
                    }

                    if (prank != null && realDoor != null)
                    {
                        sb.AppendLine($"[PASS] Level 01: FakeExitPrank and RealExitDoor_PlatformA confirmed.");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Level 01: Prank or RealDoor missing (prank={prank != null}, realDoor={realDoor != null})");
                        allPassed = false;
                    }
                }
                else if (levelNum == 2)
                {
                    var emergency = Object.FindAnyObjectByType<FakeEmergencyPrank>();
                    var floor = Object.FindAnyObjectByType<CollapsingFloor>();
                    if (emergency != null && floor != null)
                    {
                        sb.AppendLine($"[PASS] Level 02: FakeEmergencyPrank and CollapsingFloor confirmed.");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Level 02: Prank or Floor missing (emergency={emergency != null}, floor={floor != null})");
                        allPassed = false;
                    }
                }
                else if (levelNum == 3)
                {
                    var question = Object.FindAnyObjectByType<ImpossibleQuestionPrank>();
                    if (question != null)
                    {
                        sb.AppendLine($"[PASS] Level 03: ImpossibleQuestionPrank confirmed.");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Level 03: ImpossibleQuestionPrank missing.");
                        allPassed = false;
                    }
                }
                else if (levelNum == 4)
                {
                    var fakeCp = Object.FindAnyObjectByType<FakeCheckpointPrank>();
                    if (fakeCp != null)
                    {
                        sb.AppendLine($"[PASS] Level 04: FakeCheckpointPrank confirmed.");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Level 04: FakeCheckpointPrank missing.");
                        allPassed = false;
                    }
                }
            }

            // Restore Level01
            EditorSceneManager.OpenScene("Assets/_Game/Scenes/Levels/Level01.unity", OpenSceneMode.Single);

            sb.AppendLine("\n=======================================================");
            sb.AppendLine($"OVERALL PHASE 08.1 VALIDATION RESULT: {(allPassed ? "PASSED" : "FAILED")}");
            sb.AppendLine("=======================================================");

            Debug.Log(sb.ToString());
            return sb.ToString();
        }
    }
}
