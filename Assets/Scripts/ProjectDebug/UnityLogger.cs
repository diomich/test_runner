namespace ProjectDebug
{
    /// <summary>
    /// Simple ILogger implementation for interaction with Unity console.
    /// </summary>
    public class UnityLogger : ILogger
    {
        void ILogger.Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        void ILogger.Assert(bool condition, string conditionIsNotTrueMessage)
        {
            UnityEngine.Debug.Assert(condition, conditionIsNotTrueMessage);
        }
    }
}