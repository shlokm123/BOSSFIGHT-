using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using GlitchVerse.Interactables;

namespace GlitchVerse.Core
{
    /// <summary>
    /// Scene-contained coordinator managing level spawn, puzzle flow,
    /// and win/fail conditions.
    /// Publishes OnLevelComplete and OnPlayerDied as defined in docs/INTERFACES.md.
    /// </summary>
    public class LevelDirector : MonoBehaviour
    {
        public static LevelDirector Current { get; private set; }

        [Header("Chamber Identity")]
        [SerializeField] private string levelTitle = "CONTAINMENT 01";
        [SerializeField] private int chamberNumber = 1;
        [SerializeField] private string nextSceneName = "";
        [SerializeField] private bool isFinalLevel = false;

        [Header("Scene References")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private PressurePlate pressurePlate;
        [SerializeField] private GlitchVerse.Echo.EchoSwitch echoSwitch;
        [SerializeField] private ExitDoor exitDoor;

        [Header("State")]
        [SerializeField] private bool isLevelCompleted = false;

        public string LevelTitle => levelTitle;
        public int ChamberNumber => chamberNumber;
        public bool IsLevelCompleted => isLevelCompleted;
        public bool IsFinalLevel => isFinalLevel;
        public string NextSceneName => nextSceneName;
        public Transform SpawnPoint => spawnPoint;

        public event Action OnLevelComplete;
        public event Action OnPlayerDied;

        private void Awake()
        {
            Current = this;
        }

        private void Start()
        {
            if (pressurePlate != null && exitDoor != null)
            {
                pressurePlate.OnPlateStateChanged += HandlePlateStateChanged;
            }

            if (echoSwitch != null && exitDoor != null)
            {
                echoSwitch.OnSwitchStateChanged += HandlePlateStateChanged;
            }

            if (exitDoor != null)
            {
                exitDoor.OnPlayerReachedExit += HandlePlayerReachedExit;
            }
        }

        private void OnDestroy()
        {
            if (pressurePlate != null)
            {
                pressurePlate.OnPlateStateChanged -= HandlePlateStateChanged;
            }

            if (echoSwitch != null)
            {
                echoSwitch.OnSwitchStateChanged -= HandlePlateStateChanged;
            }

            if (exitDoor != null)
            {
                exitDoor.OnPlayerReachedExit -= HandlePlayerReachedExit;
            }

            if (Current == this)
            {
                Current = null;
            }
        }

        private void Update()
        {
            // If GameSession is not yet present, allow local R restart
            var kb = Keyboard.current;
            if (kb != null && kb.rKey.wasPressedThisFrame && GameSession.Instance == null)
            {
                RestartLevel();
            }
        }

        private void HandlePlateStateChanged(bool isActivated)
        {
            if (exitDoor != null)
            {
                exitDoor.SetUnlocked(isActivated);
            }
        }

        private void HandlePlayerReachedExit()
        {
            if (isLevelCompleted) return;

            isLevelCompleted = true;
            Debug.Log($"[LevelDirector] Chamber '{levelTitle}' successfully completed!");
            OnLevelComplete?.Invoke();

            if (!string.IsNullOrEmpty(nextSceneName))
            {
                StartCoroutine(LoadNextLevelDelayed(1.4f));
            }
        }

        private System.Collections.IEnumerator LoadNextLevelDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log($"[LevelDirector] Loading next chamber: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }

        public void KillPlayer()
        {
            Debug.Log("[LevelDirector] Player killed. Triggering restart.");
            OnPlayerDied?.Invoke();
            RestartLevel();
        }

        public void RestartLevel()
        {
            if (GameSession.Instance != null)
            {
                GameSession.Instance.RestartCurrentLevel();
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
