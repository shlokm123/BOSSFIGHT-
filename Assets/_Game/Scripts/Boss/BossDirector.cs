using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using GlitchVerse.Player;
using GlitchVerse.Core;
using GlitchVerse.Narrative;
using GlitchVerse.Echo;
using GlitchVerse.Mechanics;

namespace GlitchVerse.Boss
{
    /// <summary>
    /// Master State Machine Controller for Chapter VII: The Architect Boss Encounter.
    /// Manages three escalating puzzle-hazard stages:
    /// - Stage 1: Gravity Assault (evade telegraphed surges via [G] Gravity Inversion -> Node 1)
    /// - Stage 2: Reality Collapse (time-freeze moving hazard [F] + dimension swap [Q] -> Node 2)
    /// - Stage 3: Echo Protocol (record/replay echo ghost to hold shield switch [H/C] -> Node 3)
    /// - Shutdown Sequence: Dramatic core power-down, fear narrative escalation, temporary victory.
    /// </summary>
    public class BossDirector : MonoBehaviour
    {
        public static BossDirector Instance { get; private set; }

        /// <summary>
        /// Checkpoint persistence across player deaths during the encounter.
        /// Prevents replaying entire multi-stage fight on minor missteps.
        /// </summary>
        public static int SavedStageCheckpoint = 1;

        public enum BossPhase
        {
            Stage1_GravityAssault = 1,
            Stage2_RealityCollapse = 2,
            Stage3_EchoProtocol = 3,
            Defeated = 4
        }

        [Header("State")]
        [SerializeField] private BossPhase currentPhase = BossPhase.Stage1_GravityAssault;
        [SerializeField] private int coreIntegrity = 3;
        [SerializeField] private bool isDefeated = false;

        [Header("Stage Checkpoint Spawn Positions")]
        [SerializeField] private Vector3 stage1SpawnPos = new Vector3(-8.0f, -3.5f, 0f);
        [SerializeField] private Vector3 stage2SpawnPos = new Vector3(-8.0f, -3.5f, 0f);
        [SerializeField] private Vector3 stage3SpawnPos = new Vector3(-8.0f, -3.5f, 0f);

        [Header("Stage 1 Components")]
        [SerializeField] private BossHazardEmitter[] stage1Hazards;
        [SerializeField] private BossControlNode node1;

        [Header("Stage 2 Components")]
        [SerializeField] private GameObject stage2Chasm;
        [SerializeField] private FreezableHazard stage2Hazard;
        [SerializeField] private GameObject stage2PuzzleRoot;
        [SerializeField] private BossControlNode node2;

        [Header("Stage 3 Components")]
        [SerializeField] private EchoSwitch stage3EchoSwitch;
        [SerializeField] private GameObject stage3ShieldBarrier;
        [SerializeField] private BossControlNode node3;

        [Header("Architect Core Visuals")]
        [SerializeField] private GameObject architectEyeCore;
        [SerializeField] private SpriteRenderer coreRenderer;
        [SerializeField] private Light2D coreLight;
        [SerializeField] private Light2D coreAlarmStrobe;

        [Header("UI Status Overlay")]
        [SerializeField] private Text bossStatusText;

        private PlayerController2D player;
        private LevelDirector levelDirector;

        public BossPhase CurrentPhase => currentPhase;
        public int CoreIntegrity => coreIntegrity;
        public bool IsDefeated => isDefeated;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);
            levelDirector = FindAnyObjectByType<LevelDirector>(FindObjectsInactive.Exclude);

            // Restore stage checkpoint if player died in later phases
            int checkpoint = Mathf.Clamp(SavedStageCheckpoint, 1, 3);
            currentPhase = (BossPhase)checkpoint;
            coreIntegrity = 4 - checkpoint; // 3/3 for stage 1, 2/3 for stage 2, 1/3 for stage 3

            ConfigureStageState(currentPhase, isInitialLoad: true);
            UpdateBossUI();

