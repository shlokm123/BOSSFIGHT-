using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using GlitchVerse.Core;
using GlitchVerse.Player;
using GlitchVerse.Narrative;

namespace GlitchVerse.Editor
{
    public static class Phase075Validator
    {
        [MenuItem("GlitchVerse/Validate Phase 07.5 Systems")]
        public static string RunValidation()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== PHASE 07.5 VERIFICATION REPORT ===");
            bool allPassed = true;

            // 1. Build Settings Verification
            var scenes = EditorBuildSettings.scenes;
            if (scenes.Length < 4)
            {
                sb.AppendLine("[FAIL] BuildSettings has fewer than 4 scenes registered.");
                allPassed = false;
            }
            else if (!scenes[0].path.EndsWith("Level01.unity"))
            {
                sb.AppendLine($"[FAIL] First scene in BuildSettings is '{scenes[0].path}', expected Level01.unity.");
                allPassed = false;
            }
            else
            {
                sb.AppendLine($"[PASS] First scene in BuildSettings is correctly '{scenes[0].path}'.");
            }

            // 2. Validate all 4 level scenes
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
                sb.AppendLine($"--- Validating Level 0{levelNum} ---");

                // Check ChapterIntroController
                var intro = Object.FindAnyObjectByType<ChapterIntroController>(FindObjectsInactive.Include);
                if (intro == null)
                {
                    sb.AppendLine($"[FAIL] ChapterIntroController missing in Level 0{levelNum}.");
                    allPassed = false;
                }
                else
                {
                    var panel = intro.transform.Find("ChapterIntroPanel");
                    if (panel == null)
                    {
                        var panelObj = GameObject.Find("ChapterIntroPanel");
                        if (panelObj != null)
                        {
                            sb.AppendLine($"[PASS] Level 0{levelNum}: ChapterIntroController and ChapterIntroPanel confirmed.");
                        }
                        else
                        {
                            sb.AppendLine($"[FAIL] ChapterIntroPanel missing in Level 0{levelNum}.");
                            allPassed = false;
                        }
                    }
                    else
                    {
                        sb.AppendLine($"[PASS] Level 0{levelNum}: ChapterIntroController and ChapterIntroPanel confirmed.");
                    }
                }

                // Level-specific checks
                if (levelNum == 2)
                {
                    var trap = Object.FindAnyObjectByType<CollapsingFloor>(FindObjectsInactive.Include);
                    if (trap == null)
                    {
                        sb.AppendLine("[FAIL] Level 02: CollapsingFloor trap component missing.");
                        allPassed = false;
                    }
                    else
                    {
                        var col = trap.GetComponent<BoxCollider2D>();
                        sb.AppendLine($"[PASS] Level 02: CollapsingFloor found at pos={trap.transform.position}, colliderSize={col?.size}.");
                    }

                    var floorObj = GameObject.Find("Chamber_Floor");
                    var floorCols = floorObj != null ? floorObj.GetComponents<BoxCollider2D>() : null;
                    if (floorCols == null || floorCols.Length < 2)
                    {
                        sb.AppendLine("[FAIL] Level 02: Chamber_Floor does not have split colliders for trap gap.");
                        allPassed = false;
                    }
                    else
                    {
                        sb.AppendLine($"[PASS] Level 02: Chamber_Floor split colliders confirmed (count={floorCols.Length}).");
                    }
                }
                else if (levelNum == 3)
                {
                    var question = Object.FindAnyObjectByType<ImpossibleQuestionPrank>(FindObjectsInactive.Include);
                    if (question == null)
                    {
                        sb.AppendLine("[FAIL] Level 03: ImpossibleQuestionPrank missing.");
                        allPassed = false;
                    }
                    else
                    {
                        sb.AppendLine("[PASS] Level 03: ImpossibleQuestionPrank component confirmed.");
                    }

                    Text prompt = null;
                    var allTexts = Resources.FindObjectsOfTypeAll<Text>();
                    foreach (var t in allTexts)
                    {
                        if (t.gameObject.name == "ImpossibleQuestionPromptText" && t.gameObject.scene == scene)
                        {
                            prompt = t;
                            break;
                        }
                    }

                    if (prompt == null)
                    {
                        sb.AppendLine("[FAIL] Level 03: ImpossibleQuestionPromptText UI missing.");
                        allPassed = false;
                    }
                    else
                    {
                        sb.AppendLine("[PASS] Level 03: ImpossibleQuestionPromptText UI confirmed.");
                    }
                }
                else if (levelNum == 4)
                {
                    var fakeCp = Object.FindAnyObjectByType<FakeCheckpointPrank>(FindObjectsInactive.Include);
                    if (fakeCp == null)
                    {
                        sb.AppendLine("[FAIL] Level 04: FakeCheckpointPrank missing.");
                        allPassed = false;
                    }
                    else
                    {
                        sb.AppendLine("[PASS] Level 04: FakeCheckpointPrank component confirmed.");
                    }
                }
            }

            // Restore Level01
            EditorSceneManager.OpenScene("Assets/_Game/Scenes/Levels/Level01.unity", OpenSceneMode.Single);

            sb.AppendLine(allPassed ? "=== RESULT: ALL CHECKS PASSED ===" : "=== RESULT: SOME CHECKS FAILED ===");
            Debug.Log(sb.ToString());
            return sb.ToString();
        }
    }
}
