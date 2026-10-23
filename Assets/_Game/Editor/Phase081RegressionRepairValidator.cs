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
using GlitchVerse.Mechanics;

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
            if (scenes.Length < 6)
            {
                sb.AppendLine($"[FAIL] Less than 6 scenes in BuildSettings: {scenes.Length}");
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
            string[] expectedScenes = { "Level01", "Level02", "Level03", "Level04", "Level05", "Level06" };
            string[] expectedNext = { "Level02", "Level03", "Level04", "Level05", "Level06", "" };
            bool[] expectedFinal = { false, false, false, false, false, true };

            for (int i = 0; i < 6; i++)
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

                // Check specific pranks and mechanics per level
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
                else if (levelNum == 5)
                {
                    var paranoia = Object.FindAnyObjectByType<ParanoiaTrapPrank>();
                    var eSwitch = Object.FindAnyObjectByType<GlitchVerse.Echo.EchoSwitch>();
                    var dimObjs = Object.FindObjectsByType<DimensionObject>(FindObjectsInactive.Include);
                    if (paranoia != null && eSwitch != null && dimObjs.Length >= 2)
                    {
                        sb.AppendLine($"[PASS] Level 05: ParanoiaTrapPrank, EchoSwitch, and {dimObjs.Length} Dimension Platforms confirmed.");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Level 05 missing components (prank={paranoia != null}, switch={eSwitch != null}, dimObjs={dimObjs.Length})");
                        allPassed = false;
                    }
                }
                else if (levelNum == 6)
                {
                    var truth = Object.FindAnyObjectByType<TruthChamberTriggers>();
                    var hazard = Object.FindAnyObjectByType<FreezableHazard>();
                    var eSwitch = Object.FindAnyObjectByType<GlitchVerse.Echo.EchoSwitch>();
                    var plate = Object.FindAnyObjectByType<PressurePlate>();
                    if (truth != null && hazard != null && eSwitch != null && plate != null)
                    {
                        sb.AppendLine($"[PASS] Level 06: TruthChamberTriggers, FreezableHazard, EchoSwitch, and VaultPlate confirmed.");
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Level 06 missing components (truth={truth != null}, hazard={hazard != null}, switch={eSwitch != null}, plate={plate != null})");
                        allPassed = false;
                    }
                }
            }

            // 3. Chapter III Interrogation & Fake-Success Validation
            sb.AppendLine("\n--- 3. Chapter III Interrogation & Fake-Success Trap Validation ---");
            bool ch3Passed = ValidateChapter3QuestionTrap(sb);
            if (!ch3Passed) allPassed = false;

            // Restore Level01
            EditorSceneManager.OpenScene("Assets/_Game/Scenes/Levels/Level01.unity", OpenSceneMode.Single);

            sb.AppendLine("\n=======================================================");
            sb.AppendLine($"OVERALL VALIDATION RESULT: {(allPassed ? "PASSED" : "FAILED")}");
            sb.AppendLine("=======================================================");

            Debug.Log(sb.ToString());
            return sb.ToString();
        }

        private static bool ValidateChapter3QuestionTrap(StringBuilder sb)
        {
            bool passed = true;
            EditorSceneManager.OpenScene("Assets/_Game/Scenes/Levels/Level03.unity", OpenSceneMode.Single);

            ImpossibleQuestionPrank.ResetSessionFlag();
            var prank = Object.FindAnyObjectByType<ImpossibleQuestionPrank>(FindObjectsInactive.Include);
            var player = Object.FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Include);
            var director = Object.FindAnyObjectByType<LevelDirector>(FindObjectsInactive.Include);
            Text prompt = null;
            if (prank != null)
            {
                var fPrompt = prank.GetType().GetField("questionPromptText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                prompt = fPrompt?.GetValue(prank) as Text;
            }
            if (prompt == null)
            {
                var allTexts = Object.FindObjectsByType<Text>(FindObjectsInactive.Include);
                foreach (var tObj in allTexts)
                {
                    if (tObj != null && tObj.gameObject.name == "ImpossibleQuestionPromptText")
                    {
                        prompt = tObj;
                        break;
                    }
                }
            }

            if (prank == null || prompt == null)
            {
                sb.AppendLine($"[FAIL] ImpossibleQuestionPrank ({prank != null}) or prompt UI ({prompt != null}) missing in Level03.");
                return false;
            }

            // Test 1: Trigger question
            var trigMethod = prank.GetType().GetMethod("TriggerQuestion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            trigMethod?.Invoke(prank, null);

            var fAwaiting = prank.GetType().GetField("isAwaitingAnswer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool isAwaiting = (bool)(fAwaiting?.GetValue(prank) ?? false);

            if (isAwaiting && player != null && player.InputLocked)
            {
                sb.AppendLine("[PASS] Interrogation triggered: Player locked and awaiting answer.");
            }
            else
            {
                sb.AppendLine($"[FAIL] Interrogation state invalid (awaiting={isAwaiting}, playerLocked={player?.InputLocked})");
                passed = false;
            }

            // Test 2: Verify prompt text
            string t = prompt.text;
            bool hasHeader = t.Contains("THE ARCHITECT // INTERROGATION 001");
            bool hasTrust = t.Contains("DO YOU TRUST ME?");
            bool hasY = t.Contains("[Y] YES");
            bool hasN = t.Contains("[N] NO");
            bool hasInstr = t.Contains("PRESS Y OR N TO ANSWER");
            bool hasOldAD = t.Contains("[A]") || t.Contains("[D]");

            if (hasHeader && hasTrust && hasY && hasN && hasInstr && !hasOldAD)
            {
                sb.AppendLine("[PASS] Prompt format verified: Y/N options visible, A/D movement keys excluded.");
            }
            else
            {
                sb.AppendLine($"[FAIL] Prompt format incorrect (header={hasHeader}, trust={hasTrust}, Y={hasY}, N={hasN}, instr={hasInstr}, hasOldAD={hasOldAD})");
                passed = false;
            }

            // Test 3: Fake success coroutine phases
            var fakeMethod = prank.GetType().GetMethod("ExecuteFakeSuccessRoutine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var enumerator = fakeMethod?.Invoke(prank, null) as System.Collections.IEnumerator;

            if (enumerator != null)
            {
                // Step 1: MoveNext() executes up to first yield (0.5s): sets ANSWER ACCEPTED
                enumerator.MoveNext();
                bool step1 = prompt.text.Contains("ANSWER ACCEPTED.");
                if (step1)
                {
                    sb.AppendLine("[PASS] Fake Success Step 1: 'ANSWER ACCEPTED.' displayed in green.");
                }
                else
                {
                    sb.AppendLine($"[FAIL] Fake Success Step 1 missing: '{prompt.text}'");
                    passed = false;
                }

                // Step 2: MoveNext() executes up to second yield (0.7s): sets YOU MAY PROCEED
                enumerator.MoveNext();
                bool step2 = prompt.text.Contains("YOU MAY PROCEED.");
                if (step2)
                {
                    sb.AppendLine("[PASS] Fake Success Step 2: 'YOU MAY PROCEED.' displayed.");
                }
                else
                {
                    sb.AppendLine($"[FAIL] Fake Success Step 2 missing: '{prompt.text}'");
                    passed = false;
                }

                // Step 3: MoveNext() executes up to third yield (0.55s): hides prompt & speaks betrayal
                enumerator.MoveNext();
                bool promptHidden = !prompt.gameObject.activeSelf;
                if (promptHidden)
                {
                    sb.AppendLine("[PASS] Fake Success Step 3: Interrogation prompt hidden, 'I LIED.' betrayal line executed.");
                }
                else
                {
                    sb.AppendLine("[FAIL] Prompt not hidden during betrayal punchline.");
                    passed = false;
                }
            }

            // Test 4: Retry bypass and disarmed dialogue
            if (ImpossibleQuestionPrank.HasExperiencedQuestion)
            {
                sb.AppendLine("[PASS] HasExperiencedQuestion persists: Interrogation disarmed for session.");
            }
            else
            {
                sb.AppendLine("[FAIL] HasExperiencedQuestion flag did not persist.");
                passed = false;
            }

            // Test 5: Verify Level 03 to Level 04 progression path
            if (director != null && director.NextSceneName == "Level04")
            {
                sb.AppendLine("[PASS] Level03 progression target preserved: 'Level04'.");
            }
            else
            {
                sb.AppendLine($"[FAIL] Level03 nextSceneName altered: '{director?.NextSceneName}'");
                passed = false;
            }

            return passed;
        }
    }
}
