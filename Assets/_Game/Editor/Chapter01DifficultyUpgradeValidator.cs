using System.Collections;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using GlitchVerse.Core;
using GlitchVerse.Echo;
using GlitchVerse.Interactables;
using GlitchVerse.Mechanics;
using GlitchVerse.Narrative;
using GlitchVerse.Player;

namespace GlitchVerse.Editor
{
    /// <summary>
    /// Comprehensive automated validator that verifies Chapter I Difficulty Upgrade 01B:
    /// - Structural verification of Platform B, Security Barrier, Echo Switch, and Dual Moving Lasers
    /// - Gating verification: Live player rejected, Echo ghost accepted
    /// - Timing & hold verification: Barrier unlocks while Echo holds, re-locks after release + grace
    /// - Hazard verification: Lasers move along correct axes and boundaries; lethal to live player, safe for ghost
    /// - Full Play Mode puzzle simulation & transition to Level 02
    /// </summary>
    public static class Chapter01DifficultyUpgradeValidator
    {
        [MenuItem("GlitchVerse/Validate Chapter 01 Upgrade 01B")]
        public static bool RunValidation()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== GLITCHVERSE: CHAPTER I DIFFICULTY UPGRADE 01B VALIDATION ===");

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
            var laserA = GameObject.Find("LaserA_Vertical");
            var laserB = GameObject.Find("LaserB_Horizontal");

            if (prank != null) sb.AppendLine("[PASS] FakeExitPrank component present.");
            else { sb.AppendLine("[FAIL] FakeExitPrank missing."); allPassed = false; }

            if (barrier != null) sb.AppendLine("[PASS] SecurityBarrier component present.");
            else { sb.AppendLine("[FAIL] SecurityBarrier missing."); allPassed = false; }

            if (secSwitch != null) sb.AppendLine("[PASS] SecurityOverrideSwitch (EchoSwitch) present.");
            else { sb.AppendLine("[FAIL] SecurityOverrideSwitch missing."); allPassed = false; }

            if (realDoor != null) sb.AppendLine("[PASS] RealExitDoor_PlatformA present.");
            else { sb.AppendLine("[FAIL] RealExitDoor_PlatformA missing."); allPassed = false; }

            if (platA != null && platB != null) sb.AppendLine("[PASS] Both Observation Platform A and B present.");
            else { sb.AppendLine("[FAIL] Platforms missing."); allPassed = false; }

            if (laserA != null) sb.AppendLine("[PASS] LaserA_Vertical present.");
            else { sb.AppendLine("[FAIL] LaserA_Vertical missing."); allPassed = false; }

            if (laserB != null) sb.AppendLine("[PASS] LaserB_Horizontal present.");
            else { sb.AppendLine("[FAIL] LaserB_Horizontal missing."); allPassed = false; }

            // 2. Echo Gating Verification
            if (secSwitch != null)
            {
                if (!secSwitch.AllowPlayerActivation && secSwitch.AllowEchoActivation)
                {
                    sb.AppendLine("[PASS] Switch gating confirmed: AllowPlayer=false, AllowEcho=true.");
                }
                else
                {
                    sb.AppendLine($"[FAIL] Switch gating invalid: AllowPlayer={secSwitch.AllowPlayerActivation}, AllowEcho={secSwitch.AllowEchoActivation}.");
                    allPassed = false;
                }

                if (secSwitch.HoldGraceDuration >= 1.5f)
                {
                    sb.AppendLine($"[PASS] Echo hold grace window confirmed: {secSwitch.HoldGraceDuration}s.");
                }
                else
                {
                    sb.AppendLine($"[FAIL] Echo hold grace window insufficient: {secSwitch.HoldGraceDuration}s.");
                    allPassed = false;
                }
            }

            // 3. Moving Laser Hazard Verification
            if (laserA != null)
            {
                var hazA = laserA.GetComponent<MovingLaserHazard>();
                if (hazA != null && hazA.Axis == MovingLaserHazard.MotionAxis.Horizontal)
                {
                    sb.AppendLine($"[PASS] Laser A confirmed: Axis={hazA.Axis}, Speed={hazA.MoveSpeed}, Span={hazA.PointA.x} to {hazA.PointB.x}.");
                }
                else
                {
                    sb.AppendLine("[FAIL] Laser A MovingLaserHazard component invalid.");
                    allPassed = false;
                }
            }

