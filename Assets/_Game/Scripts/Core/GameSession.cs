using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GlitchVerse.Core
{
    /// <summary>
    /// Persistent global session manager holding score, timer, progression,
    /// and echo recording snapshots across scenes.
    /// Follows the architecture defined in docs/INTERFACES.md and docs/PRODUCTION_PLAN.md.
    /// </summary>
    public class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }

        [Header("Session Tracking")]
        [SerializeField] private float sessionElapsedTime;
        [SerializeField] private int currentLevelIndex = 1;
        [SerializeField] private string currentLevelName = "CONTAINMENT 01";
        [SerializeField] private int resetCount = 0;

        public float SessionElapsedTime => sessionElapsedTime;
        public int CurrentLevelIndex => currentLevelIndex;
        public string CurrentLevelName => currentLevelName;
        public int ResetCount => resetCount;

        public event Action<float> OnTimerTick;
        public event Action OnLevelRestarted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            sessionElapsedTime += Time.deltaTime;
            OnTimerTick?.Invoke(sessionElapsedTime);

            // Global restart shortcut 'R' as mandated in docs/INTERFACES.md
            var kb = Keyboard.current;
            if (kb != null && kb.rKey.wasPressedThisFrame)
            {
                RestartCurrentLevel();
            }
        }

        /// <summary>
        /// Restarts the currently active scene and increments the reset counter.
        /// </summary>
        public void RestartCurrentLevel()
        {
            resetCount++;
            OnLevelRestarted?.Invoke();
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.buildIndex);
        }

        /// <summary>
        /// Updates the current level designation.
        /// </summary>
        public void SetLevel(int index, string name)
        {
            currentLevelIndex = index;
            currentLevelName = name;
        }
    }
}
