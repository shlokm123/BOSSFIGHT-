using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using GlitchVerse.Player;

namespace GlitchVerse.Interactables
{
    /// <summary>
    /// Security containment blast door that unlocks when the chamber pressure plate is satisfied.
    /// Provides visual state feedback (Red locked / Cyan unlocked) and triggers level completion.
    /// </summary>
    public class ExitDoor : MonoBehaviour
    {
        [Header("Door Components")]
        [SerializeField] private Collider2D solidBlockerCollider;
        [SerializeField] private Collider2D exitTriggerCollider;
        [SerializeField] private SpriteRenderer doorFrameRenderer;
        [SerializeField] private SpriteRenderer doorStatusLightRenderer;
        [SerializeField] private Light2D statusLight2D;

        [Header("Visual Feedback Colors")]
        [SerializeField] private Color lockedColor = new Color(1.0f, 0.12f, 0.25f, 1f); // Emergency Red #FF1E40
        [SerializeField] private Color unlockedColor = new Color(0f, 0.95f, 1f, 1f);    // Cyan #00F0FF

        [Header("State")]
        [SerializeField] private bool isUnlocked = false;

        public bool IsUnlocked => isUnlocked;
        public event Action OnPlayerReachedExit;

        private void Awake()
        {
            ApplyDoorState(false);

            // If exitTriggerCollider is on a child object, attach forwarder
            if (exitTriggerCollider != null && exitTriggerCollider.gameObject != gameObject)
            {
                var forwarder = exitTriggerCollider.gameObject.GetComponent<ExitTriggerForwarder>() 
                             ?? exitTriggerCollider.gameObject.AddComponent<ExitTriggerForwarder>();
                forwarder.Init(this);
            }
        }

        public void SetUnlocked(bool unlocked)
        {
            if (isUnlocked != unlocked)
            {
                isUnlocked = unlocked;
                ApplyDoorState(isUnlocked);
                Debug.Log($"[ExitDoor] Door lock state updated. Unlocked: {isUnlocked}");
            }
        }

        private void ApplyDoorState(bool unlocked)
        {
            isUnlocked = unlocked;

            // Physical path blocking
            if (solidBlockerCollider != null)
            {
                solidBlockerCollider.enabled = !unlocked;
            }

            if (exitTriggerCollider != null)
            {
                exitTriggerCollider.enabled = unlocked;
            }

            // Visual feedback
            Color targetColor = unlocked ? unlockedColor : lockedColor;

            if (doorStatusLightRenderer != null)
            {
                doorStatusLightRenderer.color = targetColor;
            }

            if (statusLight2D != null)
            {
                statusLight2D.color = targetColor;
                statusLight2D.intensity = unlocked ? 1.5f : 0.8f;
            }
        }

        private void FixedUpdate()
        {
            if (!isUnlocked) return;

            // Continuous overlap check for player reaching the exit
            if (exitTriggerCollider != null)
            {
                var hits = Physics2D.OverlapBoxAll(exitTriggerCollider.bounds.center, exitTriggerCollider.bounds.size, 0f);
                foreach (var hit in hits)
                {
                    if (hit != null && (hit.GetComponent<PlayerController2D>() != null || hit.CompareTag("Player")))
                    {
                        NotifyTriggerEnter(hit);
                        break;
                    }
                }
            }
        }

        public void NotifyTriggerEnter(Collider2D other)
        {
            if (!isUnlocked) return;

            // Detect player passing through the exit
            if (other.GetComponent<PlayerController2D>() != null || other.CompareTag("Player"))
            {
                Debug.Log("[ExitDoor] Player entered unlocked exit door.");
                OnPlayerReachedExit?.Invoke();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            NotifyTriggerEnter(other);
        }
    }

    /// <summary>
    /// Helper forwarder that passes trigger events from child colliders to the parent ExitDoor.
    /// </summary>
    public class ExitTriggerForwarder : MonoBehaviour
    {
        private ExitDoor door;

        public void Init(ExitDoor parentDoor)
        {
            door = parentDoor;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (door != null)
            {
                door.NotifyTriggerEnter(other);
            }
        }
    }
}
