namespace ProjectDebug
{
    /// <summary>
    /// Simple static class using for debug in Unity3d-agnostic parts of the project.
    /// </summary>
    public static class Debug
    {
        private static ILogger _logger;

        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }
        
        public static void Log(string message)
        {
            _logger?.Log(message);
        }

        public static void Assert(bool condition, string conditionIsNotTrueMessage = "")
        {
            _logger?.Assert(condition, conditionIsNotTrueMessage);
        }
    }
}