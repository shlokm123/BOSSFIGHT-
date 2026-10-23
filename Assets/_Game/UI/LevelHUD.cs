using UnityEngine;
using UnityEngine.UI;
using GlitchVerse.Core;
using GlitchVerse.Interactables;
using GlitchVerse.Echo;

namespace GlitchVerse.UI
{
    /// <summary>
    /// Displays game title, room designation, instructions, plate/door telemetry,
    /// and completion notices using lightweight standard Unity UI.
    /// </summary>
    public class LevelHUD : MonoBehaviour
    {
        [Header("UI Text References")]
        [SerializeField] private Text titleText;
        [SerializeField] private Text levelLabelText;
        [SerializeField] private Text instructionsText;
        [SerializeField] private Text puzzleStatusText;
        [SerializeField] private Text doorStatusText;
        [SerializeField] private Text echoHintText;
        [SerializeField] private Text completionBannerText;

        [Header("Scene Dependencies")]
        [SerializeField] private PressurePlate pressurePlate;
        [SerializeField] private ExitDoor exitDoor;
        [SerializeField] private LevelDirector levelDirector;
        [SerializeField] private EchoPlayback echoPlayback;

        private void Start()
        {
            if (levelDirector == null) levelDirector = LevelDirector.Current;
            if (pressurePlate == null) pressurePlate = FindFirstObjectByType<PressurePlate>(FindObjectsInactive.Exclude);
            if (exitDoor == null) exitDoor = FindFirstObjectByType<ExitDoor>(FindObjectsInactive.Exclude);
            if (echoPlayback == null) echoPlayback = FindFirstObjectByType<EchoPlayback>(FindObjectsInactive.Exclude);

            // Setup static header labels
            if (titleText != null) titleText.text = "GLITCHVERSE: ECHO PROTOCOL";
            if (levelLabelText != null) levelLabelText.text = levelDirector != null ? levelDirector.LevelTitle : "CONTAINMENT 01";
            if (instructionsText != null) instructionsText.text = "[A/D] Move   [SPACE] Jump   [R] Restart";
            if (echoHintText != null) echoHintText.text = "[H] Replay Echo";

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

            if (levelDirector != null)
            {
                levelDirector.OnLevelComplete += HandleLevelComplete;
            }

            UpdateDoorDisplay(exitDoor != null && exitDoor.IsUnlocked);
        }

        private void OnDestroy()
        {
            if (pressurePlate != null)
            {
                pressurePlate.OnPlateStateChanged -= HandlePlateChanged;
            }

            if (levelDirector != null)
            {
                levelDirector.OnLevelComplete -= HandleLevelComplete;
            }
        }

        private void HandlePlateChanged(bool isActivated)
        {
            if (puzzleStatusText != null)
            {
                puzzleStatusText.text = isActivated 
                    ? "<color=#00F0FF>● PRESSURE PLATE: ACTIVE</color>" 
                    : "<color=#888888>○ PRESSURE PLATE: INACTIVE</color>";
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

        private void HandleLevelComplete()
        {
            if (completionBannerText != null)
            {
                completionBannerText.gameObject.SetActive(true);
                completionBannerText.text = "TEST COMPLETE\n<size=18><color=#00F0FF>SIMULATION PROTOCOL VALIDATED — PRESS R TO RESET</color></size>";
            }
        }
    }
}
