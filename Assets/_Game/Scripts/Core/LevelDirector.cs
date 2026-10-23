using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using GlitchVerse.Interactables;
using GlitchVerse.Player;

namespace GlitchVerse.Core
{
    /// <summary>
    /// Scene-contained coordinator managing level spawn, puzzle flow,
    /// and win/fail conditions.
    /// Publishes OnLevelComplete and OnPlayerDied as defined in docs/INTERFACES.md.
    /// </summary>
    public class LevelDirector : MonoBehaviour
    {
        private static LevelDirector instance;
        public static LevelDirector Current
        {
            get
            {
                if (instance == null) instance = FindAnyObjectByType<LevelDirector>();
                return instance;
            }
            private set => instance = value;
        }

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

        private PlayerController2D localPlayer;

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

            // Defensive fallback: ensure chamber number and next scene are never missing
            if (string.IsNullOrEmpty(nextSceneName) && !isFinalLevel || chamberNumber <= 0)
            {
                string activeScene = SceneManager.GetActiveScene().name;
                if (activeScene == "Level01")
                {
                    chamberNumber = 1;
                    levelTitle = "CONTAINMENT 01";
                    nextSceneName = "Level02";
                    isFinalLevel = false;
                }
                else if (activeScene == "Level02")
                {
                    chamberNumber = 2;
                    levelTitle = "CONTAINMENT 02";
                    nextSceneName = "Level03";
                    isFinalLevel = false;
                }
                else if (activeScene == "Level03")
                {
                    chamberNumber = 3;
                    levelTitle = "CONTAINMENT 03";
                    nextSceneName = "Level04";
                    isFinalLevel = false;
                }
                else if (activeScene == "Level04")
                {
                    chamberNumber = 4;
                    levelTitle = "CONTAINMENT 04";
                    nextSceneName = "Level05";
                    isFinalLevel = false;
                }
                else if (activeScene == "Level05")
                {
                    chamberNumber = 5;
                    levelTitle = "CONTAINMENT 05";
                    nextSceneName = "Level06";
                    isFinalLevel = false;
                }
                else if (activeScene == "Level06")
                {
                    chamberNumber = 6;
                    levelTitle = "CONTAINMENT 06";
                    nextSceneName = "Level07";
                    isFinalLevel = false;
                }
                else if (activeScene == "Level07")
                {
                    chamberNumber = 7;
                    levelTitle = "CONTAINMENT 07 // JUDGMENT";
                    nextSceneName = "";
                    isFinalLevel = true;
                }
            }
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

            // Subscribe to all doors in the scene (including relocated real exit doors)
            var allDoors = FindObjectsByType<ExitDoor>(FindObjectsInactive.Include);
            foreach (var d in allDoors)
            {
                if (d != null)
                {
                    d.OnPlayerReachedExit += HandlePlayerReachedExit;
                }
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

            var allDoors = FindObjectsByType<ExitDoor>(FindObjectsInactive.Include);
            foreach (var d in allDoors)
            {
                if (d != null)
                {
                    d.OnPlayerReachedExit -= HandlePlayerReachedExit;
                }
            }

            if (Current == this)
            {
                Current = null;
            }
        }

        private void Update()
        {
            if (localPlayer == null)
            {
                localPlayer = FindAnyObjectByType<PlayerController2D>(FindObjectsInactive.Exclude);
            }

            if (localPlayer != null && localPlayer.transform.position.y < -7.0f)
            {
                KillPlayer();
                return;
            }

            // If GameSession is not yet present, allow local R restart
            var kb = Keyboard.current;
            if (kb != null && kb.rKey.wasPressedThisFrame && GameSession.Instance == null)
            {
                RestartLevel();
            }
        }

        private void HandlePlateStateChanged(bool isActivated)
        {
            var prank = FindAnyObjectByType<GlitchVerse.Narrative.FakeExitPrank>();
            bool isPrankActive = prank != null && prank.IsPrankTriggered;

            var allDoors = FindObjectsByType<ExitDoor>(FindObjectsInactive.Include);
            foreach (var d in allDoors)
            {
                if (d != null)
                {
                    if (isPrankActive && d.gameObject.name.Contains("RealExit"))
                    {
                        d.SetUnlocked(true);
                    }
                    else if (isPrankActive && d.GetComponent<GlitchVerse.Narrative.FakeExitPrank>() != null)
                    {
                        d.SetUnlocked(false);
                    }
                    else
                    {
                        d.SetUnlocked(isActivated);
                    }
                }
            }
        }

        public void CompleteChamber()
        {
            HandlePlayerReachedExit();
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
            yield return new WaitForSecondsRealtime(delay);
            Time.timeScale = 1.0f; // Ensure time scale is strictly restored
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
            Time.timeScale = 1.0f;
            if (isFinalLevel && isLevelCompleted)
            {
                SceneManager.LoadScene(0);
                return;
            }
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
