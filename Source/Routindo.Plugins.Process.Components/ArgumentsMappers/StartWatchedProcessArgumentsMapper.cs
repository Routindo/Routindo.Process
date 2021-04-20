using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Services;
using Routindo.Plugins.Process.Components.Actions;
using Routindo.Plugins.Process.Components.Watchers;

namespace Routindo.Plugins.Process.Components.ArgumentsMappers
{
    [PluginItemInfo(ComponentId, "KillProcessByIdArgumentsMapper",
        "Map arguments of " + nameof(ProcessWatcher) + "to arguments of " + nameof(StartProcessAction))]
    public class StartWatchedProcessArgumentsMapper : IArgumentsMapper
    {
        public const string ComponentId = "7A17C919-0989-4088-BFBB-4BC64F55B089";
        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }
        public ArgumentCollection Map(ArgumentCollection arguments)
        {
            ArgumentCollection argumentCollection = ArgumentCollection.New();
            if (arguments.HasArgument(ProcessWatcherResultArgs.ProcessName))
            {
                argumentCollection.Add(StartProcessActionExecutionArgs.ProcessPath,
                    arguments[ProcessWatcherResultArgs.ProcessName]);
            }

            return argumentCollection;
        }
    }
}