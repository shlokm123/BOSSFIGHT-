using UnityEngine;

namespace GlitchVerse.Interactables
{
    /// <summary>
    /// Physical, pushable puzzle cube rendered with high-contrast cyan aesthetic.
    /// Resets if knocked outside room bounds.
    /// Implements IInteractable as defined in docs/INTERFACES.md.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class MovableCube : MonoBehaviour, IInteractable
    {
        [Header("Puzzle Properties")]
        [SerializeField] private string prompt = "Push or nudge cube onto plate";
        [SerializeField] private Vector3 initialSpawnPosition;
        [SerializeField] private float resetYThreshold = -7.0f;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color defaultCyan = new Color(0f, 0.94f, 1f, 1f); // #00F0FF

        private Rigidbody2D rb;

        public string InteractionPrompt => prompt;
        public bool CanInteract => true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            initialSpawnPosition = transform.position;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.color = defaultCyan;
            }

            // Ensure physical properties suit puzzle pushing
            rb.mass = 2.0f;
            rb.linearDamping = 1.5f;
            rb.angularDamping = 2.0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void Update()
        {
            // Safety boundary: prevent cube from falling out of the world
            if (transform.position.y < resetYThreshold)
            {
                ResetToSpawn();
            }
        }

        public void Interact()
        {
            // Optional direct interaction: give a small nudge towards player facing direction
            Debug.Log("[MovableCube] Interacted with cube.");
        }

        /// <summary>
        /// Restores the cube to its original authored position in the chamber.
        /// </summary>
        public void ResetToSpawn()
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            transform.position = initialSpawnPosition;
            Debug.Log("[MovableCube] Reset to initial spawn position.");
        }
    }
}
