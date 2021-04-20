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

        public WatcherResult Watch()
        {
            try
            {
                if(string.IsNullOrWhiteSpace(ProcessName))
                    throw new MissingArgumentException(ProcessWatcherArgs.ProcessName);

                var processes = System.Diagnostics.Process.GetProcessesByName(ProcessName);
                var process = processes.FirstOrDefault(e => !e.HasExited);
                if (process != null)
                {
                    return WatcherResult.Succeed(
                        ArgumentCollection.New()
                            .WithArgument(ProcessWatcherResultArgs.ProcessName, process.ProcessName)
                            .WithArgument(ProcessWatcherResultArgs.ProcessId, process.Id)
                    );
                }

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
