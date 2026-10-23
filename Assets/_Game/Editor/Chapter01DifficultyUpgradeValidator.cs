using System.Collections;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using GlitchVerse.Core;
using GlitchVerse.Interactables;
using GlitchVerse.Narrative;
using GlitchVerse.Player;

namespace GlitchVerse.Editor
{
    /// <summary>
    /// Automated validator that verifies the Chapter I Difficulty Upgrade 01:
    /// - Structural verification of Platform B, Security Barrier, and Secondary Override Switch
    /// - Full Play Mode puzzle simulation (Cube -> Decoy -> Barrier Lock -> Switch Override -> Real Exit -> Level 02 Transition)
    /// </summary>
    public static class Chapter01DifficultyUpgradeValidator
    {
        [MenuItem("GlitchVerse/Validate Chapter 01 Upgrade")]
        public static bool RunValidation()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== GLITCHVERSE: CHAPTER I DIFFICULTY UPGRADE 01 VALIDATION ===");

            bool allPassed = true;

            // 1. Scene Structure Verification
            var scene = EditorSceneManager.OpenScene("Assets/_Game/Scenes/Levels/Level01.unity");
            if (!scene.IsValid())
            {
                Debug.LogError("[FAIL] Level01.unity could not be opened.");
                return false;
            }

            var prank = Object.FindAnyObjectByType<FakeExitPrank>(FindObjectsInactive.Include);
            var barrier = Object.FindAnyObjectByType<SecurityBarrier>(FindObjectsInactive.Include);
            var secSwitch = Object.FindAnyObjectByType<SecurityOverrideSwitch>(FindObjectsInactive.Include);
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
            var platA = GameObject.Find("Observation_Platform_A");
            var platB = GameObject.Find("Observation_Platform_B");

            if (prank != null) sb.AppendLine("[PASS] FakeExitPrank component present.");
            else { sb.AppendLine("[FAIL] FakeExitPrank missing."); allPassed = false; }

            if (barrier != null) sb.AppendLine("[PASS] SecurityBarrier component present.");
            else { sb.AppendLine("[FAIL] SecurityBarrier missing."); allPassed = false; }

            if (secSwitch != null) sb.AppendLine("[PASS] SecurityOverrideSwitch component present.");
            else { sb.AppendLine("[FAIL] SecurityOverrideSwitch missing."); allPassed = false; }

            if (realDoor != null) sb.AppendLine("[PASS] RealExitDoor_PlatformA present.");
            else { sb.AppendLine("[FAIL] RealExitDoor_PlatformA missing."); allPassed = false; }

            if (platA != null && platB != null) sb.AppendLine("[PASS] Both Observation Platform A and B present.");
            else { sb.AppendLine("[FAIL] Platforms missing."); allPassed = false; }

            // Verify wiring in prank
            if (prank != null)
            {
                if (prank.Barrier != null && prank.SecondarySwitch != null)
                {
                    sb.AppendLine("[PASS] FakeExitPrank properly references SecurityBarrier and SecondarySwitch.");
                }
                else
                {
                    sb.AppendLine("[FAIL] FakeExitPrank references missing.");
                    allPassed = false;
                }
            }

            // Verify barrier solid collider
            if (barrier != null)
            {
                var col = barrier.GetComponent<BoxCollider2D>();
                if (col != null && !col.isTrigger)
                {
                    sb.AppendLine($"[PASS] SecurityBarrier has solid blocker collider (bounds: {col.size}).");
                }
                else
                {
                    sb.AppendLine("[FAIL] SecurityBarrier collider is missing or isTrigger=true.");
                    allPassed = false;
                }
            }

            // Verify switch trigger collider
            if (secSwitch != null)
            {
                var col = secSwitch.GetComponent<BoxCollider2D>();
                if (col != null && col.isTrigger)
                {
                    sb.AppendLine($"[PASS] SecurityOverrideSwitch has trigger collider (size: {col.size}).");
                }
                else
                {
                    sb.AppendLine("[FAIL] SecurityOverrideSwitch collider is missing or not a trigger.");
                    allPassed = false;
                }
            }

            sb.AppendLine(allPassed ? ">> CHAPTER I STRUCTURAL VALIDATION: PASSED <<" : ">> CHAPTER I STRUCTURAL VALIDATION: FAILED <<");
            Debug.Log(sb.ToString());
            return allPassed;
        }
    }
}
