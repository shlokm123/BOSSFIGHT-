using UnityEngine;
using GlitchVerse.Core;
using GlitchVerse.Player;

namespace GlitchVerse.Mechanics
{
    /// <summary>
    /// Moving hazard or patrol barrier that can be frozen in place using Time Freeze (F).
    /// Kills or restarts the player on physical contact if unfrozen.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class FreezableHazard : MonoBehaviour, IFreezable
    {
        [Header("Movement Path")]
        [SerializeField] private Vector3 offsetA = new Vector3(0f, 0f, 0f);
        [SerializeField] private Vector3 offsetB = new Vector3(0f, 3.5f, 0f);
        [SerializeField] private float moveSpeed = 2.5f;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer hazardRenderer;
        [SerializeField] private Color normalColor = new Color(1.0f, 0.15f, 0.25f, 1f); // Red #FF1E40
        [SerializeField] private Color frozenColor = new Color(0.2f, 0.85f, 1f, 0.9f);   // Ice Cyan #33D6FF

        private Vector3 startPos;
        private Vector3 targetA;
        private Vector3 targetB;
        private bool movingTowardsB = true;
        private bool isFrozen = false;

        public bool IsFrozen => isFrozen;

        private void Awake()
        {
            startPos = transform.position;
            targetA = startPos + offsetA;
            targetB = startPos + offsetB;

            if (hazardRenderer == null)
            {
                hazardRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            UpdateVisuals();
        }

        private void Update()
        {
            if (isFrozen) return;

            Vector3 destination = movingTowardsB ? targetB : targetA;
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, destination) < 0.05f)
            {
                movingTowardsB = !movingTowardsB;
            }
        }

        public void SetFrozen(bool frozen)
        {
            isFrozen = frozen;
            UpdateVisuals();
            Debug.Log($"[FreezableHazard] {gameObject.name} frozen state: {isFrozen}");
        }

        private void UpdateVisuals()
        {
            if (hazardRenderer != null)
            {
                hazardRenderer.color = isFrozen ? frozenColor : normalColor;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CheckHazardCollision(other);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            CheckHazardCollision(collision.collider);
        }

        private void CheckHazardCollision(Collider2D col)
        {
            // If frozen, hazard is safe or acts as a stationary platform
            if (isFrozen) return;

            if (col.GetComponent<PlayerController2D>() != null || col.CompareTag("Player"))
            {
                Debug.Log($"[FreezableHazard] Player contacted active hazard {gameObject.name}!");
                if (LevelDirector.Current != null)
                {
                    LevelDirector.Current.KillPlayer();
                }
            }
        }
    }
}