            // Wire Stage 3 EchoSwitch to lower Shield
            if (stage3EchoSwitch != null)
            {
                stage3EchoSwitch.OnSwitchStateChanged += HandleStage3SwitchChanged;
            }
        }

        private void OnDestroy()
        {
            if (stage3EchoSwitch != null)
            {
                stage3EchoSwitch.OnSwitchStateChanged -= HandleStage3SwitchChanged;
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            if (player == null)
            {
                player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);
            }

            // Pit death recovery to stage checkpoint
            if (player != null && player.transform.position.y < -7.0f && !isDefeated)
            {
                OnPlayerHitHazard();
            }
        }

        private void ConfigureStageState(BossPhase phase, bool isInitialLoad)
        {
            // Position player at current stage spawn if resuming
            if (isInitialLoad && player != null)
            {
                Vector3 spawn = GetSpawnPositionForPhase(phase);
                player.transform.position = spawn;
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.position = spawn;
                    rb.linearVelocity = Vector2.zero;
                }
            }

            switch (phase)
            {
                case BossPhase.Stage1_GravityAssault:
                    SetHazardsActive(stage1Hazards, true);
                    if (node1 != null) node1.gameObject.SetActive(true);
                    if (stage2PuzzleRoot != null) stage2PuzzleRoot.SetActive(false);
                    if (stage3ShieldBarrier != null) stage3ShieldBarrier.SetActive(false);
                    if (isInitialLoad && ArchitectDialogue.Current != null)
                    {
                        ArchitectDialogue.Current.Speak(
                            "YOU REALLY THINK YOU CAN REWRITE MY RULES?",
                            "SYS: FINAL PROTOCOL",
                            3.4f,
                            "boss_stage1_intro",
                            15f,
                            priority: true
                        );
                    }
                    break;

                case BossPhase.Stage2_RealityCollapse:
                    SetHazardsActive(stage1Hazards, false);
                    if (node1 != null) node1.ActivateNode();
                    if (stage2PuzzleRoot != null) stage2PuzzleRoot.SetActive(true);
                    if (stage2Hazard != null) stage2Hazard.gameObject.SetActive(true);
                    if (node2 != null) node2.gameObject.SetActive(true);
                    if (stage3ShieldBarrier != null) stage3ShieldBarrier.SetActive(false);
                    if (ArchitectDialogue.Current != null)
                    {
                        ArchitectDialogue.Current.Speak(
                            "I BUILT THIS REALITY. YOU ONLY LEARNED TO BREAK IT.",
                            "SYS: REALITY FAULT",
                            3.5f,
                            "boss_stage2_intro",
                            15f,
                            priority: true
                        );
                    }
                    break;

                case BossPhase.Stage3_EchoProtocol:
                    SetHazardsActive(stage1Hazards, false);
                    if (node1 != null) node1.ActivateNode();
                    if (node2 != null) node2.ActivateNode();
                    if (stage2Hazard != null) stage2Hazard.gameObject.SetActive(false);
                    if (stage2PuzzleRoot != null) stage2PuzzleRoot.SetActive(true);
                    if (stage3ShieldBarrier != null) stage3ShieldBarrier.SetActive(true);
                    if (node3 != null) node3.gameObject.SetActive(true);
                    if (ArchitectDialogue.Current != null)
                    {
                        ArchitectDialogue.Current.Speak(
                            "YOU ARE USING MY OWN SYSTEM AGAINST ME.",
                            "SYS: RECURSIVE LOOP",
                            3.5f,
                            "boss_stage3_intro",
                            15f,
                            priority: true
                        );
                    }
                    break;
            }
        }

        private Vector3 GetSpawnPositionForPhase(BossPhase phase)
        {
            switch (phase)
            {
                case BossPhase.Stage2_RealityCollapse: return stage2SpawnPos;
                case BossPhase.Stage3_EchoProtocol: return stage3SpawnPos;
                default: return stage1SpawnPos;
            }
        }

        private void SetHazardsActive(BossHazardEmitter[] emitters, bool active)
        {
            if (emitters == null) return;
            foreach (var e in emitters)
            {
                if (e != null)
                {
                    e.SetEmitterActive(active);
                }
            }
        }

        private void HandleStage3SwitchChanged(bool isActivated)
        {
            // Echo ghost or live player stepping on EchoSwitch drops the forcefield
            if (stage3ShieldBarrier != null)
            {
                stage3ShieldBarrier.SetActive(!isActivated);
                Debug.Log($"[BossDirector] Stage 3 Shield Barrier Active: {!isActivated}");
            }
        }

        public void OnNodeActivated(int stageNumber)
        {
            if (isDefeated) return;

            Debug.Log($"[BossDirector] Boss Node {stageNumber} triggered! Current Phase: {currentPhase}");

            if (stageNumber == 1 && currentPhase == BossPhase.Stage1_GravityAssault)
            {
                // Advance to Stage 2
                currentPhase = BossPhase.Stage2_RealityCollapse;
                coreIntegrity = 2;
                SavedStageCheckpoint = 2;
                UpdateBossUI();
                ConfigureStageState(currentPhase, isInitialLoad: false);
            }
            else if (stageNumber == 2 && currentPhase == BossPhase.Stage2_RealityCollapse)
            {
                // Advance to Stage 3
                currentPhase = BossPhase.Stage3_EchoProtocol;
                coreIntegrity = 1;
                SavedStageCheckpoint = 3;
                UpdateBossUI();
                ConfigureStageState(currentPhase, isInitialLoad: false);
            }
            else if (stageNumber == 3 && currentPhase == BossPhase.Stage3_EchoProtocol)
            {
                // Boss Defeated!
                currentPhase = BossPhase.Defeated;
                coreIntegrity = 0;
                isDefeated = true;
                SavedStageCheckpoint = 1; // Reset checkpoint on total victory
                UpdateBossUI();
                StartCoroutine(ExecuteBossDefeatSequence());
            }
        }

        public void OnPlayerHitHazard()
        {
            if (isDefeated) return;

            Debug.Log($"[BossDirector] Player took damage in {currentPhase}. Restoring stage checkpoint.");

            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.FlashScreenRed(0.4f, 0.4f);
            }

            // Respawn cleanly at current stage spawn position
            if (player != null)
            {
                Vector3 spawn = GetSpawnPositionForPhase(currentPhase);
                player.transform.position = spawn;
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.position = spawn;
                    rb.linearVelocity = Vector2.zero;
                }
                player.SetGravityInverted(false);
            }

            // Restore hazard integrity for current stage
            ConfigureStageState(currentPhase, isInitialLoad: false);
        }

        private IEnumerator ExecuteBossDefeatSequence()
        {
            Debug.Log("[BossDirector] INITIATING ARCHITECT SHUTDOWN SEQUENCE!");

            // Lock player controls during cinematic shutdown
            if (player != null)
            {
                player.InputLocked = true;
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }

            // Deactivate all hazards and shields
            SetHazardsActive(stage1Hazards, false);
            if (stage2Hazard != null) stage2Hazard.gameObject.SetActive(false);
            if (stage3ShieldBarrier != null) stage3ShieldBarrier.SetActive(false);

            // 1. Architect Narrative Escalation: Terror & Desperation
            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.FlashScreenRed(0.6f, 0.6f);
                ArchitectDialogue.Current.Speak(
                    "SUBJECT 075. STOP.",
                    "SYS: CORE CRITICAL",
                    2.4f,
                    "boss_defeat_1",
                    0f,
                    priority: true
                );
            }

            yield return new WaitForSecondsRealtime(2.3f);

            // 2. Chilling foreshadowing before shutdown
            if (ArchitectDialogue.Current != null)
            {
                ArchitectDialogue.Current.FlashScreenRed(0.4f, 0.5f);
                ArchitectDialogue.Current.Speak(
                    "YOU DON'T UNDERSTAND WHAT THIS DOOR CONTAINS.",
                    "SYS: CONTAINMENT BREACH",
                    3.2f,
                    "boss_defeat_2",
                    0f,
                    priority: true
                );
            }

            yield return new WaitForSecondsRealtime(3.0f);

            // 3. Physical Core Shutdown Visuals
            if (coreRenderer != null)
            {
                coreRenderer.color = new Color(0.2f, 0.35f, 0.45f, 0.7f); // Pale dormant blue-gray
            }
            if (coreLight != null)
            {
                coreLight.color = new Color(0f, 0.8f, 1f, 1f);
                coreLight.intensity = 0.3f;
            }
            if (coreAlarmStrobe != null)
            {
                coreAlarmStrobe.intensity = 0f;
            }

            yield return new WaitForSecondsRealtime(1.0f);

            // 4. Trigger Chamber Victory & Temporary Victory Screen
            if (levelDirector != null)
            {
                levelDirector.CompleteChamber();
            }
            else
            {
                var dir = FindAnyObjectByType<LevelDirector>();
                if (dir != null) dir.CompleteChamber();
            }
        }

        private void UpdateBossUI()
        {
            if (bossStatusText == null)
            {
                var found = GameObject.Find("BossCoreIntegrityText");
                if (found != null) bossStatusText = found.GetComponent<Text>();
            }

            if (bossStatusText != null)
            {
                string statusColor = coreIntegrity switch
                {
                    3 => "#FF1E40", // Alert Red
                    2 => "#FFAA00", // Amber Warning
                    1 => "#00F0FF", // Vulnerable Cyan
                    _ => "#00FF66"  // Overridden Green
                };

                string stageName = currentPhase switch
                {
                    BossPhase.Stage1_GravityAssault => "PHASE 1: GRAVITY ASSAULT",
                    BossPhase.Stage2_RealityCollapse => "PHASE 2: REALITY COLLAPSE",
                    BossPhase.Stage3_EchoProtocol => "PHASE 3: ECHO PROTOCOL",
                    _ => "ARCHITECT CORE OFFLINE"
                };

                bossStatusText.text = $"<color={statusColor}>CORE INTEGRITY: {coreIntegrity}/3</color>   <size=14><color=#888888>// {stageName}</color></size>";
            }
        }

        public static void ResetSessionCheckpoint()
        {
            SavedStageCheckpoint = 1;
        }
    }
}
