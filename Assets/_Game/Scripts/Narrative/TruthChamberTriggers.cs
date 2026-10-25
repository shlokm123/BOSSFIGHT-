using UnityEngine;
using GlitchVerse.Player;
using GlitchVerse.Echo;

namespace GlitchVerse.Narrative
{
    /// <summary>
    /// Narrative milestone trigger controller for Chapter VI (Truth).
    /// As the player solves the multi-stage quarantine chamber, the Architect
    /// sheds all sarcastic humor, speaking with chilling, matter-of-fact seriousness.
    /// Also manages Stage 1 blast gate synchronization with the elevated Echo switch.
    /// </summary>
    public class TruthChamberTriggers : MonoBehaviour
    {
        [Header("Progression Trigger Positions")]
        [SerializeField] private float stage1TriggerX = -2.0f; // After crossing Echo quarantine seal 1
        [SerializeField] private float stage2TriggerX = 2.5f;  // After crossing frozen laser node
        [SerializeField] private float stage3TriggerX = 7.6f;  // In final vault before exit door

        [Header("Stage 1 Blast Gate & Echo Synchronization")]
        [SerializeField] private EchoSwitch echoSwitch;
        [SerializeField] private GameObject blastGate;

        private bool stage1Triggered = false;
        private bool stage2Triggered = false;
        private bool stage3Triggered = false;
        private PlayerController2D player;

        private void Start()
        {
            player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);

            if (echoSwitch != null)
            {
                echoSwitch.OnSwitchStateChanged += HandleSwitchStateChanged;
                if (blastGate != null)
                {
                    blastGate.SetActive(!echoSwitch.IsActivated);
                }
            }
        }

        private void OnDestroy()
        {
            if (echoSwitch != null)
            {
                echoSwitch.OnSwitchStateChanged -= HandleSwitchStateChanged;
            }
        }

        private void HandleSwitchStateChanged(bool activated)
        {
            if (blastGate != null)
            {
                blastGate.SetActive(!activated);
            }
        }

        private void Update()
        {
            if (player == null)
            {
                player = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);
                if (player == null) return;
            }

            float px = player.transform.position.x;

            // Stage 1: Breaching initial quarantine gate
            if (!stage1Triggered && px >= stage1TriggerX)
            {
                stage1Triggered = true;
                if (ArchitectDialogue.Current != null)
                {
                    ArchitectDialogue.Current.Speak(
                        "You've spent six chambers proving you can escape.",
                        "CONTAINMENT PROTOCOL",
                        3.4f,
                        "truth_stage1_escape",
                        0f,
                        priority: true
                    );
                }
            }

            // Stage 2: Crossing frozen hazard node
            if (!stage2Triggered && px >= stage2TriggerX)
            {
                stage2Triggered = true;
                if (ArchitectDialogue.Current != null)
                {
                    ArchitectDialogue.Current.Speak(
                        "Not once did you ask why the doors were locked.",
                        "EXTERNAL THREAT",
                        3.4f,
                        "truth_stage2_doors",
                        0f,
                        priority: true
                    );
                }
            }

            // Stage 3: Approaching final quarantine vault door
            if (!stage3Triggered && px >= stage3TriggerX)
            {
                stage3Triggered = true;
                if (ArchitectDialogue.Current != null)
                {
                    ArchitectDialogue.Current.Speak(
                        "There is still time to turn back.",
                        "FINAL WARNING",
                        3.6f,
                        "truth_stage3_turnback",
                        0f,
                        priority: true
                    );
                }
            }
        }
    }
}
