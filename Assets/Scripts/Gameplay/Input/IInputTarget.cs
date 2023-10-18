namespace Gameplay.Input
{
    /// <summary>
    /// Interface for the input target - an object that will receive generated commands.
    /// </summary>
    public interface IInputTarget
    {
        void ApplyCommand(InputCommand command);
    }
}
