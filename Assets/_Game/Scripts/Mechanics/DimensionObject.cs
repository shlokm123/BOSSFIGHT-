using UnityEngine;
using GlitchVerse.Player;

namespace GlitchVerse.Mechanics
{
    public enum DimensionType
    {
        Alpha, // Dimension A (Cyan phase)
        Beta   // Dimension B (Magenta/Amber phase)
    }

    /// <summary>
    /// World platform or obstacle that toggles visibility and collision
    /// according to the active Dimension state (Q).
    /// Prevents trapping the player inside newly solid geometry.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DimensionObject : MonoBehaviour
    {
        [Header("Dimension Alignment")]
        [SerializeField] private DimensionType activeInDimension = DimensionType.Alpha;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color alphaActiveColor = new Color(0f, 0.94f, 1f, 1f);     // Cyan
        [SerializeField] private Color betaActiveColor = new Color(0.95f, 0.2f, 0.85f, 1f);  // Magenta
        [SerializeField] private float inactiveAlpha = 0.22f;

        private Collider2D col;

        public DimensionType ActiveInDimension => activeInDimension;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        /// <summary>
        /// Updates solid collision and visual opacity based on the current dimension.
        /// </summary>
        public void SetDimensionState(DimensionType currentDimension)
        {
            bool isMatching = (activeInDimension == currentDimension);

            if (isMatching)
            {
                // Anti-trapping safety check: if player is currently overlapping, nudge them out
                ResolvePlayerOverlap();
                if (col != null) col.enabled = true;

                if (spriteRenderer != null)
                {
                    Color baseCol = (activeInDimension == DimensionType.Alpha) ? alphaActiveColor : betaActiveColor;
                    baseCol.a = 1.0f;
                    spriteRenderer.color = baseCol;
                }
            }
            else
            {
                // Inactive state: pass-through, dim translucent ghost
                if (col != null) col.enabled = false;

                if (spriteRenderer != null)
                {
                    Color baseCol = (activeInDimension == DimensionType.Alpha) ? alphaActiveColor : betaActiveColor;
                    baseCol.a = inactiveAlpha;
                    spriteRenderer.color = baseCol;
                }
            }
        }

        private void ResolvePlayerOverlap()
        {
            if (col == null) return;

            Collider2D[] overlaps = Physics2D.OverlapBoxAll(col.bounds.center, col.bounds.size, 0f);
            foreach (var hit in overlaps)
            {
                var player = hit.GetComponent<PlayerController2D>();
                if (player != null)
                {
                    // Gently push player upwards onto top of platform to prevent getting stuck
                    var playerRb = player.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        float safeY = col.bounds.max.y + 0.6f;
                        playerRb.position = new Vector2(playerRb.position.x, safeY);
                        Debug.Log("[DimensionObject] Anti-trap: Nudged player above newly activated platform.");
                    }
                }
            }
        }
    }
}
