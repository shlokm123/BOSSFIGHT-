namespace GlitchVerse.Mechanics
{
    /// <summary>
    /// Contract for obstacles, hazards, and platforms that can be suspended
    /// by the Time Freeze reality manipulation mechanic.
    /// </summary>
    public interface IFreezable
    {
        bool IsFrozen { get; }
        void SetFrozen(bool frozen);
    }
}
