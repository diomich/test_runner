namespace Gameplay.Input
{
    /// <summary>
    /// Input interface used to control the character during the game.
    /// </summary>
    public interface IInput
    {
        void Start();
        void Stop();
        void SetTarget(IInputTarget target);
    }
}
