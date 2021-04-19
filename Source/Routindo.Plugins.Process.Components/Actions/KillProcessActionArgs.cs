namespace Routindo.Plugins.Process.Components.Actions
{
    public static class KillProcessActionArgs
    {
        public const string EntireProcessTree = nameof(EntireProcessTree);
        public const string WaitForExit = nameof(WaitForExit);
        public const string WaitForExitTimeout = nameof(WaitForExitTimeout);
    }
}