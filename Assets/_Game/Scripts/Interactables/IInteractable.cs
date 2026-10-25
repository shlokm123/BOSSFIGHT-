namespace GlitchVerse.Interactables
{
    /// <summary>
    /// Shared interface contract for interactable puzzle objects (switches, buttons, carryables).
    /// As defined in docs/INTERFACES.md.
    /// </summary>
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        bool CanInteract { get; }
        void Interact();
    }
}
