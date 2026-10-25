using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace GlitchVerse.Interactables
{
    /// <summary>
    /// Physical security laser barrier blocking access to the real exit in Chapter I
    /// until the Echo-only switch on Platform B is actively held by an Echo ghost.
    /// Follows Chapter I Difficulty Upgrade 01B specifications.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class SecurityBarrier : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private BoxCollider2D barrierCollider;
        [SerializeField] private SpriteRenderer barrierRenderer;
        [SerializeField] private Light2D barrierLight;
        [SerializeField] private Text signText;

        [Header("Visual Colors")]
        [SerializeField] private Color lockedColor = new Color(1.0f, 0.15f, 0.25f, 0.95f);
        [SerializeField] private Color unlockedColor = new Color(0.0f, 0.95f, 1.0f, 0.15f);

        [Header("State")]
        [SerializeField] private bool isLocked = true;
        [SerializeField] private bool isPermanentlyUnlocked = false;

        public bool IsLocked => isLocked;
        public bool IsPermanentlyUnlocked => isPermanentlyUnlocked;

        private void Awake()
        {
            if (barrierCollider == null) barrierCollider = GetComponent<BoxCollider2D>();
            if (barrierRenderer == null) barrierRenderer = GetComponent<SpriteRenderer>();
            if (barrierLight == null) barrierLight = GetComponentInChildren<Light2D>();
            UpdateVisuals();
        }

        public void SetBarrierLocked(bool locked)
        {
            if (isPermanentlyUnlocked && locked)
            {
                return; // Do not relock once player has reached exit
            }

            isLocked = locked;
            if (barrierCollider != null)
            {
                barrierCollider.enabled = locked;
            }
            UpdateVisuals();
            Debug.Log($"[SecurityBarrier] {gameObject.name} locked state set to: {locked}");
        }

        public void UnlockBarrier()
        {
            SetBarrierLocked(false);
        }

        public void SetPermanentlyUnlocked()
        {
            isPermanentlyUnlocked = true;
            SetBarrierLocked(false);
            Debug.Log($"[SecurityBarrier] {gameObject.name} permanently unlocked. Relock suppressed.");
        }

        public void ResetBarrier()
        {
            isPermanentlyUnlocked = false;
            SetBarrierLocked(true);
        }

        private void UpdateVisuals()
        {
            if (barrierRenderer != null)
            {
                barrierRenderer.color = isLocked ? lockedColor : unlockedColor;
                barrierRenderer.enabled = isLocked;
            }

            if (barrierLight != null)
            {
                barrierLight.color = isLocked ? new Color(1.0f, 0.2f, 0.25f, 1f) : new Color(0f, 0.95f, 1f, 1f);
                barrierLight.intensity = isLocked ? 1.4f : 0.4f;
            }

            if (signText != null)
            {
                signText.text = isLocked
                    ? "<color=#FF1E40>[SECURITY INTERLOCK: ACTIVE]</color>\n<size=12><color=#D600FF>TEMPORAL ECHO OVERRIDE REQUIRED</color></size>"
                    : "<color=#00F0FF>[INTERLOCK OFFLINE: ECHO DETECTED]</color>\n<size=12><color=#00FF88>ACCESS GRANTED</color></size>";
            }
        }
    }
}
