using System;
using Sound;

namespace Gameplay.Objects
{
    /// <summary>
    /// Extended IWorldObjectView interface to cover character-specific functionality.
    /// </summary>
    public interface ICharacterView : IWorldObjectView
    {
        void Greet(ISoundManager soundManager, Action onComplete);
        void SetIsMoving(bool isMoving);
        void SetIsGrounded(bool isGrounded);
    }
}