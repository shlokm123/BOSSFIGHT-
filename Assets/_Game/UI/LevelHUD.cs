using UnityEngine;
using UnityEngine.UI;
using GlitchVerse.Core;
using GlitchVerse.Interactables;
using GlitchVerse.Echo;
using GlitchVerse.Mechanics;

namespace GlitchVerse.UI
{
    /// <summary>
    /// Displays game title, room designation, instructions, timer, ability status,
    /// plate/door telemetry, echo status, and victory banner using lightweight Unity UI.
    /// Follows docs/INTERFACES.md and Phase 5 presentation requirements.
    /// </summary>
    public class LevelHUD : MonoBehaviour
    {
        [Header("UI Text References")]
        [SerializeField] private Text titleText;
        [SerializeField] private Text levelLabelText;
        [SerializeField] private Text timerText;
        [SerializeField] private Text instructionsText;
        [SerializeField] private Text abilityStatusText;
        [SerializeField] private Text puzzleStatusText;
        [SerializeField] private Text doorStatusText;
        [SerializeField] private Text echoHintText;
        [SerializeField] private Text completionBannerText;

        [Header("Completion Panel References")]
        [SerializeField] private GameObject completionPanel;
        [SerializeField] private Text completionTitleText;
        [SerializeField] private Text completionStatsText;
        [SerializeField] private Text completionPromptText;

        [Header("Scene Dependencies")]
        [SerializeField] private PressurePlate pressurePlate;
        [SerializeField] private EchoSwitch echoSwitch;
        [SerializeField] private ExitDoor exitDoor;
        [SerializeField] private LevelDirector levelDirector;
        [SerializeField] private EchoPlayback echoPlayback;
        [SerializeField] private AbilityController abilityController;

        private float roomTimer = 0f;

        private void Start()
        {
            if (levelDirector == null) levelDirector = LevelDirector.Current;
            if (pressurePlate == null) pressurePlate = FindAnyObjectByType<PressurePlate>(FindObjectsInactive.Exclude);
            if (echoSwitch == null) echoSwitch = FindAnyObjectByType<EchoSwitch>(FindObjectsInactive.Exclude);
            if (exitDoor == null) exitDoor = FindAnyObjectByType<ExitDoor>(FindObjectsInactive.Exclude);
            if (echoPlayback == null) echoPlayback = FindAnyObjectByType<EchoPlayback>(FindObjectsInactive.Exclude);
            if (abilityController == null) abilityController = FindAnyObjectByType<AbilityController>(FindObjectsInactive.Exclude);

            // Completion Panel fallbacks
            if (completionPanel == null)
            {
                var compObj = GameObject.Find("CompletionPanel");
                if (compObj != null) completionPanel = compObj;
            }
            if (completionTitleText == null)
            {
                var titleObj = GameObject.Find("CompletionTitle");
                if (titleObj != null) completionTitleText = titleObj.GetComponent<Text>();
            }
            if (completionStatsText == null)
            {
                var statsObj = GameObject.Find("CompletionStats");
                if (statsObj != null) completionStatsText = statsObj.GetComponent<Text>();
            }
            if (completionPromptText == null)
            {
                var promptObj = GameObject.Find("CompletionPrompt");
                if (promptObj != null) completionPromptText = promptObj.GetComponent<Text>();
            }
            if (completionPanel != null)
            {
                completionPanel.SetActive(false);
            }

            // Setup header labels
            if (titleText != null) titleText.text = "GLITCHVERSE: ECHO PROTOCOL";
            if (levelLabelText != null) levelLabelText.text = levelDirector != null ? levelDirector.LevelTitle : "CONTAINMENT";
            if (instructionsText != null) instructionsText.text = "[A/D] Move   [SPACE] Jump   [R] Restart";
            if (echoHintText != null) echoHintText.text = "[H] Replay Echo   [C] Clear";

            if (completionBannerText != null)
            {
                completionBannerText.gameObject.SetActive(false);
            }

            // Subscriptions
            if (pressurePlate != null)
            {
                pressurePlate.OnPlateStateChanged += HandlePlateChanged;
                HandlePlateChanged(pressurePlate.IsActivated);
            }

            if (echoSwitch != null)
            {
                echoSwitch.OnSwitchStateChanged += HandleEchoSwitchChanged;
                HandleEchoSwitchChanged(echoSwitch.IsActivated);
            }

            if (levelDirector != null)
            {
                levelDirector.OnLevelComplete += HandleLevelComplete;
            }

            UpdateDoorDisplay(exitDoor != null && exitDoor.IsUnlocked);
            UpdateAbilityStatus();
        }

        private void OnDestroy()
        {
            if (pressurePlate != null)
            {
                pressurePlate.OnPlateStateChanged -= HandlePlateChanged;
            }

            if (echoSwitch != null)
            {
                echoSwitch.OnSwitchStateChanged -= HandleEchoSwitchChanged;
            }

            if (levelDirector != null)
            {
                levelDirector.OnLevelComplete -= HandleLevelComplete;
            }
        }

        private void Update()
        {
            // Real-time room timer
            roomTimer += Time.deltaTime;
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(roomTimer / 60f);
                float seconds = roomTimer % 60f;
                timerText.text = $"TIME: {minutes:00}:{seconds:04.1f}";
            }

