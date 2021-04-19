namespace Routindo.Plugins.Process.Components.Actions
{
    public static class StartProcessActionResultArgs
    {
        public const string ProcessPath = nameof(ProcessPath);
        public const string ProcessArguments = nameof(ProcessArguments);
        public const string ProcessWorkingDirectory = nameof(ProcessWorkingDirectory);
        public const string ProcessExited = nameof(ProcessExited);
        public const string ProcessStarted = nameof(ProcessStarted);
    }
}