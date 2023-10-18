using Gameplay.Objects;

namespace Gameplay.Factory
{
    /// <summary>
    /// Interface for game objects factory
    /// </summary>
    public interface IWorldObjectFactory
    {
        WorldObject GetObjectForType(string objType);
        void ReleaseObject(WorldObject obj);
    }
}