            UpdateAbilityStatus();
        }

        private void UpdateAbilityStatus()
        {
            if (abilityStatusText == null) return;

            if (abilityController == null)
            {
                abilityStatusText.text = "";
                return;
            }

            string abilities = "";
            if (abilityController.AllowGravity)
            {
                abilities += abilityController.IsGravityInverted
                    ? "<color=#00F0FF>[G] GRAV: INVERTED</color>   "
                    : "<color=#B0BCCB>[G] GRAV: NORMAL</color>   ";
            }

            if (abilityController.AllowTimeFreeze)
            {
                abilities += abilityController.IsTimeFrozen
                    ? "<color=#00F0FF>[F] TIME: FROZEN</color>   "
                    : "<color=#B0BCCB>[F] TIME: ACTIVE</color>   ";
            }

            if (abilityController.AllowDimensionSwap)
            {
                abilities += (abilityController.CurrentDimension == DimensionType.Alpha)
                    ? "<color=#00F0FF>[Q] DIM: ALPHA</color>"
                    : "<color=#FF22D6>[Q] DIM: BETA</color>";
            }

            abilityStatusText.text = abilities;
        }

        private void HandlePlateChanged(bool isActivated)
        {
            if (puzzleStatusText != null)
            {
                puzzleStatusText.text = isActivated 
                    ? "<color=#00F0FF>● SENSOR PLATE: ACTIVE</color>" 
                    : "<color=#888888>○ SENSOR PLATE: INACTIVE</color>";
            }

            UpdateDoorDisplay(isActivated);
        }

        private void HandleEchoSwitchChanged(bool isActivated)
        {
            if (puzzleStatusText != null)
            {
                puzzleStatusText.text = isActivated 
                    ? "<color=#D600FF>● ECHO SWITCH: TRIGGERED</color>" 
                    : "<color=#888888>○ ECHO SWITCH: WAITING FOR ECHO</color>";
            }

            UpdateDoorDisplay(isActivated);
        }

        private void UpdateDoorDisplay(bool isUnlocked)
        {
            if (doorStatusText != null)
            {
                doorStatusText.text = isUnlocked 
                    ? "<color=#00F0FF>EXIT DOOR: UNLOCKED</color>" 
                    : "<color=#FF1E40>EXIT DOOR: LOCKED</color>";
            }
        }

        public GameObject CompletionPanel => completionPanel;

        private void HandleLevelComplete()
        {
            // In the final level (Level07), FinalEndingController handles the cinematic plot-twist ending sequence
            if (levelDirector != null && levelDirector.IsFinalLevel) return;

            if (completionPanel != null)
            {
                completionPanel.SetActive(true);

                if (completionTitleText != null)
                {
                    completionTitleText.text = (levelDirector != null && levelDirector.IsFinalLevel)
                        ? "ARCHITECT CORE DISABLED"
                        : "CONTAINMENT PROTOCOL CLEARED";
                }

                if (completionStatsText != null)
                {
                    int minutes = Mathf.FloorToInt(roomTimer / 60f);
                    float seconds = roomTimer % 60f;
                    int restarts = GameSession.Instance != null ? GameSession.Instance.ResetCount : 0;
                    completionStatsText.text = $"ELAPSED TIME: {minutes:00}:{seconds:00.0}   TOTAL RESTARTS: {restarts}";
                }

                if (completionPromptText != null)
                {
                    if (levelDirector != null && levelDirector.IsFinalLevel)
                    {
                        completionPromptText.text = "[R] RESTART SIMULATION FROM BEGINNING";
                    }
                    else if (levelDirector != null && !string.IsNullOrEmpty(levelDirector.NextSceneName))
                    {
                        completionPromptText.text = $"TRANSITIONING TO {levelDirector.NextSceneName.ToUpper()}...";
                    }
                    else
                    {
                        completionPromptText.text = "[SPACE] ADVANCE TO NEXT CHAMBER";
                    }
                }
            }

            if (completionBannerText != null)
            {
                completionBannerText.gameObject.SetActive(true);

                if (levelDirector != null && levelDirector.IsFinalLevel)
                {
                    completionBannerText.text = 
                        "<color=#FF1E40><size=44>ARCHITECT CORE DISABLED</size></color>\n" +
                        "<size=24><color=#00F0FF>EXTERNAL CONTAINMENT ACCESS GRANTED</color></size>\n" +
                        $"<size=18><color=#FFAA00>FINAL TIME: {Mathf.FloorToInt(roomTimer / 60f):00}:{(roomTimer % 60f):00.0}   RESTARTS: {(GameSession.Instance != null ? GameSession.Instance.ResetCount : 0)}</color></size>";
                }
                else
                {
                    string nextInfo = (levelDirector != null && !string.IsNullOrEmpty(levelDirector.NextSceneName)) 
                        ? $"TRANSITIONING TO {levelDirector.NextSceneName.ToUpper()}..." 
                        : "SIMULATION PROTOCOL VALIDATED — PRESS R TO RESET";

                    completionBannerText.text = 
                        "<size=40>TEST COMPLETE</size>\n" +
                        $"<size=18><color=#00F0FF>{nextInfo}</color></size>";
                }
            }
        }
    }
}
