namespace Gameplay.Input
{
    /// <summary>
    /// Interface to be used in the start screen to provide used controls description.
    /// </summary>
    public interface IInputDescription
    {
        string LeftDesc { get; }
        string RightDesc { get; }
        string JumpDesc { get; }
    }
}
