using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Services;
using Routindo.Plugins.Process.Components.Actions;
using Routindo.Plugins.Process.Components.Watchers;

namespace Routindo.Plugins.Process.Components.ArgumentsMappers
{
    [PluginItemInfo(ComponentId, "KillProcessByIdArgumentsMapper",
        "Map arguments of " + nameof(ProcessWatcher) + "to arguments of " + nameof(KillProcessByNameAction))]
    public class KillWatchedProcessByNameArgumentsMapper : IArgumentsMapper
    {
        public const string ComponentId = "673388C8-02B1-47A5-BE3C-76C8D3AA614B";
        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }
        public ArgumentCollection Map(ArgumentCollection arguments)
        {
            ArgumentCollection argumentCollection = ArgumentCollection.New();
            if (arguments.HasArgument(ProcessWatcherResultArgs.ProcessName))
            {
                argumentCollection.Add(KillProcessByNameActionExecutionArgs.ProcessName,
                    arguments[ProcessWatcherResultArgs.ProcessName]);
            }

            return argumentCollection;
        }
    }
}