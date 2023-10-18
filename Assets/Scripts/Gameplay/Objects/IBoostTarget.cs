using System.Numerics;

namespace Gameplay.Objects
{
    /// <summary>
    /// Interface for target of the activated boost.
    /// </summary>
    public interface IBoostTarget
    {
        void ApplySpeedModifier(float modifier);
        void ApplyPositionOffset(Vector3 offset);
    }
}