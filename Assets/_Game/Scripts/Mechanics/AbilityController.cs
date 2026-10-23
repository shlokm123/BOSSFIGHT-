using System;
using UnityEngine;
using UnityEngine.InputSystem;
using GlitchVerse.Player;

namespace GlitchVerse.Mechanics
{
    /// <summary>
    /// Central manager for the three reality manipulation mechanics:
    /// - Gravity Inversion (G)
    /// - Time Freeze (F)
    /// - Dimension Switching (Q)
    /// Follows the architecture defined in docs/INTERFACES.md.
    /// </summary>
    public class AbilityController : MonoBehaviour
    {
        [Header("Unlocked Abilities per Level")]
        [SerializeField] private bool allowGravity = false;
        [SerializeField] private bool allowTimeFreeze = false;
        [SerializeField] private bool allowDimensionSwap = false;

        [Header("Cooldowns (seconds)")]
        [SerializeField] private float gravityCooldown = 0.4f;
        [SerializeField] private float timeFreezeDuration = 4.0f;
        [SerializeField] private float dimensionCooldown = 0.35f;

        [Header("State")]
        [SerializeField] private bool isGravityInverted = false;
        [SerializeField] private bool isTimeFrozen = false;
        [SerializeField] private DimensionType currentDimension = DimensionType.Alpha;

        private float lastGravityTime = -10f;
        private float freezeTimer = 0f;
        private float lastDimensionTime = -10f;
        private PlayerController2D player;

        public bool AllowGravity { get => allowGravity; set => allowGravity = value; }
        public bool AllowTimeFreeze { get => allowTimeFreeze; set => allowTimeFreeze = value; }
        public bool AllowDimensionSwap { get => allowDimensionSwap; set => allowDimensionSwap = value; }

        public bool IsGravityInverted => isGravityInverted;
        public bool IsTimeFrozen => isTimeFrozen;
        public DimensionType CurrentDimension => currentDimension;

        public event Action<bool> OnGravityToggled;
        public event Action<bool> OnTimeFreezeToggled;
        public event Action<DimensionType> OnDimensionToggled;

        private void Awake()
        {
            player = GetComponent<PlayerController2D>();
        }

        private void Start()
        {
            // Initialize world dimension state on start
            BroadcastDimensionState();
        }

        private void Update()
        {
            if (player != null && player.InputLocked) return;

            var kb = Keyboard.current;
            if (kb == null) return;

            // 1. Gravity Inversion (G)
            if (allowGravity && kb.gKey.wasPressedThisFrame && Time.time - lastGravityTime >= gravityCooldown)
            {
                ToggleGravity();
            }

            // 2. Time Freeze (F)
            if (allowTimeFreeze && kb.fKey.wasPressedThisFrame)
            {
                ToggleTimeFreeze();
            }

            // Auto-unfreeze timer
            if (isTimeFrozen)
            {
                freezeTimer -= Time.deltaTime;
                if (freezeTimer <= 0f)
                {
                    SetTimeFreeze(false);
                }
            }

            // 3. Dimension Switching (Q)
            if (allowDimensionSwap && kb.qKey.wasPressedThisFrame && Time.time - lastDimensionTime >= dimensionCooldown)
            {
                ToggleDimension();
            }
        }

        public void ToggleGravity()
        {
            lastGravityTime = Time.time;
            isGravityInverted = !isGravityInverted;

            if (player != null)
            {
                player.SetGravityInverted(isGravityInverted);
            }

            OnGravityToggled?.Invoke(isGravityInverted);
            Debug.Log($"[AbilityController] Gravity Inversion: {(isGravityInverted ? "INVERTED" : "NORMAL")}");
        }

        public void ToggleTimeFreeze()
        {
            SetTimeFreeze(!isTimeFrozen);
        }

        public void SetTimeFreeze(bool frozen)
        {
            isTimeFrozen = frozen;
            freezeTimer = isTimeFrozen ? timeFreezeDuration : 0f;

            // Notify all freezable obstacles in the scene
            var freezables = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude);
            foreach (var mb in freezables)
            {
                if (mb is IFreezable freezable)
                {
                    freezable.SetFrozen(isTimeFrozen);
                }
            }

            OnTimeFreezeToggled?.Invoke(isTimeFrozen);
            Debug.Log($"[AbilityController] Time Freeze: {(isTimeFrozen ? "ACTIVE" : "RESUMED")}");
        }

        public void ToggleDimension()
        {
            lastDimensionTime = Time.time;
            currentDimension = (currentDimension == DimensionType.Alpha) ? DimensionType.Beta : DimensionType.Alpha;
            BroadcastDimensionState();
            OnDimensionToggled?.Invoke(currentDimension);
            Debug.Log($"[AbilityController] Dimension Shifted to: {currentDimension}");
        }

        private void BroadcastDimensionState()
        {
            var dimObjects = FindObjectsByType<DimensionObject>(FindObjectsInactive.Include);
            foreach (var d in dimObjects)
            {
                d.SetDimensionState(currentDimension);
            }
        }
    }
}