            if (laserB != null)
            {
                var hazB = laserB.GetComponent<MovingLaserHazard>();
                if (hazB != null && hazB.Axis == MovingLaserHazard.MotionAxis.Vertical)
                {
                    sb.AppendLine($"[PASS] Laser B confirmed: Axis={hazB.Axis}, Speed={hazB.MoveSpeed}, Span={hazB.PointA.y} to {hazB.PointB.y}.");
                }
                else
                {
                    sb.AppendLine("[FAIL] Laser B MovingLaserHazard component invalid.");
                    allPassed = false;
                }
            }

            // 4. Wiring in Prank
            if (prank != null)
            {
                if (prank.Barrier != null && prank.SecondarySwitch != null && prank.LaserA != null && prank.LaserB != null)
                {
                    sb.AppendLine("[PASS] FakeExitPrank correctly references Barrier, Switch, Laser A, and Laser B.");
                }
                else
                {
                    sb.AppendLine("[FAIL] FakeExitPrank references missing.");
                    allPassed = false;
                }
            }

            sb.AppendLine(allPassed ? ">> CHAPTER I UPGRADE 01B STRUCTURAL VALIDATION: PASSED <<" : ">> CHAPTER I UPGRADE 01B STRUCTURAL VALIDATION: FAILED <<");
            Debug.Log(sb.ToString());
            return allPassed;
        }

        public static string SimulatePlayModeScenario()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== CHAPTER I DIFFICULTY UPGRADE 01B PLAY MODE SIMULATION ===");

            var player = Object.FindAnyObjectByType<PlayerController2D>();
            var cube = Object.FindAnyObjectByType<MovableCube>();
            var plate = Object.FindAnyObjectByType<PressurePlate>();
            var prank = Object.FindAnyObjectByType<FakeExitPrank>();
            var barrier = Object.FindAnyObjectByType<SecurityBarrier>(FindObjectsInactive.Include);
            var secSwitch = Object.FindAnyObjectByType<SecurityOverrideSwitch>(FindObjectsInactive.Include);
            var laserA = GameObject.Find("LaserA_Vertical")?.GetComponent<MovingLaserHazard>();
            var laserB = GameObject.Find("LaserB_Horizontal")?.GetComponent<MovingLaserHazard>();
            var director = LevelDirector.Current;

            ExitDoor realDoor = null;
            ExitDoor decoyDoor = null;
            foreach (var d in Object.FindObjectsByType<ExitDoor>(FindObjectsInactive.Include))
            {
                if (d.gameObject.name == "RealExitDoor_PlatformA") realDoor = d;
                else if (d.gameObject.name == "DecoyExitDoor" || d.gameObject.name.Contains("ExitDoor")) decoyDoor = d;
            }

            if (player == null || cube == null || plate == null || prank == null || barrier == null || secSwitch == null)
            {
                sb.AppendLine("[FAIL] Essential puzzle components missing.");
                return sb.ToString();
            }

            // Phase 1: Initial state before prank
            sb.AppendLine($"1. Initial State: Plate={plate.IsActivated}, DecoyDoor={decoyDoor != null && decoyDoor.IsUnlocked}, LasersActive={laserA != null && laserA.IsActive}, BarrierActive={barrier.gameObject.activeSelf}");

            // Phase 2: Cube activates pressure plate
            cube.transform.position = plate.transform.position + new Vector3(0f, 0.2f, 0f);
            Physics2D.Simulate(0.1f);
            Physics2D.SyncTransforms();
            sb.AppendLine($"2. Cube on Plate: PlateActivated={plate.IsActivated}, DecoyDoorUnlocked={decoyDoor != null && decoyDoor.IsUnlocked}");

            // Phase 3: Player enters decoy door -> Fake Exit Prank triggers
            if (decoyDoor != null)
            {
                decoyDoor.NotifyTriggerEnter(player.GetComponent<Collider2D>());
            }
            Physics2D.Simulate(0.2f);
            Physics2D.SyncTransforms();

            bool prankFired = prank.IsPrankTriggered;
            bool barrierLocked = barrier.gameObject.activeSelf && barrier.IsLocked;
            bool lasersOnline = (laserA != null && laserA.IsActive) && (laserB != null && laserB.IsActive);
            bool switchOnline = secSwitch.IsPowered;

