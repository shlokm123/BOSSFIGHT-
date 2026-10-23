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
        [SerializeField] private float stage2TriggerX = 1.0f; // After crossing frozen laser
        [SerializeField] private float stage3TriggerX = 6.0f; // Inside final vault before exit door

        [Header("Stage 1 Blast Gate & Echo Synchronization")]
        [SerializeField] private EchoSwitch echoSwitch;
        [SerializeField] private GameObject blastGate;

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

            // Stage 2: Crossing moving hazard
            if (!stage2Triggered && px >= stage2TriggerX)
            {
                stage2Triggered = true;
                if (ArchitectDialogue.Current != null)
                {
                    ArchitectDialogue.Current.Speak(
                        "Not once did you ask why the doors were locked.",
                        "CONTAINMENT PROTOCOL",
                        3.2f,
                        "truth_stage2",
                        10f,
                        priority: true
                    );
                }
            }

            // Stage 3: Reaching final vault
            if (!stage3Triggered && px >= stage3TriggerX)
            {
                stage3Triggered = true;
                if (ArchitectDialogue.Current != null)
                {
                    ArchitectDialogue.Current.Speak(
                        "There's something outside you don't understand.",
                        "EXTERNAL THREAT",
                        3.5f,
                        "truth_stage3",
                        10f,
                        priority: true
                    );
                }
            }
        }
    }
}
