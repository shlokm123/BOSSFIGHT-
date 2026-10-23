using UnityEngine;
using UnityEngine.InputSystem;

namespace GlitchVerse.Player
{
    /// <summary>
    /// Input adapter connecting Unity's Input System to PlayerController2D.
    /// Reads 'Move' and 'Jump' actions from the 'Player' action map in InputSystem_Actions.
    /// Safely enables, disables, subscribes, and unsubscribes action callbacks without dangling references.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Reference to the project's InputSystem_Actions asset (Assets/Settings/InputSystem_Actions.inputactions).")]
        [SerializeField] private InputActionAsset inputActionsAsset;

        [Header("Target Controller")]
        [Tooltip("Target player controller to receive movement commands. If unassigned, acquired via GetComponent.")]
        [SerializeField] private PlayerController2D controller;

        // Cached Action References
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private bool _isInitialized;

        private void Awake()
        {
            if (controller == null && !TryGetComponent(out controller))
            {
                Debug.LogError($"[{nameof(PlayerInputHandler)}] Missing {nameof(PlayerController2D)} on '{gameObject.name}'.", this);
                enabled = false;
                return;
            }

            InitializeInputActions();
        }

        private void InitializeInputActions()
        {
            if (inputActionsAsset == null)
            {
                Debug.LogError($"[{nameof(PlayerInputHandler)}] No InputActionAsset assigned on '{gameObject.name}'. " +
                               "Please assign 'Assets/Settings/InputSystem_Actions.inputactions' in the Inspector.", this);
                enabled = false;
                return;
            }

            var playerMap = inputActionsAsset.FindActionMap("Player", false);
            if (playerMap == null)
            {
                Debug.LogError($"[{nameof(PlayerInputHandler)}] Action map 'Player' not found in '{inputActionsAsset.name}'.", this);
                enabled = false;
                return;
            }

            _moveAction = playerMap.FindAction("Move", false);
            _jumpAction = playerMap.FindAction("Jump", false);

            if (_moveAction == null || _jumpAction == null)
            {
                Debug.LogError($"[{nameof(PlayerInputHandler)}] Missing required actions ('Move' or 'Jump') in 'Player' map of '{inputActionsAsset.name}'.", this);
                enabled = false;
                return;
            }

            _isInitialized = true;
        }

        private void OnEnable()
        {
            if (!_isInitialized)
            {
                InitializeInputActions();
                if (!_isInitialized) return;
            }

            // Bind and enable Jump
            if (_jumpAction != null)
            {
                _jumpAction.started += OnJumpStarted;
                _jumpAction.canceled += OnJumpCanceled;
                _jumpAction.Enable();
            }

            // Enable Move
            if (_moveAction != null)
            {
                _moveAction.Enable();
            }
        }

        private void OnDisable()
        {
            // Unbind and disable Jump
            if (_jumpAction != null)
            {
                _jumpAction.started -= OnJumpStarted;
                _jumpAction.canceled -= OnJumpCanceled;
                _jumpAction.Disable();
            }

            // Disable Move
            if (_moveAction != null)
            {
                _moveAction.Disable();
            }

            // Clear input on controller when disabled
            if (controller != null)
            {
                controller.SetMoveInput(0f);
            }
        }

        private void Update()
        {
            if (!_isInitialized || controller == null || _moveAction == null) return;

            Vector2 moveValue = _moveAction.ReadValue<Vector2>();
            controller.SetMoveInput(moveValue.x);
        }

        private void OnJumpStarted(InputAction.CallbackContext context)
        {
            if (controller != null)
            {
                controller.TriggerJump();
            }
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            if (controller != null)
            {
                controller.CutJump();
            }
        }
    }
}

