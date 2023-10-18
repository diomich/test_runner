namespace ProjectDebug
{
    /// <summary>
    /// Basic logger interface to be used in Unity3d-agnostic parts of the project.
    /// </summary>
    public interface ILogger
    {
        void Log(string message);
        void Assert(bool condition, string conditionIsNotTrueMessage);
    }
}