using System;
using System.Linq;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Exceptions;
using Routindo.Contract.Services;
using Routindo.Contract.Watchers;

namespace Routindo.Plugins.Process.Components.Watchers
{
    [PluginItemInfo(ComponentUniqueId, "Watch Process",
         "Watch list of processes and check if a specific process is started."),
     ResultArgumentsClass(typeof(ProcessWatcherResultArgs))]
    public class ProcessWatcher: IWatcher
    {
        public const string ComponentUniqueId = "B548D05E-F8A2-4D70-B8E2-3E501949D525";

        public string Id { get; set; }

        public ILoggingService LoggingService { get; set; }

        [Argument(ProcessWatcherArgs.ProcessName, true)] public string ProcessName { get; set; }

        [Argument(ProcessWatcherArgs.WatchForStopped)] public bool WatchForStopped { get; set; }

        [Argument(ProcessWatcherArgs.NotifyOnlyOnFirstOccurrence)] public bool NotifyOnlyOnFirstOccurrence { get; set; } 
         
        /// <summary>
        /// The last status: true: Process running, False: Process stopped.
        /// </summary>
        private bool? _lastStatus;

        public WatcherResult Watch()
        {
            try
            {
                if(string.IsNullOrWhiteSpace(ProcessName))
                    throw new MissingArgumentException(ProcessWatcherArgs.ProcessName);

                var processes = System.Diagnostics.Process.GetProcessesByName(ProcessName);
                var process = processes.FirstOrDefault(e => !e.HasExited);

                if (!NotifyOnlyOnFirstOccurrence)
                    _lastStatus = null;

                if (WatchForStopped)
                {
                    if (process == null && (
                        // First Time after service started
                        !_lastStatus.HasValue 
                        // Or process was previously running
                        || _lastStatus.Value))
                    {
                        _lastStatus = false;
                        return WatcherResult.Succeed(
                            ArgumentCollection.New()
                                .WithArgument(ProcessWatcherResultArgs.ProcessName, ProcessName)
                        );
                    }

                    _lastStatus = process != null;
                    return WatcherResult.NotFound;
                }

                if (process != null && (
                        // First Time after service started
                        !_lastStatus.HasValue
                        // Or process was previously stopped
                        || !_lastStatus.Value)
                    )
                {
                    _lastStatus = true;
                    return WatcherResult.Succeed(
                        ArgumentCollection.New()
                            .WithArgument(ProcessWatcherResultArgs.ProcessName, process.ProcessName)
                            .WithArgument(ProcessWatcherResultArgs.ProcessId, process.Id)
                    );
                }

                _lastStatus = process != null;
                return WatcherResult.NotFound;
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);
                return WatcherResult.NotFound.WithException(exception);
            }
        }
    }
}