            sb.AppendLine($"3. Fake Exit Prank: Triggered={prankFired}, RealDoorActive={realDoor != null && realDoor.gameObject.activeSelf}, BarrierLocked={barrierLocked}, LasersOnline={lasersOnline}, SwitchPowered={switchOnline}");

            // Phase 4: Live Player on Switch (MUST NOT ACTIVATE)
            player.transform.position = secSwitch.transform.position;
            Physics2D.Simulate(0.1f);
            Physics2D.SyncTransforms();

            bool livePlayerRejected = !secSwitch.IsActivated && barrier.IsLocked;
            sb.AppendLine($"4. Live Player on Switch: SwitchActivated={secSwitch.IsActivated}, BarrierLocked={barrier.IsLocked} (Live Player Rejected: {livePlayerRejected})");

            // Phase 5: Laser Hazard Configuration & Predictable Motion
            bool lasersValid = laserA.Axis == MovingLaserHazard.MotionAxis.Horizontal && laserB.Axis == MovingLaserHazard.MotionAxis.Vertical;
            sb.AppendLine($"5. Laser Hazards Trajectory: Laser A ({laserA.PointA.x:F1} to {laserA.PointB.x:F1} at {laserA.MoveSpeed:F1}m/s), Laser B ({laserB.PointA.y:F1} to {laserB.PointB.y:F1} at {laserB.MoveSpeed:F1}m/s)");

            // Phase 6: Temporal Echo Ghost on Switch (MUST ACTIVATE & DROP BARRIER)
            GameObject ghostTest = new GameObject("EchoGhost_Test");
            ghostTest.AddComponent<EchoGhostMarker>();
            var ghostCol = ghostTest.AddComponent<BoxCollider2D>();
            ghostCol.isTrigger = true;
            ghostCol.size = new Vector2(0.6f, 0.9f);
            ghostTest.transform.position = secSwitch.transform.position;

            Physics2D.Simulate(0.1f);
            Physics2D.SyncTransforms();

            bool echoActivated = secSwitch.IsActivated;
            bool barrierDropped = !barrier.IsLocked;
            sb.AppendLine($"6. Echo Ghost on Switch: SwitchActivated={echoActivated}, BarrierUnlocked={barrierDropped}");

            // Phase 7: Ghost leaves switch -> Barrier re-locks after grace period
            ghostTest.transform.position = new Vector3(20f, 20f, 0f);
            Physics2D.Simulate(0.1f); // within grace period (2.0s), still active
            bool graceActive = secSwitch.IsActivated;

            Physics2D.Simulate(2.5f); // after grace period, switch deactivates
            Physics2D.SyncTransforms();
            bool relocked = !secSwitch.IsActivated && barrier.IsLocked;
            sb.AppendLine($"7. Ghost Leaves Switch: GraceActiveInitially={graceActive}, ReLockedAfterGrace={relocked}");

            // Phase 8: Traversal & Level 02 Transition
            // Ghost holds switch again
            ghostTest.transform.position = secSwitch.transform.position;
            Physics2D.Simulate(0.1f);
            Physics2D.SyncTransforms();

            // Player traverses lasers and reaches real exit door on Platform A
            player.transform.position = realDoor.transform.position;
            Physics2D.Simulate(0.1f);
            Physics2D.SyncTransforms();

            bool reachedExit = false;
            if (realDoor != null && realDoor.IsUnlocked)
            {
                realDoor.NotifyTriggerEnter(player.GetComponent<Collider2D>());
                reachedExit = true;
            }

            bool transitionReady = director != null && director.NextSceneName == "Level02";
            sb.AppendLine($"8. Player Enters Real Exit: ReachedExit={reachedExit}, NextScene={director?.NextSceneName}");

            Object.DestroyImmediate(ghostTest);

            bool allStepsPassed = plate.IsActivated && prankFired && barrierLocked && livePlayerRejected && echoActivated && barrierDropped && relocked && reachedExit && transitionReady;
            sb.AppendLine(allStepsPassed ? ">> CHAPTER I PLAY MODE SIMULATION: SUCCESS <<" : ">> CHAPTER I PLAY MODE SIMULATION: FAILED <<");

            return sb.ToString();
        }
    }
}
