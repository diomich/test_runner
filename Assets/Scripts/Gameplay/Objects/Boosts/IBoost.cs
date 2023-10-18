namespace Gameplay.Objects.Boosts
{
    /// <summary>
    /// An interface for any applicable to world object boost.
    /// </summary>
    public interface IBoost
    {
        void Apply(IBoostTarget target);
        void Update(float dt);
        void Discard();
        bool IsEnded { get; }
        bool IsVerticalPosChangeAvailable { get; }
        bool IsHorizontalPosChangeAvailable { get; }
    }
}