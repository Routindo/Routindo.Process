namespace Routindo.Plugins.Process.Components.Actions
{
    public static class StartProcessActionArgs
    {
        public const string ProcessPath = nameof(ProcessPath);
        public const string ProcessArguments = nameof(ProcessArguments);
        public const string ProcessWorkingDirectory = nameof(ProcessWorkingDirectory);
        public const string WaitForExit = nameof(WaitForExit);
        public const string WaitForExitTimeout = nameof(WaitForExitTimeout);
    }
}